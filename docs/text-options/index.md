Max & Min Length Options and Character Count
Contact Form 7 4.1 introduces maxlength and minlength options to some form-tags, and a new type of form-tag for a Twitter-like character count.


 
Maxlength & Minlength Options
Maxlength and minlength are based on the HTML5 attributes of the same names that specify the maximum and minimum length allowed for input fields.

Form-tag types that support both maxlength and minlength are: text, textarea, email, url, tel, quiz, and captchar (CAPTCHA Response). These form-tags (except quiz and captchar) also support user input validation based on character length.

Example:

[textarea* your-message minlength:10 maxlength:140]
With the textarea form-tag in this example, you will see a validation error message if your input is shorter than 10 characters or longer than 140 characters. However, most browsers block you from typing beyond the maxlength limit.

The old format for maxlength (e.g., [textarea* your-message 40/140]) is still available, but an explicit maxlength option (if set) will override it.


 
Character Count
You may want a character count for an input field with the maxlength option, which tells how many characters you have typed, or you can move type into the field, up to the limit of the maxlength.

To add a character count, insert a count form-tag into your form. count only works as a placeholder for a character count (integer). Note that the tag has to have the same name as the targeted input field.

For example, if you have the field (name = “your-message”):

[textarea* your-message minlength:10 maxlength:140]
and want a character count for this field, add [count your-message]:

[textarea* your-message minlength:10 maxlength:140]
[count your-message]
By default, count displays the character count in the targeted field, so its integer value increases as you type.

You can also reverse this to make it display the character count remaining up to the maxlength of the targeted field. To do so, add a down option into the count form-tag:

[textarea* your-message minlength:10 maxlength:140]
[count your-message down]
In this case, count shows “140” at first, decreasing as you type; when you have typed 140 characters, it will show “0”.

Demo


0



140

View source of above form:
[textarea* your-message minlength:10 maxlength:140]
[count your-message]

[textarea* your-message-2 minlength:10 maxlength:140]
[count your-message-2 down]