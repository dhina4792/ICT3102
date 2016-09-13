//*Data Analysis Diagram Functions*//
//Default run when document loaded
$(function () {
    $(".progress-bar").each(function () {
        each_bar_width = $(this).attr('aria-valuenow');
        $(this).width(each_bar_width + '%');
    });

    // Panel Event Listener
    $('#accordion').on('shown.bs.collapse', function (event) {
        var charts = $('#' + event.target.id).data('charts');
        //Check whether the list of diagram is being populated by checking with one of the diagram div
        if ($('#' + charts[1]).children().length == 0) {
            plotCI(charts);
            plotIMO(charts);
            plotOOG(charts);
            plotReefer(charts);
            plotRestow(charts);
        }
    });


    //asdasd
    var plot;
    var charts = $('#' + event.target.id).data('charts');

});

//CraneIntensity
function plotCI(charts) {
    var div = document.getElementById(charts[0]); //Get the Div which wrap all the span elements
    var spans = div.getElementsByTagName("span"); //Find the list of spans in the specific div
    var ps = div.getElementsByTagName("p");
    CIPlanned = parseFloat(spans[2].innerText); // [4 for ballast, 6 for restow moves, 8 reefers, 10 imo, 12 oil]
    ciAgreed = parseFloat(ps[0].innerText);


    s1 = [CIPlanned, ciAgreed];
    // Can specify a custom tick Array.
    // Ticks should match up one for each y value (category) in the series.
    var ticks = ['Planned CI', 'Actual CI'];

    var plot1 = $.jqplot(charts[1], [s1], {
        title: 'Crane Intensity', // Set Title
        height: 200, //previous 200
        width: 200,
        animate: !$.jqplot.use_excanvas,
        seriesColors: ['#64B569', '#D14E48'], // Set SeriesColour of the Bar by array form
        //gridPadding: { top: 0, bottom: 0, left: 0, right: 0 }, // Grid Padding
        //Set Grid Setting
        grid: {
            gridLineWidth: 1,
            gridLineColor: 'black',
            borderWidth: 1,
            drawBorder: true,
            drawGridlines: true,
            background: "transparent",
            shadow: true
        },
        // The "seriesDefaults" option is an options object that will
        // be applied to all series in the chart.
        seriesDefaults: {
            renderer: $.jqplot.BarRenderer,
            pointLabels: {
                show: true
            },
            formatString: "%#.2f",
            rendererOptions: {
                fillToZero: true,
                varyBarColor: true, //Set true to color each bar of a series
                barWidth: 40 //Set the width of the bar size
            }
        },
        axes: {
            // Use a category axis on the x axis and use our custom ticks.
            xaxis: {
                renderer: $.jqplot.CategoryAxisRenderer,
                numberTicks: 2,
                ticks: ticks,
                tickOptions: {
                    textColor: '#000000' //Set X Axis Color
                }
            },
            // Pad the y axis just a little so bars can get close to, but
            // not touch, the grid boundaries.  1.2 is the default padding.
            yaxis: {
                ticks: ['0.00', '1.00', '2.00', '3.00', '4.00', '5.00', '6.00','7.00','8.00'], //Setting of Y Axis ticks
                min: '0', //Set start value
                max: '5', //Set end value
                pad: 1.05,
                tickOptions: {
                    textColor: '#000000', //Set Y Axis Color
                    formatString: '%#.2f' //Set Y Axis value Format
                }
            }
        }
    });

    resizePlot(plot1);

    $(window).bind('resize', function (event, ui) {
        resizePlot(plot1);
    });
}

//IMO
function plotIMO(charts) {
    var div = document.getElementById(charts[0]); //Get the Div which wrap all the span elements
    var spans = div.getElementsByTagName("span"); //Find the list of spans in the specific div
    var IMOUtilisation = parseFloat(spans[12].innerText); // [4 for ballast, 6 for restow moves, 8 reefers, 10 imo, 12 oil]

    var plot1 = $.jqplot(charts[3], [[['Unutilised', 100 - IMOUtilisation], ['Utilised', IMOUtilisation]]], {
        title: 'IMO', // Set Title
        height: 200, //previous 200
        width: 200,
        seriesColors: ['#64B569', '#D14E48'], // Set SeriesColour of the Bar by array form
        grid: {
            drawBorder: false,
            drawGridlines: false,
            background: "transparent" ,
            shadow: false
        },
        highlighter: {
            show: true,
            showTooltip: true,
            formatString: '%s',
            tooltipLocation: 'ne',
            useAxesFormatters: false
        },
        seriesDefaults: {
            renderer: $.jqplot.PieRenderer,
            trendline: { show: false },
            formatString: "%#.2f",
            rendererOptions: {
                dataLabelThreshold: 100,
                padding: 8,
                dataLabelFormatString: '%.2f%%',
                showDataLabels: true
            }
        }
    });

    resizePlot(plot1);

    $(window).bind('resize', function (event, ui) {
        resizePlot(plot1);
    });
}

