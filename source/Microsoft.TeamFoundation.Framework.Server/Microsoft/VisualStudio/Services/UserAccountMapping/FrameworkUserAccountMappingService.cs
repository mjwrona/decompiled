// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UserAccountMapping.FrameworkUserAccountMappingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.UserAccountMapping
{
  public class FrameworkUserAccountMappingService : IUserAccountMappingService, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private readonly ActionTracer m_tracer = new ActionTracer("UserAccountMapping", nameof (FrameworkUserAccountMappingService));
    private const string c_area = "UserAccountMapping";
    private const string c_layer = "FrameworkUserAccountMappingService";

    public void ServiceStart(IVssRequestContext context)
    {
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.ValidateRequestContext(context);
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public virtual IList<Guid> QueryAccountIds(
      IVssRequestContext context,
      SubjectDescriptor userId,
      UserRole userRole,
      bool useEqualsCheckForUserRoleMatch = false,
      bool includeDeletedAccounts = false)
    {
      this.ValidateRequestContext(context);
      return (IList<Guid>) this.m_tracer.TraceAction<List<Guid>>(context, (ActionTracePoints) FrameworkUserAccountMappingService.TracePoints.QueryAccountIds, (Func<List<Guid>>) (() => this.GetUserAccountMappingClient(context).QueryAccountIdsAsync(userId, userRole, new bool?(useEqualsCheckForUserRoleMatch), new bool?(includeDeletedAccounts)).SyncResult<List<Guid>>()), nameof (QueryAccountIds));
    }

    public void ActivateMapping(
      IVssRequestContext context,
      SubjectDescriptor userId,
      Guid accountId)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      this.m_tracer.TraceAction(context, (ActionTracePoints) FrameworkUserAccountMappingService.TracePoints.ActivateMapping, (Action) (() => this.GetUserAccountMappingClient(context).ActivateUserAccountMappingAsync(userId, accountId).SyncResult()), nameof (ActivateMapping));
    }

    public void ActivateMapping(
      IVssRequestContext context,
      SubjectDescriptor userId,
      Guid accountId,
      UserRole userRole)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      this.m_tracer.TraceAction(context, (ActionTracePoints) FrameworkUserAccountMappingService.TracePoints.ActivateMappingWithUserType, (Action) (() => this.GetUserAccountMappingClient(context).ActivateUserAccountMappingAsync(userId, accountId, userRole).SyncResult()), nameof (ActivateMapping));
    }

    public void SetUserAccountLicenseInfo(
      IVssRequestContext context,
      SubjectDescriptor userId,
      Guid accountId,
      VisualStudioLevel maxVsLevelFromAccountLicense,
      VisualStudioLevel maxVsLevelFromAccountExtensions)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      this.m_tracer.TraceAction(context, (ActionTracePoints) FrameworkUserAccountMappingService.TracePoints.ActivateMappingWithUserType, (Action) (() => this.GetUserAccountMappingClient(context).SetUserAccountLicenseInfoAsync((string) userId, accountId, maxVsLevelFromAccountLicense, maxVsLevelFromAccountExtensions).SyncResult()), nameof (SetUserAccountLicenseInfo));
    }

    public void DeactivateMapping(
      IVssRequestContext context,
      SubjectDescriptor userId,
      Guid accountId)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      this.m_tracer.TraceAction(context, (ActionTracePoints) FrameworkUserAccountMappingService.TracePoints.DeactivateMapping, (Action) (() => this.GetUserAccountMappingClient(context).DeactivateUserAccountMappingAsync(userId, accountId).SyncResult()), nameof (DeactivateMapping));
    }

    public bool HasMappings(IVssRequestContext context, SubjectDescriptor userId)
    {
      this.ValidateRequestContext(context);
      return this.m_tracer.TraceAction<bool>(context, (ActionTracePoints) FrameworkUserAccountMappingService.TracePoints.HasMappings, (Func<bool>) (() => this.GetUserAccountMappingClient(context).HasMappingsAsync(userId).SyncResult<bool>()), nameof (HasMappings));
    }

    private void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckDeploymentRequestContext();
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }

    private FrameworkUserAccountMappingHttpClient GetUserAccountMappingClient(
      IVssRequestContext requestContext)
    {
      return requestContext.GetClient<FrameworkUserAccountMappingHttpClient>();
    }

    private static class TracePoints
    {
      internal static readonly TimedActionTracePoints QueryAccountIds = new TimedActionTracePoints(7720100, 7720107, 7720108, 7720109);
      internal static readonly TimedActionTracePoints ActivateMapping = new TimedActionTracePoints(7720110, 7720117, 7720118, 7720119);
      internal static readonly TimedActionTracePoints ActivateMappingWithUserType = new TimedActionTracePoints(7720120, 7720127, 7720128, 7720129);
      internal static readonly TimedActionTracePoints DeactivateMapping = new TimedActionTracePoints(7720130, 7720137, 7720138, 7720139);
      internal static readonly TimedActionTracePoints HasMappings = new TimedActionTracePoints(7720140, 7720147, 7720148, 7720149);
    }
  }
}
