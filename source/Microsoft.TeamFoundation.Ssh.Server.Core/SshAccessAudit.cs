// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.Core.SshAccessAudit
// Assembly: Microsoft.TeamFoundation.Ssh.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3DF8FBEE-AA1B-4659-8650-E7C7E1E085EB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.TeamFoundation.Framework.Server.Serialization;
using System;

namespace Microsoft.TeamFoundation.Ssh.Server.Core
{
  public class SshAccessAudit
  {
    public const string EnableSshAccessAuditFeatureFlag = "VisualStudio.Services.Identity.SshAccessAuditing.Enable";
    private readonly IEventSerializer m_eventSerializer;

    public SshAccessAudit(IEventSerializer eventSerializer) => this.m_eventSerializer = eventSerializer;

    public void AuditUnauthorizedSshRequest(
      IVssRequestContext requestContext,
      GitSshCommandInfo gitCommand,
      string reason)
    {
      this.LogSshAccessAuditEvent(requestContext, gitCommand, "Unauthorized", reason);
    }

    public void AuditAuthorizedSshRequest(
      IVssRequestContext requetContext,
      GitSshCommandInfo gitCommand)
    {
      this.LogSshAccessAuditEvent(requetContext, gitCommand, "OK");
    }

    private void LogSshAccessAuditEvent(
      IVssRequestContext requestContext,
      GitSshCommandInfo gitCommand,
      string status,
      string reason = "")
    {
      try
      {
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.SshAccessAuditing.Enable") || !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          return;
        string commandName = gitCommand.CommandName;
        AuthenticationAuditEvent e = AuthenticationAuditEvent.Create(requestContext, status, reason, commandName);
        requestContext.LogAuditEvent("User.SshAccessEvent", this.m_eventSerializer.ToObjectDict<AuthenticationAuditEvent>(e));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(13002013, "Ssh", nameof (SshAccessAudit), ex);
      }
    }
  }
}
