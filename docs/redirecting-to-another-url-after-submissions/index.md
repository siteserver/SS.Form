Redirecting to Another URL After Submissions
First of all, I believe that this tip I’m writing on this post is not necessary for 99.99% of users and, actually, I don’t recommend using it. I’m writing this for the 0.01% of you, so you can ignore it if not necessary.
 
As you know, Contact Form 7 redirects to the same URL as the form’s URL after form submissions in the default settings. However, in very rare cases, you might need to change this to make it redirect to another URL after submissions. I’ll write in this post how you can set up Contact Form 7 to do just that.

By the way, I’m often asked by users that how they can redirect to so-called “Thank You Page”. In most cases, they want to know it because they assume that redirecting to “Thank You Page” is necessary for tracking form submissions with Google Analytics. That’s not necessary at all. In fact, it’s an outdated and nonsense custom. Today you can track submissions with Google Analytics without any redirection.

So you have other reason for redirecting to another URL?

The simplest way is utilizing Contact Form 7’s custom DOM event to run JavaScript. The following is an example of script that redirects you to another URL when the wpcf7mailsent event occurs:

```
<script>
document.addEventListener( 'wpcf7mailsent', function( event ) {
    location = 'http://example.com/';
}, false );
</script>
```

Embed this snippet into your theme’s template file. Obviously, you need to replace the http://example.com/ in the code to the URL you want to redirect to.

Note: The method using on_sent_ok hook is no longer recommended. This function is scheduled to be abolished by the end of 2017.