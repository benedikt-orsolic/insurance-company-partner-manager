
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;

namespace insurance_company_partner_manager.Services;

public class PartnerDbService(IConfiguration appsettings)
{
    public readonly SQLiteConnection connection = new(@$"{appsettings.GetConnectionString("PartnerDb")}");

    private bool _isConnected = false;

    public void Connect()
    {
        if (!this._isConnected)
        {
            this.connection.Open();
            this._isConnected = true;
        }
    }

    public void Disconnect()
    {
        // Should wait for all query to finish before closing.
        this.connection.Close();
        this._isConnected = false;
    }
}