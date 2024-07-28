// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.SmtpMailRequest
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System.Net;
using System.Net.Mail;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public class SmtpMailRequest
  {
    private const int c_defaultSmtpPort = 25;

    public MailMessage MailMessage { get; private set; }

    public string Host { get; private set; }

    public int HostPort { get; private set; }

    public bool EnableSsl { get; private set; }

    public ICredentialsByHost Credentials { get; private set; }

    public SmtpMailRequest(
      MailMessage mailMessage,
      string host,
      int hostPort = 25,
      bool enableSsl = true,
      ICredentialsByHost credentials = null)
    {
      ArgumentUtility.CheckForNull<MailMessage>(mailMessage, nameof (mailMessage));
      ArgumentUtility.CheckForOutOfRange(hostPort, nameof (hostPort), 1);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(host, nameof (host));
      if (credentials == null)
        credentials = (ICredentialsByHost) CredentialCache.DefaultNetworkCredentials;
      this.EnableSsl = enableSsl;
      this.MailMessage = mailMessage;
    }

    public override string ToString() => new XDocument(new object[1]
    {
      (object) new XElement((XName) this.GetType().Name, new object[4]
      {
        (object) new XElement((XName) "Host", (object) this.Host),
        (object) new XElement((XName) "HostPort", (object) this.HostPort),
        (object) new XElement((XName) "EnableSsl", (object) this.EnableSsl),
        (object) new XElement((XName) "MailMessage", new object[17]
        {
          (object) new XElement((XName) "Attachments", (object) this.MailMessage.Attachments.Count),
          (object) new XElement((XName) "Bcc", (object) this.MailMessage.Bcc),
          (object) new XElement((XName) "Body", (object) this.MailMessage.Body),
          (object) new XElement((XName) "BodyEncoding", (object) this.MailMessage.BodyEncoding.EncodingName),
          (object) new XElement((XName) "BodyTransferEncoding", (object) this.MailMessage.BodyTransferEncoding),
          (object) new XElement((XName) "CC", (object) this.MailMessage.CC),
          (object) new XElement((XName) "DeliveryNotificationOptions", (object) this.MailMessage.DeliveryNotificationOptions),
          (object) new XElement((XName) "From", (object) this.MailMessage.From),
          (object) new XElement((XName) "Headers", (object) this.MailMessage.Headers),
          (object) new XElement((XName) "HeadersEncoding", (object) this.MailMessage.HeadersEncoding.EncodingName),
          (object) new XElement((XName) "IsBodyHtml", (object) this.MailMessage.IsBodyHtml),
          (object) new XElement((XName) "Priority", (object) this.MailMessage.Priority),
          (object) new XElement((XName) "ReplyToList", (object) this.MailMessage.ReplyToList),
          (object) new XElement((XName) "Sender", (object) this.MailMessage.Sender),
          (object) new XElement((XName) "Subject", (object) this.MailMessage.Subject),
          (object) new XElement((XName) "SubjectEncoding", (object) this.MailMessage.SubjectEncoding.EncodingName),
          (object) new XElement((XName) "To", (object) this.MailMessage.To)
        })
      })
    }).ToString();
  }
}
