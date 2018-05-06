Submit Button
A submit button is an essential component of a form. As you may know, HTML represents a submit button as an input element with submit type: <input type="submit">. You can use this HTML tag in a contact form of Contact Form 7, but you should use Contact Form 7’s own submit form tag instead.

 
This is the simplest form of submit tag:

[submit]
You can add a value like this:

[submit "Send Mail"]
Did you notice that the submit tag’s syntax is bit different than other form tags? The submit tag does not have name part, which other form tags have.

In addition to this, you can add several options to submit tag.

Available options for submit
OPTION	EXAMPLES	DESCRIPTION
id:(id)	id:foo	id attribute value of the input element.
class:(class)	class:bar	class attribute value of the input element. To set two or more classes, you can use multiple class: option, like [submit class:y2008 class:m01 class:d01].
Example:

[submit class:button id:form-submit "Send Mail"]