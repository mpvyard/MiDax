﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MidaxLib;

namespace MidaxTester
{
    public class ANN
    {
        public static void Run(List<DateTime> dates, bool generate = false, bool generate_from_db = false, bool publish_to_db = false, bool use_uat_db = false, bool fullday = false)
        {
            TestEngine testEngine = new TestEngine("ANN", dates, generate, generate_from_db, publish_to_db, use_uat_db, fullday);
            testEngine.Settings["TRADING_SIGNAL"] = "ANN_WMA_5_2_1_IX.D.DAX.DAILY.IP";
            var models = new List<Model>();
            models.Add(new ModelMacDTest(new MarketData("DAX:IX.D.DAX.DAILY.IP"), 10, 30, 90));
            List<MarketData> otherIndices = new List<MarketData>();
            otherIndices.Add(new MarketData("CAC:IX.D.CAC.DAILY.IP"));
            models.Add(new ModelANN((ModelMacD)models[0], null, null, otherIndices));
            testEngine.Run(models);
        }
    }
}
