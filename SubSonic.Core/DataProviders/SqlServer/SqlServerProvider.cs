﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSonic.DataProviders.SQLite;
using SubSonic.Schema;
using System.Data.Common;

using SubSonic.Linq.Structure;
using SubSonic.Query;

namespace SubSonic.DataProviders.SqlServer
{
    public class SqlServerProvider : DbDataProvider, IDataProvider
    {
		[ThreadStatic]
		protected static new DbConnection __sharedConnection;

        private string _insertionIdentityFetchString = "; SELECT SCOPE_IDENTITY() as new_id";
        public override string InsertionIdentityFetchString { get { return _insertionIdentityFetchString; } }
		
        public SqlServerProvider(string connectionString, string providerName) : base(connectionString, providerName)
        {}

        public override string QualifyTableName(ITable table)
        {
            string qualifiedTable;


            string qualifiedFormat = String.IsNullOrEmpty(table.SchemaName) ? "[{1}]" : "[{0}].[{1}]";
            qualifiedTable = String.Format(qualifiedFormat, table.SchemaName, table.Name);


            return qualifiedTable;
        }

        public override string QualifyColumnName(IColumn column)
        {
            string qualifiedFormat;
            qualifiedFormat = String.IsNullOrEmpty(column.SchemaName) ? "[{1}].[{2}]" : "[{0}].[{1}].[{2}]";
            return String.Format(qualifiedFormat, column.Table.SchemaName, column.Table.Name, column.Name);
        }

        public override ISchemaGenerator SchemaGenerator
        {
            get { return new Sql2005Schema(); }
        }

        public override ISqlGenerator GetSqlGenerator(SqlQuery query)
        {
            return new Sql2005Generator(query);
        }

        public override IQueryLanguage QueryLanguage { get { return new TSqlLanguage(this); } }
		
		/// <summary>
        /// Gets or sets the current shared connection.
        /// </summary>
        /// <value>The current shared connection.</value>
        public override DbConnection CurrentSharedConnection
        {
            get { return __sharedConnection; }

            protected set
            {
                if(value == null)
                {
                    __sharedConnection.Dispose();
                    __sharedConnection = null;
                }
                else
                {
                    __sharedConnection = value;
                    __sharedConnection.Disposed += __sharedConnection_Disposed;
                }
            }
        }

        protected static void __sharedConnection_Disposed(object sender, EventArgs e)
        {
            __sharedConnection = null;
        }


    }
}
