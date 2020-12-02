//using ADAM.Models;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADAM.Common
{
    /// <summary>
    /// 日志记录工具类
    /// </summary>
    public static class MyLog
    {
        /// <summary>
        /// 获取记录器
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static ADAMApiService adamApi = new ADAMApiService();

        #region 日志记录
        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="LogFile">文件名</param>
        /// <param name="LogFolder">文件夹</param>
        /// <param name="level">等级</param>
        public static  void Log(object message, string LogFile, string LogFolder, MyLogLevel level,int headerId=0)
        {
            //Task.Run(() =>
            //{
                try
                {
                    //1.写入配置
                    LogManager.Configuration = LogConfigLoad(LogFile, LogFolder);
                    //2.记录日志
                    switch (level)
                    {
                        case MyLogLevel.Info: Logger.Info(message); break;
                        case MyLogLevel.Error: Logger.Error(message); break;
                    }
                    //3.日志发送到Server
                    ////var applicationLog = new ApplicationLog()
                    ////{
                    ////    HostName = Dns.GetHostName(),
                    ////    HeaderId = headerId,
                    ////    LogCategory = LogFolder,
                    ////    LogInfo = message as string,
                    ////    LogLevel = level == MyLogLevel.Error ? "Error" : "Info",
                    ////    LogName = LogFile,
                    ////    OrgId = ADAMConfig.OrgID,
                    ////    BatchName = ADAMConfig.BatchName
                    ////};
                    ////adamApi.Log(applicationLog);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            //});

        }
        #endregion

        public static  void ScreenShot(string category, int headerId = 0,bool isError = true)
        {
                try
                {
                    Rectangle bounds = Screen.PrimaryScreen.Bounds;
                    using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                            string path = string.Empty;
                            string level = isError == true ? "_Error" : "_Info";
                            path = Directory.GetCurrentDirectory() + "\\" + ADAMConfig.OrgID + "_" + DateTime.Now.ToString("yyyyMMdd") + level;
                            string subPath = path; // your code goes here
                            Directory.CreateDirectory(subPath);
                            if (headerId != 0)
                            {
                                path = path + "\\" + category + "_HeaderId_" + headerId + "_" + DateTime.Now.ToString("HH.mm.ss") + ".png";
                            }
                            else
                            {
                                path = path + "\\" + category + "_" + DateTime.Now.ToString("HH.mm.ss") + ".png";
                            }

                            bitmap.Save(path, ImageFormat.Png);
                            //截屏发送到Server
                            bitmap.Save(stream, ImageFormat.Png);
                            ////var screenCapture = new AppScreenCapture()
                            ////{
                            ////    BatchName = ADAMConfig.BatchName,
                            ////    Category = category,
                            ////    Content = stream.ToArray(),
                            ////    HeaderId = headerId,
                            ////    HostName = Dns.GetHostName(),
                            ////    Level = isError == true ? "Error" : "Info",
                            ////    OrgId = ADAMConfig.OrgID
                            ////};
                            ////adamApi.SaveScreenCapture(screenCapture);
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
        }

       

        public static string ScreenShot(string filename)
        {
            try
            {
                Rectangle bounds = Screen.PrimaryScreen.Bounds;
                string path = @"C:\Adam\Adam_WebADI\bin\Debug" + "\\" + DateTime.Now.ToString("yyyyMMdd");

                string filepath = path + "\\" + filename + "_" + DateTime.Now.ToString("HH.mm.ss") + ".png";
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);

                        Directory.CreateDirectory(path);

                        bitmap.Save(filepath, ImageFormat.Png);
                        //截屏发送到Server
                        bitmap.Save(stream, ImageFormat.Png);

                        return filepath;
                        ////var screenCapture = new AppScreenCapture()
                        ////{
                        ////    BatchName = ADAMConfig.BatchName,
                        ////    Category = category,
                        ////    Content = stream.ToArray(),
                        ////    HeaderId = headerId,
                        ////    HostName = Dns.GetHostName(),
                        ////    Level = isError == true ? "Error" : "Info",
                        ////    OrgId = ADAMConfig.OrgID
                        ////};
                        ////adamApi.SaveScreenCapture(screenCapture);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "";
            }
        }

        #region NLog配置
        /// <summary>
        /// Nlog配置
        /// </summary>
        /// <param name="LogFile">文件名</param>
        /// <param name="LogFolder"><文件夹/param>
        private static LoggingConfiguration LogConfigLoad(string LogFile, string LogFolder)
        {
            // Step 1. Create configuration object 

            LoggingConfiguration config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 

            FileTarget fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);
            
            // Step 3. Set target properties 

            StringBuilder sb = new StringBuilder();
            string localpath = "C:\\Adam\\Adam_WebADI\\bin\\Debug";
            //var localpath = Directory.GetCurrentDirectory();
            fileTarget.FileName = localpath + string.Format(@"\LogInformation_"+ADAMConfig.OrgID+@"\${0}shortdate{1}\{2}\${3}level{4}_{5}.txt", "{", "}", LogFolder, "{", "}", LogFile);
            fileTarget.Layout = @"${newline}date：	${date}
                                ${newline}level：	${level}
                                ${newline}message：	${message}
                                ${newline}stacktrace: ${stacktrace}
                                ${newline}-----------------------------------------------------------";

            LoggingRule rule = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule);

            return config;
        }
        #endregion

    }
}
