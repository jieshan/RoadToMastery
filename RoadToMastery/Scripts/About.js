function bootstrap() {
    var arrows = document.getElementsByClassName("expandAboutText");
    for (var i = 0; i < arrows.length; i++) {
        var index = (i + 1).toString();
        arrows[i].onclick = function (param) { toggleAnswerDisplay(param); }
    }
}

function toggleAnswerDisplay(param) {
    var index = param.currentTarget.id.replace("expandAboutText", "");
    var answerDiv = document.getElementById("answer" + index);
    answerDiv.style.display = answerDiv.style.display === "" ? "inline" : "";
    var arrowDiv = document.getElementById("expandAboutText" + index);
    arrowDiv.className = "expandAboutText glyphicon glyphicon-chevron-" + (arrowDiv.className.indexOf("-down") >= 0 ? "up" : "down");
}

bootstrap();