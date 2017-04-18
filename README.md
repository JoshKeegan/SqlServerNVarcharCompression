# SQL Server NVarchar Compression #
An installable assembly for SQL Server that adds Deflate (the algorithm used by GZip) Compression functions for nvarchar values.  

Download the latest release from the [Download page](https://github.com/JoshKeegan/SqlServerNVarcharCompression/releases).  
  
Please note that where possible, it is wise to perform (de)compression in the application tier and just store or retrieve compressed binaries in the database.  
Performing (de)compression in the database will use database server CPU cycles (and MS license SQL Server per core remember) for something that could easily be moved to the application tier, and using scalar-valued functions in queries can prevent them from going parallel.  

However, all is not lost! There are cases where it makes sense to have this functionality in the database.  
Examples include:
* Compressing data already stored in the database without having to load it out to the application tier and then back in to the database.  
* Debugging. During development it's fairly normal to want to view problem data. Having a decompression function on the dastabase makes this much easier.  
In addition to this, it guarantees that any application connecting to the database has the ability to read & write compressed data, without having to implement the compression algorithm.  
If it is ever used for production CRUD queries, please bear in mind the scaling issues mentioned earlier.
  
## Installation ##
Please follow the instructions in the INSTALL file for installation instructions.

## License ##
MIT. Copy in LICENSE file