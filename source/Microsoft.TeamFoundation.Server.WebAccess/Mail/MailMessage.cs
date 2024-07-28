// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Mail.MailMessage
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.Mail
{
  public class MailMessage
  {
    private static readonly char[] MailAddressSeparators = new char[2]
    {
      ',',
      ';'
    };
    private const string ReplyByHeader = "Reply-By";
    private const string ReplyByDateFormat = "r";

    public EmailRecipients To { get; set; }

    public EmailRecipients CC { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }

    public EmailRecipients ReplyTo { get; set; }

    public DateTime? ReplyBy { get; set; }

    public string InReplyTo { get; set; }

    public string MessageId { get; set; }

    public System.Net.Mail.MailMessage CreateMailMessage(
      IVssRequestContext requestContext,
      bool prohibitExternalEmailAddresses,
      out List<Guid> toRecipientsWithoutEmail)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
      toRecipientsWithoutEmail = this.AddRecipients(mailMessage.To, this.To, requestContext, prohibitExternalEmailAddresses);
      this.AddRecipients(mailMessage.CC, this.CC, requestContext, prohibitExternalEmailAddresses);
      this.AddRecipients(mailMessage.ReplyToList, this.ReplyTo, requestContext, prohibitExternalEmailAddresses);
      this.RemoveRedundantCCRecipients(mailMessage);
      MailMessage.SetDefaultFrom(requestContext, mailMessage);
      if (this.ReplyBy.HasValue)
        MailMessage.SetReplyBy(mailMessage, this.ReplyBy.Value);
      mailMessage.HeadersEncoding = Encoding.UTF8;
      if (!string.IsNullOrEmpty(this.MessageId))
        mailMessage.Headers.Add("Message-ID", string.Format(this.MessageId, (object) DateTime.UtcNow.Ticks));
      if (!string.IsNullOrEmpty(this.InReplyTo))
        mailMessage.Headers.Add("In-Reply-To", this.InReplyTo);
      mailMessage.Subject = this.Subject;
      mailMessage.Body = SafeHtmlWrapper.MakeSafe(this.Body);
      mailMessage.IsBodyHtml = true;
      return mailMessage;
    }

    private List<Guid> AddRecipients(
      MailAddressCollection mailAddressCollection,
      EmailRecipients emailRecipients,
      IVssRequestContext requestContext,
      bool prohibitExternalEmailAddresses)
    {
      List<Guid> guidList = new List<Guid>();
      if (emailRecipients == null)
        return guidList;
      List<string> stringList = new List<string>();
      if (emailRecipients.TfIds != null)
      {
        TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
        foreach (Guid teamFoundationId in ((IEnumerable<Guid>) emailRecipients.TfIds).Distinct<Guid>())
        {
          try
          {
            string preferredEmailAddress = service.GetPreferredEmailAddress(requestContext, teamFoundationId);
            if (!string.IsNullOrEmpty(preferredEmailAddress))
              stringList.Add(preferredEmailAddress);
            else
              guidList.Add(teamFoundationId);
          }
          catch (IdentityNotFoundException ex)
          {
            TeamFoundationEventLog.Default.Log(requestContext, ex.Message, ex.ToString(), TeamFoundationEventId.IdentityNotFoundException, EventLogEntryType.Warning);
          }
        }
      }
      if (!prohibitExternalEmailAddresses && emailRecipients.EmailAddresses != null)
      {
        foreach (string recipients in ((IEnumerable<string>) emailRecipients.EmailAddresses).Distinct<string>())
        {
          foreach (string splitMailAddress in (IEnumerable<string>) MailMessage.SplitMailAddresses(recipients))
          {
            if (!stringList.Contains(splitMailAddress))
              stringList.Add(splitMailAddress);
          }
        }
      }
      foreach (string addresses in stringList)
        mailAddressCollection.Add(addresses);
      return guidList;
    }

    private void RemoveRedundantCCRecipients(System.Net.Mail.MailMessage smtpMessage)
    {
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (MailAddress mailAddress in (Collection<MailAddress>) smtpMessage.To)
        stringSet.Add(mailAddress.Address);
      List<MailAddress> mailAddressList = new List<MailAddress>();
      foreach (MailAddress mailAddress in (Collection<MailAddress>) smtpMessage.CC)
      {
        if (stringSet.Contains(mailAddress.Address))
          mailAddressList.Add(mailAddress);
      }
      foreach (MailAddress mailAddress in mailAddressList)
        smtpMessage.CC.Remove(mailAddress);
    }

    private static ICollection<string> SplitMailAddresses(string recipients) => recipients != null ? (ICollection<string>) ((IEnumerable<string>) recipients.Split(MailMessage.MailAddressSeparators, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (item => item.Trim())).Where<string>((Func<string, bool>) (item => !string.IsNullOrEmpty(item))).Distinct<string>().ToArray<string>() : (ICollection<string>) Array.Empty<string>();

    private static void SetReplyBy(System.Net.Mail.MailMessage message, DateTime dueByDate)
    {
      string str = dueByDate.ToString("r", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
      message.Headers["Reply-By"] = str;
    }

    private static void SetDefaultFrom(IVssRequestContext requestContext, System.Net.Mail.MailMessage message)
    {
      TeamFoundationMailService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationMailService>();
      message.From = service.FromAddress;
    }
  }
}
