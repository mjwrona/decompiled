// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationMailService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationMailService : ITeamFoundationMailService, IVssFrameworkService
  {
    public const string SenderKey = "Sender";
    internal static readonly Encoding MailHeaderEncoding = Encoding.UTF8;
    public static readonly Encoding MailBodyEncoding = Encoding.UTF8;
    private static readonly TeamFoundationJobReference[] s_sendMailJobReferenceArray = new TeamFoundationJobReference[1]
    {
      new TeamFoundationJobReference(new Guid("B1516502-4633-432B-BDB3-74C802C5F2B7"), JobPriorityClass.High)
    };
    private Dictionary<string, IMailSenderFactory> m_mailSenderFactories = new Dictionary<string, IMailSenderFactory>();
    private const int DefaultMaxRetryAttempts = 5;
    private const int DefaultJobDelayInSeconds = 30;
    private const int DefaultRetryIntervalInSeconds = 900;
    private const int DefaultSendTimeoutHosted = 10000;
    private const int DefaultSendTimeoutOnPrem = 30000;
    private const int DefaultMaxSendFailures = 3;
    private const int DefaultMaxEmailBodySizeInBytes = 1048576;
    private List<Tuple<string, string>> m_customSmtpHeaders = new List<Tuple<string, string>>();
    internal static readonly string CustomSmtpHeadersRoot = FrameworkServerConstants.NotificationRootPath + "/CustomSmtpHeaders/";
    private const string c_Area = "MailService";
    private const string c_Layer = "Service";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(1001000, "MailService", "Service", nameof (ServiceStart));
      try
      {
        systemRequestContext.CheckDeploymentRequestContext();
        systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), FrameworkServerConstants.NotificationRootPath + "/...", FrameworkServerConstants.MailServiceRegistryRootPath + "/...");
        this.LoadSettings(systemRequestContext);
        foreach (IMailSenderFactory extension in (IEnumerable<IMailSenderFactory>) systemRequestContext.GetExtensions<IMailSenderFactory>(ExtensionLifetime.Service))
          this.m_mailSenderFactories[extension.MailService] = extension;
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1001000, "MailService", "Service", ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(1001000, "MailService", "Service", nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(1001000, "MailService", "Service", nameof (ServiceEnd));
      try
      {
        systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(1001000, "MailService", "Service", ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(1001000, "MailService", "Service", nameof (ServiceEnd));
      }
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceEnter(1001020, "MailService", "Service", nameof (OnRegistryChanged));
      try
      {
        if (!changedEntries.Any<RegistryEntry>())
          return;
        this.LoadSettings(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1001020, "MailService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1001020, "MailService", "Service", nameof (OnRegistryChanged));
      }
    }

    public void Send(IVssRequestContext requestContext, MailMessage message)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<MailMessage>(message, nameof (message));
      this.Send(requestContext, (IEnumerable<MailMessage>) new MailMessage[1]
      {
        message
      });
    }

    public void Send(IVssRequestContext requestContext, IEnumerable<MailMessage> messages)
    {
      requestContext.TraceEnter(1001030, "MailService", "Service", nameof (Send));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<IEnumerable<MailMessage>>(messages, nameof (messages));
        if (!this.Enabled)
          throw new InvalidOperationException(FrameworkResources.MailServiceDisabledError());
        IEnumerable<MailMessage> source = this.ValidateMessages(requestContext, messages);
        if (source.Any<MailMessage>())
        {
          VssMailSender mailSender = this.GetMailSender(requestContext);
          try
          {
            foreach (MailMessage mailMessage in source)
            {
              if (!mailSender.ShouldReuse)
              {
                mailSender.Dispose();
                mailSender = this.GetMailSender(requestContext);
              }
              if (mailMessage.From == null)
                this.SetReplyToAddress(mailMessage);
              if (mailMessage.Sender == null)
                this.SetSender(requestContext, mailMessage);
              this.ApplyCustomHeaders(requestContext, mailMessage);
              requestContext.TraceEnter(1001030, "MailService", "Service", "Send_SingleMail");
              try
              {
                using (requestContext.AcquireConnectionLock(ConnectionLockNameType.Email))
                  mailSender.SendSynchronously(requestContext, mailMessage, "MailService", "Service");
              }
              catch (Exception ex)
              {
                requestContext.Trace(1001030, TraceLevel.Error, "MailService", "Service", "Error sending email: {0}", (object) ex);
                throw;
              }
              finally
              {
                requestContext.TraceLeave(1001030, "MailService", "Service", "Send_SingleMail");
              }
            }
          }
          finally
          {
            mailSender.Dispose();
          }
        }
        else
          requestContext.Trace(1001030, TraceLevel.Warning, "MailService", "Service", "No messages passed verification");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1001030, "MailService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1001030, "MailService", "Service", nameof (Send));
      }
    }

    public virtual MailRequest QueueMailJob(IVssRequestContext requestContext, MailMessage message)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForNull<MailMessage>(message, nameof (message));
      return this.QueueMailJob(requestContext, (IEnumerable<MailMessage>) new MailMessage[1]
      {
        message
      }).FirstOrDefault<MailRequest>();
    }

    public ICollection<MailRequest> QueueMailJob(
      IVssRequestContext requestContext,
      IEnumerable<MailMessage> messages)
    {
      requestContext.TraceEnter(1001040, "MailService", "Service", nameof (QueueMailJob));
      requestContext.CheckDeploymentRequestContext();
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<IEnumerable<MailMessage>>(messages, nameof (messages));
        if (!this.Enabled)
          throw new InvalidOperationException(FrameworkResources.MailServiceDisabledError());
        ICollection<MailRequest> mailRequests = (ICollection<MailRequest>) null;
        IEnumerable<MailMessage> mailMessages = this.ValidateMessages(requestContext, messages);
        if (mailMessages.Any<MailMessage>())
          mailRequests = this.QueueMessages(requestContext, mailMessages);
        else
          requestContext.Trace(1001040, TraceLevel.Warning, "MailService", "Service", "No messages passed verification");
        return mailRequests ?? (ICollection<MailRequest>) new List<MailRequest>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1001040, "MailService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1001040, "MailService", "Service", nameof (QueueMailJob));
      }
    }

    internal ICollection<MailRequest> QueueMessages(
      IVssRequestContext requestContext,
      IEnumerable<MailMessage> messagesToBeQueued)
    {
      requestContext.GetService<IdentityService>();
      ICollection<MailRequest> mailRequests = (ICollection<MailRequest>) null;
      Guid requesterTfId = Guid.Empty;
      if (requestContext.UserContext != (IdentityDescriptor) null)
        requesterTfId = requestContext.GetUserId(true);
      using (MailQueueComponent component = requestContext.CreateComponent<MailQueueComponent>())
      {
        component.MaxRetryCount = this.MaxRetryAttempts;
        mailRequests = component.AddMailRequests(messagesToBeQueued, requesterTfId);
      }
      if (mailRequests.Count > 0)
        this.QueueMailJob(requestContext);
      return mailRequests;
    }

    internal void QueueMailJob(IVssRequestContext requestContext) => requestContext.GetService<TeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<TeamFoundationJobReference>) TeamFoundationMailService.s_sendMailJobReferenceArray, this.SendMailJobDelay);

    public void SetReplyToAddress(MailMessage message) => this.SetReplyToAddress(message, string.Empty);

    public void SetReplyToAddress(MailMessage message, string replyToOverride)
    {
      MailAddress mailAddress = string.IsNullOrEmpty(replyToOverride) ? this.FromAddress : new MailAddress(replyToOverride);
      if (this.UseReplyTo)
      {
        message.From = this.FromAddress;
        message.ReplyToList.Add(mailAddress);
      }
      else
        message.From = mailAddress;
    }

    public VssMailSender GetMailSender(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(this.MailService, "MailService");
      IMailSenderFactory mailSenderFactory;
      if (!this.m_mailSenderFactories.TryGetValue(this.MailService, out mailSenderFactory))
        throw new MailServiceUnavailbleException(FrameworkResources.MailServiceMissingPlugin((object) this.MailService));
      return mailSenderFactory.CreateMailSender(requestContext, this.SendTimeout, this.MaxMailSenderFailures);
    }

    public void LoadSettings(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1001060, "MailService", "Service", nameof (LoadSettings));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        RegistryEntryCollection registryEntryCollection1 = service.ReadEntriesFallThru(requestContext, (RegistryQuery) (FrameworkServerConstants.NotificationRootPath + "/**"));
        this.Enabled = false;
        if (registryEntryCollection1["EmailEnabled"].GetValue<bool>(true))
        {
          this.UseReplyTo = registryEntryCollection1["UseReplyTo"].GetValue<bool>(true);
          int defaultValue = requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? 30000 : 10000;
          this.SendTimeout = registryEntryCollection1["SendTimeout"].GetValue<int>(defaultValue);
          this.MaxMailSenderFailures = registryEntryCollection1["MaxMailSenderFailures"].GetValue<int>(3);
          this.MailService = registryEntryCollection1["MailService"].GetValue("Smtp");
          string address = registryEntryCollection1["EmailNotificationFromAddress"].GetValue(string.Empty);
          if (!string.IsNullOrEmpty(address))
          {
            try
            {
              this.FromAddress = new MailAddress(address);
              this.Enabled = true;
            }
            catch (Exception ex)
            {
              requestContext.TraceException(0, "MailService", "Service", ex);
            }
          }
          try
          {
            int length = TeamFoundationMailService.CustomSmtpHeadersRoot.Length;
            this.m_customSmtpHeaders = new List<Tuple<string, string>>();
            foreach (RegistryEntry registryEntry in registryEntryCollection1)
            {
              if (registryEntry.Path.Length > length && registryEntry.Path.StartsWith(TeamFoundationMailService.CustomSmtpHeadersRoot, StringComparison.OrdinalIgnoreCase))
                this.m_customSmtpHeaders.Add(new Tuple<string, string>(registryEntry.Path.Substring(length), registryEntry.Value));
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1001060, "MailService", "Service", ex);
          }
          RegistryEntryCollection registryEntryCollection2 = service.ReadEntriesFallThru(requestContext, (RegistryQuery) (FrameworkServerConstants.MailServiceRegistryRootPath + "/*"));
          this.LogLevel = registryEntryCollection2["LogLevel"].GetValue<int>(1);
          this.LogAllExceptions = registryEntryCollection2["LogAllExceptions"].GetValue<bool>(false);
          this.MaxRetryAttempts = registryEntryCollection2["MaxRetryAttempts"].GetValue<int>(5);
          this.SendMailJobDelay = registryEntryCollection2["JobDelay"].GetValue<int>(30);
          this.RetryInterval = registryEntryCollection2["RetryInterval"].GetValue<int>(900);
          this.LastRetry = registryEntryCollection2["LastRetry"].GetValue<DateTime>(DateTime.MinValue);
          this.MaxEmailBodySize = registryEntryCollection2["MaxEmailBodySizeInBytes"].GetValue<int>(1048576);
          string str1 = string.Format("SmtpConfig: sendTimeout={0} mailService={1} replyTo={2} from='{3}'", (object) this.SendTimeout, (object) this.MailService, (object) this.UseReplyTo, (object) this.FromAddress);
          string str2 = string.Format("ServiceConfig: logLevel={0} allExcept={1} maxRetries={2} jobDelay={3} retryInterval={4} lastRetry={5} maxEmailSize={6}", (object) this.LogLevel, (object) this.LogAllExceptions, (object) this.MaxRetryAttempts, (object) this.SendMailJobDelay, (object) this.RetryInterval, (object) this.LastRetry, (object) this.MaxEmailBodySize);
          requestContext.TraceAlways(1001060, TraceLevel.Info, "MailService", "Service", str1 + " " + str2);
        }
        else
          requestContext.Trace(1001060, TraceLevel.Warning, "MailService", "Service", "Mail disabled");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1001060, "MailService", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1001060, "MailService", "Service", nameof (LoadSettings));
      }
    }

    private void ApplyCustomHeaders(IVssRequestContext requestContext, MailMessage mm)
    {
      try
      {
        foreach (Tuple<string, string> customSmtpHeader in this.m_customSmtpHeaders)
          mm.Headers[customSmtpHeader.Item1] = customSmtpHeader.Item2;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1001050, "MailService", "Service", ex);
      }
    }

    public virtual bool Enabled { get; private set; }

    public virtual MailAddress FromAddress { get; private set; }

    public bool UseReplyTo { get; private set; }

    public int MaxRetryAttempts { get; private set; }

    public int SendMailJobDelay { get; private set; }

    public DateTime LastRetry { get; set; }

    public int RetryInterval { get; private set; }

    public bool LogAllExceptions { get; private set; }

    public int LogLevel { get; private set; }

    public int MaxEmailBodySize { get; private set; }

    public string MailService { get; private set; }

    public int SendTimeout { get; private set; }

    public int MaxMailSenderFailures { get; private set; }

    public IEnumerable<MailMessage> ValidateMessages(
      IVssRequestContext requestContext,
      IEnumerable<MailMessage> messages)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<MailMessage>>(messages, nameof (messages));
      return messages.Where<MailMessage>((Func<MailMessage, bool>) (m => this.IsValidMessage(requestContext, m)));
    }

    public void ValidateMessage(IVssRequestContext requestContext, MailMessage message)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<MailMessage>(message, nameof (message));
      if (message.IsMessageBodyOversized(this.MaxEmailBodySize))
        throw new MailValidationException(FrameworkResources.SendingOversizedEmailIsRejected((object) (this.MaxEmailBodySize / 1024)));
    }

    private bool IsValidMessage(IVssRequestContext requestContext, MailMessage message)
    {
      try
      {
        this.ValidateMessage(requestContext, message);
        return true;
      }
      catch (MailValidationException ex)
      {
        requestContext.Trace(1001071, TraceLevel.Warning, "MailService", "Service", ex.Message);
        return false;
      }
    }

    private void SetSender(IVssRequestContext requestContext, MailMessage message)
    {
      string header = message.Headers["Sender"];
      if (string.IsNullOrEmpty(header))
        return;
      try
      {
        message.Sender = new MailAddress(header);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1001032, TraceLevel.Warning, "MailService", "Service", "Failed to resolve Sender {0}", (object) header);
      }
    }
  }
}
