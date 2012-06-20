using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSonic.Schema;
using System.Data.Common;
using SubSonic.Linq.Structure;
using SubSonic.Query;


namespace SubSonic.DataProviders.SQLite
{

    public class SQLiteProvider : DbDataProvider, IDataProvider
    {
		[ThreadStatic]
		protected static new DbConnection __sharedConnection;

        public override string InsertionIdentityFetchString { get { return String.Empty; } }

        public SQLiteProvider(string connectionString, string providerName) : base(connectionString, providerName)
        {}

        public override string QualifyTableName(ITable table)
        {
            string qualifiedTable;

            qualifiedTable = qualifiedTable = String.Format("`{0}`", table.Name);


            return qualifiedTable;
        }

        public override string QualifyColumnName(IColumn column)
        {
            string qualifiedFormat;
            qualifiedFormat = "`{2}`";
            return String.Format(qualifiedFormat, column.Table.SchemaName, column.Table.Name, column.Name);
        }

        public override ISchemaGenerator SchemaGenerator
        {
            get { return new SQLiteSchema(); }
        }

        public override ISqlGenerator GetSqlGenerator(SqlQuery query)
        {
            return new SQLiteGenerator(query);
        }

        public override IQueryLanguage QueryLanguage { get { return new SQLiteLanguage(this); } }

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
