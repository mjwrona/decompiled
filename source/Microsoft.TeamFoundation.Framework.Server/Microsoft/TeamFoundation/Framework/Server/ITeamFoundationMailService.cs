// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationMailService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationMailService))]
  public interface ITeamFoundationMailService : IVssFrameworkService
  {
    bool Enabled { get; }

    MailAddress FromAddress { get; }

    bool UseReplyTo { get; }

    int MaxRetryAttempts { get; }

    int SendMailJobDelay { get; }

    DateTime LastRetry { get; set; }

    int RetryInterval { get; }

    bool LogAllExceptions { get; }

    int LogLevel { get; }

    int MaxEmailBodySize { get; }

    void LoadSettings(IVssRequestContext requestContext);

    MailRequest QueueMailJob(IVssRequestContext requestContext, MailMessage message);

    ICollection<MailRequest> QueueMailJob(
      IVssRequestContext requestContext,
      IEnumerable<MailMessage> messages);

    void Send(IVssRequestContext requestContext, MailMessage message);

    void Send(IVssRequestContext requestContext, IEnumerable<MailMessage> messages);

    void ValidateMessage(IVssRequestContext requestContext, MailMessage message);

    IEnumerable<MailMessage> ValidateMessages(
      IVssRequestContext requestContext,
      IEnumerable<MailMessage> messages);

    void SetReplyToAddress(MailMessage message, string replyToOverride);

    VssMailSender GetMailSender(IVssRequestContext requestContext);
  }
}
