var maximized = false;
var activated = false;
var NightMode;

function init() {
    document.getElementById("close").addEventListener("click", quit);
    document.getElementById("size").addEventListener("click", resize);
    document.getElementById("hide").addEventListener("click", hidden);
    document.getElementById("settings").addEventListener("click", settings);

    NightMode = document.getElementById("night_mode");
    NightMode.addEventListener("click", switch_state);

    document.getElementById("close_popup").addEventListener("click",close_popup);
}

function quit() {
    window.close();
}

function resize() {
    if (maximized == true) {
        document.getElementById("size_img").src = "MaximizeButton.png";
        maximized = false;
    }
    else {
        document.getElementById("size_img").src = "MinimizeButton.png";
        maximized = true;
    }
    window.size();
}

function change() {
    if (maximized == true) {
        document.getElementById("size_img").src = "MaximizeButton.png";
        maximized = false;
    }
    else {
        document.getElementById("size_img").src = "MinimizeButton.png";
        maximized = true;
    }
}

function hidden() {
    window.hide();
}

function settings() {
    document.getElementById("modal_1").style.display = "flex";
} 

function switch_state() {
    if (activated == false) {
        document.getElementById("night_mode_circle").style.animation = "state 0.5s forwards";
        NightMode.style.animation = "color 0.5s forwards";

        activated = true;
    }
    else {
        document.getElementById("night_mode_circle").style.animation = "state_backwards 0.5s forwards";
        NightMode.style.animation = "color_backwards 0.5s forwards";

        activated = false;
    }
}

function close_popup() {
    document.getElementById("modal_1").style.display = "none";
}

window.addEventListener("DOMContentLoaded", init);
