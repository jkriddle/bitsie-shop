Bitsie.Shop
============

.NET Application Framework

## Installation

1. Read Install/Readme.md
2. Run Install/Renamer.exe to rename the solution to an appropriate project name.
3. Open Src/[ProjectName].sln
4. Clean and Rebuild
5. Create a database and update the connection string in Src/[ProjectName].Web/NHibernate.config accordingly.
6. Run Src/[ProjectName].Build and specify the type of installation. This will configure/create the database.
7. Run Src/[ProjectName].Web to launch. If you see the homepage you are good to go!