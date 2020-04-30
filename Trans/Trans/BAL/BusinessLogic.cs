using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Trans.Entity;
using Trans.Exceptions;

namespace Trans.BAL
{
    public class BusinessLogic
    {
        private string appid = string.Empty;
        private string staticPassword = string.Empty;
        private string targetUrl = string.Empty;
        private Random ranDom = new Random();

        /// <summary>
        /// Construct a instance of BusinessLogic;
        /// </summary>
        public BusinessLogic()
        {
            appid = ConfigurationManager.AppSettings["appid"] as string;
            staticPassword = ConfigurationManager.AppSettings["staticPassword"] as string;
            targetUrl = ConfigurationManager.AppSettings["targetUrl"] as string;
        }

        /// <summary>
        /// Gets the target language.
        /// </summary>
        /// <param name="inputContent">The input content.</param>
        /// <param name="inputLanguage">The input type.</param>
        /// <param name="targetLanguage">The targer Language.</param>
        /// <returns></returns>
        public string GetTargetLanguage(string inputContent, string inputLanguage, string targetLanguage)
        {
            using (WebClient wc = new WebClient())
            {

                String content = InputFilter(inputContent);
                int salt = ranDom.Next();

                // appid + q + salt + staticPassword.
                string sign = Utils.Utils.GetMd5(appid + content + salt.ToString() + staticPassword);

                string url = string.Format(
                                            targetUrl,
                                            content,
                                            inputLanguage,
                                            targetLanguage,
                                            appid,
                                            salt,
                                            sign
                                            );
                var buffer = wc.DownloadData(url);
                string result = Encoding.UTF8.GetString(buffer);
                StringBuilder targetLanguagereslut = new StringBuilder();
            
                StringReader sr = new StringReader(result);
                using (JsonTextReader jtr = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    if (result.IndexOf("error_code") > -1)
                    {
                        InputException inputException = new InputException();
                        throw inputException;
                    }
                    else
                    {
                        var r = serializer.Deserialize<TransObj>(jtr);
                        if(r.Transresults.Count < 2)
                        {
                            targetLanguagereslut.Append(r.Transresults[0].Dst);
                        }
                        else
                        {
                            foreach(var item in r.Transresults)
                            {
                                targetLanguagereslut.Append("1. /n");
                                targetLanguagereslut.Append(item.Dst);
                                targetLanguagereslut.Append(" /n");

                            }
                        }
                    }
                }

            

                return targetLanguagereslut.ToString();
            }
        }

        private string InputFilter(string inputContent)
        {
            //                             .Replace(@"%", "%25")  // 1. % 指定特殊字符 % 25
            //                             .Replace(@"+", "%2B")  // 2. + URL 中+号表示空格 %2B
            //                             .Replace(@" ", "%20")  // 3. 空格 URL中的空格可以用+号或者编码 %20 
            //                             .Replace(@"/", "%2F")  // 4. / 分隔目录和子目录 %2F 
            //                             .Replace(@"?", "%3F")  // 5. ? 分隔实际的 URL 和参数 %3F
            //                             .Replace(@"#", "%23")  // 6. 表示书签 % 23
            //                             .Replace(@"&", "%26")  // 7. & URL 中指定的参数间的分隔符 %26 
            //                             .Replace(@"=", "%3D")  // 8. = URL 中指定参数的值 %3D
            var sb = new StringBuilder(inputContent);
            sb
                //.Replace(@"%", "%25")
                .Replace(@"+", " ")
                //.Replace(@" ", "%20")
                //.Replace(@"/", "%2F")
                //.Replace(@"?", "%3F")
                .Replace(@"#", " ")
                .Replace(@"$", " ")
                .Replace(@"&", " ")
                //.Replace(@"=", "%3D")
                .Replace(@":", " ")
                ;
            return sb.ToString();
        }
    }
}
