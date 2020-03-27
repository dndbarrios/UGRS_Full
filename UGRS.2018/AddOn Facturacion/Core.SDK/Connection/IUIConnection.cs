// file:	Connection\IUIConnection.cs
// summary:	Declares the IUIConnection interface

using SAPbouiCOM;

namespace Core.SDK.Connection
{
    /// <summary> Interface for UI connection. </summary>
    /// <remarks> Ranaya, 04/05/2017. </remarks>

    public interface IUIConnection : IConnection
    {
        Application GetApplication();
    }
}
