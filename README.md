This is currently in Production: 
<br />
https://portal-active.azurewebsites.net/

<h1>Portal Active</h1>

- An API that provides the client with various features and functionalities including
  <br />
    &nbsp; &nbsp; &nbsp;-> Secure Authentication System
  <br />
    &nbsp; &nbsp; &nbsp;-> Personal Account
  <br />
    &nbsp; &nbsp; &nbsp;-> Social Activities such as Positng Activities to the Portal, commenting on others' Activitiies, Joining others' activities
  <br />
   &nbsp; &nbsp; &nbsp;-> Following other Users and so much more

- Implemented the Clean Architecture Concept in this project to organize the management and make the Application feasable for more updates and integration
- Added Cloud Service for Image Upload that makes our server more adjustable in size rather than having the images in the static files
- Added a UI that guides the user for using the application
- Added Facebook Login
  <br> &nbsp; &nbsp; &nbsp; <h6>This feature will be working only for the testers of the app, since facebook require busimess validation for the login to be available to all facebook users, which is why you will see the error when using the login</h6>
- Added Email Verification Using two services:
   &nbsp; &nbsp; &nbsp; -> MailerSend which provides a trial but limited to number of differnet users to send them messages
   &nbsp; &nbsp; &nbsp; -> Gmail SMTP Server which allows my gmail to send automatic verifcation messages
- Added Refresh Tokens to secure the users' JWT Token even more, making it's expiry only 10 mins

  <h1>Future Features</h1>
  - Password Reset
  - More Profile Settings

