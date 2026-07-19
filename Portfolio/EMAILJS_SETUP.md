# 🚀 EmailJS Setup - Get Working in 3 Minutes!

Your portfolio now has **EmailJS integrated** and ready to receive emails directly to your inbox!

## ⚡ Quick Setup (3 steps, 3 minutes)

### Step 1: Create EmailJS Account (1 min)

1. Go to **[https://www.emailjs.com/](https://www.emailjs.com/)**
2. Click **"Sign Up Free"**
3. Sign up with Gmail or email
4. Verify email

### Step 2: Get Your Public Key (30 seconds)

1. Click on your **Profile** icon (top right)
2. Go to **Account Settings**
3. Find **API Keys** section
4. Copy your **Public Key** (looks like: `abc123xyz...`)
5. Open `script.js` in your editor
6. Find this line (around line 6):

```javascript
const EMAILJS_PUBLIC_KEY = "YOUR_PUBLIC_KEY";
```

7. Replace `"YOUR_PUBLIC_KEY"` with your actual key:

```javascript
const EMAILJS_PUBLIC_KEY = "j7x8k9p2q3w4e5r6t7y8"; // Example - use your actual key
```

### Step 3: Create Email Service (1.5 mins)

1. Go back to EmailJS dashboard
2. Click **Email Services** (left sidebar)
3. Click **Add Service**
4. Choose **Gmail** (recommended)
5. Click **Connect with Gmail**
6. Select your email: `kanuparthicnu@gmail.com`
7. Authorize EmailJS
8. Copy the **Service ID** (looks like: `service_xxxxx`)
9. Open `script.js` again
10. Find this line (around line 7):

```javascript
const EMAILJS_SERVICE_ID = "service_kanupathi";
```

11. Replace with your Service ID:

```javascript
const EMAILJS_SERVICE_ID = "service_abc123xyz"; // Your actual Service ID
```

### Step 4: Create Email Template (1.5 mins)

1. In EmailJS dashboard, click **Email Templates** (left sidebar)
2. Click **Create New Template**
3. Name it: `portfolio_contact`
4. In the template editor, set:
   - **To Email**: `kanuparthicnu@gmail.com`
   - **Subject**: `New Portfolio Contact from {{from_name}}`
   - **Content**: Copy this:

```
Hello,

You have a new message from your portfolio!

Name: {{from_name}}
Email: {{from_email}}
Message:
{{message}}

---
Reply directly to {{from_email}}
```

5. Click **Save**
6. Copy the **Template ID** (should be `template_portfolio`)
7. Update `script.js` if needed:

```javascript
const EMAILJS_TEMPLATE_ID = "template_portfolio";
```

## ✅ That's It! You're Done!

Now when someone fills the contact form on your portfolio, the email will be sent directly to your inbox!

---

## 📝 What Happens When Someone Submits?

1. ✅ Visitor fills name, email, and message
2. ✅ Clicks "Send Message"
3. ✅ Email is sent via EmailJS to `kanuparthicnu@gmail.com`
4. ✅ You can reply directly to their email
5. ✅ Visitor sees success message

---

## 🔍 Test It

1. Go to your portfolio
2. Scroll to "Get In Touch" section
3. Fill the form with test data
4. Click "Send Message"
5. Check your email inbox (check spam folder too)
6. You should see the email within 10 seconds!

---

## ❓ Troubleshooting

**Not receiving emails?**

- Check spam folder
- Verify Gmail is connected in EmailJS
- Make sure template is published
- Check browser console (F12) for errors

**Want to check if it's working?**

- Open browser DevTools (F12)
- Go to Console tab
- Look for: `✓ EmailJS initialized successfully`

**Rate Limits?**

- Free tier: 100 emails per month
- Need more? Upgrade to paid plan (just $4/month for 5,000 emails)

---

## 📞 Support

- EmailJS Help: https://www.emailjs.com/docs/
- Common Issues: https://www.emailjs.com/docs/faq/

**Done! Your portfolio now receives real emails!** 🎉
