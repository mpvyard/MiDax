﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using dto.endpoint.positions.create.otc.v2;
using MidaxLib;
using NLapack.Matrices;
using NLapack.Numbers;

namespace MidaxTester
{   
    class Program
    {
        /*
        static async void tmp()
        {
            try
            {
                CreatePositionRequest cpr = new CreatePositionRequest();
                cpr.epic = "IX.D.DAX.DAILY.IP";
                DateTime expiry = DateTime.Today.AddDays(int.Parse(Config.Settings["TRADE_EXPIRY_DAYS"]));
                cpr.expiry = expiry.ToString("dd-MMM-yy").ToUpper();
                SIGNAL_CODE code = SIGNAL_CODE.SELL;
                cpr.direction = code.ToString();
                cpr.size = 10;
                cpr.orderType = "MARKET";
                cpr.guaranteedStop = false;
                cpr.forceOpen = false;
                cpr.currencyCode = Config.Settings["TRADE_CURRENCY"];
                IGTradingStreamingClient igApiClient = new IGTradingStreamingClient();
                igApiClient.Connect("ksbitlsoftdemo", "Kotik0483", "8d341413c2eae2c35bb5b47a594ef08ae18cb3b7");            
                var createPositionResponse = await igApiClient._igRestApiClient.createPositionV2(cpr);
                Log.Instance.WriteEntry("Yes");
            }
            catch(Exception e)
            {
                Log.Instance.WriteEntry("Nooo");
            }
        }*/

