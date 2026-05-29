
// ========================================
// PORTFOLIO JAVASCRIPT - Easy to Modify
// ========================================

console.log("Portfolio Loaded Successfully 🚀");

// ========== MOBILE MENU TOGGLE ==========
const navToggle = document.getElementById("navToggle");
const navMenu = document.getElementById("navMenu");

if (navToggle) {
    navToggle.addEventListener("click", () => {
        navMenu.classList.toggle("active");
        
        // Animate hamburger
        const spans = navToggle.querySelectorAll("span");
        spans[0].style.transform = navMenu.classList.contains("active") ? "rotate(45deg) translate(10px, 10px)" : "none";
        spans[1].style.opacity = navMenu.classList.contains("active") ? "0" : "1";
        spans[2].style.transform = navMenu.classList.contains("active") ? "rotate(-45deg) translate(7px, -7px)" : "none";
    });

    // Close menu when link is clicked
    const navLinks = navMenu.querySelectorAll(".nav-link");
    navLinks.forEach(link => {
        link.addEventListener("click", () => {
            navMenu.classList.remove("active");
            navToggle.querySelectorAll("span").forEach(span => {
                span.style.transform = "none";
                span.style.opacity = "1";
            });
        });
    });
}

// ========== SMOOTH SCROLL BUTTON ==========
const viewProjectsBtn = document.querySelector("button");
if (viewProjectsBtn && !viewProjectsBtn.classList.contains("btn-secondary")) {
    viewProjectsBtn.addEventListener("click", () => {
        document.getElementById("projects").scrollIntoView({
            behavior: "smooth"
        });
    });
}

// ========== DOWNLOAD RESUME ==========
const downloadBtn = document.querySelector(".download-resume");
if (downloadBtn) {
    downloadBtn.addEventListener("click", (e) => {
        e.preventDefault();
        alert("Resume download feature will be implemented soon.\n\nFor now, you can:\n1. Save as PDF from your browser (Ctrl+P)\n2. Contact directly for resume");
    });
}

// ========== CONTACT FORM SUBMISSION ==========
const contactForm = document.getElementById("contactForm");
if (contactForm) {
    contactForm.addEventListener("submit", (e) => {
        e.preventDefault();
        
        // Get form values
        const name = contactForm.querySelector("input[type='text']").value;
        const email = contactForm.querySelector("input[type='email']").value;
        const message = contactForm.querySelector("textarea").value;
        
        // Show success message
        if (name && email && message) {
            alert(`Thank you ${name}!\n\nYour message has been received.\nI'll get back to you at ${email} soon!`);
            
            // Reset form
            contactForm.reset();
            
            // You can integrate with email service here (e.g., FormSubmit, EmailJS)
            // Example using FormSubmit (free service):
            // const formData = new FormData(contactForm);
            // fetch('https://formsubmit.co/YOUR_EMAIL@gmail.com', {
            //     method: 'POST',
            //     body: formData
            // });
        }
    });
}

// ========== SCROLL REVEAL ANIMATIONS ==========
const observerOptions = {
    threshold: 0.1,
    rootMargin: '0px 0px -100px 0px'
};

const observer = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.style.opacity = "1";
            entry.target.style.transform = "translateY(0)";
        }
    });
}, observerOptions);

// Animate cards on scroll
document.addEventListener("DOMContentLoaded", () => {
    const cards = document.querySelectorAll(".service-card, .project-card, .education-card, .testimonial-card");
    cards.forEach(card => {
        card.style.opacity = "0";
        card.style.transform = "translateY(20px)";
        card.style.transition = "opacity 0.6s ease, transform 0.6s ease";
        observer.observe(card);
    });
});

// ========== ACTIVE NAV LINK HIGHLIGHTING ==========
const sections = document.querySelectorAll("section");
const navLinks = document.querySelectorAll(".nav-link");

window.addEventListener("scroll", () => {
    let current = "";
    
    sections.forEach(section => {
        const sectionTop = section.offsetTop;
        if (pageYOffset >= sectionTop - 60) {
            current = section.getAttribute("id");
        }
    });

    navLinks.forEach(link => {
        link.classList.remove("active");
        if (link.getAttribute("href").slice(1) === current) {
            link.style.color = "var(--primary-color)";
            link.style.backgroundColor = "rgba(88, 166, 255, 0.1)";
        } else {
            link.style.color = "var(--text-light)";
            link.style.backgroundColor = "transparent";
        }
    });
});

// ========== TYPING EFFECT (Optional - for hero subtitle) ==========
// Uncomment to enable typing effect on hero section
/*
const heroText = document.querySelector(".hero-text h3");
const originalText = heroText.textContent;
heroText.textContent = "";

let charIndex = 0;
const typeEffect = setInterval(() => {
    if (charIndex < originalText.length) {
        heroText.textContent += originalText.charAt(charIndex);
        charIndex++;
    } else {
        clearInterval(typeEffect);
    }
}, 50);
*/

// ========== UTILITY FUNCTIONS ==========

// Smooth scroll to any element
function scrollToElement(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({ behavior: 'smooth' });
    }
}

// Add to global scope
window.scrollToElement = scrollToElement;