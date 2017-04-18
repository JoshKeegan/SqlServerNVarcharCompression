/*
 * Sql Server NVarChar Compression
 * CLR Code to be installed on Sql Server
 * Authors:
 *  Josh Keegan 18/04/2017
 */

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

using Microsoft.SqlServer.Server;

namespace SqlServerNVarcharCompression
{
    public class UserDefinedFunctions
    {
        [SqlFunction(IsDeterministic = true, IsPrecise = true, DataAccess = DataAccessKind.None, SystemDataAccess = SystemDataAccessKind.None)]
        public static SqlBytes CompressNVarChar(SqlString sqlString)
        {
            if (sqlString.IsNull)
            {
                return SqlBytes.Null;
            }

            // Get the bytes for this string. Use UTF-8 as that's already more compact that UTF-16/Unicode for strings that are 
            //  mostly ASCII
            byte[] bytes = Encoding.UTF8.GetBytes(sqlString.Value);

            // Compress
            using (MemoryStream outStream = new MemoryStream())
            {
                using (DeflateStream deflateStream = new DeflateStream(outStream, CompressionMode.Compress, true))
                {
                    deflateStream.Write(bytes, 0, bytes.Length);
                }
                return new SqlBytes(outStream.ToArray());
            }
        }

        [SqlFunction(IsDeterministic = true, IsPrecise = true, DataAccess = DataAccessKind.None, SystemDataAccess = SystemDataAccessKind.None)]
        public static SqlString DecompressNVarChar(SqlBytes sqlBytes)
        {
            if (sqlBytes.IsNull)
            {
                return SqlString.Null;
            }

            // Assume decompressed size of quadruple the compressed one. Not an issue if it's larger, but will just
            //  be less optimal as it will copy the underlying byte[] over to a new (larger) one
            int initialMemStreamSize = sqlBytes.Value.Length * 4;

            // Decompress
            using (MemoryStream compressedStream = new MemoryStream(sqlBytes.Value))
            using (DeflateStream deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress, true))
            using (MemoryStream decompressedStream = new MemoryStream(initialMemStreamSize))
            {
                byte[] buffer = new byte[Math.Min(4096, initialMemStreamSize)];

                int count;
                while ((count = deflateStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    decompressedStream.Write(buffer, 0, count);
                }

                // Convert decompressed bytes to a .NET string
                string s = Encoding.UTF8.GetString(decompressedStream.GetBuffer(), 0, (int) decompressedStream.Length);

                // Make an SQL Server string from the managed .NET one
                return new SqlString(s);
            }
        }
    }
}
