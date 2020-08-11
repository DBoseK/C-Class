using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;

namespace eOT.Util
{
    public class MailHelper
    {
        public static readonly string MailServer = ConfigurationManager.AppSettings["smtpserver"];
        public static readonly string mailbcc = ConfigurationManager.AppSettings["emailBcc"];
        public static readonly string SysMailAddress = "Sys.Admin@163.com";

        public static bool SendMailSmtp(string strFrom, string strTo, string strCc, string strSub, string strCon, Dictionary<string, byte[]> fileByteDict = null, List<string> filePathList = null)
        {

            MailMessage msg = new MailMessage();
            msg.IsBodyHtml = true;
            msg.Subject = strSub;
            msg.Body = strCon;            
            msg.From = new MailAddress(SysMailAddress);

            if (!string.IsNullOrEmpty(strTo))
            {
                foreach (string to in strTo.Split(';'))
                {
                    if (!string.IsNullOrEmpty(to))
                    {
                        msg.To.Add(to);
                    }
                    
                }
            }
            if (!string.IsNullOrEmpty(strCc))
            {
                foreach (string cc in strCc.Split(';'))
                {
                    if (!string.IsNullOrEmpty(cc))
                    {
                        msg.CC.Add(cc);
                    }
                }
            }

            if (!string.IsNullOrEmpty(mailbcc))
            {
                foreach (string bcc in mailbcc.Split(';'))
                {
                    if (!string.IsNullOrEmpty(bcc))
                    {
                        msg.Bcc.Add(bcc);
                    }
                }
            }


            if (fileByteDict != null)
            {
                foreach (string filename in fileByteDict.Keys)
                {
                    System.IO.Stream stream = new System.IO.MemoryStream(fileByteDict[filename]);
                    //附件对象 
                    System.Net.Mail.Attachment data = new System.Net.Mail.Attachment(stream, filename);
                    //加入邮件附件 
                    msg.Attachments.Add(data);
                }
            }

            if (filePathList != null)
            {
                foreach (string filepath in filePathList)
                {
                    //附件对象 
                    System.Net.Mail.Attachment data = new System.Net.Mail.Attachment(filepath, System.Net.Mime.MediaTypeNames.Application.Octet);
                    //加入邮件附件 
                    msg.Attachments.Add(data);
                }
            }

            SmtpClient smtpmail = new SmtpClient(MailServer);
            smtpmail.Credentials = new System.Net.NetworkCredential();

            try
            {
                smtpmail.Send(msg);
                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}
