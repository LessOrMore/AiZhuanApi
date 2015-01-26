using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Xml;
using System.Configuration;
namespace WebAPI.Data
{
    public class MailHelper
    {
         public static void Send(string server, string sender, string recipient, string subject,
    string body, bool isBodyHtml, Encoding encoding, bool isAuthentication, params string[] files)
        {
            SmtpClient smtpClient = new SmtpClient(server);
            MailMessage message = new MailMessage(sender, recipient);
            message.IsBodyHtml = isBodyHtml;

            message.SubjectEncoding = encoding;
            message.BodyEncoding = encoding;

            message.Subject = subject;
            message.Body = body;

            message.Attachments.Clear();
            if (files != null && files.Length != 0)
            {
                for (int i = 0; i < files.Length; ++i)
                {
                    Attachment attach = new Attachment(files[i]);
                    message.Attachments.Add(attach);
                }
            }

            if (isAuthentication == true)
            {
                smtpClient.Credentials = new NetworkCredential(SmtpConfig.Create().SmtpSetting.User,
                    SmtpConfig.Create().SmtpSetting.Password);
            }
            smtpClient.Send(message);
        }

        //发送邮件
        public static void Send(string recipient, string subject, string body)
        {
           
            SmtpConfig config = SmtpConfig.Create();
            Send(config.SmtpSetting.Server, config.SmtpSetting.Sender, recipient, subject, body, true, Encoding.Default, true, null);
           
        }

        public static void Send(string Recipient, string Sender, string Subject, string Body)
        {
            SmtpConfig config = SmtpConfig.Create();
            Send(config.SmtpSetting.Server, Sender, Recipient, Subject, Body, true, Encoding.UTF8, true, null);
        }
       
    }

    #region  邮箱相关配置
    public class SmtpConfig
    {
        private SmtpSetting smtpSetting;
        public SmtpSetting SmtpSetting
        {
            get
            {
                if (smtpSetting == null)
                {
                    smtpSetting = new SmtpSetting();
                    smtpSetting.Server = ConfigurationManager.AppSettings["Server"].ToString().Trim();
                    smtpSetting.Authentication = Convert.ToBoolean(ConfigurationManager.AppSettings["Authentication"].ToString());
                    smtpSetting.User = ConfigurationManager.AppSettings["User"].ToString().Trim();
                    smtpSetting.Password = ConfigurationManager.AppSettings["Password"].ToString().Trim();
                    smtpSetting.Sender = ConfigurationManager.AppSettings["Sender"].ToString().Trim();
                }
                return smtpSetting;
            }
        }

        private static SmtpConfig _smtpConfig;
        public static SmtpConfig Create()
        {
            if (_smtpConfig == null)
            {
                _smtpConfig = new SmtpConfig();
            }
            return _smtpConfig;
        }
    }
    #endregion 

    #region 邮箱参数相关
    public class SmtpSetting
    {
        private string _server;
        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }
        private bool _authentication;

        public bool Authentication
        {
            get { return _authentication; }
            set { _authentication = value; }
        }
        private string _user;

        public string User
        {
            get { return _user; }
            set { _user = value; }
        }
        private string _sender;

        public string Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }
        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
    }


    #endregion 
}