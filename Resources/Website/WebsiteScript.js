var maximized = false;
var activated = false;
var NightMode;

const Messages = {
    HIDE: 2,
    CLOSE: 3,
}

function init() {
    document.getElementById("close").addEventListener("click", quit);
    document.getElementById("size").addEventListener("click", size);
    document.getElementById("hide").addEventListener("click", hide);
    document.getElementById("settings").addEventListener("click", settings);

    window.onkeydown = function (e) {
        if (e.key === "Escape")
        CefSharp.PostMessage(Messages.CLOSE);
    }

    NightMode = document.getElementById("night_mode");
    NightMode.addEventListener("click", switch_state);

    document.getElementById("close_popup").addEventListener("click",close_popup);
}


function quit() {
    window.close();
}

function size() {
    if (maximized == true) {
        document.getElementById("size_img").src = "MaximizeButton.png";
        maximized = false;
    }
    else {
        document.getElementById("size_img").src = "MinimizeButton.png";
        maximized = true;
    }
    CefSharp.PostMessage(maximized ? 1 : 0);
}

function hide() {
    CefSharp.PostMessage(Messages.HIDE);
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
