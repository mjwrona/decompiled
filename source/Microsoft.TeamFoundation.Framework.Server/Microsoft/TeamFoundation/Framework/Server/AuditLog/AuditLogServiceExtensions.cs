// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.AuditLogServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Audit;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class AuditLogServiceExtensions
  {
    public static AuditLogEntry CreateAuditLogEntryRaw(
      this IVssRequestContext requestContext,
      string actionId = null,
      IDictionary<string, object> data = null,
      Guid targetHostId = default (Guid),
      Guid projectId = default (Guid))
    {
      return requestContext.CreateAuditLogEntryRaw(actionId, data, new AuditLogContextOverride(targetHostId, projectId));
    }

    public static AuditLogEntry CreateAuditLogEntryRaw(
      this IVssRequestContext requestContext,
      string actionId,
      IDictionary<string, object> data,
      AuditLogContextOverride contextOverride)
    {
      if (!requestContext.ShouldAuditLogEvents())
        return (AuditLogEntry) null;
      bool flag = ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext);
      Guid projectId = contextOverride.ProjectId;
      Guid targetHostId = contextOverride.TargetHostId;
      string str1 = contextOverride.UserAgent ?? requestContext.UserAgent;
      string str2 = contextOverride.IPAddress ?? (flag ? (string) null : requestContext.RemoteIPAddress());
      AuditLogEntry auditLogEntryRaw = new AuditLogEntry()
      {
        Id = Guid.NewGuid(),
        ActivityId = requestContext.ActivityId,
        AuthenticationMechanism = requestContext.GetAuthenticationMechanism(),
        IPAddress = str2,
        UserAgent = str1,
        ScopeId = requestContext.RootContext.ServiceHost.InstanceId,
        ProjectId = projectId,
        Timestamp = DateTime.UtcNow,
        ScopeType = (AuditScopeType) requestContext.ServiceHost.HostType,
        ActionId = actionId,
        Data = data
      };
      if (requestContext.UserContext.IsAadServicePrincipalType())
      {
        auditLogEntryRaw.ActorClientId = requestContext.GetUserActor().Identifier;
        auditLogEntryRaw.ActorCUID = new Guid();
        auditLogEntryRaw.ActorUserId = new Guid();
      }
      else
      {
        auditLogEntryRaw.ActorClientId = new Guid();
        auditLogEntryRaw.ActorCUID = requestContext.GetUserCuid();
        auditLogEntryRaw.ActorUserId = requestContext.GetUserId();
      }
      if (auditLogEntryRaw.ActorClientId == new Guid() && auditLogEntryRaw.ActorUserId == new Guid())
        requestContext.TraceAlways(31000802, TraceLevel.Warning, nameof (AuditLogServiceExtensions), nameof (CreateAuditLogEntryRaw), string.Format("Failed to resolve an actor id for the audit log entry, <Id: {0}, ActionId: {1}, Scope: {2} ({3})>\r\nIsAadServicePrincipalType: {4}\r\nIdentityType: {5}\r\nUserActorIdentifier: {6}\r\nUserCUID: {7}\r\nUserId: {8}\r\nAuthenticationMechanism: {9}\r\nUserAgent: {10}\r\nProjectId: {11}", (object) auditLogEntryRaw.Id, (object) auditLogEntryRaw.ActionId, (object) auditLogEntryRaw.ScopeId, (object) auditLogEntryRaw.ScopeType, (object) requestContext.UserContext.IsAadServicePrincipalType(), (object) requestContext.UserContext.IdentityType, (object) requestContext.GetUserActor()?.Identifier, (object) requestContext.GetUserCuid(), (object) requestContext.GetUserId(), (object) auditLogEntryRaw.AuthenticationMechanism, (object) auditLogEntryRaw.UserAgent, (object) auditLogEntryRaw.ProjectId));
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.IsFeatureEnabled("VisualStudio.Services.Audit.AllowDeploymentDispatch"))
      {
        auditLogEntryRaw.ScopeType = AuditScopeType.Unknown;
        auditLogEntryRaw.ScopeId = targetHostId;
      }
      else
      {
        TeamFoundationServiceHostProperties hostProperties;
        if (targetHostId != new Guid() && targetHostId != requestContext.ServiceHost.InstanceId && requestContext.IsHostDescendant(targetHostId, out hostProperties))
        {
          auditLogEntryRaw.ScopeId = hostProperties.Id;
          auditLogEntryRaw.ScopeType = (AuditScopeType) hostProperties.HostType;
        }
      }
      string authorizationId = requestContext.GetAuthorizationId();
      if (!string.IsNullOrEmpty(authorizationId))
      {
        AuditLogEntry auditLogEntry = auditLogEntryRaw;
        auditLogEntry.AuthenticationMechanism = auditLogEntry.AuthenticationMechanism + " authorizationId: " + authorizationId;
      }
      auditLogEntryRaw.CorrelationId = requestContext.EnsureAuditCorrelationId(auditLogEntryRaw.Id);
      return auditLogEntryRaw;
    }

    public static bool IsHostDescendant(
      this IVssRequestContext requestContext,
      Guid targetHostId,
      out TeamFoundationServiceHostProperties hostProperties)
    {
      hostProperties = (TeamFoundationServiceHostProperties) null;
      if (targetHostId == new Guid() || requestContext == null)
        return false;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationHostManagementService service = vssRequestContext.GetService<ITeamFoundationHostManagementService>();
      hostProperties = service.QueryServiceHostProperties(vssRequestContext, targetHostId);
      if (hostProperties != null && (requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment) || requestContext.ServiceHost.InstanceId == targetHostId || requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Application) && hostProperties.HostType.HasFlag((Enum) TeamFoundationHostType.ProjectCollection) && requestContext.ServiceHost.InstanceId == hostProperties.ParentId))
        return true;
      throw new InvalidAuditScopeIdException(string.Format("The provided target host id {0} is not a child of the current request context {1}", (object) targetHostId, (object) requestContext.ServiceHost.InstanceId));
    }

    public static bool ShouldAuditLogEvents(this IVssRequestContext requestContext)
    {
      bool flag = requestContext.ExecutionEnvironment.IsHostedDeployment && (requestContext.RequestPriority != VssRequestContextPriority.Low || !requestContext.IsServicingContext);
      if (flag)
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          try
          {
            flag = requestContext.IsOrganizationAadBacked();
          }
          catch (Exception ex)
          {
            requestContext.TraceException(31000801, TraceLevel.Warning, nameof (AuditLogServiceExtensions), nameof (ShouldAuditLogEvents), ex);
          }
        }
      }
      return flag;
    }

    public static void HandlePostAuditLog(this IVssRequestContext requestContext) => requestContext.GetService<IAuditLogService>().HandlePostLog(requestContext);

    internal static void AddAuditCorrelationId(
      this ServicingJobData jobData,
      IVssRequestContext requestContext)
    {
      if (!requestContext.ShouldAuditLogEvents())
        return;
      jobData.ServicingTokens[RequestContextItemsKeys.AuditLogCorrelationId] = requestContext.EnsureAuditCorrelationId(Guid.NewGuid(), (Action) (() =>
      {
        if (!requestContext.IsHostProcessType(HostProcessType.ApplicationTier))
          return;
        requestContext.Trace(31000800, TraceLevel.Info, nameof (AuditLogServiceExtensions), nameof (AddAuditCorrelationId), string.Format("{0} is queuing {1} ServicingJob without a CorrelationId", (object) Assembly.GetEntryAssembly(), (object) jobData.JobTitle));
      })).ToString();
    }

    internal static void SetAuditCorrelationId(
      this IVssRequestContext requestContext,
      IDictionary<string, string> tokens)
    {
      string g;
      if (!tokens.TryGetValue(RequestContextItemsKeys.AuditLogCorrelationId, out g))
        return;
      requestContext.Items[RequestContextItemsKeys.AuditLogCorrelationId] = (object) new Guid(g);
    }

    public static Guid EnsureAuditCorrelationId(this IVssRequestContext requestContext) => requestContext.EnsureAuditCorrelationId(Guid.NewGuid());

    public static Guid EnsureAuditCorrelationId(
      this IVssRequestContext requestContext,
      Guid defaultGuid,
      Action callBack = null)
    {
      IVssRequestContext rootContext = requestContext.RootContext;
      Guid guid;
      if (!rootContext.TryGetItem<Guid>(RequestContextItemsKeys.AuditLogCorrelationId, out guid) || guid == new Guid())
      {
        if (callBack != null)
          callBack();
        guid = defaultGuid;
        rootContext.Items[RequestContextItemsKeys.AuditLogCorrelationId] = (object) guid;
      }
      return guid;
    }

    public static string AsTableRowKeyPrefix(this DateTime dateTime) => string.Format("{0:D19}", (object) (DateTime.MaxValue - dateTime).Ticks);
  }
}
