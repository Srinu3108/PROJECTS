# 🚀 QUICK UPDATE GUIDE

## Most Common Updates

### 1. Add a New Project (30 seconds)

Open `data.js` and add to the `projects` array:

```javascript
{
    name: "Project Name",
    description: "What it does",
    technologies: ["Tech1", "Tech2"],
    github: "https://github.com/user/project",
    liveDemo: "https://project-live.com"
},
```

**Done!** Portfolio updates automatically.

---

### 2. Update Contact Info (20 seconds)

In `data.js`, update:

```javascript
email: "newemail@gmail.com",
phone: "+91-7981210743",
github: "https://github.com/yourusername",
linkedin: "https://www.linkedin.com/in/srinivasulu-k-36b862270/",
```

---

### 3. Add New Experience (30 seconds)

In `data.js`, add to `experience` array:

```javascript
{
    years: "2024 - Present",
    title: "Your New Job",
    company: "Company Name",
    highlights: [
        "Achievement 1",
        "Achievement 2",
        "Achievement 3"
    ]
},
```

---

### 4. Change Theme Colors (1 minute)

Open `style.css`, find this section:

```css
:root {
  --primary-color: #58a6ff; /* Change this */
  --secondary-color: #238636; /* And this */
  --success-color: #79c0ff; /* And this */
}
```

Find a nice color:

- [Color Picker](https://htmlcolorcodes.com)
- [Color Palette](https://coolors.co)

---

### 5. Add a Skill (10 seconds)

In `data.js`, find skills section:

```javascript
skills: {
    backend: ["ASP.NET Core", "NEW_SKILL_HERE"],
    frontend: ["HTML5", "CSS3"],
    // ...
}
```

---

### 6. Add Testimonial (30 seconds)

In `data.js`, add to `testimonials`:

```javascript
{
    text: "What they said about you",
    author: "Their Name",
    title: "Position at Company",
    stars: 5
},
```

---

## Common Edits in HTML

### Edit About Me Text

In `index.html`, find the About section:

```html
<section class="about" id="about">
  <div class="about-text">
    <p>Your text here</p>
  </div>
</section>
```

### Change Hero Subtitle

In `index.html`:

```html
<h3>Your new subtitle here</h3>
```

### Update Download Resume Link

In `index.html`:

```html
<a href="your-resume-url-here" class="btn btn-primary download-resume">
  <i class="fas fa-download"></i> Download Resume
</a>
```

---

## File Quick Reference

| File         | What to Edit                                 |
| ------------ | -------------------------------------------- |
| `data.js`    | All your info (email, projects, skills, etc) |
| `index.html` | Section text, hero description, logos        |
| `style.css`  | Colors, fonts, spacing                       |
| `script.js`  | Advanced features, animations                |

---

## Icon Reference

Common icons you can use (from Font Awesome):

```
Code: <i class="fas fa-code"></i>
Database: <i class="fas fa-database"></i>
Mobile: <i class="fas fa-mobile-alt"></i>
Gears: <i class="fas fa-cogs"></i>
Bug: <i class="fas fa-bug"></i>
Graduation: <i class="fas fa-graduation-cap"></i>
GitHub: <i class="fab fa-github"></i>
LinkedIn: <i class="fab fa-linkedin"></i>
Twitter: <i class="fab fa-twitter"></i>
Email: <i class="fas fa-envelope"></i>
Phone: <i class="fas fa-phone"></i>
Location: <i class="fas fa-map-marker-alt"></i>
Download: <i class="fas fa-download"></i>
External Link: <i class="fas fa-external-link-alt"></i>
Star: <i class="fas fa-star"></i>
```

[View all icons →](https://fontawesome.com/icons)

---

## Color Codes

```
Primary Blue: #58a6ff
Success Green: #238636
Light Blue: #79c0ff
Dark Background: #0d1117
Secondary Dark: #161b22
Border: #30363d
Light Text: #c9d1d9
Error Red: #f85149
Gold Stars: #ffd700
```

---

## Simple GitHub Link Format

```
Your Repo: https://github.com/your-username/project-name
Live Site: https://project-name.com OR https://your-username.github.io/project-name
```

---

## Need Help?

### Contact Form Not Working?

→ Set up FormSubmit.co (see README.md)

### Images Not Showing?

→ Ensure path is `images/filename.jpg` and file exists

### Portfolio Looks Broken?

→ Clear cache: Ctrl+Shift+Delete → Clear all time

### Mobile Menu Not Working?

→ Refresh page (Ctrl+F5)

---

## Testing Your Changes

1. Save file
2. Refresh browser (Ctrl+F5)
3. Check desktop view (F12 → Elements)
4. Check mobile view (F12 → Toggle device toolbar)

---

## Before Sharing

✅ Update all placeholder emails and links  
✅ Add real projects with working links  
✅ Add profile photo  
✅ Test on mobile  
✅ Proofread for typos  
✅ Check all social media links work

---

**Remember:** All changes in `data.js` automatically reflect on your portfolio! 🎉
