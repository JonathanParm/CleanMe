CleanMe.Domain			(Domain entities)			contains core entities and business rules. Declares business rules and contracts (interfaces).
CleanMe.Application		(Service layer logic)		uses CleanMe.Domain and implements services. Implements business logic using these contracts.
CleanMe.Infrastructure	(Database/external APIs)	depends on CleanMe.Domain for entities and provides implementations like repositories. Implements the repository and data access logic.
CleanMe.Shared			(ViewModels, Helpers, DTOs) stores shared ViewModels, helper classes, DTOs, and constants.
CleanMe.Web				(Frontend - MVC)			references CleanMe.Application for business logic and controllers.
CleanMe.Tests			(Unit tests)				references all projects to test them.

Follows Domain-Driven Design (DDD) where Domain defines business rules, and Infrastructure implements them.

https://icons.getbootstrap.com/ use to get icons

change default project to CleanMe.Infrastructure
PM> Add-Migration <MigrationName>
PM> Update-Database

when have multiple db context
Add-Migration <MigrationName> -Context <YourDbContext>

undo using
Remove-Migration


https://ej2.syncfusion.com/home/aspnetcore.html use to get library of useful components

Admin123*
Cleaner123*
Tester123*

to show debug console:
add to following to launchSettings.json under "profiles" and change application properties general 'output type' from windows application to console application.

  "profiles": {
    "http": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "dotnetRunMessages": true,
      "applicationUrl": "http://localhost:5173"
    },

rate for cleaner may change within month
cleaner for an area may change within month
rate for each cleaner may be different
each cleaner has a default rate. however an atm may have a different rate from default.

when rate for an atm is set, then notification that rate is different from standard rate.

staff, consultant, supplier/creditor.

When create invoice for month, it must be confirmed as correct. Until confirmed changes can be made.
Once confirmed then invoice preserved. Can reprint any invoice from history.
Confirmed invoice preserved in separate readonly table.

When asset finishon date is within month, then show in that month.

Only include assets when has a clean scheduled in month.

Asset schedule is default schedule adjusted by amendments that start before or within month, and finish within or after month.

Amendment report to show asset number, asset, date range, amendment details.

Amendment record locked when invoiced/payroll completed. Somehow confirm ammendments for previous month.

TODO !!!

When print schedule, amendments are merged into report.

Printed schedule missing name of area in heading. IMPORTANT.

Printed schedule missing staff/contractor name and address, and Schedule for <Month> <Year> above schedule

Printed schedule missing column totals for each week

Printed schedule missing clean frequency totals - weekly: nn fortnightly: nn monthly: nn quarterly: nn

Asset type has a base default staff/contractor pay rate and default client charge rate that can be overwritten for a client

When print schedule notify on screen that reports have been created in folder

Staff names missing second letter in first and last names.

When generating Schedule do not override existing schedule PDF files once amendments locked. IMPORTANT.

Generating Schedule screen option to display on screen or PDF. IMPORTANT.   

Generating Schedule screen option to select 2 or more cleaners and show their schedule possibly as summaries. Needs more definition.