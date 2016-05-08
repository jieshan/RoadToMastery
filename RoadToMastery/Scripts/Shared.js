var imgEndpoint = "http://ddragon.leagueoflegends.com/cdn/6.9.1/img/champion/";

var iconsContainerdiv = document.createElement("div");
document.getElementById("championSelector").appendChild(iconsContainerdiv);

for(var i = 0; i < alphabeticalIds.length; i++) {
    var champDiv = document.createElement("a");
    champDiv.className = "champNavBar";
    var champImg = document.createElement("img");
    champImg.className = "champNavBarIcon";
    champImg.src = imgEndpoint + images[alphabeticalIds[i].toString()];
    champDiv.appendChild(champImg);
    champDiv.title = names[alphabeticalIds[i].toString()];
    champDiv.href = "/Home/Champion/" + alphabeticalIds[i];
    iconsContainerdiv.appendChild(champDiv);
}