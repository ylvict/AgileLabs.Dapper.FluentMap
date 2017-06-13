using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using static AgileLabs.Dapper.FluentMap.AgileMapper;

namespace AgileLabs.Dapper.FluentMap.SqlBuilders
{
    /// <summary>
    /// Defines methods for building specialized SQL queries.
    /// </summary>
    internal interface ISqlBuilder
    {
        /// <summary>
        /// Builds an insert query using the specified table name, column names and parameter names.
        /// A query to fetch the new id will be included as well.
        /// </summary>
        /// <param name="tableName">The name of the table to query.</param>
        /// <param name="columnNames">The names of the columns in the table.</param>
        /// <param name="paramNames">The names of the parameters in the database command.</param>
        /// <param name="keyProperty">
        /// The key property. This can be used to query a specific column for the new id. This is
        /// optional.
        /// </param>
        /// <returns>An insert query including a query to fetch the new id.</returns>
        string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty);

        /// <summary>
        /// Builds an pagination query using the specified page number and page size.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="whereSql">Sql including where</param>
        /// <param name="orderBySql">Order by clause.</param>
        /// <param name="orderByAsc">Order by type.</param>
        /// <param name="pageNo">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The pagination query.</returns>
        string BuildPagination(string tableName, string whereSql, string orderBySql, bool orderByAsc, int pageNo, int pageSize);
    }

    internal sealed class SqlServerSqlBuilder : ISqlBuilder
    {
        public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
        {
            return $"set nocount on insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}) select cast(scope_identity() as int)";
        }

        public string BuildPagination(string tableName, string sql, string orderBySql, bool orderByAsc, int pageNo, int pageSize)
        {
            var start = pageNo >= 1 ? (pageNo - 1) * pageSize : 0;
            var end = pageNo * pageSize;

            if (string.IsNullOrWhiteSpace(orderBySql))
            {
                orderBySql = " ORDER BY ID ";
            }

            return
            $" SELECT  * FROM ( SELECT ROW_NUMBER() OVER ( {orderBySql} ) AS RowNum, * " +
            $" FROM {tableName} " +
            " ) AS RowConstrainedResult " +
            $" WHERE RowNum >= {start} " +
            $" AND RowNum <= {end} " +
            " ORDER BY RowNum " + (orderByAsc ? "ASC" : "DESC");
        }
    }

    internal sealed class SqlServerCeSqlBuilder : ISqlBuilder
    {
        public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
        {
            return $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}) select cast(@@IDENTITY as int)";
        }

        public string BuildPagination(string tableName, string sql, string orderBySql, bool orderByAsc, int pageNo, int pageSize)
        {
            var start = pageNo >= 1 ? (pageNo - 1) * pageSize : 0;
            var end = pageNo * pageSize;

            return
                $" SELECT  * FROM ( SELECT ROW_NUMBER() OVER ( {orderBySql} ) AS RowNum, * " +
                $" FROM {tableName} " +
                " ) AS RowConstrainedResult " +
                $" WHERE RowNum >= {start} " +
                $" AND RowNum <= {end} " +
                " ORDER BY RowNum " + (orderByAsc ? "ASC" : "DESC");
        }
    }

    internal sealed class SqliteSqlBuilder : ISqlBuilder
    {
        public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
        {
            return $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}); select last_insert_rowid() id";
        }

        public string BuildPagination(string tableName, string sql, string orderBySql, bool orderByAsc, int pageNo, int pageSize)
        {
            var start = pageNo >= 1 ? (pageNo - 1) * pageSize : 0;
            return $" {sql} {orderBySql} LIMIT {start}, {pageSize} ";
        }
    }

    internal sealed class MySqlSqlBuilder : ISqlBuilder
    {
        public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
        {
            return $"insert into {tableName} (`{string.Join("`, `", columnNames)}`) values ({string.Join(", ", paramNames)}); select LAST_INSERT_ID() id";
        }

        public string BuildPagination(string tableName, string sql, string orderBySql, bool orderByAsc, int pageNo, int pageSize)
        {
            var start = pageNo >= 1 ? (pageNo - 1) * pageSize : 0;
            return $" {sql} {orderBySql} LIMIT {start}, {pageSize} ";
        }
    }

    internal sealed class PostgresSqlBuilder : ISqlBuilder
    {
        public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
        {
            var sql = $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)})";

            if (keyProperty != null)
            {
                var keyColumnName = Resolvers.Column(keyProperty);

                sql += " RETURNING " + keyColumnName;
            }
            else
            {
                // todo: what behavior is desired here?
                throw new Exception("A key property is required for the PostgresSqlBuilder.");
            }

            return sql;
        }

        public string BuildPagination(string tableName, string sql, string orderBySql, bool orderByAsc, int pageNo, int pageSize)
        {
            var start = pageNo >= 1 ? (pageNo - 1) * pageSize : 0;
            return $" {sql} {orderBySql} OFFSET {start} LIMIT {pageSize} ";
        }
    }
}