//OOG
function plotOOG(charts) {
    var div = document.getElementById(charts[6]); //Get the Div which wrap all the span elements
    var tgg = div.getElementsByTagName("td"); //Find the list of spans in the specific div
    var OOGUtilisation = parseFloat(tgg[18].innerText); // [4 for ballast, 6 for restow moves, 8 reefers, 10 imo, 12 oil]

    var plot1 = $.jqplot(charts[4], [[['Unutilised', 100 - OOGUtilisation], ['Utilised', OOGUtilisation]]], {
        title: 'OOG', // Set Title
        height: 200, //previous 200
        width: 200,
        seriesColors: ['#64B569', '#D14E48'], // Set SeriesColour of the Bar by array form
        grid: {
            drawBorder: false,
            drawGridlines: false,
            background: "transparent",
            shadow: false
        },
        highlighter: {
            show: true,
            showTooltip: true,
            formatString: '%s',
            tooltipLocation: 'ne',
            useAxesFormatters: false
        },
        seriesDefaults: {
            renderer: $.jqplot.PieRenderer,
            trendline: { show: false },
            formatString: "%#.2f",
            rendererOptions: {
                dataLabelThreshold: 100,
                padding: 8,
                dataLabelFormatString: '%.2f%%',
                showDataLabels: true
            }
        }
    });

    resizePlot(plot1);

    $(window).bind('resize', function (event, ui) {
        resizePlot(plot1);
    });
}

//Reefer
function plotRestow(charts) {
    var div = document.getElementById(charts[0]); //Get the Div which wrap all the span elements
    var spans = div.getElementsByTagName("span"); //Find the list of spans in the specific div
    var RestowPercentage = parseFloat(spans[8].innerText); // [4 for ballast, 6 for restow moves, 8 reefers, 10 imo, 12 oil]
    var plot1 = $.jqplot(charts[2], [[['Unutilised', 100 - RestowPercentage], ['Utilised', RestowPercentage]]], {
        title: 'Restow', // Set Title
        height: 200, //previous 200
        width: 200,
        seriesColors: ['#64B569', '#D14E48'], // Set SeriesColour of the Bar by array form
        grid: {
            drawBorder: false,
            drawGridlines: false,
            background: "transparent",
            shadow: false
        },
        highlighter: {
            show: true,
            showTooltip: true,
            formatString: '%s',
            tooltipLocation: 'ne',
            useAxesFormatters: false
        },
        seriesDefaults: {
            renderer: $.jqplot.PieRenderer,
            trendline: { show: false },
            formatString: "%#.2f",
            rendererOptions: {
                dataLabelThreshold: 100,
                padding: 8,
                dataLabelFormatString: '%.2f%%',
                showDataLabels: true
            }
        }
    });

    resizePlot(plot1);

    $(window).bind('resize', function (event, ui) {
        resizePlot(plot1);
    });
}

//Reefer
function plotReefer(charts) {
    var div = document.getElementById(charts[0]); //Get the Div which wrap all the span elements
    var spans = div.getElementsByTagName("span"); //Find the list of spans in the specific div
    var ReefersUtilisation = parseFloat(spans[10].innerText); // [4 for ballast, 6 for restow moves, 8 reefers, 10 imo, 12 oil]
    var plot1 = $.jqplot(charts[5], [[['Unutilised', 100 - ReefersUtilisation], ['Utilised', ReefersUtilisation]]], {
        title: 'Reefer', // Set Title
        height: 200, //previous 200
        width: 200,
        seriesColors: ['#64B569', '#D14E48'], // Set SeriesColour of the Bar by array form
        grid: {
            drawBorder: false,
            drawGridlines: false,
            background: "transparent",
            shadow: false
        },
        highlighter: {
            show: true,
            showTooltip: true,
            formatString: '%s',
            tooltipLocation: 'ne',
            useAxesFormatters: false
        },
        seriesDefaults: {
            renderer: $.jqplot.PieRenderer,
            trendline: { show: false },
            formatString: "%#.2f",
            rendererOptions: {
                dataLabelThreshold: 100,
                padding: 8,
                dataLabelFormatString: '%.2f%%',
                showDataLabels: true
            }
        }
    });

        resizePlot(plot1);

    $(window).bind('resize', function (event, ui) {
        resizePlot(plot1);
    });
}

function resizePlot($plot) {

    var timer;
    clearTimeout(timer);
    timer = setTimeout(function () {
        $($plot.targetId).height($(window).height() * 0.35);
        $plot.replot({ resetAxes: true});
    }, 100);

}