        static void Main(string[] args)
        {      
            List<string> tests = new List<string>();
            tests.Add(@"..\..\expected_results\mktdata_26_8_2015.csv");

            Dictionary<string, string> dicSettings = new Dictionary<string, string>();
            dicSettings["APP_NAME"] = "Midax";
            dicSettings["PUBLISHING_START_TIME"] = "2015-08-26 00:00:01";
            dicSettings["PUBLISHING_STOP_TIME"] = "2015-08-26 23:59:59";
            dicSettings["PUBLISHING_CONTACTPOINT"] = "192.168.1.26";
            //dicSettings["PUBLISHING_CSV"] = @"..\..\expected_results\new_results.csv";   // uncomment this line to generate new test results
            dicSettings["REPLAY_MODE"] = "CSV";
            dicSettings["REPLAY_CSV"] = Config.TestList(tests);
            dicSettings["REPLAY_POPUP"] = "1";
            dicSettings["TRADING_START_TIME"] = "2015-08-26 08:00:00";
            dicSettings["TRADING_STOP_TIME"] = "2015-08-26 09:00:00";
            dicSettings["TRADING_CLOSING_TIME"] = "2015-08-26 08:57:00";
            dicSettings["TRADING_MODE"] = "REPLAY";
            dicSettings["TRADING_SIGNAL"] = "MacD_10_60_IX.D.DAX.DAILY.IP";
            dicSettings["TRADING_LIMIT_PER_BP"] = "10";
            dicSettings["TRADE_EXPIRY_DAYS"] = "1";
            dicSettings["TRADE_CURRENCY"] = "GBP";
            Config.Settings = dicSettings;

            //tmp();

            Console.WriteLine("Testing calibration...");

            // Test the optimization of function a * cos(b * x) + b * sin(a * x) using Levenberg Marquardt
            LevenbergMarquardt.objective_func objFunc = (NRealMatrix x) => { NRealMatrix y = new NRealMatrix(x.Rows, 1);
                                                 for (int idxRow = 0; idxRow < y.Rows; idxRow++)
                                                     y.SetAt(idxRow, 0, new NDouble(2 * Math.Cos(x[idxRow, 0]) + Math.Sin(2 * x[idxRow, 0])));
                                                 return y; };
            List<double> inputs = new List<double>();
            Random rnd = new Random(155);    
            for (int idxPt = 0; idxPt < 10; idxPt++)
                inputs.Add(rnd.NextDouble() * 2);
            List<Value> modelParams = new List<Value>();
            modelParams.Add(new Value(-0.2)); modelParams.Add(new Value(0.3));
            LevenbergMarquardt.model_func modelFunc = (NRealMatrix x, NRealMatrix weights) => { NRealMatrix y = new NRealMatrix(x.Rows, 1);
                                                double a = weights[0, 0]; double b = weights[0, 1];                                                
                                                for (int idxRow = 0; idxRow < y.Rows; idxRow++)
                                                     y.SetAt(idxRow, 0, new NDouble(a * Math.Cos(b * x[idxRow, 0]) + b * Math.Sin(a * x[idxRow, 0])));
                                                return y; };
            Func<double,double,double,double> derA = (double a, double b, double x) => Math.Cos(b * x) + b * x * Math.Cos(a * x);
            Func<double,double,double,double> derB = (double a, double b, double x) => - a * x * Math.Sin(b * x) + Math.Sin(a * x);
            LevenbergMarquardt.model_func jacFunc = (NRealMatrix x, NRealMatrix weights) =>
            {
                NRealMatrix jac = new NRealMatrix(x.Rows, 2);
                double a = weights[0, 0]; double b = weights[0, 1];
                for (int idxRow = 0; idxRow < jac.Rows; idxRow++)
                {
                    jac.SetAt(idxRow, 0, new NDouble(-derA(a, b, x[idxRow, 0])));
                    jac.SetAt(idxRow, 1, new NDouble(-derB(a, b, x[idxRow, 0])));
                }
                return jac; 
            };
            LevenbergMarquardt calibModel = new LevenbergMarquardt(objFunc, inputs, modelParams, modelFunc, jacFunc);
            calibModel.Solve();
            if (Math.Abs(modelParams[0].X - 2) > calibModel.ObjectiveError || Math.Abs(modelParams[1].X - 1) > calibModel.ObjectiveError)
                throw new ApplicationException("LevenbergMarquardt calibration error");

            // Parity-2 problem
            NeuralNetwork ann = new NeuralNetwork(2, 1, new List<int>() { 2 });
            List<List<double>> annInputs = new List<List<double>>();
            annInputs.Add(new List<double>() { -1, -1 });
            annInputs.Add(new List<double>() { -1, 1 });
            annInputs.Add(new List<double>() { 1, -1 });
            annInputs.Add(new List<double>() { 1, 1 });
            List<List<double>> annOutputs = new List<List<double>>();
            annOutputs.Add(new List<double>() { 1 });
            annOutputs.Add(new List<double>() { -1 });
            annOutputs.Add(new List<double>() { -1 });
            annOutputs.Add(new List<double>() { 1 });
            // test forward propagation
            ann._outputs.Neurons[0].Weights[0].X = 1;
            ann._outputs.Neurons[0].Weights[1].X = -1;
            ann._outputs.Neurons[0].Weights[2].X = -1;
            ann._innerLayers[0].Neurons[0].Weights[0].X = 1;
            ann._innerLayers[0].Neurons[0].Weights[1].X = 1;
            ann._innerLayers[0].Neurons[0].Weights[2].X = 1;
            ann._innerLayers[0].Neurons[1].Weights[0].X = 1;
            ann._innerLayers[0].Neurons[1].Weights[1].X = 1;
            ann._innerLayers[0].Neurons[1].Weights[2].X = -1;
            ann._inputs.Neurons[0].Value.X = -1;
            ann._inputs.Neurons[1].Value.X = -1;
            if (Math.Abs(ann._outputs.Neurons[0].Activation() - -0.38873457229297215) > calibModel.ObjectiveError)
                throw new ApplicationException("Neural network forward propagation error");
            // Test neural network training for parity-2 problem
            ann = new NeuralNetwork(2, 1, new List<int>() { 2 });
            ann.Train(annInputs, annOutputs);

            // Test neural network training for parity-3 problem
            ann = new NeuralNetwork(3, 1, new List<int>() { 2 });
            annInputs = new List<List<double>>();
            annInputs.Add(new List<double>() {-1,-1,-1});
            annInputs.Add(new List<double>() {-1,-1, 1});
            annInputs.Add(new List<double>() {-1, 1,-1});
            annInputs.Add(new List<double>() {-1, 1, 1});
            annInputs.Add(new List<double>() { 1,-1,-1});
            annInputs.Add(new List<double>() { 1,-1, 1});
            annInputs.Add(new List<double>() { 1, 1,-1});
            annInputs.Add(new List<double>() { 1, 1, 1});
            annOutputs = new List<List<double>>();
            annOutputs.Add(new List<double>() { -1 });
            annOutputs.Add(new List<double>() {  1 });
            annOutputs.Add(new List<double>() {  1 });
            annOutputs.Add(new List<double>() { -1 });
            annOutputs.Add(new List<double>() {  1 });
            annOutputs.Add(new List<double>() { -1 });
            annOutputs.Add(new List<double>() { -1 });
            annOutputs.Add(new List<double>() {  1 });
            ann.Train(annInputs, annOutputs);

            MarketDataConnection.Instance.Connect(null);
            
            MarketData index = new MarketData("DAX:IX.D.DAX.DAILY.IP");

            List<MarketData> marketData = new List<MarketData>();
            marketData.Add(new MarketData("Adidas AG:ED.D.ADSGY.DAILY.IP"));
            marketData.Add(new MarketData("Allianz SE:ED.D.ALVGY.DAILY.IP"));
            marketData.Add(new MarketData("BASF SE:ED.D.BAS.DAILY.IP"));
            marketData.Add(new MarketData("Bayer AG:ED.D.BAY.DAILY.IP"));

            ModelTest model = new ModelTest(index, marketData);
            Console.WriteLine("Testing live indicators and signals...");
            model.StartSignals();
            Console.WriteLine("Testing daily indicators...");

            if (!dicSettings.ContainsKey("PUBLISHING_CSV"))
            {
                // Test exceptions. the program is expected to throw exceptions here, just press continue if you are debugging
                // all exceptions should be handled, and the program should terminate with a success message box
                Console.WriteLine("Testing expected exceptions...");
                string expected;
                List<string> testError = new List<string>();
                testError.Add(@"..\..\expected_results\mktdata_26_8_2015_error.csv");
                dicSettings["REPLAY_CSV"] = Config.TestList(testError);
                MarketDataConnection.Instance.Connect(null);                
                bool success = false;
                var modelErr = new ModelTest(index, marketData);
                try
                {
                    modelErr.StartSignals();
                }
                catch (Exception exc)
                {
                    expected = "Time series do not accept values in the past";
                    success = (exc.Message == expected);
                    if (!success)
                        model.ProcessError(exc.Message, expected);
                }
                if (!success)
                    model.ProcessError("An expected exception has not been thrown");
                try
                {
                    modelErr.StopSignals();
                }
                catch (Exception exc)
                {
                    expected = "Test failed: indicator WMA_1D_IX.D.DAX.DAILY.IP time 23:59 expected value 9964.360169 != 9973.319141068080279635978306";
                    success = (exc.Message == expected);
                    if (!success)
                        model.ProcessError(exc.Message, expected);
                }
                try
                {
                    model.StopSignals();
                }
                catch (Exception exc)
                {
                    expected = "Test failed: indicator WMA_1D_IX.D.DAX.DAILY.IP time 23:59 expected value 9964.360169 != 9973.319141068080279635978306";
                    success = (exc.Message == expected);
                    if (!success)
                        model.ProcessError(exc.Message, expected);
                }
                success = false;
                try
                {
                    MarketDataConnection.Instance = new ReplayConnection();
                    MarketDataConnection.Instance.Connect(null);
                    model = new ModelTest(new MarketData(index.Id), (from data in marketData select new MarketData(data.Id)).ToList());
                    model.StartSignals();
                }
                catch (Exception exc)
                {
                    expected = "Test failed: indicator WMA_1_IX.D.DAX.DAILY.IP time 08:41 expected value 9975.133333 != 9975.356666666666666666666746";
                    success = (exc.Message == expected);
                    if (!success)
                        model.ProcessError(exc.Message, expected);
                }
                if (!success)
                    model.ProcessError("An expected exception has not been thrown");
                success = false;
                try
                {
                    model.StopSignals();
                }
                catch (Exception exc)
                {
                    expected = "Test failed: indicator WMA_1D_IX.D.DAX.DAILY.IP time 23:59 expected value 9964.360169 != 9982.822762679691659529031265";
                    success = (exc.Message == expected);
                    if (!success)
                        model.ProcessError(exc.Message, expected);
                }
                if (!success)
                    model.ProcessError("An expected exception has not been thrown");
                success = false;
            }
            string statusSuccess = "Tests passed successfully";
            Console.WriteLine(statusSuccess);

            if (dicSettings["REPLAY_POPUP"] == "1")
                MessageBox.Show(statusSuccess);
        }        
    }
}
