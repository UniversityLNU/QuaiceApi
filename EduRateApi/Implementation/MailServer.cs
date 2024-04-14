using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using EduRateApi.Interfaces;
using EduRateApi.Dtos;

namespace EduRateApi.Implementation
{
    public class MailServer : IMailService
    {
        public void SendMail(SendMailDTO sendMail)
        {
            string apiKeyFilePath = "Config/mailConfig.json";
            string apiKeyJson = File.ReadAllText(apiKeyFilePath);
            var apiKeyObject = JObject.Parse(apiKeyJson);
            string apiKey = apiKeyObject["API_KEY"].ToString();

            if (!Configuration.Default.ApiKey.ContainsKey("api-key"))
            {
                Configuration.Default.ApiKey.Add("api-key", apiKey);
            }

            var apiInstance = new TransactionalEmailsApi();
            string SenderName = "John Doe";
            string SenderEmail = "example@example.com";
            SendSmtpEmailSender Email = new SendSmtpEmailSender(SenderName, SenderEmail);
            string ToEmail = sendMail.email;
            string ToName = sendMail.userName;
            SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo(ToEmail, ToName);
            List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>();
            To.Add(smtpEmailTo);
            string BccName = "Janice Doe";
            string BccEmail = "example2@example2.com";
            SendSmtpEmailBcc BccData = new SendSmtpEmailBcc(BccEmail, BccName);
            List<SendSmtpEmailBcc> Bcc = new List<SendSmtpEmailBcc>();
            Bcc.Add(BccData);
            string CcName = "John Doe";
            string CcEmail = "example3@example2.com";
            SendSmtpEmailCc CcData = new SendSmtpEmailCc(CcEmail, CcName);
            List<SendSmtpEmailCc> Cc = new List<SendSmtpEmailCc>();
            Cc.Add(CcData);

            string htmlContent = sendMail.htmlTemplate
                .Replace("{{UserFullName}}", sendMail.userName)
                .Replace("{{FundraisingTitle}}", sendMail.fundraisingTitle);
            string TextContent = null;
            string Subject = "My {{params.subject}}";
            string ReplyToName = "John Doe";
            string ReplyToEmail = "replyto@domain.com";
            SendSmtpEmailReplyTo ReplyTo = new SendSmtpEmailReplyTo(ReplyToEmail, ReplyToName);
            JObject Headers = new JObject();
            Headers.Add("Some-Custom-Name", "unique-id-1234");
            long? TemplateId = null;
            JObject Params = new JObject();
            Params.Add("parameter", "My param value");
            Params.Add("subject", "New Subject");
            List<string> Tags = new List<string>();
            Tags.Add("mytag");
            SendSmtpEmailTo1 smtpEmailTo1 = new SendSmtpEmailTo1(ToEmail, ToName);
            List<SendSmtpEmailTo1> To1 = new List<SendSmtpEmailTo1>();
            To1.Add(smtpEmailTo1);
            Dictionary<string, object> _parmas = new Dictionary<string, object>();
            _parmas.Add("params", Params);
            SendSmtpEmailReplyTo1 ReplyTo1 = new SendSmtpEmailReplyTo1(ReplyToEmail, ReplyToName);
            SendSmtpEmailMessageVersions messageVersion = new SendSmtpEmailMessageVersions(To1, _parmas, Bcc, Cc, ReplyTo1, Subject);
            List<SendSmtpEmailMessageVersions> messageVersiopns = new List<SendSmtpEmailMessageVersions>();
            messageVersiopns.Add(messageVersion);
            try
            {
                var sendSmtpEmail = new SendSmtpEmail(Email, To, Bcc, Cc, htmlContent, TextContent, Subject, ReplyTo, null, Headers, TemplateId, Params, messageVersiopns, Tags);
                CreateSmtpEmail result =  apiInstance.SendTransacEmail(sendSmtpEmail);
            }
            catch (Exception e)
            {

            }
            finally
            {
   
            }
        }


    }
}
