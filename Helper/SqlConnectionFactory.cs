﻿using Microsoft.Data.SqlClient;

namespace TalksyApp.Helper
{
	public class SqlConnectionFactory
	{
		private readonly string _connectionString;
		public SqlConnectionFactory(string connectionString)
		{
			_connectionString = connectionString;
		}

		public SqlConnection Create()
		{
			return new SqlConnection(_connectionString);
		}
	}

}
