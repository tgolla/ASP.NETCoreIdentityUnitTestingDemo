 ```WebApplication.InitializeIdentity```

The ```WebApplication.Identity.Initialize``` project exist to seed the Identity database with roles and/or users.

## Implementation
Implementation of the ```WebApplication.Identity.Initialize``` project requires the creation of an identity database and configuration of several application settings.

If you have not yet created an identity database do so using SQL Server Management Studio and then refer to the ```WebApplication.Identity``` project ```README.md``` file implementation step 1 for applying migrations to your identity database.

After creating the database you will need to configure the following application settings in either the base ```appsettings.json``` file and/or one or more environment specific application settings file (i.e. ```appsettings.Development.json```).



Property | Description
--- | ---
```Roles``` | A list of the applications roles to be created. i.e. "Roles": [ "Administrator", "Manager", "Mechanic" ]
```User``` | The user to be created.
```UserPassword``` | The user's password.
```UserRoles``` | A list of the user's roles.

The ```User``` property is broken down further into the following properties.

Property | Description
--- | ---
```UserName``` | The username.
```Email``` | The user's email address.

More information concerning the defined configuration values can be found in the incode documentation of ```appsettings.cs``` in this project and ```ApplicationUser.cs``` in the ```WebApplication.Identity``` project.  

Once you have configured the application settings you can execute the application to create the roles and/or user in the Identity database.  Note: existing roles or users will not be modified.

Execution of the console application can be done either inside Visual Studio or from the command line.
Inside Visual Studio you will need to select the appropriate debug launch setting for the environment you desire (i.e. WebApplication.Identity.Initialize (Developemnt)).
When executing from a command line you simple type the environment as the first argument.

```WebApplication.Identity.Initialize Production``` 



