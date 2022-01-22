var navMenu = document.querySelector(".navMenuTel");
var btnMenu = document.querySelector(".menuOpener");
btnMenu.addEventListener("click", showMenu);

function showMenu() {
  if (navMenu.style.display === "none") {
    navMenu.style.display = "flex";
  } else {
    navMenu.style.display = "none";
  }
}
