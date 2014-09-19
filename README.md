SonarCompanion
==============

The SonarCompanion Visual Studio extension adds integration with [SonarQube](http://sonarqube.org) code analysis to have quick and easy navigation
from issues found by Sonar and your solution.

### Usage
- Install the extension through the Extensions and Updates window in Visual Studio or by cloning this repo and building the VSIX yourself.
- Download the **sonarcompanion.properties** file from this repository and add it to the root of your solution
- Edit the **sonarcompanion.properties** file and configure the URI to point to your SonarQube repository
- Open the issues window by clicking the Tools / Sonar Companion menu item

### Visual Studio Gallery
The extension can be downloaded from the Visual Studio Gallery [here](http://visualstudiogallery.msdn.microsoft.com/c55ea298-2ecd-4f35-b483-34e2184cc4fb)


### Changelist
**Version 1.0.3**

- Added the new sonarcompanion.properties file to enable per-solution settings
- Added auto-refresh option.
- Integration with Visual Studio has been reworked to be less tightly coupled
- And lots of behind-the-scenes changes 

### Contributors
- Sander van Vliet [@SanderMvanVliet](http://twitter.com/SanderMvanVliet)
- Bjorn Kuiper [SouthernSun](https://github.com/southernsun)