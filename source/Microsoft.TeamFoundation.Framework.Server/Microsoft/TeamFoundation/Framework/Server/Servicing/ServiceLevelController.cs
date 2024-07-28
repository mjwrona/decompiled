// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Servicing.ServiceLevelController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Servicing;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Framework.Server.Servicing
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Servicing", ResourceName = "ServiceLevel")]
  public class ServiceLevelController : TfsApiController
  {
    [HttpGet]
    public ServiceLevelData GetServiceLevel()
    {
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      ServiceLevelData serviceLevelData = new ServiceLevelData();
      IVssRequestContext vssRequestContext = tfsRequestContext.To(TeamFoundationHostType.Deployment).Elevate();
      ITeamFoundationDatabaseManagementService service1 = vssRequestContext.GetService<ITeamFoundationDatabaseManagementService>();
      ITeamFoundationHostManagementService service2 = vssRequestContext.GetService<ITeamFoundationHostManagementService>();
      if (tfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        ServiceLevelController.UpdateCollectionServiceLevel(tfsRequestContext, vssRequestContext, service1, service2, serviceLevelData);
      else if (!tfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        ServiceLevelController.UpdateOrganizationServiceLevel(tfsRequestContext, vssRequestContext, service1, service2, serviceLevelData);
      else
        ServiceLevelController.UpdateDeploymentServiceLevel(vssRequestContext, service1, service2, serviceLevelData);
      return serviceLevelData;
    }

    private static void UpdateCollectionServiceLevel(
      IVssRequestContext collectionRequestContext,
      IVssRequestContext deploymentContext,
      ITeamFoundationDatabaseManagementService dbms,
      ITeamFoundationHostManagementService hms,
      ServiceLevelData serviceLevelData)
    {
      IServiceHostInternal serviceHostInternal = collectionRequestContext.ServiceHost.ServiceHostInternal();
      serviceLevelData.CollectionHostServiceLevel = hms.QueryServiceHostPropertiesCached(deploymentContext, collectionRequestContext.ServiceHost.InstanceId).ServiceLevel;
      serviceLevelData.CollectionDatabaseServiceLevel = dbms.GetDatabase(deploymentContext, serviceHostInternal.DatabaseId).ServiceLevel;
      Dataspace dataspace = collectionRequestContext.GetService<IDataspaceService>().QueryDataspace(collectionRequestContext, "Build", Guid.Empty, false);
      if (dataspace != null && dataspace.DatabaseId != serviceHostInternal.DatabaseId)
        serviceLevelData.BuildServiceLevel = dbms.GetDatabase(deploymentContext, dataspace.DatabaseId).ServiceLevel;
      ServiceLevelController.UpdateOrganizationServiceLevel(collectionRequestContext.To(TeamFoundationHostType.Application), deploymentContext, dbms, hms, serviceLevelData);
    }

    private static void UpdateOrganizationServiceLevel(
      IVssRequestContext organizationRequestContext,
      IVssRequestContext deploymentContext,
      ITeamFoundationDatabaseManagementService dbms,
      ITeamFoundationHostManagementService hms,
      ServiceLevelData serviceLevelData)
    {
      IServiceHostInternal serviceHostInternal = organizationRequestContext.ServiceHost.ServiceHostInternal();
      serviceLevelData.AccountHostServiceLevel = hms.QueryServiceHostPropertiesCached(deploymentContext, organizationRequestContext.ServiceHost.InstanceId).ServiceLevel;
      if (serviceHostInternal.DatabaseId != -2)
        serviceLevelData.AccountDatabaseServiceLevel = dbms.GetDatabase(deploymentContext, serviceHostInternal.DatabaseId).ServiceLevel;
      ServiceLevelController.UpdateDeploymentServiceLevel(deploymentContext, dbms, hms, serviceLevelData);
    }

    private static void UpdateDeploymentServiceLevel(
      IVssRequestContext deploymentContext,
      ITeamFoundationDatabaseManagementService dbms,
      ITeamFoundationHostManagementService hms,
      ServiceLevelData serviceLevelData)
    {
      IServiceHostInternal serviceHostInternal = deploymentContext.ServiceHost.ServiceHostInternal();
      serviceLevelData.DeploymentHostServiceLevel = hms.QueryServiceHostPropertiesCached(deploymentContext, deploymentContext.ServiceHost.InstanceId).ServiceLevel;
      serviceLevelData.ConfigurationDatabaseServiceLevel = dbms.GetDatabase(deploymentContext, serviceHostInternal.DatabaseId).ServiceLevel;
      IVssRegistryService service = deploymentContext.GetService<IVssRegistryService>();
      serviceLevelData.ServicingAreas = service.GetValue(deploymentContext, (RegistryQuery) FrameworkServerConstants.ServicingAreas, false, (string) null);
      if (!deploymentContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      serviceLevelData.AccountHostServiceLevel = serviceLevelData.DeploymentHostServiceLevel;
      serviceLevelData.AccountDatabaseServiceLevel = serviceLevelData.ConfigurationDatabaseServiceLevel;
    }
  }
}
