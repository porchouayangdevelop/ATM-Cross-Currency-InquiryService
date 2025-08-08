

using Google.Apis.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

/// <summary>
/// Summary description for Service1
/// </summary>
[WebService(Namespace = "http://www.apb.com.la/", Name = "SMS4ATM_UAT")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Service1 : System.Web.Services.WebService
{

    private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    [WebMethod]
    public string ExchangeRate()
    {
        IExchangeRate exchange = Factory.GetInstanceECHR();
        return exchange.ExchangeRate();
    }


    [WebMethod]
    public string Interest_Rate()
    {
        IInterestRate interest = Factory.GetInstanceINTR();
        return interest.Interest_Rate();
    }


    [WebMethod]
    public String QueryAccName(String ATMID, String AccNum)
    {
        QueryAccName q = Factory.GetInstance();
        return q.QueryAccName(ATMID.Trim()?.ToUpper()?.ToString(), AccNum.Trim()?.ToString());
    }


    [WebMethod]
    public String DepositAccName(String Deposit_ccy, String AccNum)
    {
        IDeposit deposit = Factory.DepositAccName();

        logger.Info($"Request {Deposit_ccy}, {AccNum}");
        string result = deposit.ProcessDeposit(Deposit_ccy, AccNum);
        logger.Info(result);
        return result;
    }

}
