// var userId = localStorage.getItem("userId");

const url4 = `http://localhost:42480/api/User/byIDUser/15`;

async function getuser() {
  var response = await fetch(url4);
  console.log(response);
  var result = await response.json();
  console.log(result);

  var profile = document.getElementById("profile2");

  profile.innerHTML = `
       <h6 class="m-0">${result.name}</h6>
      <span>${result.email}</span>
    `;
}

getuser();

async function getusertwo() {
  var response = await fetch(url4);
  console.log(response);
  var result = await response.json();
  console.log(result);

  var profile1 = document.getElementById("profile1");

  profile1.innerHTML = `
       <h6 class="m-0">${result.name}</h6>
      <span>${result.email}</span>
    `;
}

getusertwo();

async function getusertree() {
  var response = await fetch(url4);
  console.log(response);
  var result = await response.json();
  console.log(result);

  var imgContainer = document.getElementById("img");
  if (imgContainer && result.image) {
    imgContainer.innerHTML = `<img src="${result.image}" alt="img" />`;
  } else {
    console.error("Image container or image data is missing");
  }
}

getusertree();
