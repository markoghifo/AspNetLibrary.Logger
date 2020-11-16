using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Logger
{
    public class Log
    {
        public Log(string logPath)
        {
            path = generatePath(logPath);
        }
        private object o = new object();
        private string path = string.Empty;
        public void WriteTolog(string ip, string source, object request, object response)
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    lock (o)
                    {
                         using (TextWriter tw = new StreamWriter(path + DateTime.Now.ToString("yyyyMMdd") + "Log.txt", true))
                        {
                            tw.WriteLine("TIME : " + DateTime.Now.TimeOfDay.ToString());
                            tw.WriteLine("IP : " + ip);
                            tw.WriteLine("SOURCE : " + source.ToUpper());
                            tw.WriteLine("REQUEST : " + JsonConvert.SerializeObject(request));
                            tw.WriteLine("RESPONSE : " + JsonConvert.SerializeObject(response));
                            tw.WriteLine();
                            tw.Close();
                        }
                    }                   
                }
            }
            catch (Exception)
            {
                //wat to do
            }
        }

        public void WriteToExceptionLog(string source, object request, Exception exception)
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    lock (o)
                    {
                        using (TextWriter tw = new StreamWriter(path + DateTime.Now.ToString("yyyyMMdd") + "ExceptionLog.txt", true))
                        {
                            tw.WriteLine("TIME : " + DateTime.Now.TimeOfDay.ToString());
                            tw.WriteLine("SOURCE : " + source.ToUpper());
                            tw.WriteLine("REQUEST : " + JsonConvert.SerializeObject(request));
                            if (exception != null)
                            {
                                tw.WriteLine("EXCEPTION : " + exception.ToString());
                            }
                            tw.WriteLine();
                            tw.Close();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }


        internal string generatePath(string logPath)
        {
            try
            {
                path = logPath + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\";

                if (!File.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception)
            {
            }
            return path;
        }

        public static string GetClientIpAddress(HttpRequestMessage request)
        {
            string ip = string.Empty;
            try
            {
                if (request.Properties.ContainsKey("MS_HttpContext"))
                {
                    ip = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                }

                if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
                {
                    RemoteEndpointMessageProperty prop;
                    prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                    ip = prop.Address;
                }
            }
            catch (Exception)
            {
            }
            return ip;
        }

    }
}
