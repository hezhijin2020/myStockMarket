using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HZJ.StockMarket
{
    public class StockMarket
    {
        /// <summary>
        /// 时分K线
        /// </summary>
        private static readonly string  MinKLine = "http://image.sinajs.cn/newchart/min/n/{0}.gif";
        /// <summary>
        /// 日K线
        /// </summary>
        private static readonly string  TodayKLine = "http://image.sinajs.cn/newchart/daily/n/{0}.gif";
        /// <summary>
        /// 周K线
        /// </summary>
        private static readonly string WeekKLine = "http://image.sinajs.cn/newchart/weekly/n/{0}.gif";
        /// <summary>
        /// 月K线
        /// </summary>
        private static readonly string MonthlyKLine = "http://image.sinajs.cn/newchart/monthly/n/{0}.gif";
        /// <summary>
        /// 获取时时信息
        /// </summary>
        private static readonly string JsonUrl = "http://hq.sinajs.cn/list=";

        /// <summary>
        /// 获取股票时实信息
        /// </summary>
        /// <param name="Codes">股票代码集用 ',' 分隔</param>
        /// <param name="IsSimple">是否简单模式</param>
        /// <returns></returns>
        public static List<StockModel> GetStockInfo(string Codes, bool IsSimple)
        {
            List<StockModel> dic = new List<StockModel>();
            if (IsSimple)
            {
                Codes = ToSimlpeCodes(Codes);
            }
            
            string RequestUrl = GetRequstUrl(Codes);
            var jsons = RequsetJson(RequestUrl);
            if (jsons == "")
                return null;
            var Ary = jsons.Split(';');
            foreach (string json in Ary)
            {
                if (json == ""||json.Length<=10)
                    continue;
                StockModel m =IsSimple? SimpleJsonToStockModel(json): JsonToStockModel(json);
                dic.Add(m);
            }
            return dic;
        }

        /// <summary>
        /// 转成简单模式的COde
        /// </summary>
        /// <param name="Codes">代码集</param>
        /// <returns></returns>
        private static string ToSimlpeCodes(string Codes)
        {
            string[] ary = Codes.Split(',');
            for(int i=0;i<ary.Length;i++)
            {
                ary[i] = "s_" + ary[i];
            }
           return String.Join(",", ary);
        }
         
        /// <summary>
        ///获取实时记录信息
        /// </summary>
        /// <param name="Codes">代码集</param>
        /// <param name="IsSimple">是否简单模式</param>
        /// <returns></returns>
        public static List<StockModel> GetStockInfo(List<string> Codes, bool IsSimple = false)
        {
            List<StockModel> dic = new List<StockModel>();
           
            foreach (string c in Codes)
            {
                string RequestUrl = GetRequstUrl(c,IsSimple);
                var json = RequsetJson(RequestUrl);
                if (json == "")
                    return null;
                 StockModel m = IsSimple? SimpleJsonToStockModel(json): JsonToStockModel(json);
                dic.Add(m);
            }
            return dic;
        }

       /// <summary>
       /// 根据地址发送请求，返回请求的JSON数据
       /// </summary>
       /// <param name="requstUrl">请求地址</param>
       /// <returns></returns>
        private static string RequsetJson(string requstUrl)
        {
            try
            {
                WebRequest httpWeb = (HttpWebRequest)HttpWebRequest.Create(requstUrl);
                WebResponse resp = httpWeb.GetResponse();
                string json;
                using (Stream strm = resp.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(strm, Encoding.Default))
                    {
                        json = reader.ReadToEnd();
                    }
                }
                return json;
            }
            catch (Exception ex){
                 System.Windows.Forms.MessageBox.Show(ex.Message);
                return "";
            }
        }

       /// <summary>
       /// 请求的JSON转成实体
       /// </summary>
       /// <param name="Json">JSON</param>
       /// <returns></returns>
        private static StockModel JsonToStockModel(string Json)
        {

            #region 
            //0：”大秦铁路”，股票名字；
            //1：”27.55″，今日开盘价；
            //2：”27.25″，昨日收盘价；
            //3：”26.91″，当前价格；
            //4：”27.55″，今日最高价；
            //5：”26.20″，今日最低价；
            //6：”26.91″，竞买价，即“买一”报价；
            //7：”26.92″，竞卖价，即“卖一”报价；
            //8：”22114263″，成交的股票数，由于股票交易以一百股为基本单位，所以在使用时，通常把该值除以一百；
            //9：”589824680″，成交金额，单位为“元”，为了一目了然，通常以“万元”为成交金额的单位，所以通常把该值除以一万；
            //10：”4695″，“买一”申请4695股，即47手；
            //11：”26.91″，“买一”报价；
            //12：”57590″，“买二”
            //13：”26.90″，“买二”
            //14：”14700″，“买三”
            //15：”26.89″，“买三”
            //16：”14300″，“买四”
            //17：”26.88″，“买四”
            //18：”15100″，“买五”
            //19：”26.87″，“买五”
            //20：”3100″，“卖一”申报3100股，即31手；
            //21：”26.92″，“卖一”报价
            //(22, 23), (24, 25), (26, 27), (28, 29)分别为“卖二”至“卖四的情况”
            //30：”2008 - 01 - 11″，日期；
            //31：”15:05:32″，时间；
            //32 股票状态
            #endregion
            string[] objs = Json.Split(',');

            StockModel model = new StockModel();
            {
                var name = objs[0];
                model.Code = name.Substring(name.LastIndexOf('_')+1, 8);
                model.StockName = name.Substring(name.LastIndexOf('"')+1);

                model.TodayFirstPrice = double.Parse(objs[1]);
                model.YesterdayPrice = double.Parse(objs[2]);
                model.NowPrice= double.Parse(objs[3]);
                model.MaxPrice= double.Parse(objs[4]);
                model.MinPrice= double.Parse(objs[5]);
                model.BuyPrice= double.Parse(objs[6]);
                model.SalePrice= double.Parse(objs[7]);
                model.DealNum= long.Parse(objs[8])/10000;
                model.DealMoney= double.Parse(objs[9])/10000;

                model.BuyNum1 = int.Parse(objs[10]);
                model.BuyNum2 = int.Parse(objs[12]);
                model.BuyNum3 = int.Parse(objs[14]);
                model.BuyNum4 = int.Parse(objs[16]);
                model.BuyNum5 = int.Parse(objs[18]);

                model.BuyPrice1 = double.Parse(objs[11]);
                model.BuyPrice2 = double.Parse(objs[13]);
                model.BuyPrice3 = double.Parse(objs[15]);
                model.BuyPrice4 = double.Parse(objs[17]);
                model.BuyPrice5 = double.Parse(objs[19]);


                model.SaleNum1 = int.Parse(objs[20]);
                model.SaleNum2 = int.Parse(objs[22]);
                model.SaleNum3 = int.Parse(objs[24]);
                model.SaleNum4 = int.Parse(objs[26]);
                model.SaleNum5 = int.Parse(objs[28]);

                model.SalePrice1 = double.Parse(objs[22]);
                model.SalePrice2 = double.Parse(objs[23]);
                model.SalePrice3 = double.Parse(objs[25]);
                model.SalePrice4 = double.Parse(objs[27]);
                model.SalePrice5 = double.Parse(objs[29]);

                model.ToDay =  DateTime.Parse(objs[30]);
                model.NowTime = objs[31];
                model.StatusCode =objs[32];
                model.DiffPrice=  Math.Round((model.NowPrice - model.YesterdayPrice), 2);
                model.DiffPercent=  model.YesterdayPrice==0?0:Math.Round(((model.NowPrice - model.YesterdayPrice) / model.YesterdayPrice), 4);
    }
            return model;
        }

        /// <summary>
        /// 请求的简单模式JSON转成实体
        /// </summary>
        /// <param name="Json">简单模式JSON</param>
        /// <returns></returns>
        private static StockModel SimpleJsonToStockModel(string simlpeJson)
        {
            //varhq_str_s_sh000001="上证指数,3094.668,-128.073,-3.97,436653,5458126";
            // 数据含义分别为：指数名称，当前点数，当前价格，涨跌率，成交量（手），成交额（万元）；
            string[] objs = simlpeJson.Split(',');
            StockModel model = new StockModel();
            {
                var name = objs[0];
                model.Code = name.Substring(name.LastIndexOf('_') + 1, 8);
                model.StockName = name.Substring(name.LastIndexOf('"') + 1);
                model.NowPrice =double.Parse(objs[2]);
                model.DiffPercent =double.Parse(objs[3]);
                model.DiffPrice = double.Parse(objs[1]);
                model.DealNum = int.Parse(objs[4].ToString());
                model.DealMoney = double.Parse(objs[5].ToString().Remove(objs[5].ToString().IndexOf( '"')));
            }
            return model;
        }

        /// <summary>
        /// 根据代码获取请求地址
        /// </summary>
        /// <param name="Code">代码</param>
        /// <param name="IsSimple">是否简单模式</param>
        /// <returns></returns>
        private static string GetRequstUrl(string Code, bool IsSimple = false)
        {
            string RequstUrl = JsonUrl;
            if (IsSimple)
            {
                RequstUrl = RequstUrl + "s_";
            }
            return RequstUrl + Code;
        }

        /// <summary>
        /// 验证代码
        /// </summary>
        /// <param name="Code">代码</param>
        /// <returns></returns>
        public static bool ValiCode(ref string Code)
        {
            string FullCode = "";

            if (Code == null || Code.Length < 6)
            {
                return false ;
            }
            else if (Code.Length == 6)
            {
                if (  Code.StartsWith("300")
                     || Code.StartsWith("730")
                     || Code.StartsWith("600")
                     || Code.StartsWith("601")
                     || Code.StartsWith("603")
                     || Code.StartsWith("605")
                     || Code.StartsWith("688")
                     || Code.StartsWith("900")
                     )
                {
                    FullCode = "sh";
                }
                else if (Code.StartsWith("000")
                    || Code.StartsWith("002")
                    || Code.StartsWith("200")
                    || Code.StartsWith("399"))
                {
                    FullCode = "sz";
                }
                else
                {
                    return false;
                }
                Code= FullCode + Code;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据代码获取股票名称
        /// </summary>
        /// <param name="Code">代码</param>
        /// <returns></returns>
        public static string GetCodeName(string Code)
        {
            var SimpleUrl = GetRequstUrl(Code, true);
            var SimpleJson = RequsetJson(SimpleUrl);
            var model = SimpleJsonToStockModel(SimpleJson);
            if (model == null)
            {
                return "";
            }
            else
            {
                return model.StockName;
            }
        }

    }
    public class StockModel
    {
        public string Code { get; set; }
        public string StockName { get; set; }
        public double TodayFirstPrice { get; set; }
        public double YesterdayPrice { get; set; }
        public double NowPrice { get; set; }
        public double MaxPrice { get; set; }
        public double MinPrice { get; set; }
        public double BuyPrice { get; set; }
        public double SalePrice { get; set; }
        public long DealNum { get; set; }
        public double DealMoney { get; set; }
        public double BuyPrice1 { get; set; }
        public int BuyNum1 { get; set; }
        public double BuyPrice2 { get; set; }
        public int BuyNum2 { get; set; }

        public double BuyPrice3 { get; set; }
        public int BuyNum3{ get; set; }

        public double BuyPrice4 { get; set; }
        public int BuyNum4 { get; set; }

        public double BuyPrice5 { get; set; }
        public int BuyNum5 { get; set; }


        public double SalePrice1 { get; set; }
        public int SaleNum1 { get; set; }

        public double SalePrice2 { get; set; }
        public int SaleNum2 { get; set; }

        public double SalePrice3 { get; set; }
        public int SaleNum3 { get; set; }

        public double SalePrice4 { get; set; }
        public int SaleNum4 { get; set; }

        public double SalePrice5 { get; set; }
        public int SaleNum5 { get; set; }

        public System.DateTime ToDay { get; set; }
        public string NowTime { get; set; }
        public string StatusCode { get; set; }

        public double DiffPrice { get; set; } = 0;
        public double DiffPercent { get; set; } = 0;
        
    }

}
