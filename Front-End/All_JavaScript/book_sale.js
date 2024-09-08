const url100 = `http://localhost:42480/api/Books/bySaleBooks`;
const urlCategory = `http://localhost:42480/api/Category/byIDCategory/`;

async function getCategoryById(categoryId) {
  try {
    const response = await fetch(urlCategory + categoryId);

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const result = await response.json();
    return result.categoryName || "Unknown"; // نعيد اسم الفئة أو "غير معروف" إذا لم يتوفر الاسم
  } catch (error) {
    console.error("Error fetching category:", error);
    return "Unknown"; // في حالة حدوث خطأ، نعيد "غير معروف"
  }
}

async function getBooksOnSale() {
  try {
    const responsesale = await fetch(url100);
    console.log(responsesale);

    if (!responsesale.ok) {
      throw new Error(`HTTP error! status: ${responsesale.status}`);
    }

    const resultsale = await responsesale.json();
    console.log(resultsale);

    const card = document.getElementById("saleprodect");

    if (resultsale.length === 0) {
      card.innerHTML = "<p>No books available on sale.</p>";
      return;
    }

    // Filter and generate cards for books with discounts
    const booksHTML = await Promise.all(
      resultsale
        .filter((book) => parseFloat(book.discountPercentage) > 0) // Filter books with discount
        .map(async (book) => {
          // Calculate discounted price
          const price = parseFloat(book.price) || 0;
          const discount = parseFloat(book.discountPercentage) || 0;
          const discountedPrice = price - (price * discount) / 100;

          // جلب اسم الفئة بناءً على المعرف
          const categoryName = await getCategoryById(book.categoryId);

          // Generate HTML for the book
          return `
            <div class="swiper-slide">
              <div class="books-card style-1 wow fadeInUp" data-wow-delay="0.2s" style="border: 1px solid #ddd; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 8px rgba(0,0,0,0.1);">
                <div class="dz-media" style="position: relative;">
                  <img src="${book.imageUrl || "default-image.jpg"}" alt="${
            book.title || "No title available"
          }" style="width: 100%; height: auto; border-bottom: 1px solid #ddd;" />
                  <span style="position: absolute; top: 10px; right: 10px; background-color: #ff4d4d; color: white; padding: 5px 10px; border-radius: 5px;">${discount}% Off</span>
                </div>
                <div class="dz-content" style="padding: 15px;">
                  <h4 class="title" style="font-size: 1.2rem; font-weight: bold; margin-bottom: 10px;">${
                    book.title || "Untitled"
                  }</h4>
                  <p class="description" style="font-size: 0.9rem; color: #555;">${
                    book.description || "No description available"
                  }</p>
                  <p class="category" style="font-size: 0.9rem; color: #888; margin-bottom: 10px;"><strong>Category:</strong> ${categoryName}</p>
                  <div class="price" style="font-size: 1rem; color: #000; display: flex; justify-content: space-between; align-items: center;">
                    <span class="price-num discounted" style="font-size: 1.5rem; color: #888; font-weight: bold;">$${discountedPrice.toFixed(
                      2
                    )}</span> 
                    <span class="price-num original" style="font-size: 1rem; text-decoration: line-through; color: #ff4d4d;">$${price.toFixed(
                      2
                    )}</span>
                  </div>
                  <a href="shop-cart.html" class="btn btn-secondary btnhover btnhover2" style="margin-top: 15px; color: white; display: block; text-align: center; padding: 10px; border-radius: 5px;">
                    <i class="flaticon-shopping-cart-1 m-r10"></i> Add to cart
                  </a>
                </div>
              </div>
            </div>
          `;
        })
    );

    card.innerHTML = booksHTML.join(""); // Join the array into a single HTML string

    // Initialize Swiper after loading the books
    // new Swiper(".swiper-container.swiper-two", {
    //   slidesPerView: 1,
    //   spaceBetween: 10,
    //   navigation: {
    //     nextEl: ".swiper-button-next",
    //     prevEl: ".swiper-button-prev",
    //   },
    //   pagination: {
    //     el: ".swiper-pagination-two",
    //     clickable: true,
    //   },
    //   breakpoints: {
    //     640: { slidesPerView: 2, spaceBetween: 20 },
    //     768: { slidesPerView: 3, spaceBetween: 30 },
    //     1024: { slidesPerView: 4, spaceBetween: 40 },
    //   },
    // });
  } catch (error) {
    console.error("Error fetching books:", error);
    const card = document.getElementById("saleprodect");
    card.innerHTML = "<p>Failed to load books. Please try again later.</p>";
  }
}

getBooksOnSale();
