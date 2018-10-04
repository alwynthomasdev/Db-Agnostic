using Dapper;
using System;
using System.Collections.Generic;

namespace DbAgnostic
{
    //wrapper for SqlMapper.GridReader
    public class QueryMultipleResultReader : IDisposable
    {
        SqlMapper.GridReader _Reader;
        internal QueryMultipleResultReader(SqlMapper.GridReader rdr)
        {
            _Reader = rdr;
        }

        public IEnumerable<dynamic> Read() => _Reader.Read();
        public IEnumerable<T> Read<T>() => _Reader.Read<T>();

        public void Dispose()
        {
            _Reader.Dispose();
        }
    }
}
