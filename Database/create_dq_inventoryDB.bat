echo off

rem batch file to run a script to create a db
rem 9/5/2019

rem sqlcmd -S localhost -E -i dq_inventory.sql
rem sqlcmd -S localhost\mssqlserver -E -i dq_inventory.sql
sqlcmd -S localhost\sqlexpress -E -i dq_inventory.sql

ECHO .
ECHO if no error messages appear DB was created
PAUSE