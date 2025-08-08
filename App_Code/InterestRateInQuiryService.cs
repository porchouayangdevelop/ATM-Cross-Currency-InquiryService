using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

/// <summary>
/// Summary description for Interest_RateDTO
/// </summary>
public class InterestRateInQuiryService : IInterestRate
{
    dbConfig dbConfig;

    public InterestRateInQuiryService()
    {
        this.dbConfig = new dbConfig();
    }

    public string Interest_Rate()
    {

        String response = "";
        String savingLAK = "";
        String savingUSD_THB = "";
        String m3 = "";
        String m6 = "";
        String m12 = "";
        String m24 = "";
        String m36 = "";
        String m48 = "";
        String m60 = "";

        try
        {
            using (var con = dbConfig.GetConnection())
            {
                con.Open();
                using (var cmd = new MySqlCommand("getInterestRate", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            savingLAK = reader.GetString(0)?.Trim()?.ToString();
                            savingUSD_THB = reader.GetString(1)?.Trim()?.ToString();
                            m3 = reader.GetString(2)?.Trim()?.ToString();
                            m6 = reader.GetString(3)?.Trim()?.ToString();
                            m12 = reader.GetString(4)?.Trim()?.ToString();
                            m24 = reader.GetString(5)?.Trim()?.ToString();
                            m36 = reader.GetString(6)?.Trim()?.ToString();
                            m48 = reader.GetString(7)?.Trim()?.ToString();
                            m60 = reader.GetString(8)?.Trim()?.ToString();
                        }

                        response = $"{savingLAK};{savingUSD_THB}|{m3}|{m6}|{m12}|{m24}|{m36}|{m48}|{m60}|";
                 
                     System.Diagnostics.Debug.WriteLine($"response: {response}");
                    }
                }
                con.Dispose();
            }

        }
        catch (MySqlException e)
        {
            System.Diagnostics.Debug.WriteLine(e.StackTrace);
        }
        return response;

    }
}