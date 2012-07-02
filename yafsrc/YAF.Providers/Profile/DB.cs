/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Providers.Profile
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;

    using YAF.Classes;
    using YAF.Classes.Pattern;
    using YAF.Core; 
    using YAF.Types.Interfaces; 
    using YAF.Classes.Data;
    using YAF.Utils;
    using YAF.Types;

    #endregion

    /// <summary>
    /// The yaf profile db conn manager.
    /// </summary>
    public class MsSqlProfileDbConnectionProvider : MsSqlDbConnectionProvider
    {
        #region Properties

        /// <summary>
        ///   Gets ConnectionString.
        /// </summary>
        public override string ConnectionString
        {
            get
            {
                if (YafContext.Application[YafProfileProvider.ConnStrAppKeyName] != null)
                {
                    return YafContext.Application[YafProfileProvider.ConnStrAppKeyName] as string;
                }

                return Config.ConnectionString;
            }
        }

        #endregion
    }

    /// <summary>
    /// The db.
    /// </summary>
    public class DB
    {
        #region Constants and Fields

        /// <summary>
        ///   The _db access.
        /// </summary>
        private readonly IDbAccess _dbAccess = new MsSqlDbAccess(new MsSqlProfileDbConnectionProvider());

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DB" /> class.
        /// </summary>
        public DB()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Current.
        /// </summary>
        public static DB Current
        {
            get
            {
                return PageSingleton<DB>.Instance;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The add profile column.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="columnType">
        /// The column type.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        public void AddProfileColumn([NotNull] string name, SqlDbType columnType, int size)
        {
            // get column type...
            string type = columnType.ToString();

            if (size > 0)
            {
                type += "(" + size + ")";
            }

            string sql = "ALTER TABLE {0} ADD [{1}] {2} NULL".FormatWith(MsSqlDbAccess.GetObjectName("prov_Profile"), name, type);

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                this._dbAccess.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The delete inactive profiles.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="inactiveSinceDate">
        /// The inactive since date.
        /// </param>
        /// <returns>
        /// The delete inactive profiles.
        /// </returns>
        public int DeleteInactiveProfiles([NotNull] object appName, [NotNull] object inactiveSinceDate)
        {
            using (var cmd = this._dbAccess.GetCommand("prov_profile_deleteinactive"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ApplicationName", appName);
                cmd.AddParam("InactiveSinceDate", inactiveSinceDate);
                return Convert.ToInt32(this._dbAccess.ExecuteScalar(cmd));
            }
        }

        /// <summary>
        /// The delete profiles.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="userNames">
        /// The user names.
        /// </param>
        /// <returns>
        /// The delete profiles.
        /// </returns>
        public int DeleteProfiles([NotNull] object appName, [NotNull] object userNames)
        {
            using (var cmd = this._dbAccess.GetCommand("prov_profile_deleteprofiles"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ApplicationName", appName);
                cmd.AddParam("UserNames", userNames);
                return Convert.ToInt32(this._dbAccess.ExecuteScalar(cmd));
            }
        }

        /// <summary>
        /// The get number inactive profiles.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="inactiveSinceDate">
        /// The inactive since date.
        /// </param>
        /// <returns>
        /// The get number inactive profiles.
        /// </returns>
        public int GetNumberInactiveProfiles([NotNull] object appName, [NotNull] object inactiveSinceDate)
        {
            using (var cmd = this._dbAccess.GetCommand("prov_profile_getnumberinactiveprofiles"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ApplicationName", appName);
                cmd.AddParam("InactiveSinceDate", inactiveSinceDate);
                return Convert.ToInt32(this._dbAccess.ExecuteScalar(cmd));
            }
        }

        /// <summary>
        /// The get profile structure.
        /// </summary>
        /// <returns>
        /// </returns>
        public DataTable GetProfileStructure()
        {
            string sql = @"SELECT TOP 1 * FROM {0}".FormatWith(MsSqlDbAccess.GetObjectName("prov_Profile"));

            using (var cmd = new SqlCommand(sql))
            {
                cmd.CommandType = CommandType.Text;
                return this._dbAccess.GetData(cmd);
            }
        }

        /// <summary>
        /// The get profiles.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="userNameToMatch">
        /// The user name to match.
        /// </param>
        /// <param name="inactiveSinceDate">
        /// The inactive since date.
        /// </param>
        /// <returns>
        /// </returns>
        public DataSet GetProfiles([NotNull] object appName, [NotNull] object pageIndex, [NotNull] object pageSize, [NotNull] object userNameToMatch, [NotNull] object inactiveSinceDate)
        {
            using (var cmd = this._dbAccess.GetCommand("prov_profile_getprofiles"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ApplicationName", appName);
                cmd.AddParam("PageIndex", pageIndex);
                cmd.AddParam("PageSize", pageSize);
                cmd.AddParam("UserNameToMatch", userNameToMatch);
                cmd.AddParam("InactiveSinceDate", inactiveSinceDate);
                return this._dbAccess.GetDataset(cmd);
            }
        }

    public static bool GetDbTypeAndSizeFromString(string providerData, out SqlDbType dbType, out int size)
    {
        return LegacyDb.GetDbTypeAndSizeFromString(providerData, out dbType, out size);
    }
        /// <summary>
        /// The get provider user key.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// The get provider user key.
        /// </returns>
        public object GetProviderUserKey([NotNull] object appName, [NotNull] object username)
        {
            DataRow row = Membership.DB.Current.GetUser(appName.ToString(), null, username.ToString(), false);

            if (row != null)
            {
                return row["UserID"];
            }

            return null;
        }

        /// <summary>
        /// The set profile properties.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="settingsColumnsList">
        /// The settings columns list.
        /// </param>
        public void SetProfileProperties([NotNull] object appName, [NotNull] object userID, [NotNull] SettingsPropertyValueCollection values, [NotNull] List<SettingsPropertyColumn> settingsColumnsList)
        {
            using (var cmd = new SqlCommand())
            {
                string table = MsSqlDbAccess.GetObjectName("prov_Profile");

                StringBuilder sqlCommand = new StringBuilder("IF EXISTS (SELECT 1 FROM ").Append(table);
                sqlCommand.Append(" WHERE UserId = @UserID) ");
                cmd.AddParam("UserID", userID);

                // Build up strings used in the query
                var columnStr = new StringBuilder();
                var valueStr = new StringBuilder();
                var setStr = new StringBuilder();
                int count = 0;

                foreach (SettingsPropertyColumn column in settingsColumnsList)
                {
                    // only write if it's dirty
                    if (values[column.Settings.Name].IsDirty)
                    {
                        columnStr.Append(", ");
                        valueStr.Append(", ");
                        columnStr.Append(column.Settings.Name);
                        string valueParam = "@Value" + count;
                        valueStr.Append(valueParam);
                        cmd.AddParam(valueParam, values[column.Settings.Name].PropertyValue);

                        if (column.DataType != SqlDbType.Timestamp)
                        {
                            if (count > 0)
                            {
                                setStr.Append(",");
                            }

                            setStr.Append(column.Settings.Name);
                            setStr.Append("=");
                            setStr.Append(valueParam);
                        }

                        count++;
                    }
                }

                columnStr.Append(",LastUpdatedDate ");
                valueStr.Append(",@LastUpdatedDate");
                setStr.Append(",LastUpdatedDate=@LastUpdatedDate");
                cmd.AddParam("@LastUpdatedDate", DateTime.UtcNow);

                sqlCommand.Append("BEGIN UPDATE ").Append(table).Append(" SET ").Append(setStr.ToString());
                sqlCommand.Append(" WHERE UserId = '").Append(userID.ToString()).Append("'");

                sqlCommand.Append(" END ELSE BEGIN INSERT ").Append(table).Append(" (UserId").Append(columnStr.ToString());
                sqlCommand.Append(") VALUES ('").Append(userID.ToString()).Append("'").Append(valueStr.ToString()).Append(
                    ") END");

                cmd.CommandText = sqlCommand.ToString();
                cmd.CommandType = CommandType.Text;

                this._dbAccess.ExecuteNonQuery(cmd);
            }
        }

        #endregion

        /*
        public static void ValidateAddColumnInProfile( string columnName, SqlDbType columnType )
        {
            SqlCommand cmd = new SqlCommand( sprocName );
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.AddParam( "@ApplicationName", appName );
            cmd.AddParam( "@Username", username );
            cmd.AddParam( "@IsUserAnonymous", isAnonymous );

            return cmd;
        }
        */

        public static void SetPropertyValues(int boardId, string appname, int userId, SettingsPropertyValueCollection collection, bool dirtyOnly = true)
        {
            if (userId == 0 || collection.Count < 1)
            {
                return;
            }
            bool itemsToSave = true;
            if (dirtyOnly)
            {
                itemsToSave = collection.Cast<SettingsPropertyValue>().Any(pp => pp.IsDirty);
            }

            // First make sure we have at least one item to save

            if (!itemsToSave)
            {
                return;
            }

            // load the data for the configuration
            List<SettingsPropertyColumn> spc = LoadFromPropertyValueCollection(collection);

            if (spc != null && spc.Count > 0)
            {
                // start saving...
                LegacyDb.SetProfileProperties(boardId, appname, userId, collection, spc, dirtyOnly);
            }
        }

        public void SetProfileProperties([NotNull] int boardId, [NotNull] object appName, [NotNull] int userID, [NotNull] SettingsPropertyValueCollection values, [NotNull] List<SettingsPropertyColumn> settingsColumnsList, bool dirtyOnly)
        {
            string userName = string.Empty;
            var dtu = LegacyDb.UserList(boardId, userID, true, null, null, true);
            foreach (var typedUserList in dtu)
            {
                userName = typedUserList.Name;
                break;

            }
            if (userName.IsNotSet())
            {
                return;
            }
            using (var conn = new MsSqlDbConnectionManager().OpenDBConnection)
            {
                var cmd = new SqlCommand();

                cmd.Connection = conn;

                string table = MsSqlDbAccess.GetObjectName("UserProfile");
                StringBuilder sqlCommand = new StringBuilder("IF EXISTS (SELECT 1 FROM ").Append(table);
                sqlCommand.Append(" WHERE UserId = @UserID AND ApplicationName = @ApplicationName) ");
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@ApplicationName", appName);

                // Build up strings used in the query
                var columnStr = new StringBuilder();
                var valueStr = new StringBuilder();
                var setStr = new StringBuilder();
                int count = 0;

                foreach (SettingsPropertyColumn column in settingsColumnsList)
                {
                    // only write if it's dirty
                    if (!dirtyOnly || values[column.Settings.Name].IsDirty)
                    {
                        columnStr.Append(", ");
                        valueStr.Append(", ");
                        columnStr.Append(column.Settings.Name);
                        string valueParam = "@Value" + count;
                        valueStr.Append(valueParam);
                        cmd.Parameters.AddWithValue(valueParam, values[column.Settings.Name].PropertyValue);

                        if ((column.DataType != SqlDbType.Timestamp) || column.Settings.Name != "LastUpdatedDate" || column.Settings.Name != "LastActivity")
                        {
                            if (count > 0)
                            {
                                setStr.Append(",");
                            }

                            setStr.Append(column.Settings.Name);
                            setStr.Append("=");
                            setStr.Append(valueParam);
                        }

                        count++;
                    }
                }

                columnStr.Append(",LastUpdatedDate ");
                valueStr.Append(",@LastUpdatedDate");
                setStr.Append(",LastUpdatedDate=@LastUpdatedDate");
                cmd.Parameters.AddWithValue("@LastUpdatedDate", DateTime.UtcNow);

                // MembershipUser mu = System.Web.Security.Membership.GetUser(userID);

                columnStr.Append(",LastActivity ");
                valueStr.Append(",@LastActivity");
                setStr.Append(",LastActivity=@LastActivity");
                cmd.Parameters.AddWithValue("@LastActivity", DateTime.UtcNow);

                columnStr.Append(",ApplicationName ");
                valueStr.Append(",@ApplicationName");
                setStr.Append(",ApplicationName=@ApplicationName");
                // cmd.Parameters.AddWithValue("@ApplicationID", appId);

                columnStr.Append(",IsAnonymous ");
                valueStr.Append(",@IsAnonymous");
                setStr.Append(",IsAnonymous=@IsAnonymous");
                cmd.Parameters.AddWithValue("@IsAnonymous", 0);

                columnStr.Append(",UserName ");
                valueStr.Append(",@UserName");
                setStr.Append(",UserName=@UserName");
                cmd.Parameters.AddWithValue("@UserName", userName);

                sqlCommand.Append("BEGIN UPDATE ").Append(table).Append(" SET ").Append(setStr.ToString());
                sqlCommand.Append(" WHERE UserId = ").Append(userID.ToString()).Append("");

                sqlCommand.Append(" END ELSE BEGIN INSERT ").Append(table).Append(" (UserId").Append(columnStr.ToString());
                sqlCommand.Append(") VALUES (").Append(userID.ToString()).Append("").Append(valueStr.ToString()).Append(
                  ") END");

                cmd.CommandText = sqlCommand.ToString();
                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetProfileStructure()
        {
            string sql = @"SELECT TOP 1 * FROM {0}".FormatWith(MsSqlDbAccess.GetObjectName("UserProfile"));

            using (var cmd = MsSqlDbAccess.GetCommand(sql, true))
            {
                cmd.CommandType = CommandType.Text;
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        public bool GetDbTypeAndSizeFromString(string providerData, out SqlDbType dbType, out int size)
        {
            size = -1;
            dbType = SqlDbType.NVarChar;

            if (providerData.IsNotSet())
            {
                return false;
            }

            // split the data
            string[] chunk = providerData.Split(new[] { ';' });

            // first item is the column name...
            string columnName = chunk[0];

            // get the datatype and ignore case...
            dbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), chunk[1], true);

            if (chunk.Length > 2)
            {
                // handle size...
                if (!Int32.TryParse(chunk[2], out size))
                {
                    throw new ArgumentException("Unable to parse as integer: " + chunk[2]);
                }
            }

            return true;
        }

        static List<SettingsPropertyColumn> LoadFromPropertyValueCollection(SettingsPropertyValueCollection collection)
        {
            List<SettingsPropertyColumn> settingsColumnsList = new List<SettingsPropertyColumn>();
            // clear it out just in case something is still in there...


            // validiate all the properties and populate the internal settings collection
            foreach (SettingsPropertyValue value in collection)
            {
                SqlDbType dbType;
                int size;

                // parse custom provider data...
                GetDbTypeAndSizeFromString(
                   value.Property.Attributes["CustomProviderData"].ToString(), out dbType, out size);

                // default the size to 256 if no size is specified
                if (dbType == SqlDbType.NVarChar && size == -1)
                {
                    size = 256;
                }

                settingsColumnsList.Add(new SettingsPropertyColumn(value.Property, dbType, size));
            }

            // sync profile table structure with the db...
            DataTable structure = LegacyDb.GetProfileStructure();

            // verify all the columns are there...
            foreach (SettingsPropertyColumn column in settingsColumnsList)
            {
                // see if this column exists
                if (!structure.Columns.Contains(column.Settings.Name))
                {
                    // if not, create it...
                    LegacyDb.AddProfileColumn(column.Settings.Name, column.DataType, column.Size);
                }
            }
            return settingsColumnsList;
        }
    }
}