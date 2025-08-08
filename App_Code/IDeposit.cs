using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// Summary description for DepositInterface
/// </summary>
public interface IDeposit
{
    string ProcessDeposit(string Deposit_ccy, string AccNum);
}