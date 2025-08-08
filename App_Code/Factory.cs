using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Factory
/// </summary>
public class Factory
{

    public static IExchangeRate GetInstanceECHR() {
        return new ExchangeRateInQuiryService();
    }

    public static IInterestRate GetInstanceINTR()
    { 
        return new InterestRateInQuiryService();
    }

    public static QueryAccName GetInstance()
    {
        return new QueryAccNameService();
    }

    public static ITransfer GetTransferInstance()
    {
        return new TransferInQuiryService();
    }

    public static IDeposit  DepositAccName() {
        return new DepositInQuiryService();
    }

}