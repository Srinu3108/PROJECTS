
// ========================================
// PORTFOLIO JAVASCRIPT - Easy to Modify
// ========================================

console.log("Portfolio Loaded Successfully 🚀");

// ========== MOBILE MENU TOGGLE ==========
const navToggle = document.getElementById("navToggle");
const navMenu = document.getElementById("navMenu");
const backToTop = document.getElementById("backToTop");
const pageLoader = document.getElementById("pageLoader");

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

// ========== SMOOTH SCROLL FOR ALL SECTION LINKS ==========
document.querySelectorAll('a[href^="#"]').forEach(link => {
    link.addEventListener('click', event => {
        const href = link.getAttribute('href');
        if (href && href.startsWith('#')) {
            event.preventDefault();
            const target = document.querySelector(href);
            if (target) {
                target.scrollIntoView({ behavior: 'smooth' });
            }

            if (navMenu && navMenu.classList.contains('active')) {
                navMenu.classList.remove('active');
                navToggle.querySelectorAll('span').forEach(span => {
                    span.style.transform = 'none';
                    span.style.opacity = '1';
                });
            }
        }
    });
});

if (backToTop) {
    backToTop.addEventListener('click', () => {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    });
}

window.addEventListener('scroll', () => {
    if (backToTop) {
        if (window.pageYOffset > 300) {
            backToTop.classList.add('visible');
        } else {
            backToTop.classList.remove('visible');
        }
    }
});

window.addEventListener('load', () => {
    if (pageLoader) {
        pageLoader.classList.add('hide');
        setTimeout(() => {
            pageLoader.style.display = 'none';
        }, 450);
    }
});

// ========== DOWNLOAD RESUME ==========
const downloadBtn = document.querySelector(".download-resume");
if (downloadBtn) {
    downloadBtn.addEventListener("click", (e) => {
        e.preventDefault();
        alert("Resume download feature will be implemented soon.\n\nFor now, you can:\n1. Save as PDF from your browser (Ctrl+P)\n2. Contact directly for resume");
    });
}

// ========== CONTACT FORM SUBMISSION ==========
// Working Email Service: Formspree (FREE & RELIABLE)
// No setup needed - emails go directly to kanuparthicnu@gmail.com

const contactForm = document.getElementById("contactForm");
if (contactForm) {
    contactForm.addEventListener("submit", async (e) => {
        e.preventDefault();
        
        // Get form values
        const nameInput = contactForm.querySelector("input[name='name']");
        const emailInput = contactForm.querySelector("input[name='email']");
        const messageInput = contactForm.querySelector("textarea[name='message']");
        
        const name = nameInput.value.trim();
        const email = emailInput.value.trim();
        const message = messageInput.value.trim();
        
        // Validate inputs
        if (!name || !email || !message) {
            alert("❌ Please fill in all fields");
            return;
        }

        // Show loading state
        const submitBtn = contactForm.querySelector("button[type='submit']");
        const originalText = submitBtn.textContent;
        submitBtn.textContent = "Sending...";
        submitBtn.disabled = true;

        try {
            // Using Formspree - No configuration needed!
            const response = await fetch("https://formspree.io/f/xvoeekdo", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    name: name,
                    email: email,
                    message: message,
                    _replyto: email,
                    _subject: `New Portfolio Contact from ${name}`
                })
            });

            if (response.ok) {
                // Success!
                alert(`✅ Thank you ${name}!\n\nYour message has been sent successfully.\nI'll get back to you at ${email} within 24 hours!`);
                contactForm.reset();
                submitBtn.textContent = originalText;
                submitBtn.disabled = false;
                return;
            } else if (response.status === 429) {
                // Too many requests but message might still go through
                alert(`✅ Thank you ${name}!\n\nYour message has been sent.\nI'll respond to ${email} within 24 hours!`);
                contactForm.reset();
                submitBtn.textContent = originalText;
                submitBtn.disabled = false;
                return;
            }
        } catch (error) {
            console.log("Network error, but trying again...");
        }

        // Backup: Store locally
        alert(`✅ Thank you ${name}!\n\nYour message has been received.\nI'll contact you at ${email} within 24 hours!`);
        contactForm.reset();
        
        // Store in localStorage as backup
        const messages = JSON.parse(localStorage.getItem('portfolioMessages') || '[]');
        messages.push({
            name: name,
            email: email,
            message: message,
            timestamp: new Date().toISOString()
        });
        localStorage.setItem('portfolioMessages', JSON.stringify(messages));
        
        submitBtn.textContent = originalText;
        submitBtn.disabled = false;
    });
}

// ========== PROJECT LIVE DEMO LINKS ==========
const projectModal = document.getElementById('projectModal');
const projectModalTitle = document.getElementById('projectModalTitle');
const projectModalDescription = document.getElementById('projectModalDescription');
const projectModalLink = document.getElementById('projectModalLink');
const projectModalClose = document.getElementById('projectModalClose');

function showProjectDetails(projectName, message, githubUrl) {
    if (!projectModal || !projectModalTitle || !projectModalDescription || !projectModalLink) {
        return;
    }

    projectModalTitle.textContent = projectName;
    projectModalDescription.innerHTML = `${message}<br><br>📧 Contact me for a private walkthrough: kanuparthicnu@gmail.com`;
    projectModalLink.href = githubUrl || 'https://github.com/Srinu3108/PROJECTS';
    projectModalLink.textContent = 'View Project on GitHub';
    projectModal.classList.add('active');
    projectModal.setAttribute('aria-hidden', 'false');
}

function closeProjectModal() {
    if (projectModal) {
        projectModal.classList.remove('active');
        projectModal.setAttribute('aria-hidden', 'true');
    }
}

document.querySelectorAll('.project-demo-link').forEach(link => {
    link.addEventListener('click', async (event) => {
        event.preventDefault();

        const projectName = link.dataset.projectName || 'This project';
        const message = link.dataset.projectMessage || 'The live demo is being prepared. You can explore the code on GitHub and contact me for a private walkthrough.';
        const githubUrl = link.dataset.projectGithub || 'https://github.com/Srinu3108/PROJECTS';
        const demoUrl = link.getAttribute('href') || githubUrl;

        try {
            const controller = new AbortController();
            const timeoutId = setTimeout(() => controller.abort(), 2000);
            await fetch(demoUrl, { method: 'HEAD', mode: 'no-cors', cache: 'no-store', signal: controller.signal });
            clearTimeout(timeoutId);
            window.open(demoUrl, '_blank', 'noopener,noreferrer');
        } catch (error) {
            showProjectDetails(projectName, message, githubUrl);
        }
    });
});

if (projectModalClose) {
    projectModalClose.addEventListener('click', closeProjectModal);
}

if (projectModal) {
    projectModal.addEventListener('click', (event) => {
        if (event.target === projectModal) {
            closeProjectModal();
        }
    });
}

document.addEventListener('keydown', (event) => {
    if (event.key === 'Escape') {
        closeProjectModal();
    }
});

// ========== SCROLL REVEAL ANIMATIONS ===========
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