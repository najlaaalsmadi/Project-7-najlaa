const url10 = `http://localhost:42480/api/Books`;

async function getbook() {
  try {
    var responsesale = await fetch(url10);
    console.log(responsesale);

    if (!responsesale.ok) {
      throw new Error(`HTTP error! status: ${responsesale.status}`);
    }

    var resultsale = await responsesale.json();
    console.log(resultsale);

    var card = document.getElementById("card");

    if (resultsale.length === 0) {
      card.innerHTML = "<p>No books available at the moment.</p>";
      return;
    }

    // Generate and display cards for each book
    card.innerHTML = resultsale
      .map(
        (book) => `
      <div class="swiper-slide">
        <div class="books-card style-1 wow fadeInUp" data-wow-delay="0.2s">
          <div class="dz-media">
               <img src="${book.imageUrl || "default-image.jpg"}" alt="${
          book.title || "No title available"
        }" />
          </div>
          <div class="dz-content">
            <h4 class="title">${book.title || "Untitled"}</h4>
            <span class="price">${
              book.price ? `$${book.price}` : "Price not available"
            }</span>
            <a href="shop-cart.html" class="btn btn-secondary btnhover btnhover2">
              <i class="flaticon-shopping-cart-1 m-r10"></i> Add to cart
            </a>
          </div>
        </div>
      </div>
    `
      )
      .join(""); // Join the array into a single HTML string

    // Initialize Swiper after loading the books
    new Swiper(".swiper-container.swiper-two", {
      slidesPerView: 1,
      spaceBetween: 10,
      navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
      },
      pagination: {
        el: ".swiper-pagination-two",
        clickable: true,
      },
      breakpoints: {
        640: { slidesPerView: 2, spaceBetween: 20 },
        768: { slidesPerView: 3, spaceBetween: 30 },
        1024: { slidesPerView: 4, spaceBetween: 40 },
      },
    });
  } catch (error) {
    console.error("Error fetching books:", error);
    card.innerHTML = "<p>Failed to load books. Please try again later.</p>";
  }
}

getbook();
