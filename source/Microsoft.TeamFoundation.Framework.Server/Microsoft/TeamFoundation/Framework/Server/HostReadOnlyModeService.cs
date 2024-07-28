// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostReadOnlyModeService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class HostReadOnlyModeService : IHostReadOnlyModeService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.CheckHostedDeployment();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool IsReadOnly(IVssRequestContext requestContext, Guid hostId)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.CheckHostedDeployment();
      TeamFoundationServiceHostProperties serviceHostProperties = requestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(requestContext, hostId);
      if (serviceHostProperties == null)
        throw new HostDoesNotExistException(hostId);
      using (DatabasePartitionComponent component = DatabasePartitionComponent.CreateComponent(requestContext.GetService<TeamFoundationDatabaseManagementService>().GetDatabase(requestContext, serviceHostProperties.DatabaseId, true).GetDboConnectionInfo()))
        return component.IsReadOnlyPartition((component.QueryPartition(hostId) ?? throw new DatabasePartitionNotFoundException(hostId)).PartitionId);
    }

    public void UpdateReadOnlyState(
      IVssRequestContext requestContext,
      Guid hostId,
      bool isReadOnly,
      bool waitForCompletion)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.CheckHostedDeployment();
      TeamFoundationServiceHostProperties serviceHostProperties = requestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(requestContext, hostId);
      using (DatabasePartitionComponent component = DatabasePartitionComponent.CreateComponent(requestContext.GetService<TeamFoundationDatabaseManagementService>().GetDatabase(requestContext, serviceHostProperties.DatabaseId, true).GetDboConnectionInfo()))
        component.SetReadOnlyPartitionState(hostId, isReadOnly, waitForCompletion);
    }
  }
}
