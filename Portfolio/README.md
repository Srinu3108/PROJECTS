# Srinu's Professional Portfolio

A modern, professional portfolio website designed to showcase your work, attract clients, and serve as a resume. Built with clean, maintainable code for easy updates.

## 📁 Project Structure

```
Portfolio/
├── index.html          # Main HTML file (all sections)
├── style.css          # All styling (organized with CSS variables)
├── script.js          # JavaScript functionality
├── data.js            # Easy-to-update configuration file
├── README.md          # This file
├── assets/            # Additional assets
└── images/            # Your project images
```

## 🎨 Key Features

✅ **Professional Design** - Dark theme with modern UI  
✅ **Fully Responsive** - Works perfectly on mobile, tablet, and desktop  
✅ **Easy to Update** - All personal info in `data.js`  
✅ **SEO Optimized** - Meta tags and proper structure  
✅ **Performance** - Fast loading and smooth animations  
✅ **Accessible** - Semantic HTML and keyboard navigation

## 📝 How to Update Your Portfolio

### 1. **Update Personal Information** (EASIEST WAY)

Edit `data.js` and update these fields:

```javascript
personal: {
    name: "Srinivasulu Kanuparthi",
    title: "Your Title",
    email: "kanuparthisnu@gmail.com",
    phone: "+91-7981210743",
    location: "Chennai, Tamil Nadu",
    github: "https://github.com/yourprofile",
    linkedin: "https://linkedin.com/in/yourprofile",
    twitter: "https://twitter.com/yourhandle",
}
```

### 2. **Update Projects**

In `data.js`, modify the projects array:

```javascript
projects: [
  {
    name: "Project Name",
    description: "Brief description of what the project does",
    technologies: ["Tech1", "Tech2", "Tech3"],
    github: "https://github.com/username/project",
    liveDemo: "https://project-live-url.com",
  },
  // Add more projects...
];
```

### 3. **Update Experience**

In `data.js`, update your work history:

```javascript
experience: [
  {
    years: "2023 - Present",
    title: "Your Job Title",
    company: "Company Name",
    highlights: [
      "Achievement 1",
      "Achievement 2",
      // Add more achievements
    ],
  },
  // Add more roles...
];
```

### 4. **Update Education & Certifications**

In `data.js`, update your education:

```javascript
education: [
  {
    icon: "fas fa-graduation-cap", // Change icon if needed
    title: "Your Degree",
    institution: "University - Year",
    details: "Additional details like GPA or specialization",
  },
  // Add more education entries...
];
```

### 5. **Update Skills**

In `data.js`, organize your skills by category:

```javascript
skills: {
    backend: ["Tech1", "Tech2", "Tech3"],
    frontend: ["Tech1", "Tech2"],
    databases: ["Tech1", "Tech2"],
    tools: ["Tech1", "Tech2"]
}
```

### 6. **Update Testimonials**

In `data.js`, add client testimonials:

```javascript
testimonials: [
  {
    text: "What the client said about you",
    author: "Client Name",
    title: "Client Position at Company",
    stars: 5,
  },
];
```

### 7. **Add Profile Image** (Optional)

1. Place your image in the `images/` folder
2. In `index.html`, add this after `<section class="about" id="about">`:

```html
<div class="profile-image">
  <img src="images/profile.jpg" alt="Profile Photo" />
</div>
```

3. Add this CSS to `style.css`:

```css
.profile-image {
  width: 200px;
  height: 200px;
  border-radius: 50%;
  margin: 0 auto 30px;
  border: 3px solid var(--primary-color);
  overflow: hidden;
}

.profile-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
```

### 8. **Customize Colors**

Edit the color variables at the top of `style.css`:

```css
:root {
  --primary-color: #58a6ff; /* Main blue */
  --secondary-color: #238636; /* Green */
  --dark-bg: #0d1117; /* Dark background */
  --dark-secondary: #161b22; /* Slightly lighter background */
  --border-color: #30363d; /* Border color */
  --text-light: #c9d1d9; /* Light text */
  --success-color: #79c0ff; /* Success/hover color */
  --danger-color: #f85149; /* Accent/error color */
}
```

## 🚀 Deployment

