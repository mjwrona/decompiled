// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.UserMapping.FrameworkUserAccountMappingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.UserMapping.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.UserMapping
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FrameworkUserAccountMappingService : IUserAccountMappingService, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private readonly ActionTracer m_tracer = new ActionTracer("UserMapping", nameof (FrameworkUserAccountMappingService));
    private const string c_area = "UserMapping";
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
      Guid userId,
      UserType userType,
      bool useEqualsCheckForUserTypeMatch = false,
      bool includeDeletedAccounts = false)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      return (IList<Guid>) this.m_tracer.TraceAction<List<Guid>>(context, (ActionTracePoints) FrameworkUserAccountMappingService.TracePoints.QueryAccountIds, (Func<List<Guid>>) (() => FrameworkUserAccountMappingService.GetUserMappingClient(context).QueryAccountIdsAsync(userId.ToString(), userType, new bool?(useEqualsCheckForUserTypeMatch), new bool?(includeDeletedAccounts)).SyncResult<List<Guid>>()), nameof (QueryAccountIds));
    }

    public void ActivateMapping(IVssRequestContext context, Guid userId, Guid accountId)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      this.m_tracer.TraceAction(context, (ActionTracePoints) FrameworkUserAccountMappingService.TracePoints.ActivateMapping, (Action) (() => FrameworkUserAccountMappingService.GetUserMappingClient(context).ActivateUserAccountMappingAsync(userId, accountId).SyncResult()), nameof (ActivateMapping));
    }

    public void ActivateMapping(
      IVssRequestContext context,
      Guid userId,
      Guid accountId,
      UserType userType)
    {
      this.ValidateRequestContext(context);
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      this.m_tracer.TraceAction(context, (ActionTracePoints) FrameworkUserAccountMappingService.TracePoints.ActivateMappingWithUserType, (Action) (() => FrameworkUserAccountMappingService.GetUserMappingClient(context).ActivateUserAccountMappingAsync(userId, accountId, userType).SyncResult()), nameof (ActivateMapping));
    }

    public Guid GetSyncUserAccountMappingsStatus(IVssRequestContext requestContext)
    {
      this.ValidateProjectCollectionRequestContext(requestContext);
      return FrameworkUserAccountMappingService.GetUserMappingClient(requestContext).GetSyncUserAccountMappingsStatusAsync().SyncResult<Guid>();
    }

    public Guid QueueSyncUserAccountMappings(IVssRequestContext requestContext)
    {
      this.ValidateProjectCollectionRequestContext(requestContext);
      return FrameworkUserAccountMappingService.GetUserMappingClient(requestContext).QueueSyncUserAccountMappingsAsync().SyncResult<Guid>();
    }

    private void ValidateRequestContext(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckDeploymentRequestContext();
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
    }

    private void ValidateProjectCollectionRequestContext(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckProjectCollectionRequestContext();
    }

    private static UserMappingHttpClient GetUserMappingClient(IVssRequestContext requestContext) => requestContext.GetClient<UserMappingHttpClient>();

    private static class TracePoints
    {
      internal static readonly TimedActionTracePoints QueryAccountIds = new TimedActionTracePoints(7720100, 7720107, 7720108, 7720109);
      internal static readonly TimedActionTracePoints ActivateMapping = new TimedActionTracePoints(7720110, 7720117, 7720118, 7720119);
      internal static readonly TimedActionTracePoints ActivateMappingWithUserType = new TimedActionTracePoints(7720120, 7720127, 7720128, 7720129);
    }
  }
}
