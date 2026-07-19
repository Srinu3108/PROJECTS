# 📧 Email Setup Guide - Portfolio Contact Form

Your contact form is ready, but you need to configure an email service to receive messages. Here are your best options:

## Option 1: EmailJS (Recommended - Most Popular) ⭐

### Steps:

1. Go to [EmailJS.com](https://www.emailjs.com/)
2. Sign up for free (200 emails/month)
3. Create an email service:
   - Click "Add Service"
   - Connect Gmail or other email provider
   - Copy your **Service ID**
4. Create an email template:
   - Click "Create Template"
   - Set template ID (e.g., "portfolio_contact")
   - Configure the email content
   - Copy your **Template ID**
5. Get your **Public Key** from Account settings
6. Update `script.js` - Replace these lines in the contact form submission:

```javascript
// Replace the fetch part with:
emailjs.init("YOUR_PUBLIC_KEY");

emailjs
  .send(
    "YOUR_SERVICE_ID", // Service ID
    "YOUR_TEMPLATE_ID", // Template ID
    {
      to_email: "kanuparthicnu@gmail.com",
      from_name: name,
      from_email: email,
      message: message,
    },
  )
  .then((response) => {
    alert(
      `Thank you ${name}!\n\nYour message has been sent!\nI'll respond at ${email}`,
    );
    contactForm.reset();
  })
  .catch((error) => {
    alert("Error sending message. Please try again.");
  });
```

## Option 2: Formspree (Simple Setup)

1. Go to [Formspree.io](https://formspree.io/)
2. Create new form
3. Add your email: kanuparthicnu@gmail.com
4. Copy the form ID (looks like: `f/xxxxx`)
5. Update script.js with your form ID:

```javascript
const response = await fetch("https://formspree.io/f/YOUR_FORM_ID", {
  // ... rest of code
});
```

## Option 3: Netlify Forms (If Hosting on Netlify)

1. Deploy your portfolio to Netlify
2. Add `netlify` attribute to form: `<form netlify>`
3. Emails will automatically go to your Netlify account
4. Set up notifications in Netlify dashboard

## Current Status:

✅ **What works now:**

- Form collects all information
- Shows success message
- Stores data in browser localStorage as backup

❌ **What needs setup:**

- Actually sending emails requires one of the above services

## Testing Locally:

Open browser DevTools (F12) → Application → LocalStorage → portfolioMessages
You'll see all submitted messages stored locally as backup.

---

**Need help? Check the service documentation or update with your API keys!**