### Host on GitHub Pages (Free)

1. Create a GitHub repository named `your-username.github.io`
2. Push your portfolio files to the repository
3. Your site will be live at `https://your-username.github.io`

### Host on Netlify (Free)

1. Go to [netlify.com](https://netlify.com)
2. Click "New site from Git"
3. Connect your GitHub repository
4. Deploy!

### Host on Vercel (Free)

1. Go to [vercel.com](https://vercel.com)
2. Click "Import Project"
3. Import from GitHub
4. Deploy!

## 📧 Enable Email Contact Form

### Option 1: FormSubmit (Easiest)

1. Go to [formsubmit.co](https://formsubmit.co)
2. In `script.js`, uncomment this section (around line 70):

```javascript
// Uncomment these lines:
const formData = new FormData(contactForm);
fetch("https://formsubmit.co/YOUR_EMAIL@gmail.com", {
  method: "POST",
  body: formData,
});
```

3. Replace `YOUR_EMAIL@gmail.com` with your actual email

### Option 2: EmailJS

1. Go to [emailjs.com](https://emailjs.com) and sign up
2. Follow their documentation to set up your email service
3. Integrate with your contact form

## 📱 Responsive Design

The portfolio is fully responsive and includes:

- Mobile menu toggle
- Optimized layouts for all screen sizes
- Touch-friendly navigation
- Fast loading on mobile devices

Test on mobile by pressing `F12` → Toggle device toolbar in your browser.

## ✨ Additional Customizations

### Change Theme to Light Mode

Replace the color variables in `style.css`:

```css
:root {
  --dark-bg: #ffffff;
  --dark-secondary: #f5f5f5;
  --text-light: #333333;
  /* Update other colors as needed */
}
```

### Add Dark/Light Theme Toggle

Add this to `script.js`:

```javascript
const themeToggle = document.createElement("button");
themeToggle.textContent = "🌙";
themeToggle.style.position = "fixed";
themeToggle.style.bottom = "20px";
themeToggle.style.right = "20px";
themeToggle.addEventListener("click", () => {
  document.body.classList.toggle("dark-mode");
});
document.body.appendChild(themeToggle);
```

### Add Animations on Scroll

Already included! Cards animate when they come into view.

## 🔧 Troubleshooting

### Portfolio not loading properly?

- Clear browser cache (Ctrl+Shift+Delete)
- Check browser console for errors (F12)
- Ensure all files are in the correct folder

### Mobile menu not working?

- Make sure `script.js` is loaded after HTML
- Check browser console for JavaScript errors

### Images not showing?

- Ensure image paths are correct in `index.html`
- Check that images are in the `images/` folder
- Use relative paths like `images/project1.jpg`

## 📚 Resources

- [CSS Variables Guide](https://developer.mozilla.org/en-US/docs/Web/CSS/--*)
- [Font Awesome Icons](https://fontawesome.com/icons)
- [Responsive Design Tips](https://web.dev/responsive-web-design-basics/)
- [GitHub Pages Docs](https://pages.github.com/)

## 💡 Tips for Success

1. **Keep it Updated** - Update your portfolio regularly with new projects
2. **Use Real Content** - Replace placeholder text with your actual information
3. **Add Project Links** - Include links to GitHub repos and live demos
4. **Professional Photos** - Use good quality profile and project images
5. **Get Feedback** - Share your portfolio with others and get feedback
6. **SEO Optimization** - Keep titles and descriptions relevant for search engines

## 📄 Portfolio Sections

- **Home** - Eye-catching hero section
- **About** - Your professional summary
- **Services** - What you offer to clients
- **Projects** - Your best work with tech stack
- **Experience** - Work history with timeline
- **Education** - Degrees and certifications
- **Skills** - Technical expertise by category
- **Testimonials** - Client feedback and reviews
- **Contact** - Multiple ways to reach you

## 🎯 Next Steps

1. Update `data.js` with your actual information
2. Add your projects with GitHub links
3. Deploy to GitHub Pages, Netlify, or Vercel
4. Share your portfolio URL with potential clients
5. Keep it updated with new projects!

---

**Built with ❤️ using HTML, CSS & JavaScript**

Last Updated: May 2026
