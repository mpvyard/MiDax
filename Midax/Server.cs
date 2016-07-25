﻿using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidaxLib;
using Midax;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
using NLapack.Matrices;
using System.Runtime.InteropServices;

public class Server
{
    public class App : Ice.Application
    {
        Trader _trader;        

        public override int run(string[] args)
        {           
            try
            {
                if (args.Length != 0)
                    throw new ApplicationException("starting: too many arguments in application call.");

                Log.APPNAME = Midax.Properties.Settings.Default.APP_NAME;
                
                Dictionary<string, string> dicSettings = new Dictionary<string, string>();
                List<string> stockList = new List<string>();
                foreach (SettingsPropertyValue prop in Midax.Properties.Settings.Default.PropertyValues)
                {
                    if (prop.Name == "STOCKS")
                    {
                        string[] stockArray = new string[100];
                        ((StringCollection)prop.PropertyValue).CopyTo(stockArray, 0);
                        foreach (string stock in stockArray)
                        {
                            if (stock != null && stock != "")
                                stockList.Add(stock);
                        }
                    }
                    else
                        dicSettings.Add(prop.Name, (string)prop.PropertyValue);
                }
                Config.Settings = dicSettings;
            
                Ice.ObjectAdapter adapter = communicator().createObjectAdapter("MidaxIce");
                Ice.Properties properties = communicator().getProperties();
                Ice.Identity id = communicator().stringToIdentity(properties.getProperty("Identity"));

                Thread.Sleep(10000);

                var index = IceStreamingMarketData.Instance;
                var tradingIndex = new MarketData(dicSettings["INDEX_DOW"]);
                var dax = new MarketData(dicSettings["INDEX_DAX"]);
                var gbpusd = new MarketData(dicSettings["FX_GBPUSD"]);
                List<MarketData> otherIndices = new List<MarketData>();
                otherIndices.Add(new MarketData(dicSettings["INDEX_CAC"]));
                var models = new List<Model>();
                var macD_10_30_90_dax = new ModelMacD(dax, 10, 30, 90);
                var macD_10_30_90_gbpusd = new ModelMacD(gbpusd, 10, 30, 90);
                var macDV_10_30_90_dow = new ModelMacDV(index, 10, 30, 90, tradingIndex);
                models.Add(macD_10_30_90_dax);
                models.Add(macD_10_30_90_gbpusd);
                models.Add(macDV_10_30_90_dow);
                models.Add(new ModelANN(macD_10_30_90_dax, null, null, otherIndices));
                models.Add(new ModelMacDCascade(macDV_10_30_90_dow));
                models.Add(new ModelMole(macD_10_30_90_dax));
                models.Add(new ModelMole(macD_10_30_90_gbpusd));
                _trader = new Trader(models, communicator().shutdown); 
                _trader.Init(Config.GetNow);

                adapter.add(new MidaxIceI(_trader, properties.getProperty("Ice.ProgramName")), id);
                adapter.activate();
                communicator().waitForShutdown();
            }
            catch (SEHException exc)
            {
                Log.Instance.WriteEntry("Midax server interop error: " + exc.ToString() + ", Error code: " + exc.ErrorCode, EventLogEntryType.Error);
            }
            catch (Exception exc)
            {
                Log.Instance.WriteEntry("Midax server error: " + exc.ToString(), EventLogEntryType.Error);
            }

            return 0;
        }             
    }

    static public int Main(string[] args)
    {
        App app = new App();
        return app.main(args);
    }
}

