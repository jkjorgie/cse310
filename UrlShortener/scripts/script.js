let usageChart = null;

function addRow() {
  toggleNewUrlInputDiv();
}

function shortenURL() {

  //add new url to table
  addShortenedUrlToTable(
    document.getElementById("linkName").value,
    document.getElementById("originalUrl").value,
    generateShortUrlKey(document.getElementById("originalUrl").value, 0),
    0,
    true
  );

  //reset fields after url is shortened
  document.getElementById("linkName").value = "";
  document.getElementById("originalUrl").value = "";
  toggleNewUrlInputDiv();
}

function addShortenedUrlToTable(name, origUrl, shortUrl, uses, active) {
  const table = document.querySelector(".url-table");
  const tBody = table.getElementsByTagName("tbody")[0];

  const row = document.createElement("tr");

  //create name cell
  const cellName = document.createElement("td");
  cellName.textContent = name;
  row.appendChild(cellName);

  //create original url cell
  const cellOrigUrl = document.createElement("td");
  const aTag = document.createElement("a");
  aTag.href = origUrl;
  aTag.textContent = origUrl;
  cellOrigUrl.appendChild(aTag);
  row.appendChild(cellOrigUrl);

  //create short url cell
  const cellShortUrl = document.createElement("td");
  const shortLink = document.createElement("a");
  shortLink.href = `http://127.0.0.1:5500/UrlShortener/index.html?red=${shortUrl}`;
  shortLink.textContent = shortUrl;
  shortLink.target = "_blank"; // Opens in a new tab
  cellShortUrl.appendChild(shortLink);
  row.appendChild(cellShortUrl);

  //create uses cell
  const cellUses = document.createElement("td");
  cellUses.textContent = uses;
  cellUses.classList.add("uses");
  row.appendChild(cellUses);

  //create active checkbox cell
  const cellActive = document.createElement("td");
  cellActive.classList.add("active-cb");
  const checkbox = document.createElement("input");
  checkbox.type = "checkbox";
  checkbox.checked = active;
  cellActive.appendChild(checkbox);
  row.appendChild(cellActive);

  //create delete button
  const cellDelete = document.createElement("td");
  const deleteButton = document.createElement("button");
  deleteButton.textContent = "Delete";
  deleteButton.classList.add("delete-row");
  deleteButton.onclick = function () {
    row.remove();
    saveUrlsToLocalStorage();
    renderUsageChart();
  };
  cellDelete.appendChild(deleteButton);
  row.appendChild(cellDelete);

  tBody.appendChild(row);

  saveUrlsToLocalStorage();
  renderUsageChart();
}

function toggleNewUrlInputDiv() {
  const newUrlDiv = document.querySelector(".new-url-input");

  if (newUrlDiv.classList.contains("flex")) {
    newUrlDiv.classList.remove("flex");
    newUrlDiv.classList.add("none");
  } else {
    newUrlDiv.classList.remove("none");
    newUrlDiv.classList.add("flex");
  }
}

function generateShortUrlKey(url, count) {
  
  //generate hash
  let hash = 0;
  const input = url + count;
  for (let i = 0; i < input.length; i++) {
    const char = input.charCodeAt(i);
    hash = (hash << 5) - hash + char;
    hash |= 0; // Convert to 32-bit integer
  }

  const shortKey = `${Math.abs(hash).toString(36)}${count}`;

  // Check if the key already exists in the table
  const existingKeys = Array.from(document.querySelectorAll(".url-table tbody tr td:nth-child(3)"))
    .map(td => td.textContent.trim());

  // if dup exists, call this function recursively with a count appended
  if (existingKeys.includes(shortKey)) {
    return generateShortUrlKey(url, count + 1);
  }

  return shortKey;
}

function loadShortenedUrls() {
  try {

    //grab local storage data if it exists
    const storedData = localStorage.getItem("shortenedUrls");
    if (!storedData) {
      console.warn("No shortened URLs found in localStorage.");
      return;
    }
    const data = JSON.parse(storedData);

    //loop through json items to add data to table
    data.forEach((item) => {
      addShortenedUrlToTable(
        item["Link Name"],
        item["Original URL"],
        item["Shortened Key"],
        item["Uses"],
        item["Active"]
      );
    });
  } catch (error) {
    console.error("Error loading from localStorage:", error);
  }
}

function saveUrlsToLocalStorage() {
  const rows = document.querySelectorAll(".url-table tbody tr");
  const data = [];

  //loop through table
  rows.forEach((row) => {
    const cells = row.querySelectorAll("td");

    //set json properties
    const linkName = cells[0].textContent.trim();
    const originalUrl =
      cells[1].querySelector("a")?.href || cells[1].textContent.trim();
    const shortenedKey = cells[2].textContent.trim();
    const uses = parseInt(cells[3].textContent.trim(), 10);
    const active = cells[4].querySelector("input[type=checkbox]")?.checked
      ? true
      : false;

    //add json to obj
    data.push({
      "Link Name": linkName,
      "Original URL": originalUrl,
      "Shortened Key": shortenedKey,
      Uses: uses,
      Active: active,
    });
  });

  //set local storage item
  localStorage.setItem("shortenedUrls", JSON.stringify(data));
}

function handleRedirectByQueryParam() {
  const params = new URLSearchParams(window.location.search);
  const shortKey = params.get("red");

  if (!shortKey) return;

  const storedData = JSON.parse(localStorage.getItem("shortenedUrls")) || [];
  const match = storedData.find(item => item["Shortened Key"] === shortKey);

  if (match) {
    // Increment use count
    match["Uses"] = (parseInt(match["Uses"], 10) || 0) + 1;

    // Save updated data
    localStorage.setItem("shortenedUrls", JSON.stringify(storedData));

    // Redirect to original URL
    window.location.href = match["Original URL"];
  } else {
    console.warn(`No match found for short key: ${shortKey}`);
  }
}

function renderUsageChart() {
  const storedData = JSON.parse(localStorage.getItem("shortenedUrls")) || [];

  const top = storedData
    .sort((a, b) => b.Uses - a.Uses)
    .slice(0, 5);

  const labels = top.map(item => item["Link Name"] || item["Shortened Key"]);
  const uses = top.map(item => item["Uses"]);

  const ctx = document.getElementById("usageChart").getContext("2d");

  // Destroy existing chart if it exists
  if (usageChart) {
    usageChart.destroy();
  }

  usageChart = new Chart(ctx, {
    type: "bar",
    data: {
      labels: labels,
      datasets: [{
        label: "Uses",
        data: uses,
        backgroundColor: "rgba(54, 162, 235, 0.6)",
        borderColor: "rgba(54, 162, 235, 1)",
        borderWidth: 1
      }]
    },
    options: {
      responsive: true,
      scales: {
        y: {
          beginAtZero: true,
          ticks: {
            precision: 0
          }
        }
      }
    }
  });
}

document.addEventListener("DOMContentLoaded", function () {

  //add listener for if the user hits enter after entering the original url
  const originalUrlInput = document.getElementById("originalUrl");
  originalUrlInput.addEventListener("keydown", function (event) {
    if (event.key === "Enter") {
      event.preventDefault();
      shortenURL();
    }
  });

  //load shortened urls from local storage
  loadShortenedUrls();

  //load chart
  renderUsageChart();
});

handleRedirectByQueryParam();