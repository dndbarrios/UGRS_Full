// file:	Connection\Connection.cs
// summary:	Implements the connection class

namespace Core.SDK.Connection
{
    /// <summary> A connection. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    public class Connection : IConnection
    {

        /// <summary> The connection string. </summary>
        protected string ConnectionString;

        /// <summary> Gets connection string. </summary>
        /// <remarks> Ranaya, 04/05/2017. </remarks>
        /// <returns> The connection string. </returns>

        public string GetConnectionString()
        {
            return this.ConnectionString;
        }
    }
}
