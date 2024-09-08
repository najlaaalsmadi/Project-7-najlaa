const url = "http://localhost:42480/api/Category";

async function getCategories() {
  const response = await fetch(url, { method: "GET" });

  const result = await response.json();
  console.log(result);

  const container = document.getElementById("Category");

  container.innerHTML = "";

  const selectElement = document.createElement("select");
  selectElement.className = "default-select";

  result.forEach((category) => {
    const option = document.createElement("option");
    option.value = category.id;
    option.textContent = category.name;
    selectElement.appendChild(option);
  });

  container.appendChild(selectElement);
}

getCategories();
