## Additional Settings

You can include additional settings to each contact form by adding code snippets in the specific format into the Additional Settings field in the contact form’s edit screen.
 
By default, Contact Form 7 supports the following types of settings.

### Subscribers-Only Mode

```
subscribers_only: true
```

You may want to ensure that only logged-in users can submit your contact form. In such cases, use the subscribers-only mode. In this mode, non-logged-in users can’t submit the contact form and will see a message informing them that login is required, while logged-in users can use it as usual.

No anti-spam verification will be provided for contact forms in the subscribers-only mode since only welcome people are supposed to be able to use them. If this assumption is not applicable to your site, subscribers-only mode probably isn’t a good option for you.

### Demo Mode

```
demo_mode: on
```

If you set demo_mode: on in the Additional Settings field, the contact form will be in the demo mode. In this mode, the contact form will skip the process of sending mail and just display “completed successfully” as a response message.

### Skip Mail

```
skip_mail: on
```

The skip_mail setting works in the almost same manner as the demo_mode, but the skip_mail skips the mail sending only. Unlike demo_mode, skip_mail doesn’t affect other activities like storing messages with Flamingo.

### Acceptance as Validation

```
acceptance_as_validation: on
```

By default, an acceptance checkbox behaves differently from other types of fields; it does not display a validation error message even when the box is not checked. If you set acceptance_as_validation: on in the Additional Settings field, acceptance checkboxes in the contact form behave in the same way as other form fields.

For details, see Acceptance Checkbox.

### Flamingo Settings
You can customize the Subject and From field values shown in the admin menu of Flamingo. For more details, see Save Submitted Messages with Flamingo.

### Suppressing Message Storage

```
do_not_store: true
```

This setting tells message storage modules, such as Flamingo, not to store messages through this contact form.

### JavaScript Code

```
on_sent_ok: "alert('sent ok');"
on_submit: "alert('submit');"
```

If you set on_sent_ok: followed by a one-line JavaScript code, you can tell the contact form the code that should be performed when the mail is sent successfully. Likewise, with on_submit:, you can tell the code that should be performed when the form submitted regardless of the outcome.

See also: Tracking Form Submissions with Google Analytics and Redirecting to Another URL After Submissions

> Note: on_sent_ok and on_submit have been officially removed from Contact Form 7 5.0. You can use DOM events instead of these settings.