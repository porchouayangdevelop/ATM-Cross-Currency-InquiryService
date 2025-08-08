using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ExchangeRateDTO
/// </summary>
public class ExchangeRateInQuiryService : IExchangeRate
{


    private dbConfig con;
    private saveLog log;


    public ExchangeRateInQuiryService()
    {
        this.con = new dbConfig();
        this.log = new saveLog();
    }

    public string ExchangeRate()
    {


        //System.Diagnostics.Debug.WriteLine(string.Concat(spit[0], '.', spit[1], spit[2], spit[3]));



        var response = "";
        var USDOnetoTwentyBuy = ""; var USDOnetoTwentySell = "";
        var USDFitytoHundredBuy = ""; var USDFitytoHundredSell = "";
        var fullUSDBuy = ""; var fullUSDSell = "";
        var THBBuy = ""; var THBSell = "";
        var CNYBuy = ""; var CNYSell = "";
        var EURBuy = ""; var EURSell = "";
        var VNDBuy = ""; var VNDSell = "";
        var dtDb = "";




        var dt = DateTime.Now;
        try
        {
            using (var call = con.GetConnection())
            {
                call.Open();
                using (var cmd = new MySqlCommand("getExchangeRate", call))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dtDb = reader.GetString(0)?.ToString();
                            USDOnetoTwentyBuy = $"{reader.GetString(1)?.Trim()?.ToString()}"; USDOnetoTwentySell = $"{reader.GetString(2)?.Trim().ToString()}";
                            USDFitytoHundredBuy = $"{reader.GetString(3)?.Trim().ToString()}"; USDFitytoHundredSell = $"{reader.GetString(4)?.Trim()?.ToString()}";
                            fullUSDBuy = $"{reader.GetString(5)?.Trim()?.ToString()}"; fullUSDSell = $"{reader.GetString(6)?.Trim()?.ToString()}";
                            THBBuy = $"{reader.GetString(7)?.Trim()?.ToString()}"; THBSell = $"{reader.GetString(8)?.Trim()?.ToString()}";
                            EURBuy = $"{reader.GetString(9)?.Trim()?.ToString()}"; EURSell = $"{reader.GetString(10)?.Trim()?.ToString()}";
                            CNYBuy = $"{reader.GetString(11)?.Trim()?.ToString()}"; CNYSell = $"{reader.GetString(12)?.Trim()?.ToString()}";
                            VNDBuy = $"{reader.GetString(13)?.Trim()?.ToString()}"; VNDSell = $"{reader.GetString(14)?.Trim()?.ToString()}";
                        }
                    }
                }

                //B = Buy; S= Sell;
                int usd1_20B = int.Parse(USDOnetoTwentyBuy); int usd1_20S = int.Parse(USDOnetoTwentySell);
                int usd5_100B = int.Parse(USDOnetoTwentyBuy); int usd5_100S = int.Parse(USDFitytoHundredSell);
                int fullUSDB = int.Parse(fullUSDBuy); int fullUSDS = int.Parse(fullUSDSell);
                int eurB = int.Parse(EURBuy); int eurS = int.Parse(EURSell);
                int cnyB = int.Parse(CNYBuy); int cnyS = int.Parse(CNYSell);


                var usd1B = usd1_20B.ToString("C").Substring(1, 6).Replace(',', '.'); var usd1S = usd1_20S.ToString("C").Substring(1, 6).Replace(',', '.');
                var usd5B = usd5_100B.ToString("C").Substring(1, 6).Replace(',', '.'); var usd5S = usd5_100S.ToString("C").Substring(1, 6).Replace(',', '.');
                var fUSDB = fullUSDB.ToString("C").Replace(',', '.').Substring(1, 6); var fUSDS = fullUSDS.ToString("C").Replace(',', '.').Substring(1, 6);
                var eur_B = eurB.ToString("C").Substring(1, 6).Replace(',', '.'); var eur_S = eurS.ToString("C").Substring(1, 6).Replace(',', '.');
                var cny_B = cnyB.ToString("N").Replace(',', '.').Substring(0, 5); var cny_S = cnyS.ToString("N").Replace(',', '.').Substring(0, 5);

                //System.Diagnostics.Debug.WriteLine($"{usd1S}|{usd1B};" +
                //    $"{usd5S}|{usd5B};" +
                //    $"{eur_S}|{eur_B};" +
                //    $"{cny_S}|{cny_B}");


                System.Diagnostics.Debug.WriteLine($"{dtDb};" +
                    //$"USD||{usd1S}|{usd1B};(1-20)||||;" +
                    //$"USD||{usd5S}|{usd5B};(50-100)||||;" +
                    $"USD||{fUSDB}|{fUSDS};" +
                    $"{THBBuy}{THBSell}" +
                    $"EUR|_||{eur_B}|{eur_S};" +
                    $"CNY|_||{cny_B} | {cny_S};" +
                    $"{VNDBuy}{VNDSell}");


                response = $"{dtDb};" +
                    //$"USD||{usd1S}|{usd1B};(1-20)||||;" +
                    //$"USD||{usd5S}|{usd5B};(50-100)||||;" +
                    $";" +
                    $" USD|{fUSDB}|_|{fUSDS}|_;" +
                    $" THB|{THBBuy}|_|{THBSell}|_;" +
                    $" EUR|{eur_B}|_|{eur_S}|_;" +
                    $" CNY|{cny_B}|_|{cny_S}|_;" +
                    $" VND|{VNDBuy}|_|{VNDSell}|_";


                call.Dispose();

                var param = new MySqlParameter[]
                {
                    new MySqlParameter("log",response)
                };

                //log.InsertData("proc_logExchangRate", param); 


            }
        }
        catch (MySqlException e)
        {
            System.Diagnostics.Debug.WriteLine(e.SqlState);
        }
        catch (NullReferenceException ex)
        {
            System.Diagnostics.Debug.WriteLine($"{ex.Message}");
        }
        catch (FormatException e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }
        return response;
    }

}