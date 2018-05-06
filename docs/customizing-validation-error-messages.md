Customizing Validation Error Messages
When form submission includes invalid inputs (e.g., required fields are blank or email fields have invalid formats), the form displays validation error messages. This post describes how you can customize the validation error messages that Contact Form 7 produces.

Changing Text
You can change the text used in each type of error message. For example, the default message for required fields is “The field is required.” To change this message, go to the admin page and edit the text in the Messages section.

Static or Floating-tip
You have two different style options for validation error messages: static and floating-tip. Static is the current default option. Floating-tip was once the default option in Ajax submission mode before Contact Form 7 3.6 demoted it because of poor accessibility.

Look at these demo forms to see the differences between the two styles.

Static style
Lorem ipsum dolor sit amet, 
 adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea 
 consequat.

Floating-tip style
Lorem ipsum dolor sit amet, 
 adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea 
 consequat.
 
Each style has advantages and disadvantages.

ADVANTAGE	DISADVANTAGE
STATIC STYLE (DEFAULT)	
Good accessibility
Same behavior in Ajax and non-Ajax submission modes
Possible to break the layout when used for inline fields
FLOATING-TIP STYLE	
Not possible to break the layout even when used for inline fields
Poor accessibility
Only works in Ajax submission mode
Normally, static style is recommended as it causes less accessibility issues. However, if you have inline fields in a form and inserting static error message blocks after them breaks the form layout, using floating-tip style is the better option.

Switching the validation error message style can be done in a simple step. You can apply floating-tip style to specific fields or to all fields in a form. The basic rule is that when an element has the class ‘use-floating-validation-tip’, all fields under the element use floating-tip style.

Let me show some examples. To apply floating-tip style to a specific input field, wrap the field with an element that has the ‘use-floating-validation-tip’ class.

Example:

<span class="use-floating-validation-tip">[text* your-name]</span>
If you want to apply floating-tip style to all fields in a form, add the ‘use-floating-validation-tip’ class to the form element by adding the html_class attribute to [contact-form-7] shortcode.

Example:

[contact-form-7 id="1234" title="Contact form 1" html_class="use-floating-validation-tip"]
In non-Ajax submission mode, static style is used regardless of settings.