// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationHostManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (IInternalTeamFoundationHostManagementService))]
  public interface ITeamFoundationHostManagementService : IVssFrameworkService
  {
    DeploymentType DeploymentType { get; }

    bool IsHosted { get; }

    int HostDormancySeconds { get; }

    void CreateServiceHost(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostProperties hostProperties,
      ISqlConnectionInfo connectionInfo);

    void CreateServiceHost(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostProperties hostProperties,
      ISqlConnectionInfo connectionInfo,
      CreateHostOptions createOptions);

    void UpdateServiceHost(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostProperties hostProperties);

    void DeleteServiceHost(
      IVssRequestContext requestContext,
      Guid hostId,
      HostDeletionReason deletionReason,
      DeleteHostResourceOptions deleteHostResourceOptions);

    HostProperties QueryServiceHostPropertiesCached(IVssRequestContext requestContext, Guid hostId);

    IEnumerable<HostProperties> QueryChildrenServiceHostPropertiesCached(
      IVssRequestContext requestContext,
      Guid parentHostId);

    IList<HostProperties> QueryChildrenServiceHostProperties(
      IVssRequestContext requestContext,
      IList<Guid> parentHostIds);

    TeamFoundationServiceHostProperties QueryServiceHostProperties(
      IVssRequestContext requestContext,
      Guid hostId);

    IList<HostProperties> QueryServiceHostPropertiesBatch(
      IVssRequestContext requestContext,
      ICollection<Guid> hostIds);

    TeamFoundationServiceHostProperties QueryServiceHostProperties(
      IVssRequestContext requestContext,
      Guid hostId,
      ServiceHostFilterFlags filterFlags);

    List<HostProperties> QueryServiceHostProperties(
      IVssRequestContext requestContext,
      int databaseId,
      int maxResults);

    ICollection<HostProperties> QueryServiceHostProperties(
      IVssRequestContext requestContext,
      int databaseId,
      int batchSize,
      Guid? minHostId);

    TeamFoundationExecutionState QueryExecutionState(IVssRequestContext requestContext);

    TeamFoundationExecutionState QueryExecutionState(IVssRequestContext requestContext, Guid hostId);

    TeamFoundationHostReadyState QueryHostReadyState(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostProperties hostProperties);

    bool PingHostProcess(IVssRequestContext requestContext, Guid processId, TimeSpan pingTimeout);

    DateTime GetConfigDataTierTime(IVssRequestContext requestContext);

    void StartHost(IVssRequestContext requestContext, Guid hostId, ServiceHostSubStatus subStatus = ServiceHostSubStatus.None);

    void Stop(IVssRequestContext systemRequestContext);

    bool StopHost(
      IVssRequestContext requestContext,
      Guid hostId,
      ServiceHostSubStatus subStatus,
      string reason,
      TimeSpan timeout);

    void DetectInactiveProcesses(IVssRequestContext requestContext);

    IVssRequestContext BeginUserRequest(
      IVssRequestContext requestContext,
      Guid instanceId,
      IdentityDescriptor userContext,
      bool throwIfShutdown = false);

    IVssRequestContext BeginUserRequest(
      IVssRequestContext requestContext,
      Guid instanceId,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      bool throwIfShutdown = false);

    IVssRequestContext BeginRequest(
      IVssRequestContext requestContext,
      Guid instanceId,
      RequestContextType contextType,
      bool loadIfNecessary = true,
      bool throwIfShutdown = true);

    List<ServiceHostHistoryEntry> QueryServiceHostHistory(
      IVssRequestContext requestContext,
      int watermark);
  }
}
