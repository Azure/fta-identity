using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Contoso.Intranet.WebApp
{
    public partial class _Default : Page
    {
        protected readonly Dictionary<string, string> UserInformation = new Dictionary<string, string>();
        protected readonly Dictionary<string, string> DatabaseInformation = new Dictionary<string, string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get current user information.
            try
            {
                foreach (var claim in ((System.Security.Claims.ClaimsIdentity)User.Identity).Claims)
                {
                    this.UserInformation[claim.Type] = claim.Value;
                }
            }
            catch (Exception exc)
            {
                this.UserInformation["ExceptionMessage"] = exc.Message;
                this.UserInformation["ExceptionDetails"] = exc.ToString();
            }

            // Get database information.
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
                this.DatabaseInformation["ConnectionString"] = connectionString;
                using (var sqlConnection = new SqlConnection(connectionString))
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "SELECT @@VERSION, CURRENT_USER, ORIGINAL_LOGIN(), SUSER_SNAME()";
                    sqlConnection.Open();
                    using (var reader = sqlCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            this.DatabaseInformation["@@VERSION"] = reader.GetString(0);
                            this.DatabaseInformation["CURRENT_USER"] = reader.GetString(1);
                            this.DatabaseInformation["ORIGINAL_LOGIN"] = reader.GetString(2);
                            this.DatabaseInformation["SUSER_SNAME"] = reader.GetString(3);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                this.DatabaseInformation["ExceptionMessage"] = exc.Message;
                this.DatabaseInformation["ExceptionDetails"] = exc.ToString();
            }
        }
    }
}