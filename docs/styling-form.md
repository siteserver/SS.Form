Styling Contact Form
How do I style contact form? This is a common question on the support forum. Contact Form 7 doesn’t provide any customization for styling. Editing CSS style sheets is the best method to style contact forms. In this article, I’ll show you some important steps for styling your contact forms. If you know about CSS, my explanation is simple. If you are not familiar with CSS, please learn CSS first with these informative websites:

Learning CSS – W3C
CSS Tutorial – W3Schools
Learn CSS | MDN – Mozilla Developer Network
CSS Basics

 
Which Style Sheet Should I Edit?
Any style sheet is okay, but I recommend editing your theme’s main style sheet. It’s better not to edit style sheets in the plugin because your changes will be overwritten when the plugin is updated. Themes can be updated, but they are generally updated less frequently than plugins. If your theme is updated often, you might make a child theme and manage the style sheet in the child theme.

Style Fields in Contact Form
Let’s see how we can style individual fields in a contact form. There are several types of input fields. The most common field is a single-line text input field so let’s add a style rule for it:

1
2
3
4
5
6
input[type="text"]
{
    background-color: #fff;
    color: #000;
    width: 50%;
}
This selector matches all ‘input’ elements whose ‘type’ attribute has exactly the value ‘text’ (i.e. single-line text input fields). Also, this style rule has three properties specifying white as background color, black as foreground (text) color, and 50% as width of field.

You may want to apply this style rule to other types of fields. Let’s add selectors for an email address input field and a multi-line text input area:

1
2
3
4
5
6
7
8
input[type="text"],
input[type="email"],
textarea
{
    background-color: #fff;
    color: #000;
    width: 50%;
}
Now this style is applied to every part of your site. You may want to limit it to contact forms. Contact Form 7’s form has a wrapper element that has ‘wpcf7’ class. You can limit the scope of target by adding ancestor selectors:

1
2
3
4
5
6
7
8
.wpcf7 input[type="text"],
.wpcf7 input[type="email"],
.wpcf7 textarea
{
    background-color: #fff;
    color: #000;
    width: 50%;
}

 
Style Specific Fields
You might want to style only specific fields. First, add an ‘id’ or ‘class’ option to the form-tags of the fields in the contact form that you want to style:

[text text-123 id:very-special-field]
Then add style rules using the id or class:

1
2
3
4
5
#very-special-field
{
    color: #f00;
    border: 1px solid #f00;
}
Style Whole of Contact Form
As I mentioned earlier, the top-level element of contact form has ‘wpcf7’ class. To style the whole contact form, add style rules for the class selector:

1
2
3
4
5
.wpcf7
{
    background-color: #f7f7f7;
    border: 2px solid #0f0;
}
This style rule gives your contact forms a light gray background and green border.

See Also
Why does my email address input field look different than other text input fields?
Custom Layout for Checkboxes and Radio Buttons
Can I add id and class attributes to a form element?
Buzztone has written a comprehensive and detailed article on Styling Contact Form 7 Forms. The article shows people, with suitable HTML & CSS skills, how to change the appearance of their Contact Form 7 Forms to meet their particular requirements.
SHARE THIS: