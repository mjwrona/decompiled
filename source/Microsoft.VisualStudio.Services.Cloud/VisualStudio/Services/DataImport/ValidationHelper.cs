// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DataImport.ValidationHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.DataImport;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.HostManagement;
using Microsoft.VisualStudio.Services.HostManagement.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.DataImport
{
  public class ValidationHelper
  {
    private const string c_area = "ValidationHelper";
    private const string c_layer = "DataImport";

    public void ValidateDataImportAccountRegion(
      IVssRequestContext requestContext,
      DatabaseDataImportRequest import)
    {
      ArgumentUtility.CheckForNull<DatabaseDataImportRequest>(import, nameof (import));
      if (!(requestContext.ServiceInstanceType() != ServiceInstanceTypes.SPS))
        return;
      string property = import.Properties["DataImport.AccountRegion"];
      ArgumentUtility.CheckStringForNullOrEmpty(property, "import.AccountRegion");
      IInstanceManagementService service = requestContext.GetService<IInstanceManagementService>();
      IList<ServiceInstance> instancesForRegion = service.GetServiceInstancesForRegion(requestContext, property);
      if (instancesForRegion == null || !instancesForRegion.Any<ServiceInstance>())
      {
        requestContext.Trace(610015, TraceLevel.Info, nameof (ValidationHelper), "DataImport", "No ServiceInstances in Region: " + property);
        foreach (ServiceInstance serviceInstance in (IEnumerable<ServiceInstance>) service.GetServiceInstances(requestContext))
          requestContext.Trace(610016, TraceLevel.Error, nameof (ValidationHelper), "DataImport", string.Format("Service Instance {0}, with Regions and Weights: {1}", (object) serviceInstance.InstanceId, (object) serviceInstance.Regions));
        throw new InvalidRegionException(property);
      }
    }

    public void ValidateDataImportAccountOwner(
      IVssRequestContext requestContext,
      DatabaseDataImportRequest import)
    {
      ArgumentUtility.CheckForNull<DatabaseDataImportRequest>(import, nameof (import));
      string property = import.Properties["DataImport.AccountOwner"];
      ArgumentUtility.CheckStringForNullOrEmpty(property, "import.AccountOwner");
      Guid guid = new Guid(property);
      ArgumentUtility.CheckForEmptyGuid(guid, "import.AccountOwner");
      if (requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
      {
        guid
      }, QueryMembership.None, (IEnumerable<string>) null).Single<Microsoft.VisualStudio.Services.Identity.Identity>() == null)
        throw new IdentityNotFoundException("The account owner was not found.");
    }

    public void ValidateDataImportDatabase(
      IVssRequestContext requestContext,
      DatabaseDataImportRequest import)
    {
      ArgumentUtility.CheckForNull<DatabaseDataImportRequest>(import, nameof (import));
      ArgumentUtility.CheckStringForNullOrEmpty(import.ConnectionString, "connectionString");
      this.ValidateSourceConnectionString(requestContext, import.RequestId, import.ConnectionString);
      ISqlConnectionInfo connectionInfo = SqlConnectionInfoFactory.Create(import.ConnectionString);
      this.ValidateSupportedMilestones(requestContext, connectionInfo, (string) null);
    }

    public void CheckTargetCollectionHost(
      IVssRequestContext requestContext,
      DatabaseDataImportRequest import)
    {
      this.CheckTargetCollectionHost(requestContext, import.HostId);
    }

    public bool CheckHostUpgradeRequest(
      IVssRequestContext requestContext,
      HostUpgradeDataImportRequest hostUpgrade)
    {
      this.CheckTargetCollectionHost(requestContext, hostUpgrade.HostId);
      ValidationHelper.CheckHostStatuses(requestContext, hostUpgrade.HostId, false, true);
      IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      TeamFoundationServiceHostProperties serviceHostProperties = vssRequestContext1.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(vssRequestContext1, hostUpgrade.HostId);
      if (serviceHostProperties == null)
        throw new ApplicationException(string.Format("Failed to Find Host with Id {0}", (object) hostUpgrade.HostId));
      ISqlRegistryService service = vssRequestContext1.GetService<ISqlRegistryService>();
      string registryPathPattern = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Configuration/DataImport/{0}/TargetDatabaseId", (object) hostUpgrade.RequestId);
      IVssRequestContext requestContext1 = vssRequestContext1;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @new RegistryQuery(registryPathPattern);
      int invalidDatabaseId = DatabaseManagementConstants.InvalidDatabaseId;
      int databaseId = service.GetValue<int>(requestContext1, in local, invalidDatabaseId);
      if (databaseId == DatabaseManagementConstants.InvalidDatabaseId)
        throw new ApplicationException("Failed to find valid Target Database Id in " + registryPathPattern);
      IVssRequestContext vssRequestContext2 = requestContext.Elevate();
      ITeamFoundationDatabaseProperties database = vssRequestContext2.GetService<ITeamFoundationDatabaseManagementService>().GetDatabase(vssRequestContext2, databaseId, false);
      return !ServiceLevel.AreServiceLevelsEquivalent(vssRequestContext1, serviceHostProperties.ServiceLevel, database.ServiceLevel);
    }

    public void CheckOnlinePostHostUpgrade(
      IVssRequestContext requestContext,
      OnlinePostHostUpgradeDataImportRequest postHostUpgrade)
    {
      ArgumentUtility.CheckForNull<OnlinePostHostUpgradeDataImportRequest>(postHostUpgrade, nameof (postHostUpgrade));
      ArgumentUtility.CheckForEmptyGuid(postHostUpgrade.RequestId, "RequestId");
      ArgumentUtility.CheckForEmptyGuid(postHostUpgrade.ServicingJobId, "ServicingJobId");
      ArgumentUtility.CheckForEmptyGuid(postHostUpgrade.HostId, "HostId");
      this.CheckTargetCollectionHost(requestContext, postHostUpgrade.HostId);
      ValidationHelper.CheckHostStatuses(requestContext, postHostUpgrade.HostId, false, true);
    }

    public void CheckStopHostAfterUpgrade(
      IVssRequestContext requestContext,
      StopHostAfterUpgradeDataImportRequest stopHost)
    {
      ArgumentUtility.CheckForNull<StopHostAfterUpgradeDataImportRequest>(stopHost, nameof (stopHost));
      ArgumentUtility.CheckForEmptyGuid(stopHost.RequestId, "RequestId");
      ArgumentUtility.CheckForEmptyGuid(stopHost.ServicingJobId, "ServicingJobId");
      ArgumentUtility.CheckForEmptyGuid(stopHost.HostId, "HostId");
      this.CheckTargetCollectionHost(requestContext, stopHost.HostId);
      ValidationHelper.CheckHostStatuses(requestContext, stopHost.HostId, true, true);
    }

    public void CheckObtainDatabaseHold(
      IVssRequestContext requestContext,
      ObtainDatabaseHoldDataImportRequest obtainDatabaseHold)
    {
      ArgumentUtility.CheckForNull<ObtainDatabaseHoldDataImportRequest>(obtainDatabaseHold, nameof (obtainDatabaseHold));
      ArgumentUtility.CheckForEmptyGuid(obtainDatabaseHold.RequestId, "RequestId");
      ArgumentUtility.CheckForEmptyGuid(obtainDatabaseHold.ServicingJobId, "ServicingJobId");
      ArgumentUtility.CheckForEmptyGuid(obtainDatabaseHold.HostId, "HostId");
      if (obtainDatabaseHold.HostToMovePostImport == DataImportHostMove.Both && Guid.Empty.Equals(obtainDatabaseHold.NeighborHostId))
        throw new InvalidOperationException(string.Format("{0} can only be set to {1} if {2} is specified", (object) "HostToMovePostImport", (object) obtainDatabaseHold.HostToMovePostImport, (object) "NeighborHostId"));
      this.CheckTargetCollectionHost(requestContext, obtainDatabaseHold.HostId);
      ValidationHelper.CheckHostStatuses(requestContext, obtainDatabaseHold.HostId, true, false);
    }

    public void CheckHostMoveRequest(
      IVssRequestContext requestContext,
      HostMoveDataImportRequest hostMove)
    {
      ArgumentUtility.CheckForNull<HostMoveDataImportRequest>(hostMove, nameof (hostMove));
      ArgumentUtility.CheckForEmptyGuid(hostMove.RequestId, "RequestId");
      ArgumentUtility.CheckForEmptyGuid(hostMove.ServicingJobId, "ServicingJobId");
      ArgumentUtility.CheckForEmptyGuid(hostMove.HostId, "HostId");
      if (hostMove.HostToMovePostImport == DataImportHostMove.Both && Guid.Empty.Equals(hostMove.NeighborHostId))
        throw new InvalidOperationException(string.Format("{0} can only be set to {1} if {2} is specified", (object) "HostToMovePostImport", (object) hostMove.HostToMovePostImport, (object) "NeighborHostId"));
      this.CheckTargetCollectionHost(requestContext, hostMove.HostId);
      ValidationHelper.CheckHostStatuses(requestContext, hostMove.HostId, true, false);
      ISqlRegistryService service1 = requestContext.GetService<ISqlRegistryService>();
      string path = string.Format("/Configuration/DataImport/{0}/CompletedPreHostMoveChecks{1}", (object) hostMove.RequestId, (object) DataImportHostMove.Collection);
      ISqlRegistryService registryService1 = service1;
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery registryQuery = (RegistryQuery) path;
      ref RegistryQuery local1 = ref registryQuery;
      if (registryService1.GetValue<bool>(requestContext1, in local1, false))
        return;
      ISqlRegistryService registryService2 = service1;
      IVssRequestContext requestContext2 = requestContext;
      registryQuery = (RegistryQuery) "/Configuration/DataImport/OptOut";
      ref RegistryQuery local2 = ref registryQuery;
      int num = (int) registryService2.GetValue<DataImportOptOut>(requestContext2, in local2, DataImportOptOut.None);
      ITeamFoundationHostManagementService service2 = requestContext.GetService<ITeamFoundationHostManagementService>();
      TeamFoundationServiceHostProperties serviceHostProperties1 = service2.QueryServiceHostProperties(requestContext, hostMove.HostId);
      if (serviceHostProperties1 == null)
        throw new HostDoesNotExistException(hostMove.HostId);
      TeamFoundationServiceHostProperties serviceHostProperties2 = service2.QueryServiceHostProperties(requestContext, serviceHostProperties1.ParentId);
      if (serviceHostProperties2 == null)
        throw new HostDoesNotExistException(serviceHostProperties1.ParentId);
      if (serviceHostProperties2.Status != TeamFoundationServiceHostStatus.Started)
        throw new InvalidOperationException(string.Format("Data Import Host Move should only be started when the Account Host is started host {0} is currently {1}", (object) serviceHostProperties1.ParentId, (object) serviceHostProperties2.Status));
      if (num == 0)
      {
        TeamFoundationDatabaseManagementService service3 = requestContext.GetService<TeamFoundationDatabaseManagementService>();
        ITeamFoundationDatabaseProperties database1 = service3.GetDatabase(requestContext, serviceHostProperties1.DatabaseId, true);
        if (serviceHostProperties2.DatabaseId != -2)
        {
          if (serviceHostProperties1.DatabaseId == serviceHostProperties2.DatabaseId)
            throw new InvalidOperationException(string.Format("Data Import for Services that don't Opt-Out of the Copy Phase should have their Account and Collection Hosts in different Databases prior to Host Move.\r\nAccount {0} and Collection {1} hosts are both in database {2}", (object) serviceHostProperties1.ParentId, (object) hostMove.HostId, (object) serviceHostProperties1.DatabaseId));
          ITeamFoundationDatabaseProperties database2 = service3.GetDatabase(requestContext, serviceHostProperties2.DatabaseId, true);
          if (string.Equals(database1.PoolName, database2.PoolName, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(string.Format("Data Import for Services that don't Opt-Out of the Copy Phase should not have their Account and Collection Hosts in Databases within the same pool.\r\nAccount {0} host database Id {1} and Collection {2} host database Id {3} are both in pool {4}", (object) serviceHostProperties1.ParentId, (object) serviceHostProperties2.DatabaseId, (object) hostMove.HostId, (object) serviceHostProperties2.DatabaseId, (object) database1.PoolName));
        }
        if (!string.Equals(DatabaseManagementConstants.CollectionImportPool, database1.PoolName, StringComparison.OrdinalIgnoreCase))
          throw new InvalidOperationException(string.Format("Data Import for Services that don't Opt-Out of the Copy Phase should have Collection Hosts in Databases within the {0} pool.\r\nCollection {1} host database Id {2} is in pool {3}", (object) DatabaseManagementConstants.CollectionImportPool, (object) hostMove.HostId, (object) serviceHostProperties1.DatabaseId, (object) database1.PoolName));
      }
      requestContext.GetService<IVssRegistryService>().SetValue<bool>(requestContext, path, true);
    }

    public void CheckActivateRequest(
      IVssRequestContext requestContext,
      ActivateDataImportRequest activate)
    {
      ArgumentUtility.CheckForNull<ActivateDataImportRequest>(activate, nameof (activate));
      if (string.IsNullOrEmpty(activate.TargetDatabaseDowngradeSize))
      {
        if (activate.HostToMovePostImport == DataImportHostMove.None)
          throw new InvalidOperationException(string.Format("When moving {0} {1} is required.", (object) DataImportHostMove.None, (object) "TargetDatabaseDowngradeSize"));
      }
      else
      {
        DatabaseServiceObjective result;
        if (!System.Enum.TryParse<DatabaseServiceObjective>(activate.TargetDatabaseDowngradeSize, out result))
          throw new InvalidOperationException("Failed to parse " + activate.TargetDatabaseDowngradeSize + " as a " + result.GetType().Name);
      }
    }

    public void CheckSupportedMilestoneInRequest(
      IVssRequestContext requestContext,
      DatabaseDataImportRequest import,
      string restrictedMilestones)
    {
      if (string.IsNullOrEmpty(import.ConnectionString))
        return;
      ISqlConnectionInfo connectionInfo = SqlConnectionInfoFactory.Create(import.ConnectionString);
      this.ValidateSupportedMilestones(requestContext, connectionInfo, restrictedMilestones);
    }

    public static void CheckHostStatuses(
      IVssRequestContext requestContext,
      Guid hostId,
      bool allowImporting,
      bool allowUpgrading)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      Dictionary<TeamFoundationServiceHostStatus, ServiceHostSubStatus> source = new Dictionary<TeamFoundationServiceHostStatus, ServiceHostSubStatus>();
      if (allowImporting)
        source[TeamFoundationServiceHostStatus.Stopped] = ServiceHostSubStatus.Importing;
      if (allowUpgrading)
        source[TeamFoundationServiceHostStatus.Started] = ServiceHostSubStatus.UpgradeDuringImport;
      if (!source.Any<KeyValuePair<TeamFoundationServiceHostStatus, ServiceHostSubStatus>>())
        throw new ArgumentException("At least one of allowImporting or allowUpgrading needs to be true").AsFatalReparentCollectionException<ArgumentException>();
      TeamFoundationServiceHostProperties serviceHostProperties = requestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(requestContext, hostId);
      if (serviceHostProperties == null)
        throw new HostDoesNotExistException(hostId);
      ServiceHostSubStatus serviceHostSubStatus;
      if (!source.TryGetValue(serviceHostProperties.Status, out serviceHostSubStatus))
        throw new ApplicationException(string.Format("Unexpected Hosts status during import, expecting {0}. {1}", (object) string.Join<TeamFoundationServiceHostStatus>(", ", (IEnumerable<TeamFoundationServiceHostStatus>) source.Keys), (object) serviceHostProperties));
      if (serviceHostSubStatus != serviceHostProperties.SubStatus)
        throw new ApplicationException(string.Format("Unexpected Sub Status during Import.  Expected {0}. {1}", (object) serviceHostSubStatus, (object) serviceHostProperties)).AsFatalReparentCollectionException<ApplicationException>();
    }

    private ServiceHostProperties CheckTargetCollectionHost(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      ServiceHostProperties serviceHostProperties = requestContext.GetService<IHostManagementService>().GetServiceHostProperties(requestContext, hostId);
      if (serviceHostProperties == null)
        throw new HostDoesNotExistException(hostId);
      return serviceHostProperties.HostType == ServiceHostType.Collection ? serviceHostProperties : throw new UnexpectedHostTypeException((TeamFoundationHostType) serviceHostProperties.HostType);
    }

    private void ValidateSupportedMilestones(
      IVssRequestContext requestContext,
      ISqlConnectionInfo connectionInfo,
      string restrictedMilestones)
    {
      string snapshotServiceLevel = this.GetSnapshotServiceLevel(connectionInfo);
      new SupportedMilestoneHelper(requestContext, restrictedMilestones).CheckIsSupported(snapshotServiceLevel);
    }

    internal void ValidateSupportedMilestonesFromProperties(
      IVssRequestContext requestContext,
      DatabaseDataImportRequest request)
    {
      PropertyPair property;
      string str = request.Properties.TryGetProperty("DataImport.TfsVersion", out property) ? property.Value : string.Empty;
      new SupportedMilestoneHelper(requestContext, (string) null).CheckIsSupported(str);
    }

    public virtual void ValidateSourceConnectionString(
      IVssRequestContext requestContext,
      Guid importId,
      string connectionString)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      DataImportConnectionStringHelper connectionStringHelper = new DataImportConnectionStringHelper(importId, connectionString);
      try
      {
        connectionStringHelper.ValidateConnectionString(requestContext);
      }
      catch (SqlException ex)
      {
        throw new InvalidImportSourceConnectionStringException(HostingResources.InvalidDataImportConnectionString((object) OutboundVIPs(), (object) ConnectionStringUtility.MaskPassword(connectionString), (object) ex.Message), (Exception) ex);
      }
      catch (TimeoutException ex)
      {
        throw new ImportSourceConnectionTimeoutException(HostingResources.TimeOutDuringDataImportConnection((object) OutboundVIPs(), (object) ConnectionStringUtility.MaskPassword(connectionString)), (Exception) ex);
      }
      DataImportConnectionStringHelper.CheckStaticProperties(connectionStringHelper.GetDatabaseProperties());

      string OutboundVIPs()
      {
        IEnumerable<IPAddress> outboundViPs = requestContext.GetService<IDeploymentInformationService>().GetOutboundVIPs(requestContext);
        outboundViPs.ForEach<IPAddress>((Action<IPAddress>) (x => requestContext.TraceAlways(15080304, TraceLevel.Info, nameof (ValidationHelper), "DataImport", x.ToString())));
        return string.Join<IPAddress>(Environment.NewLine, outboundViPs);
      }
    }

    internal virtual string GetSnapshotServiceLevel(ISqlConnectionInfo connectionInfo)
    {
      using (ExtendedAttributeComponent componentRaw = connectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
        return componentRaw.ReadServiceLevelStamp();
    }
  }
}
