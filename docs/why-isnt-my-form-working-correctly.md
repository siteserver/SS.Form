Why isn’t My AJAX Contact Form Working Correctly?

 
Contact Form 7 supports AJAX submissions. Therefore, a correctly configured contact form in Contact Form 7 works exactly like the following demo-form. Try inputting any text into fields and submitting it.

Validation errors occurred. Please confirm the fields and submit it again.
Please fill the required field.
Please fill the required field.
Your Name (required)

Please fill the required field.

Your Email (required)

Please fill the required field.

Your Message



Validation errors occurred. Please confirm the fields and submit it again.
How was it? Did it work like the contact form on your site? Some of you might be surprised to note the differences between your form and this demo form, and think, “My form always reloads the page after submitting, but this form doesn’t.”

If your form doesn’t work like this demo form does, it is probable that Contact Form 7’s JavaScript is not functioning on your site. I’ll show you a few possible causes for this.

JavaScript file is not loaded
This is the cause that I’ve been seeing the most recently. This is due to your template, which is missing calling functions for queuing JavaScript. The functions required are wp_head() and wp_footer(), and they are in header.php and footer.php, respectively, in most correct themes.

Conflicts with other JavaScript
Many plugins and themes load their own JavaScript. Some of them may have been created incorrectly and therefore conflict with other plugins. In most cases, you can find JavaScript errors with Firebug, an add-on for Firefox, when such conflicts occur.

HTML structure is not valid
Like other JavaScript, Contact Form 7’s JavaScript traverses and manipulates the structure of HTML. Therefore, if the original HTML structure is not valid, it will fail to work. You can check whether your HTML is valid or not with an HTML validator. I recommend the W3C Markup Validation Service for use in such a case.