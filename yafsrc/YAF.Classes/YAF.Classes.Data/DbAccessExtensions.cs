// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbAccessExtensions.cs" company="">
//   
// </copyright>
// <summary>
//   The db access extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YAF.Classes.Data
{
	using System;
	using System.Data;
	using System.Data.Common;

	using YAF.Types;
	using YAF.Types.Interfaces;

	/// <summary>
	/// The db access extensions.
	/// </summary>
	public static class DbAccessExtensions
	{
		#region Public Methods

		/// <summary>
		/// The replace command text.
		/// </summary>
		/// <param name="dbCommand">
		/// The db command.
		/// </param>
		public static DbCommand ReplaceCommandText([NotNull] this DbCommand dbCommand)
		{
			var commandText = dbCommand.CommandText;

			commandText = commandText.Replace("{databaseOwner}", Config.DatabaseOwner);
			commandText = commandText.Replace("{objectQualifier}", Config.DatabaseObjectQualifier);

			dbCommand.CommandText = commandText;

			return dbCommand;
		}

		/// <summary>
		/// Test the DB Connection.
		/// </summary>
		/// <param name="dbAccess">
		/// The db Access.
		/// </param>
		/// <param name="exceptionMessage">
		/// outbound ExceptionMessage
		/// </param>
		/// <returns>
		/// true if successfully connected
		/// </returns>
		public static bool TestConnection([NotNull] this IDbAccess dbAccess, [NotNull] out string exceptionMessage)
		{
			exceptionMessage = string.Empty;
			bool success = false;

			try
			{
				using (var connection = dbAccess.GetConnectionManager())
				{
					// attempt to connect to the db...
					var conn = connection.OpenDBConnection;
				}

				// success
				success = true;
			}
			catch (Exception x)
			{
				// unable to connect...
				exceptionMessage = x.Message;
			}

			return success;
		}

		#endregion
	}
}