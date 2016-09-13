using Microsoft.Ajax.Utilities;
using StowagePlanAnalytics_ITP_2016.DAL;
using StowagePlanAnalytics_ITP_2016.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace StowagePlanAnalytics_ITP_2016.Calculations
{
    public class CalculationOptimized
    {
        //the tier where the hatch is,all containers above this tier is stored above the hatch and requires the hatch to be
        //removed.
        const int HATCHTIER = 80;
        //current vessel on this particular voyage
        Vessel vessel;
        //list of usefulinfomodels of one voyage.This has the calculations for all 
        //the trips of a particular vovyage
        public List<UsefulInfo> listOfUsefulInfoModels;
        public CalculationOptimized(Voyage voyage)
        {
            //initlaizing the current vessel for calculations later
            vessel = voyage.Vessel;
            //list of usefulinfomodels initialized 
            listOfUsefulInfoModels = new List<UsefulInfo>();
            int index = 0;


            //goes through each trip in the voyage and creates a new model and sets certain information required by the
            //usefullInfo and passes it the relevant calculation funciton
            //if its a trip without a previous trip,it will just do a partial calculations else it will pass in the current trip and the previous trip.
            foreach (var currentTrip in voyage.Trips)
            {
                index++;
                Trip previousTrip = null;
                //setting the variables for the usefulinfo
                UsefulInfo usefullInfo = new UsefulInfo();
                usefullInfo.VoyageID = voyage.VoyageID;
                usefullInfo.Ballast = (currentTrip.Ballast / 1000);
                usefullInfo.BunkerHFO = (currentTrip.BunkerOilHFO / 1000);
                usefullInfo.BunkerMGOMDO = (currentTrip.BunkerOilMGOMDO / 1000);
                usefullInfo.DepPortCode = currentTrip.DepPort.PortCode;
                usefullInfo.VesselCode = voyage.Vessel.VesselCode;
                usefullInfo.MaxWeight = voyage.Vessel.MaxWeight;
                usefullInfo.ReefersCapacity = voyage.Vessel.MaxReefers;
                usefullInfo.TEUCapacity = voyage.Vessel.TEUCapacity;
                usefullInfo.VesselTEUClassCode = voyage.Vessel.VesselTEUClassCode;
                usefullInfo.ServiceCode = voyage.ServiceCode;
                usefullInfo.ArrPortCode = currentTrip.ArrivalPort;
                usefullInfo.CIAgreed = currentTrip.DepPort.NoOfCranes;
                //determining if there is a previous trip to assign the appropriate calculations
                previousTrip = getPreviousTrip(currentTrip, voyage.Trips);
                if (previousTrip != null)
                {
                    try
                    {
                        fullCalculations(currentTrip, previousTrip, usefullInfo);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("File;" + index.ToString());
                    }

                }
                else
                {
                    try
                    {
                        partialCalculations(currentTrip, usefullInfo);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("File;" + index.ToString());
                    }
                }
            }
        }
        ///<summary>
        ///finds the previous trip(stif equvilavent) for calculations
        ///</summary>
        public Trip getPreviousTrip(Trip currentTrip, IEnumerable<Trip> trips)
        {
            var previousTrip = trips.FirstOrDefault(x => x.DepPort.SequenceNo == (currentTrip.DepPort.SequenceNo - 1));
            if (previousTrip != null)
            {
                return (Trip)previousTrip;
            }
            return null;
        }
        ///<summary>
        ///does the full calculations which requires the current trip object and its previous trip object
        ///</summary>
        public void fullCalculations(Trip currentTrip, Trip previousTrip, UsefulInfo usefullInfo)
        {
            //variables used in calculations
            int hatchMoves = 0, currentBay = 0, reshiftingMoves = 0, totalNoOfContainers = 0;

            //Owner,bay moves,non-discharged containers dictonaries 
            SortedDictionary<string, int> ownerCount = new SortedDictionary<string, int>();
            SortedDictionary<int, int> bayMoves = new SortedDictionary<int, int>();
            SortedDictionary<int, Container> nonDischargedContainers = new SortedDictionary<int, Container>();
            SortedDictionary<string, Container> currentTripContainers = new SortedDictionary<string, Container>();
            SortedDictionary<string, int> hatchMove = new SortedDictionary<string, int>();

            //lists for calculations
            List<Container> dischargedContainers = new List<Container>();
            List<Container> restowContainers = new List<Container>();
            List<Container> reshiftContainers = new List<Container>();
            List<Container> containersToMove = new List<Container>();
            List<Container> containersToMoveBelowHatch = new List<Container>();
            List<Container> loadingContainers = new List<Container>();

            //loop through each container in the current trip to count oog,imo,iso,owner,reefers,loaded containers,total number of containers on 
            //the trip currently
            foreach (var currentTripContainer in currentTrip.Containers)
            {

                if (ValidateStowLocation(currentTripContainer))
                {
                    //store all the currentTrip containers into a dictonary for later reference
                    currentTripContainers[currentTripContainer.containerID] = currentTripContainer;

                    //count number of containers  on the ship on the current trip
                    totalNoOfContainers += 1;
                    //calculate total weight on board the ship on current trip
                    usefullInfo.WeightOnboard += currentTripContainer.weight;

                    //check if container is a oog unit,then increment count
                    if (currentTripContainer.oog)
                    {
                        usefullInfo.OOGUnits++;
                    }
                    //check if container is a imo unit,then increment count
                    if (currentTripContainer.imo)
                    {
                        usefullInfo.IMOUnits++;
                    }
                    //increment the count of TEU onboard by checking the iso of the container,if it starts with a 2 it is 
                    //equvailvent to 1 TEU,other isos are automatically considered as 2 TEUs
                    if (currentTripContainer.equipmentISO[0] == '2')
                    {
                        usefullInfo.TEUOnboard += 1;
                    }
                    if (currentTripContainer.equipmentISO[0] != '2')
                    {
                        usefullInfo.TEUOnboard += 2;
                    }
                    //populate owner dictionary to count the number of containers 
                    //for its relevant owner
                    if (currentTripContainer.owner != null)
                    {
                        if (ownerCount.ContainsKey(currentTripContainer.owner))
                        {
                            ownerCount[currentTripContainer.owner] += 1;
                        }
                        else
                        {
                            ownerCount[currentTripContainer.owner] = 1;
                        }
                    }

                    //REEFERS ONBOARD
                    //Check if the current container is a reefer 
                    //then increase count of reefers on board
                    if (currentTripContainer.reefer == true)
                    {
                        usefullInfo.ReefersOnboard += 1;
                    }
                    //LOADED MOVES
                    //check if the current container is loaded in the current port,then increase the loaded moves count
                    if (currentTripContainer.loadPort == currentTrip.DeparturePort)
                    {
                        usefullInfo.LoadedMoves += 1;
                        loadingContainers.Add(currentTripContainer);

                        // count the number of loading containers based on the bay number
                        currentBay = currentTripContainer.bay;
                        if (bayMoves.ContainsKey(currentBay))
                        {
                            bayMoves[currentBay] += 1;
                        }
                        else
                        {
                            bayMoves[currentBay] = 1;
                        }
                        //Check if the current container is a reefer AND also loaded in the current port
                        if (currentTripContainer.reefer == true)
                        {
                            usefullInfo.ReefersLoaded += 1;
                        }
                    }
                }

            }
            //loop through each container in the previous trip to count discharged containers,discharged reefers and non discharged containers
            foreach (var previousTripContainer in previousTrip.Containers)
            {
                if (ValidateStowLocation(previousTripContainer))
                {
                    //REEFERS DISCHARGED
                    //check if the previous trip's container is discharged at the current port AND is a reefer
                    //then increase reefersdischarged count
                    if (previousTripContainer.dischargePort == currentTrip.DeparturePort && previousTripContainer.reefer == true)
                    {
                        usefullInfo.ReefersDischarged += 1;
                    }
                    //DISCHARGED MOVES
                    //check if if the previous trip's container is discharged at the current port then increment the discharged containers count
                    //if not store it into a list of containers which are not discharged
                    if (previousTripContainer.dischargePort == currentTrip.DeparturePort)
                    {
                        dischargedContainers.Add(previousTripContainer);
                        usefullInfo.DischargedMoves += 1;

                        // count the number of discharged container moves based on the bay number
                        currentBay = previousTripContainer.bay;
                        if (bayMoves.ContainsKey(currentBay))
                        {
                            bayMoves[currentBay] += 1;
                        }
                        else
                        {
                            bayMoves[currentBay] = 1;
                        }
                    }
                    else
                    {
                        //add the containers which are not going to be discharged at the current port into the dictonary 
                        nonDischargedContainers[previousTripContainer.stowLocation] = previousTripContainer;
                    }
                }
            }
            foreach (KeyValuePair<string, int> kv in ownerCount)
            {
                //concenntate all the items in the owner dictonary into one and store into the model
                usefullInfo.OwnerCount += (kv.Key + ":" + kv.Value + ",");
            }

            usefullInfo.OwnerCount = usefullInfo.OwnerCount.Remove(usefullInfo.OwnerCount.Length - 1);

            //SHIFTING
            //check the non discharged containers dictonary,against the current trip containers dictonary
            //if the stow location has changed,it means the container has been shifted
            //increment the shifted containers count
            for (int index = 0; index < nonDischargedContainers.Count; index++)
            {
                var nonDischargedContainer = nonDischargedContainers.ElementAt(index);

                if (currentTripContainers.ContainsKey(nonDischargedContainer.Value.containerID) && (nonDischargedContainer.Key != currentTripContainers[nonDischargedContainer.Value.containerID].stowLocation))
                {
                    reshiftContainers.Add(nonDischargedContainer.Value);
                    nonDischargedContainers.Remove(nonDischargedContainer.Key);
                    index -= 1;
                }
            }
            //SHIFTING(FOR CI)
            for (int i = 0; i < reshiftContainers.Count; i++)
            {
                currentBay = (reshiftContainers[i].bay);
                if (bayMoves.ContainsKey(currentBay))
                {
                    bayMoves[currentBay] += 1;
                }
                else
                {
                    bayMoves[currentBay] = 2;
                }
            }

            //order the discharged containers by stowlocation and group by BBRR
            //this is to ensure that the list will not have all the last container which has to be discharged for each row
            dischargedContainers = dischargedContainers.OrderBy(container => container.stowLocation).ToList().DistinctBy(x => (x.stowLocation / 100)).ToList().OrderBy(container => container.stowLocation).ToList();

            //RESTOW PART 1
            //check the discharged containers list against the non discharged containers list
            for (int i = 0; i < dischargedContainers.Count; i++)
            {
                //int highestTier = 96;
                int highestTier = vessel.bayDictionary[dischargedContainers[i].bay].MaxTier;
                //get dischargedContainers[i].BB highest tier(e.g 96) from database
                //checking tier is to add 2 to it until it reaches the highest tier of the BB
                int checkingTier = 0;
                //check if the container supposed to be discharged is 20 footer,if it is then continue
                if (dischargedContainers[i].equipmentISO[0] == '2')
                {
                    //this is to check the even bay number it need to check(e.g for bay 3 the even bay is 2
                    //and it cannot be 4.Hence the bayNotoAdd determines the number to add 
                    int evenBay = dischargedContainers[i].bay + 1;
                    int bayNoToAdd = 0;
                    if (evenBay % 4 == 2)
                    {
                        bayNoToAdd = 0 + 10000;

                    }
                    else if (evenBay % 4 == 0)
                    {
                        bayNoToAdd = 0 - 10000;
                    }
                    do
                    {
                        //add 2 to the checking tier and check if there is any containers above the container which is supposed 
                        // to be discharged until it reaches the highest tier
                        checkingTier += 2;
                        //check if there is any 20 footers above the discharged container,then restow
                        if (nonDischargedContainers.ContainsKey(dischargedContainers[i].stowLocation + checkingTier))
                        {
                            usefullInfo.RestowMoves += 1;
                            restowContainers.Add(nonDischargedContainers[dischargedContainers[i].stowLocation + checkingTier]);
                            nonDischargedContainers.Remove(dischargedContainers[i].stowLocation + checkingTier);

                        }
                        //check if there is any 40 footers above the discharged container,then restow
                        if (nonDischargedContainers.ContainsKey(dischargedContainers[i].stowLocation + checkingTier + bayNoToAdd))
                        {
                            usefullInfo.RestowMoves += 1;
                            restowContainers.Add(nonDischargedContainers[dischargedContainers[i].stowLocation + checkingTier + bayNoToAdd]);
                            nonDischargedContainers.Remove(dischargedContainers[i].stowLocation + checkingTier + bayNoToAdd);
                        }
                    } while ((dischargedContainers[i].tier + checkingTier) <= highestTier);
                }
                //check if the container supposed to be discharged is 40 footer,if it is then continue
                else if (dischargedContainers[i].equipmentISO[0] != '2')
                {
                    do
                    {
                        //add 2 to the checking tier and check if there is any containers above the container which is supposed 
                        // to be discharged until it reaches the highest tier
                        checkingTier += 2;
                        //check if there is any 40 footers above the discharged container,then restow
                        if (nonDischargedContainers.ContainsKey(dischargedContainers[i].stowLocation + checkingTier))
                        {
                            usefullInfo.RestowMoves += 1;
                            restowContainers.Add(nonDischargedContainers[dischargedContainers[i].stowLocation + checkingTier]);
                            nonDischargedContainers.Remove(dischargedContainers[i].stowLocation + checkingTier);
                        }
                        //check if there is any 20 footers above the discharged container,then restow
                        if (nonDischargedContainers.ContainsKey(dischargedContainers[i].stowLocation + checkingTier + 10000))
                        {
                            usefullInfo.RestowMoves += 1;
                            restowContainers.Add(nonDischargedContainers[dischargedContainers[i].stowLocation + checkingTier + 10000]);
                            nonDischargedContainers.Remove(dischargedContainers[i].stowLocation + checkingTier + 10000);
                        }
                        //check if there is any 20 footers above the discharged container,then restow
                        if (nonDischargedContainers.ContainsKey(dischargedContainers[i].stowLocation + checkingTier - 10000))
                        {
                            usefullInfo.RestowMoves += 1;
                            restowContainers.Add(nonDischargedContainers[dischargedContainers[i].stowLocation + checkingTier - 10000]);
                            nonDischargedContainers.Remove(dischargedContainers[i].stowLocation + checkingTier - 10000);
                        }
                    } while ((dischargedContainers[i].tier + checkingTier) <= highestTier);
                }
            }

            //ALL CONTAINER MOVES TILL NOW
            //combine all containers which has to be moved(discharged and restowed and reshifted at this current point into 
            //an ordered list by stowlocaiton
            containersToMove = loadingContainers.Concat(dischargedContainers)
                                    .Concat(restowContainers)
                                    .Concat(reshiftContainers)
                                    .OrderBy(container => container.stowLocation)
                                    .ToList();

            //BELOW HATCH container MOVES
            for (int i = 0; i < containersToMove.Count; i++)
            {
                //check if the contaiers which has to be moved are below the hatch tier
                if (containersToMove[i].tier < HATCHTIER)
                {
                    //if they are below the hatch tier,add it into a list of containers to be moved below the hatch
                    containersToMoveBelowHatch.Add(containersToMove[i]);
                }
            }

            int hatchNumber = 0;
            for (int i = 0; i < containersToMoveBelowHatch.Count; i++)
            {
                int[] hatchPattern = vessel.bayDictionary[containersToMoveBelowHatch[i].bay].HatchPattern.Split(',').Select(int.Parse).ToArray();
                hatchNumber = getHatchPosition(containersToMoveBelowHatch[i].row, hatchPattern);
                if (!hatchMove.ContainsKey((containersToMoveBelowHatch[i].bay) + "." + hatchNumber))
                {
                    if ((containersToMoveBelowHatch[i].bay % 2) == 0)
                    {
                        hatchMove[(containersToMoveBelowHatch[i].bay) + "." + hatchNumber] = 1;
                        hatchMove[(containersToMoveBelowHatch[i].bay + 1) + "." + hatchNumber] = 1;
                        hatchMove[(containersToMoveBelowHatch[i].bay - 1) + "." + hatchNumber] = 1;
                    }
                    else
                    {
                        hatchMove[(containersToMoveBelowHatch[i].bay) + "." + hatchNumber] = 1;
                    }
                }
            }
            //RESTOW PART 2
            //second restow checking.Essentialy what this loop does is to go through each container which has not been discharged yet
            //it would first then check if its above the hatch(containers below the hatch which require restowing has been accounted for earlier)
            //if the container is above the hatch,it would then get the a sorted dictonary of which hatch covers which row for that dischargedcontainer's bay
            //it would then check if its a 40 or 20 footer,after that checking it would combine,the discharged container's bay and the hatch index its under
            //i.e 1.1 would mean bay 1 hatch 1
            //next it would check hatchMove dictonary to see if that particular key exists which means that hatch has to be moved,it would then restow that container
            //if its 1 40 footer it will merely check its odd bay's -1 or +1

            for (int index = 0; index < nonDischargedContainers.Count(); index++)
            {
                var nonDischargedContainer = nonDischargedContainers.ElementAt(index);
                if (nonDischargedContainer.Value.tier >= HATCHTIER)
                {
                    //based on bay,need to reterive hatch pattern(i.e Bay kv.Key's(BB) hatch pattern is { 6,4,4,6}
                    SortedDictionary<int, int> rowHatch = getHatchCoverage(vessel.bayDictionary[nonDischargedContainer.Value.bay].HatchPattern.Split(',').Select(int.Parse).ToArray());
                    //TODO: ACCOUNT FOR 40 foot containers
                    if (nonDischargedContainer.Value.equipmentISO[0] == '2')
                    {
                        //check if hatchMove contains particular key
                        if (hatchMove.ContainsKey(nonDischargedContainer.Value.bay.ToString() + "." + rowHatch[nonDischargedContainer.Value.row].ToString()))
                        {
                            usefullInfo.RestowMoves += 1;
                            restowContainers.Add(nonDischargedContainer.Value);
                            nonDischargedContainers.Remove(nonDischargedContainer.Key);
                            index -= 1;
                        }
                    }
                    else if (nonDischargedContainer.Value.equipmentISO[0] != '2')
                    {
                        //check if hatchMove contains particular key where bay is -1 OR +1
                        if (hatchMove.ContainsKey((nonDischargedContainer.Value.bay - 1).ToString() + "." + rowHatch[nonDischargedContainer.Value.row].ToString()) || hatchMove.ContainsKey((nonDischargedContainer.Value.bay + 1).ToString() + "." + rowHatch[nonDischargedContainer.Value.row].ToString()))
                        {
                            usefullInfo.RestowMoves += 1;
                            restowContainers.Add(nonDischargedContainer.Value);
                            nonDischargedContainers.Remove(nonDischargedContainer.Key);
                            index -= 1;
                        }
                    }
                }
            }

            //CHECK FOR DOUBLE RESTOW COUNT
            //once the containers which has to be restowed has been confirmed
            //run a for loop with the restowcontainers list against the containers to be shifted
            //if there is container which is in both list,remove the container from the restow list
            for (int i = 0; i < restowContainers.Count; i++)
            {
                for (int j = 0; j < reshiftContainers.Count; j++)
                {
                    if (restowContainers[i].containerID == reshiftContainers[j].containerID)
                    {
                        restowContainers.RemoveAt(i);
                        i -= 1;
                    }
                }
            }
            //RESTOW(FOR CI)
            for (int i = 0; i < restowContainers.Count; i++)
            {
                currentBay = (restowContainers[i].bay);
                if (bayMoves.ContainsKey(currentBay))
                {
                    bayMoves[currentBay] += 2;
                }
                else
                {
                    bayMoves[currentBay] = 2;
                }
            }
            // HATCH MOVES (FOR CI)
            //once the hatch list has been finalized
            //this step will count the number of hatch moves for each bay
            //the hatchMove dictioary contains all the hatches that will be moved
            //Eg. 61.3 -> bay 61, Hatch 3
            //if the bay number already exist in the bayMoves dictionary,
            //it will increase its current value by 2 moves (Remove + Putting back Hatch)
            //else it will create a new keypair value for the bay number
            foreach (KeyValuePair<string, int> kv in hatchMove)
            {
                currentBay = Int32.Parse(kv.Key.Split('.')[0]);
                if (bayMoves.ContainsKey(currentBay))
                {
                    bayMoves[currentBay] += 2;
                }
                else
                {
                    bayMoves[currentBay] = 2;
                }
            }

            //ALL CONTAINER MOVES(FINAL)
            //combine all containers which has to be moved(discharged and restowed and reshifted)
            //this list is the final list and would not have any duplicates
            containersToMove = loadingContainers.Concat(dischargedContainers)
                                    .Concat(restowContainers)
                                    .Concat(reshiftContainers)
                                    .OrderBy(container => container.stowLocation)
                                    .ToList();


            //TEU
            //calculate the TEU remaining by subtracting the TEU on board from the vessel's capacity
            //derive the TEU utilsation of the vessel's TEU capacity
            usefullInfo.TEURemaining = usefullInfo.TEUCapacity - usefullInfo.TEUOnboard;
            usefullInfo.TEUUtilisation = Math.Round((usefullInfo.TEUOnboard * 100.0 / usefullInfo.TEUCapacity), 2);

            //WEIGHT
            //calculate the weight remaining by subtracting the weight on board from the vessel's capacity
            //derive the weight utilsation of the vessel's weight capacity
            usefullInfo.WeightOnboard /= 1000;
            usefullInfo.WeightRemaining = usefullInfo.MaxWeight - usefullInfo.WeightOnboard;
            usefullInfo.WeightUtilisation = Math.Round((usefullInfo.WeightOnboard * 100.0 / usefullInfo.MaxWeight), 2);

            //OOG
            //derive the OOG utilsation of the total number of containers on the ship currently at this trip
            usefullInfo.OOGUtilisation = Math.Round((usefullInfo.OOGUnits * 100.0 / totalNoOfContainers), 2);

            //IMO
            //derive the IMO utilsation of the total number of containers on the ship currently at this trip
            usefullInfo.IMOUtilisation = Math.Round((usefullInfo.IMOUnits * 100.0 / totalNoOfContainers), 2);

            //REEFERS
            //calculate the remaining reefers slots in the vessel by subtracting the reefers onboard and adding the reefers slot which were discharged
            // to the vessel's reefer's capacity
            usefullInfo.RemainingReefersSlots = usefullInfo.ReefersCapacity - usefullInfo.ReefersOnboard;
            usefullInfo.ReefersUtilisation = Math.Round((usefullInfo.ReefersOnboard * 100.0 / totalNoOfContainers), 2); ;

            //RESHIFT,RESTOW,HATCH,DISCHARGED,LOADING,
            reshiftingMoves = reshiftContainers.Count;
            usefullInfo.RestowMoves = restowContainers.Count * 2;
            hatchMoves = hatchMove.Count();
            hatchMoves *= 2;

            //TOTAL MOVES
            usefullInfo.TotalMoves = hatchMoves + usefullInfo.DischargedMoves + usefullInfo.RestowMoves + usefullInfo.LoadedMoves + reshiftingMoves;
            usefullInfo.ReshiftingMoves = reshiftingMoves;
            int totalMoves = usefullInfo.TotalMoves;

            usefullInfo.RestowPercentage = Math.Round((usefullInfo.RestowMoves * 100.0 / usefullInfo.TotalMoves), 2);

            //CRANE INTENSITY
            //Once total moves has been calculated
            //Need to perform SQL query to get port's crane

            //craneAssignment is a 2D array
            //[i,0] - Starting bay handled by the crane
            //[i,1] - Last bay handled by the crane
            //[i,2] - Total number of moves handled by the crane
            int[,] craneAssignment = new int[currentTrip.DepPort.NoOfCranes, 3];
            int lastAccessed = -1, currentMoves = 0, avgMoves = 0, highestMoves = 0;

            //the first forloop will loop through the number of crane in the particular port
            for (int i = 0; i < currentTrip.DepPort.NoOfCranes; i++)
            {
                totalMoves -= currentMoves;
                //average move is calculated using the current total moves / number of cranes
                avgMoves = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalMoves / (currentTrip.DepPort.NoOfCranes - i))));

                currentMoves = 0;
                //this forloop will loop through all the keyvaluepair in the bayMoves dictionary
                for (int j = 0; j < bayMoves.Count; j++)
                {
                    KeyValuePair<int, int> kv = bayMoves.ElementAt(j);
                    if (kv.Key > lastAccessed)
                    {
                        // if currentMoves is lesser than the average moves, it will add the value of the current bay to it
                        if (currentMoves < avgMoves)
                        {
                            // To save the starting bay handled by the crane
                            if (craneAssignment[i, 0] == 0)
                            {
                                craneAssignment[i, 0] = kv.Key;
                            }
                            //currentMoves will keep track of the total moves assigned to the particular crane
                            currentMoves += kv.Value;
                            //lastAccessed will keep track of the current bay position in the bayMoves dictionary
                            lastAccessed = kv.Key;
                            if ((j + 1) >= bayMoves.Count())
                            {
                                craneAssignment[i, 1] = lastAccessed;
                                craneAssignment[i, 2] = currentMoves;
                            }
                        }
                        else
                        {
                            craneAssignment[i, 1] = lastAccessed;
                            craneAssignment[i, 2] = currentMoves;
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < craneAssignment.GetLength(0); i++)
            {
                usefullInfo.CraneAllocation += (i + 1).ToString() + "-" + craneAssignment[i, 0].ToString() + "," + craneAssignment[i, 1].ToString() + "-" + craneAssignment[i, 2].ToString() + ";";
            }
            usefullInfo.CraneAllocation = usefullInfo.CraneAllocation.Remove(usefullInfo.CraneAllocation.Length - 1);

            totalMoves = 0;
            //To find out what is the highest assigned moves from all the crane
            for (int i = 0; i < craneAssignment.GetLength(0); i++)
            {
                totalMoves += craneAssignment[i, 2];
                if (craneAssignment[i, 2] > highestMoves)
                {
                    highestMoves = craneAssignment[i, 2];
                }
            }

            usefullInfo.CIPlanned = Math.Round((totalMoves * 1.0 / highestMoves), 2);
            usefullInfo.RestowCost = usefullInfo.RestowMoves * currentTrip.DepPort.CostOfMove;

            listOfUsefulInfoModels.Add(usefullInfo);

        }
        ///<summary>
        ///This function currently checks if a container with its stowlocation is a valid one(e.g row must not be 00)
        ///</summary>
        public bool ValidateStowLocation(Container container)
        {
            if (container.bay == 0 || container.row == 0 || container.tier == 0)
            {
                return false;
            }
            return true;
        }
        ///<summary>
        ///does partial calculations which requires the current trip object only
        ///</summary>
        public void partialCalculations(Trip currentTrip, UsefulInfo usefullInfo)
        {
            //variables used in calculations
            int hatchMoves = 0, currentBay = 0, totalNoOfContainers = 0;

            //Owner,bay moves
            SortedDictionary<int, int> bayMoves = new SortedDictionary<int, int>();
            SortedDictionary<string, int> ownerCount = new SortedDictionary<string, int>();
            //List of loaded containers
            List<Container> loadingContainers = new List<Container>();

            //loop through each container in the current trip to count oog,imo,iso,owner,reefers,loaded containers,total number of containers on 
            //the trip currently
            foreach (var currentTripContainer in currentTrip.Containers)
            {

                if (ValidateStowLocation(currentTripContainer))
                {
                    totalNoOfContainers += 1;
                    usefullInfo.WeightOnboard += currentTripContainer.weight;

                    if (currentTripContainer.oog)
                    {
                        usefullInfo.OOGUnits++;
                    }
                    if (currentTripContainer.imo)
                    {
                        usefullInfo.IMOUnits++;
                    }
                    if (currentTripContainer.equipmentISO[0] == '2')
                    {
                        usefullInfo.TEUOnboard += 1;
                    }
                    if (currentTripContainer.equipmentISO[0] != '2')
                    {
                        usefullInfo.TEUOnboard += 2;
                    }
                    if (currentTripContainer.owner != null)
                    {
                        if (ownerCount.ContainsKey(currentTripContainer.owner))
                        {
                            ownerCount[currentTripContainer.owner] += 1;
                        }
                        else
                        {
                            ownerCount[currentTripContainer.owner] = 1;
                        }
                    }

                    //REEFERS ONBOARD
                    if (currentTripContainer.reefer == true)
                    {
                        usefullInfo.ReefersOnboard += 1;
                    }
                    //LOADED MOVES
                    if (currentTripContainer.loadPort == currentTrip.DeparturePort)
                    {
                        usefullInfo.LoadedMoves += 1;
                        loadingContainers.Add(currentTripContainer);

                        // count the number of loading containers based on the bay number
                        currentBay = currentTripContainer.bay;
                        if (bayMoves.ContainsKey(currentBay))
                        {
                            bayMoves[currentBay] += 1;
                        }
                        else
                        {
                            bayMoves[currentBay] = 1;
                        }
                        if (currentTripContainer.reefer == true)
                        {
                            usefullInfo.ReefersLoaded += 1;
                        }

                    }


                }


            }
            foreach (KeyValuePair<string, int> kv in ownerCount)
            {
                //concenntate all the items in the owner dictonary into one and store into the model
                usefullInfo.OwnerCount += (kv.Key + ":" + kv.Value + ",");

            }
            usefullInfo.OwnerCount = usefullInfo.OwnerCount.Remove(usefullInfo.OwnerCount.Length - 1);

            //TOTAL MOVES
            usefullInfo.TotalMoves = hatchMoves + usefullInfo.LoadedMoves;
            int totalMoves = usefullInfo.TotalMoves;
            //TEU
            usefullInfo.TEURemaining = usefullInfo.TEUCapacity - usefullInfo.TEUOnboard;
            usefullInfo.TEUUtilisation = Math.Round((usefullInfo.TEUOnboard * 100.0 / usefullInfo.TEUCapacity), 2);

            //OOG
            usefullInfo.OOGUtilisation = Math.Round((usefullInfo.OOGUnits * 100.0 / totalNoOfContainers), 2);
            //IMO
            usefullInfo.IMOUtilisation = Math.Round((usefullInfo.IMOUnits * 100.0 / totalNoOfContainers), 2);

            //WEIGHT
            usefullInfo.WeightOnboard /= 1000;
            usefullInfo.WeightRemaining = usefullInfo.MaxWeight - usefullInfo.WeightOnboard;
            usefullInfo.WeightUtilisation = Math.Round((usefullInfo.WeightOnboard * 100.0 / usefullInfo.MaxWeight), 2);
            //REEFERS
            usefullInfo.RemainingReefersSlots = usefullInfo.ReefersCapacity - usefullInfo.ReefersOnboard;
            usefullInfo.ReefersUtilisation = Math.Round((usefullInfo.ReefersOnboard * 100.0 / totalNoOfContainers), 2);


            //CRANE INTENSITY
            //Need to perform SQL query to get port's crane

            int[,] craneAssignment = new int[currentTrip.DepPort.NoOfCranes, 3];
            int lastAccessed = -1, currentMoves = 0, avgMoves = 0, highestMoves = 0;

            for (int i = 0; i < currentTrip.DepPort.NoOfCranes; i++)
            {
                totalMoves -= currentMoves;
                avgMoves = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalMoves / (currentTrip.DepPort.NoOfCranes - i))));

                currentMoves = 0;

                for (int j = 0; j < bayMoves.Count; j++)
                {
                    KeyValuePair<int, int> kv = bayMoves.ElementAt(j);
                    if (kv.Key > lastAccessed)
                    {
                        if (currentMoves < avgMoves)
                        {
                            // To save the first bay handled by the crane
                            if (craneAssignment[i, 0] == 0)
                            {
                                craneAssignment[i, 0] = kv.Key;
                            }
                            currentMoves += kv.Value;
                            lastAccessed = kv.Key;
                            if ((j + 1) >= bayMoves.Count())
                            {
                                craneAssignment[i, 1] = lastAccessed;
                                craneAssignment[i, 2] = currentMoves;
                            }
                        }
                        else
                        {
                            craneAssignment[i, 1] = lastAccessed;
                            craneAssignment[i, 2] = currentMoves;
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < craneAssignment.GetLength(0); i++)
            {
                usefullInfo.CraneAllocation += (i + 1).ToString() + "-" + craneAssignment[i, 0].ToString() + "," + craneAssignment[i, 1].ToString() + "-" + craneAssignment[i, 2].ToString() + ";";
            }
            usefullInfo.CraneAllocation = usefullInfo.CraneAllocation.Remove(usefullInfo.CraneAllocation.Length - 1);

            totalMoves = 0;
            for (int i = 0; i < craneAssignment.GetLength(0); i++)
            {
                totalMoves += craneAssignment[i, 2];
                if (craneAssignment[i, 2] > highestMoves)
                {
                    highestMoves = craneAssignment[i, 2];
                }
            }

            usefullInfo.CIPlanned = Math.Round((totalMoves * 1.0 / highestMoves), 2);

            usefullInfo.RestowCost = usefullInfo.RestowMoves * currentTrip.DepPort.CostOfMove;
            
            listOfUsefulInfoModels.Add(usefullInfo);

        }
        ///<summary>
        ///checks how many twinlifts can be achieved from the containers which has to be moved,by checking if two containers which has to
        /// be moved is beside each other in bay.
        /// E.g if container A stow location is 102030 and container B stowlocation is 302030..count it has a twinlift
        ///</summary>
        public int twinLift(List<Container> containersToMove)
        {
            int twinLifts = 0;
            int bayNoToAdd = 0;

            for (int i = 0; i < containersToMove.Count(); i++)
            {
                int evenBay = containersToMove[i].bay + 1;
                if (evenBay % 4 == 2)
                {
                    bayNoToAdd = 0 + 20000;
                }
                else if (evenBay % 4 == 0)
                {
                    bayNoToAdd = 0 - 20000;
                }
                for (int j = i + 1; j < containersToMove.Count(); j++)
                {
                    if (containersToMove[i].equipmentISO[0] == '2' && containersToMove[j].equipmentISO[0] == '2')
                    {

                        if (combineNumber(containersToMove[i].row, containersToMove[i].tier) == combineNumber(containersToMove[j].row, containersToMove[j].tier) && containersToMove[i].stowLocation + bayNoToAdd == containersToMove[j].stowLocation)
                        {
                            twinLifts += 1;
                            containersToMove.RemoveAt(j);
                            j -= 1;
                            containersToMove.RemoveAt(i);
                            i -= 1;
                        }
                    }
                }
            }
            return twinLifts;
        }
        ///<summary>
        ///Combines two numbers for computation
        /// E.g if want to get RRTT,pass in the row and tier value and it will combine to give RRTT
        ///</summary>
        public int combineNumber(int no1, int no2)
        {
            return Int32.Parse(string.Format("{0:#.00}", no1 / 100.0).Substring(1) + string.Format("{0:#.00}", no2 / 100.0).Substring(1));


        }
        ///<summary>
        ///This function will find out which hatch position the container belongs to
        ///rowNo - row number of the container
        ///hatchPattern - hatch pattern of the bay. 
        ///6,4,4,6] - There are 4 hatch for the bay, 1st hatch covers 6 containers, 2nd hatch covers the following 4 containers and so on
        ///</summary>
        public int getHatchPosition(int rowNo, int[] hatchPattern)
        {
            int HatchIndex = 0, hatchCoverage = 0, rowsProcessed = 0;
            //if rowNo is even (port side)
            if ((rowNo % 2) == 0)
            {
                //if number of hatch is odd number
                if ((hatchPattern.Length % 2) == 1)
                {
                    //the middle hatch will be the middle index of the hatch pattern array
                    HatchIndex = (hatchPattern.Length / 2);
                    //using the hatch index to find out the number of rows covered by the hatch
                    hatchCoverage = hatchPattern[HatchIndex] / 2;
                }
                else
                {
                    //if there are even number of hatches
                    //the (middle hatch index - 1) will be the center hatch covering the port side containers 
                    HatchIndex = (hatchPattern.Length / 2) - 1;
                    hatchCoverage = hatchPattern[HatchIndex];
                }
                // first rowNo in port side is row 2
                rowsProcessed = 2;
                for (int i = HatchIndex; i >= 0; i--)
                {
                    if (i != HatchIndex)
                    {
                        hatchCoverage = hatchPattern[i];
                    }
                    for (int j = rowsProcessed; j < (rowsProcessed + (hatchCoverage * 2)); j += 2)
                    {
                        if (rowNo == j)
                        {
                            // return hatch number
                            return i + 1;
                        }
                    }
                    rowsProcessed = rowsProcessed + (hatchCoverage * 2);        // to keep track of last processed row number
                }
            }
            // if rowNo is odd (starboard side)
            else
            {
                //if number of hatch is odd number
                if ((hatchPattern.Length % 2) == 1)
                {
                    //the middle hatch will be the middle index of the hatch pattern array
                    HatchIndex = (hatchPattern.Length / 2);
                    //using the hatch index to find out the number of rows covered by the hatch
                    hatchCoverage = (hatchPattern[HatchIndex] / 2) + 1;
                }
                else
                {
                    //if there are even number of hatches
                    //the middle hatch index will be the center hatch covering the starboard side containers 
                    HatchIndex = (hatchPattern.Length / 2);
                    hatchCoverage = hatchPattern[HatchIndex];
                }
                // first rowNo in starboard side is row 1
                rowsProcessed = 1;
                for (int i = HatchIndex; i < hatchPattern.Length; i++)
                {
                    if (i != HatchIndex)
                    {
                        hatchCoverage = hatchPattern[i];
                    }
                    for (int j = rowsProcessed; j < (rowsProcessed + (hatchCoverage * 2)); j += 2)
                    {
                        if (rowNo == j)
                        {
                            // return hatch number
                            return i + 1;
                        }
                    }
                    // to keep track of last processed row number
                    rowsProcessed = rowsProcessed + (hatchCoverage * 2);
                }
            }
            // return -1 == error
            return -1;
        }
        ///<summary>
        ///This function will return the hatchCoverageList containing rows and their respective hatch index
        ///</summary>
        public SortedDictionary<int, int> getHatchCoverage(int[] hatchPattern)
        {
            SortedDictionary<int, int> hatchCoverageList = new SortedDictionary<int, int>();
            int HatchIndex = 0, hatchCoverage = 0, rowsProcessed = 0;
            // if number of hatch is odd number
            if ((hatchPattern.Length % 2) == 1)
            {
                HatchIndex = (hatchPattern.Length / 2);
                hatchCoverage = hatchPattern[HatchIndex] / 2;
            }
            else
            {
                HatchIndex = (hatchPattern.Length / 2) - 1;
                hatchCoverage = hatchPattern[HatchIndex];
            }
            //first rowNo for the port side is row 2
            rowsProcessed = 2;
            for (int i = HatchIndex; i >= 0; i--)
            {
                if (i != HatchIndex)
                {
                    hatchCoverage = hatchPattern[i];
                }
                for (int j = rowsProcessed; j < (rowsProcessed + (hatchCoverage * 2)); j += 2)
                {
                    //Add the row number and its respective hatch index into the hatchCoverageList
                    hatchCoverageList.Add(j, (i + 1));
                }
                // to keep track of last processed row number
                rowsProcessed = rowsProcessed + (hatchCoverage * 2);
            }

            //once the rows at port side has been calculated
            //it will calculate the rows at starboard side
            if ((hatchPattern.Length % 2) == 1)
            {
                HatchIndex = (hatchPattern.Length / 2);
                hatchCoverage = (hatchPattern[HatchIndex] / 2) + 1;
            }
            else
            {
                HatchIndex = (hatchPattern.Length / 2);
                hatchCoverage = hatchPattern[HatchIndex];
            }
            // first rowNo for the starboard side is row 1
            rowsProcessed = 1;
            for (int i = HatchIndex; i < hatchPattern.Length; i++)
            {
                if (i != HatchIndex)
                {
                    hatchCoverage = hatchPattern[i];
                }
                for (int j = rowsProcessed; j < (rowsProcessed + (hatchCoverage * 2)); j += 2)
                {
                    //Add the row number and its respective hatch index into the hatchCoverageList
                    hatchCoverageList.Add(j, (i + 1));
                }
                // to keep track of last processed row number
                rowsProcessed = rowsProcessed + (hatchCoverage * 2);
            }
            return hatchCoverageList;
        }
    }
}