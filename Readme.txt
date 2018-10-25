This is my dotnet core base implementation.

The idea is to create a webserver project that can be used to serve several sites while also acting as a base implementation for other programming ideas, features and tasks.

I am building it to be easy to pull down and test with anywhere while simultaneously allowing a variety of database types and server operating systems.

At the core will be a Content management system that can serve multiple sites with authorization and encryption built in.

This base will remain functional rather than styled so that it can be cloned and used as a base for other projects.

If you want to download the project to have a go yourself you will need an appsettings.json file with the following:

{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "CheckNotConsentNeeded": "false"
}

and an appsettings-custom.json with this in (for testing purposes Password of the PasswordHash is "UserPassword"):

{
  "InitialConfig":"true",
  "Protector":"test.protector",
  "TestUser": {
    "UserName": "test@criv.uk",
    "Email": "test@criv.uk",
    "PhoneNumber": "0000000000",
    "NormalizedUserName": "TEST@CRIV.UK",
    "NormalizedEmail": "TEST@CRIV.UK",
    "PasswordHash": "AQAAAAEAACcQAAAAEH42gxSffx6QDd0SiDfk4fHzyvzYxWM39N6THgmwZ384co4FXqPKM7ywXPodVBev9A==",
    "UserType": 1
  },
  "ConnectionStrings": {
    "DefaultConnection": "server=127.0.0.1;user=admin;password=password;persistsecurityinfo=True;port=1433;database=serverdb"
  },
  "PrimaryCertificate": {
    "location": "test1.pfx",
    "password": "test1pass1"
  },
  "BackupCertificate": {
    "location": "test2.pfx",
    "password": "test2pass1"
  }
}

or in other words to personalise:

{
  "InitialConfig": "true",
  "Protector": "YOUR.PROTECTOR.NAME",
  "TestUser": {
    "UserName": "your-username",
    "Email": "your@email.com",
    "PhoneNumber": "YOURNUMBER",
    "NormalizedUserName": "YOUR-USERNAME",
    "NormalizedEmail": "YOUR@EMAIL.COM",
    "PasswordHash": "ENCRYPTEDPASSWORDHASH",
    "UserType": 1
  },
  "ConnectionStrings": {
    "DefaultConnection": "ENCRYPTEDCONNECTIONSTRING"
    /*"DefaultConnection": "server=127.0.0.1;user=yourdbuser;password=yourdbpassword;persistsecurityinfo=True;port=yourport;database=yourdb"*/
  },
  "PrimaryCertificate": {
    "location": "ENCRYPTEDLOCATION",
    "password": "ENCRYPTEDPASSWORD"
  },
  "BackupCertificate": {
    "location": "ENCRYPTEDLOCATION",
    "password": "ENCRYPTEDPASSWORD"
  }
}
