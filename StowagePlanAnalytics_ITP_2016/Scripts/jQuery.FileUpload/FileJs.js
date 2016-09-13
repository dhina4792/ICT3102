function startLoadingBar() {
    // creates <div class="overlay"></div> and 
    // adds it to the DOM
    var div = document.createElement("div");
    div.className += "overlay";
    document.body.appendChild(div);

    var div_loading = document.createElement("div");
    div_loading.className += "loading-screen";
    div.appendChild(div_loading);

    var div_loading_2 = document.createElement("div");
    div_loading_2.className += "loading-div";
    div_loading.appendChild(div_loading_2);

    var h1_processing = document.createElement("h1");
    h1_processing.innerText = "Processing, Please wait!";
    div_loading_2.appendChild(h1_processing);

    var img = document.createElement("img");
    img.setAttribute("src", "/Content/jQuery.FileUpload/img/loading.gif")
    div_loading_2.appendChild(img);
}