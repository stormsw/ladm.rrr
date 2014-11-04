LADM provides data model for cad registrations
The key objects are RRR, Property, Party.

For RRR operations we take any operation modified state of this object List (RRR, Property, Party).

In general actions are create-update-delete.

Each spatial objects exists in 1 or n RRR layer depends on it type.
Any RRR may target Party (or it suppsed to be for any Party).
Any RRR may target Property (or it supposed to point any property)

The Container for collecting SU (spatial units) of homogenous RRR is LAUnit.
It may address same Party and same set of properties while it gets same right type.
LAUnit may not have spatial units.

You need to have db to start tests - modify App.config to any of local db, sql express or sql server installations
(other configs supported by EntityFramework may work too)

 <connectionStrings>
 <!-- for localdb use this with correct filepath -->
    <add name="LadmDbConnectionString" connectionString="Data Source=(LocalDb)\v11.0;Initial Catalog=Specifications.mdf;Integrated Security=SSPI;AttachDBFilename=C:\TestLocalDb\Specifications.mdf" providerName="System.Data.SqlClient" />
    <!-- this one for SQLEXPRESS -->
	<add name="LadmDbConnectionString" connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=LADM_BDD_TEST;Integrated Security=true;Password=;Pooling=False" providerName="System.Data.SqlClient"/>
    <!-- This for SQL server instance (you need account able to drop and create db)-->
	<add name="LadmDbConnectionString" connectionString="Data Source=PLAGIS1;User ID=sa;Password=Paragon11;Initial Catalog=LADM_BDD_TEST;Integrated Security=false;" providerName="System.Data.SqlClient"/>
</connectionStrings>