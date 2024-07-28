// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.SummaryMailService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class SummaryMailService : ReleaseManagement2ServiceBase
  {
    private static readonly char[] MailAddressSeparators = new char[2]
    {
      ',',
      ';'
    };
    private const string ReplyByHeader = "Reply-By";
    private const string ReplyByDateFormat = "r";

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public IEnumerable<SummaryMailSection> GetReleaseSummaryMailSections(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int releaseId,
      IList<MailSectionType> sectionsToBeIncluded)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (sectionsToBeIncluded == null)
        throw new ArgumentNullException(nameof (sectionsToBeIncluded));
      Release release = SummaryMailService.GetRelease(requestContext, projectInfo, releaseId);
      List<SummaryMailSection> sections = new List<SummaryMailSection>();
      foreach (MailSectionType sectionType in (IEnumerable<MailSectionType>) sectionsToBeIncluded)
      {
        Func<IVssRequestContext, ProjectInfo, Release, SummaryMailSection> sectionHtmlGenerator = SummaryMailHelper.GetSectionHtmlGenerator(sectionType);
        sections.Add(sectionHtmlGenerator(requestContext, projectInfo, release));
      }
      SummaryMailHelper.OrderSectionsByRank(sections);
      return (IEnumerable<SummaryMailSection>) sections;
    }

    public void SendReleaseSummaryMail(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int releaseId,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.MailMessage mailMessage)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectInfo == null)
        throw new ArgumentNullException(nameof (projectInfo));
      if (mailMessage == null)
        throw new ArgumentNullException(nameof (mailMessage));
      IList<MailSectionType> sectionsToBeIncluded = mailMessage.Sections ?? (IList<MailSectionType>) new List<MailSectionType>();
      List<SummaryMailSection> list = this.GetReleaseSummaryMailSections(requestContext, projectInfo, releaseId, sectionsToBeIncluded).ToList<SummaryMailSection>();
      System.Net.Mail.MailMessage mailMessage1 = (System.Net.Mail.MailMessage) null;
      try
      {
        ITeamFoundationMailService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationMailService>();
        mailMessage1 = SummaryMailService.CreateMailMessage(requestContext, mailMessage, requestContext.ExecutionEnvironment.IsHostedDeployment);
        XElement summaryMailHtml = SummaryMailHtmlGenerator.GetSummaryMailHtml((IList<SummaryMailSection>) list, mailMessage1.Body);
        mailMessage1.Body = SummaryMailHelper.SafeConvertXElementToString((XNode) summaryMailHtml);
        mailMessage1.IsBodyHtml = true;
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        System.Net.Mail.MailMessage message = mailMessage1;
        service.QueueMailJob(requestContext1, message);
      }
      catch (Exception ex)
      {
        throw new ReleaseManagementException(Resources.ErrorSendMail, ex);
      }
      finally
      {
        mailMessage1?.Dispose();
      }
    }

    private static Release GetRelease(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int releaseId)
    {
      return requestContext.GetService<ReleasesService>().GetRelease(requestContext, projectInfo.Id, releaseId).ToContract(requestContext, projectInfo.Id, true, ApprovalFilters.ManualApprovals | ApprovalFilters.AutomatedApprovals);
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Being disposed by the caller.")]
    private static System.Net.Mail.MailMessage CreateMailMessage(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.MailMessage mailMessage,
      bool prohibitExternalEmailAddresses)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      System.Net.Mail.MailMessage mailMessage1 = new System.Net.Mail.MailMessage();
      SummaryMailService.AddRecipients(mailMessage1.To, mailMessage.To, requestContext, prohibitExternalEmailAddresses);
      SummaryMailService.AddRecipients(mailMessage1.CC, mailMessage.CC, requestContext, prohibitExternalEmailAddresses);
      SummaryMailService.AddRecipients(mailMessage1.ReplyToList, mailMessage.ReplyTo, requestContext, prohibitExternalEmailAddresses);
      SummaryMailService.RemoveRedundantCCRecipients(mailMessage1);
      if (mailMessage.SenderType == SenderType.RequestingUser)
        SummaryMailService.SetFromAndSender(mailMessage1, requestContext);
      if (mailMessage.ReplyBy.HasValue)
        SummaryMailService.SetReplyBy(mailMessage1, mailMessage.ReplyBy.Value);
      mailMessage1.HeadersEncoding = Encoding.UTF8;
      if (!string.IsNullOrEmpty(mailMessage.MessageId))
      {
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, mailMessage.MessageId, (object) DateTime.UtcNow.Ticks);
        mailMessage1.Headers.Add("Message-ID", str);
      }
      if (!string.IsNullOrEmpty(mailMessage.InReplyTo))
        mailMessage1.Headers.Add("In-Reply-To", mailMessage.InReplyTo);
      mailMessage1.Subject = mailMessage.Subject;
      mailMessage1.Body = SafeHtmlWrapper.MakeSafe(mailMessage.Body);
      mailMessage1.IsBodyHtml = true;
      return mailMessage1;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static void AddRecipients(
      MailAddressCollection mailAddressCollection,
      EmailRecipients emailRecipients,
      IVssRequestContext requestContext,
      bool prohibitExternalEmailAddresses)
    {
      if (emailRecipients == null)
        return;
      List<string> enumerable = new List<string>();
      if (emailRecipients.TfsIds != null)
      {
        enumerable.AddRange(RmIdentityHelper.GetEmailAddresses(requestContext, emailRecipients.TfsIds, false));
        if (enumerable.IsNullOrEmpty<string>())
          requestContext.Trace(1900013, TraceLevel.Info, "ReleaseManagementService", "Service", "SummaryMailService: could not load email addresses for identities {0}", (object) string.Join<Guid>(",", (IEnumerable<Guid>) emailRecipients.TfsIds));
      }
      if (!prohibitExternalEmailAddresses && emailRecipients.EmailAddresses != null)
      {
        foreach (string recipients in ((IEnumerable<string>) emailRecipients.EmailAddresses).Distinct<string>())
        {
          foreach (string splitMailAddress in (IEnumerable<string>) SummaryMailService.SplitMailAddresses(recipients))
          {
            if (!enumerable.Contains(splitMailAddress))
              enumerable.Add(splitMailAddress);
          }
        }
      }
      foreach (string addresses in enumerable)
        mailAddressCollection.Add(addresses);
    }

    private static void RemoveRedundantCCRecipients(System.Net.Mail.MailMessage smtpMessage)
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

    private static ICollection<string> SplitMailAddresses(string recipients) => recipients != null ? (ICollection<string>) ((IEnumerable<string>) recipients.Split(SummaryMailService.MailAddressSeparators, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (item => item.Trim())).Where<string>((Func<string, bool>) (item => !string.IsNullOrEmpty(item))).Distinct<string>().ToArray<string>() : (ICollection<string>) Array.Empty<string>();

    private static void SetReplyBy(System.Net.Mail.MailMessage message, DateTime dueByDate)
    {
      string str = dueByDate.ToString("r", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
      message.Headers["Reply-By"] = str;
    }

    private static void SetFromAndSender(System.Net.Mail.MailMessage message, IVssRequestContext requestContext)
    {
      if (!(requestContext.UserContext != (IdentityDescriptor) null))
        return;
      try
      {
        Guid userId = requestContext.GetUserId(true);
        IEnumerable<string> emailAddresses = RmIdentityHelper.GetEmailAddresses(requestContext, new Guid[1]
        {
          userId
        }, false);
        if (emailAddresses.IsNullOrEmpty<string>())
          return;
        TeamFoundationMailService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationMailService>();
        message.From = new MailAddress(emailAddresses.First<string>());
        message.Sender = service.FromAddress;
      }
      catch (IdentityNotFoundException ex)
      {
        TeamFoundationEventLog.Default.Log(requestContext, ex.Message, ex.ToString(), TeamFoundationEventId.IdentityNotFoundException, EventLogEntryType.Warning);
      }
    }
  }
}
