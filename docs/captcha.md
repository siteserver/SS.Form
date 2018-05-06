## CAPTCHA

[Edit: 2015-09-17] Contact Form 7 4.3 and later recommend reCAPTCHA instead of Really Simple CAPTCHA. For more details, see Contact Form 7 4.3.

Contact Form 7 allows you to insert a CAPTCHA into your contact form to prevent bots from submitting forms. Contact Form 7 utilizes Really Simple CAPTCHA as its officially-sanctioned CAPTCHA module, so you will need to install the Really Simple CAPTCHA plugin before you use CAPTCHA in your form.

The rest of this article will explain how to use CAPTCHA with Contact Form 7, based on the assumption that you have already installed Really Simple CAPTCHA.

### Before using CAPTCHA

Really Simple CAPTCHA requires that GD and FreeType PHP libraries be installed on your server in order to create CAPTCHA images. If you are not certain if they are installed, ask your server administrator.

Contact Form 7 creates a temporary folder and stores any CAPTCHA files there. In most cases, the location of the temporary folder is wp-contents/uploads/wpcf7_captcha, but it can be different depending on your settings.

This folder is created automatically, but sometimes it can fail. A possible cause for this is that the parent folder doesn’t have sufficient writing permissions. In such cases, you can change the permissions or create a folder manually.

You can also change the path of the temporary folder by setting the WPCF7_CAPTCHA_TMP_DIR constant in your wp-config.php like this:

define( 'WPCF7_CAPTCHA_TMP_DIR', '/your/file/path' );
If WPCF7_CAPTCHA_TMP_DIR is defined, this directory is used as the temporary folder instead.

Make certain your temporary folder for CAPTCHA files exists and is writable. Otherwise, CAPTCHA can not be created.

### How to use CAPTCHA

To add a CAPTCHA into your contact form, you must utilize captchac and captchar form tags.

captchac means CAPTCHA-Challenge and it represents an  element for a CAPTCHA image. captchar means CAPTCHA-Response and it represents an  element for a response input field.

A captchac tag must always be paired with a captchar tag with the same name. For example, tags shown below are valid:

[captchac captcha-1] [captchar captcha-1]
But these are invalid because they have different names. In this case, the CAPTCHA and its response do not match:

[captchac captcha-2] [captchar captcha-3]
CAPTCHA-Challenge
captchac means CAPTCHA-Challenge and it represents a CAPTCHA image ( in HTML).

Available options for captchac
OPTION	EXAMPLES	DESCRIPTION
id:(id)	id:foo	id attribute value of the img element.
class:(class)	class:bar	class attribute value of the img element. To set two or more classes, you can use multiple class: option, like [captchac your-captcha class:y2008 class:m01 class:d01].
size:(sml)	size:s	Image size. Only size:s (60×20), size:m (72×24) and size:l (84×28) are available.
fg:#(hex)	fg:#ff0000	Foreground color of the image. Put RGB color code in hex format after fg:#.
bg:#(hex)	bg:#00ffff	Background color of the image. Put RGB color code in hex format after bg:#.
Example:

[captchac your-captcha size:s fg:#ffffff bg:#000000]
CAPTCHA-Response
captchar means CAPTCHA-Response and it represents a response input field ( in HTML).

Available options for captchar
OPTION	EXAMPLES	DESCRIPTION
(size)/(maxlength)	40/100
20/
/50	Field size and max length. You can omit one of them.
id:(id)	id:foo	id attribute value of the input element.
class:(class)	class:bar	class attribute value of the input element. To set two or more classes, you can use multiple class: option, like [captchar your-captcha class:y2008 class:m01 class:d01].
placeholder
watermark		Use the value as placeholder text instead of as default value. watermark works as an alias of placeholder
Example:

[captchar your-captcha 40/100]
Demo
Note: This is a demo. This form doesn’t send a mail practically.

![](assets/captcha/01.png)

View source of above form:

```
1) Default
Input this code: [captchac captcha-170]
[captchar captcha-170 4/4]

2) Small size, inverted
Input this code: [captchac captcha-778 size:s fg:#ffffff bg:#000000]
[captchar captcha-778 4/4]

3) Large size, green text
Input this code: [captchac captcha-118 size:l fg:#00ff00 bg:#ffffff]
[captchar captcha-118 4/4]

[submit "Send"]
```