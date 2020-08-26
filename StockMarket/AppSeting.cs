#region << 版 本 注 释 >>
//----------------------------------------------------------------
// Copyright © 2020  版权所有：湖南办事处（IT-hezhijin）
// 唯一码：05c96f8d-4f6e-4079-a8a6-4857607058c4
// 文件名：AppSeting
// 文件功能描述：
// 创建者：HZJ-(zhijinhe2020) 
// 计算机名：IT-HZJ
// QQ: 413961980
// 时间：2020-08-25 9:37:36
// 修改人：HZJ-(zhijinhe2020) 
// 时间：2020-08-25 9:37:36
// 修改说明：
// 版本：V1.0.0   当前系统CLR（运行时版.NET）版本号:4.0.30319.42000
//----------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HZJ.StockMarket
{
    public static class AppSeting
    {

        private static readonly string ConfigFile = Application.StartupPath + "\\Codes.ini";
        #region 配置文件读取与写入


        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <returns></returns>
        public static List<string> Read()
        {
            var strcodes = ReadToStr();
            return new List<string>(strcodes.Split(',').AsEnumerable<string>());
        }


        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <returns></returns>
        public static bool Save(List<string> StockList)
        {
            try
            {
                if (!File.Exists(ConfigFile))
                {
                    File.Create(ConfigFile);
                }

                var contents = "";
                foreach (string s in StockList)
                {
                    contents = contents + s + ",";
                }
                if (contents == "")
                    return false;
                File.WriteAllText(ConfigFile, contents.Remove(contents.LastIndexOf(',')));
            }
            catch (IOException ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <returns></returns>
        public static string ReadToStr()
        {
            if (!File.Exists(ConfigFile))
            {
                File.Create(ConfigFile);

                var contents = "sz399001,sz000001";
                File.WriteAllText(ConfigFile, contents);
            }
           return File.ReadAllText(ConfigFile);
        }

        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <returns></returns>
        public static bool SaveToStr(string Codes)
        {
            try {
                File.WriteAllText(ConfigFile, Codes);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      

        #endregion
    }
}
