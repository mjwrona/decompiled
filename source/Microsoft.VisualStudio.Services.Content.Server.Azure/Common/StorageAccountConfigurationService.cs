// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Common.StorageAccountConfigurationService
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Common
{
  public class StorageAccountConfigurationService : 
    IStorageAccountConfigurationService,
    IVssFrameworkService
  {
    private static readonly TraceData TraceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "ServiceShared"
    };
    private StrongBoxConnectionString[] defaultDomainConnectionStrings;
    private StrongBoxConnectionString[] nonDefaultConnectionStrings;
    private string locationMode;

    public StorageAccountConfigurationService()
    {
    }

    public Microsoft.Azure.Storage.RetryPolicies.LocationMode? BlobLocationMode => StorageAccountUtilities.ParseLocationMode(this.locationMode);

    public Microsoft.Azure.Cosmos.Table.LocationMode? TableLocationMode => StorageAccountUtilities.ParseTableLocationMode(this.locationMode);

    internal StorageAccountConfigurationService(IVssRequestContext deploymentRequestContext) => this.ServiceStart(deploymentRequestContext);

    public void ServiceStart(IVssRequestContext deploymentRequestContext)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      IVssRequestContext elevatedDeploymentRequestContext = deploymentRequestContext.Elevate();
      this.ReadConnectionStrings(deploymentRequestContext);
      this.ReadLocationMode(deploymentRequestContext);
      this.RegisterStrongBoxChanges(elevatedDeploymentRequestContext);
    }

    public void ServiceEnd(IVssRequestContext deploymentRequestContext) => this.UnregisterStrongBoxChanges(deploymentRequestContext.Elevate());

    public IEnumerable<StrongBoxConnectionString> GetStorageAccounts(
      IVssRequestContext requestContext,
      PhysicalDomainInfo physicalDomainInfo = null)
    {
      if (physicalDomainInfo == null)
        return (IEnumerable<StrongBoxConnectionString>) this.defaultDomainConnectionStrings;
      List<StrongBoxConnectionString> list = this.GetStorageAccountsByDomain(physicalDomainInfo).ToList<StrongBoxConnectionString>();
      int count1 = list.Count;
      int count2 = physicalDomainInfo.Shards.Count;
      if (!requestContext.ServiceHost.IsProduction)
        return (IEnumerable<StrongBoxConnectionString>) list;
      if (count1 == count2)
        return (IEnumerable<StrongBoxConnectionString>) list;
      throw new InvalidDomainShardListException(string.Format("{0} shards were expected for DomainId {1}, but only {2} were retrieved from StrongBox.", (object) count2, (object) physicalDomainInfo.DomainId.Serialize(), (object) count1));
    }

    private IEnumerable<StrongBoxConnectionString> GetStorageAccountsByDomain(
      PhysicalDomainInfo physicalDomainInfo)
    {
      foreach (StrongBoxConnectionString connectionString in physicalDomainInfo.DomainId.Equals(WellKnownDomainIds.DefaultDomainId) ? (IEnumerable<StrongBoxConnectionString>) this.defaultDomainConnectionStrings : (IEnumerable<StrongBoxConnectionString>) this.nonDefaultConnectionStrings)
      {
        if (physicalDomainInfo.Shards.Contains(StorageAccountUtilities.GetAccountInfo(connectionString.ConnectionString).Name))
          yield return connectionString;
      }
    }

    private void RegisterStrongBoxChanges(
      IVssRequestContext elevatedDeploymentRequestContext)
    {
      ITeamFoundationStrongBoxService service = elevatedDeploymentRequestContext.GetService<ITeamFoundationStrongBoxService>();
      service.RegisterNotification(elevatedDeploymentRequestContext, new StrongBoxItemChangedCallback(this.OnLocationModeSecretChanged), "StorageAccountInfo", (IEnumerable<string>) new string[1]
      {
        "LocationMode"
      });
      service.RegisterNotification(elevatedDeploymentRequestContext, new StrongBoxItemChangedCallback(this.OnConnectionStringSecretChanged), StorageAccountUtilities.StrongBoxConnectionStringDrawer, (IEnumerable<string>) new string[1]
      {
        StorageAccountUtilities.GetStorageAccountKeyBaseName(elevatedDeploymentRequestContext) + "*"
      });
    }

    private void UnregisterStrongBoxChanges(
      IVssRequestContext elevatedDeploymentRequestContext)
    {
      ITeamFoundationStrongBoxService service = elevatedDeploymentRequestContext.GetService<ITeamFoundationStrongBoxService>();
      service.UnregisterNotification(elevatedDeploymentRequestContext, new StrongBoxItemChangedCallback(this.OnLocationModeSecretChanged));
      service.UnregisterNotification(elevatedDeploymentRequestContext, new StrongBoxItemChangedCallback(this.OnConnectionStringSecretChanged));
    }

    private void ReadConnectionStrings(IVssRequestContext deploymentContext)
    {
      using (deploymentContext.Enter(ContentTracePoints.StorageAccountConfigurationService.ReadStorageAccountsCall, nameof (ReadConnectionStrings)))
      {
        IEnumerable<StrongBoxConnectionString> connectionStrings = StorageAccountUtilities.ReadAllStorageAccounts(deploymentContext);
        IEnumerable<StrongBoxConnectionString> source = StorageAccountUtilities.ReadAllStorageAccountsForDomainId(deploymentContext).Except<StrongBoxConnectionString>(connectionStrings, (IEqualityComparer<StrongBoxConnectionString>) new StrongBoxConnectionStringKeyComparer());
        this.defaultDomainConnectionStrings = connectionStrings.ToArray<StrongBoxConnectionString>();
        this.nonDefaultConnectionStrings = source.ToArray<StrongBoxConnectionString>();
        deploymentContext.TraceAlways(ContentTracePoints.StorageAccountConfigurationService.ReadStorageAccountsInfo, string.Format("Loaded {0} default domain accounts. Loaded {1} domain-specific accounts.", (object) this.defaultDomainConnectionStrings.Length, (object) this.nonDefaultConnectionStrings.Length));
      }
    }

    private void ReadLocationMode(IVssRequestContext deploymentContext)
    {
      using (deploymentContext.Enter(ContentTracePoints.StorageAccountConfigurationService.ReadLocationModeCall, nameof (ReadLocationMode)))
      {
        string locationMode = StorageAccountUtilities.GetLocationMode(deploymentContext);
        Interlocked.Exchange<string>(ref this.locationMode, locationMode);
        deploymentContext.TraceAlways(ContentTracePoints.StorageAccountConfigurationService.ReadLocationModeInfo, "Loaded location mode of " + locationMode);
      }
    }

    private void OnLocationModeSecretChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      this.ReadLocationMode(requestContext.To(TeamFoundationHostType.Deployment).Elevate());
    }

    private void OnConnectionStringSecretChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      using (requestContext.Enter(ContentTracePoints.StorageAccountConfigurationService.OnConnectionStringSecretChangedCall, nameof (OnConnectionStringSecretChanged)))
      {
        foreach (StrongBoxItemName itemName in itemNames)
        {
          StrongBoxItemInfo itemInfo = service.GetItemInfo(vssRequestContext, itemName.DrawerId, itemName.LookupKey);
          StrongBoxConnectionString updatedValue = new StrongBoxConnectionString(service.GetString(vssRequestContext, itemInfo), itemName.LookupKey);
          bool andUpdate = this.FindAndUpdate(this.defaultDomainConnectionStrings, updatedValue);
          if (!andUpdate)
            andUpdate = this.FindAndUpdate(this.nonDefaultConnectionStrings, updatedValue);
          if (andUpdate)
            requestContext.TraceAlways(ContentTracePoints.StorageAccountConfigurationService.ConnectionStringUpdated, "Updated connection string for " + updatedValue.StrongBoxItemKey + ".");
          else
            requestContext.TraceError(ContentTracePoints.StorageAccountConfigurationService.ConnectionStringMissing, "Unexpected error: Received an update for " + updatedValue.StrongBoxItemKey + ", but this item was not found.");
        }
      }
    }

    private bool FindAndUpdate(
      StrongBoxConnectionString[] connectionStringsToUpdate,
      StrongBoxConnectionString updatedValue)
    {
      for (int index = 0; index < connectionStringsToUpdate.Length; ++index)
      {
        if (connectionStringsToUpdate[index].StrongBoxItemKey == updatedValue.StrongBoxItemKey)
        {
          Interlocked.Exchange<StrongBoxConnectionString>(ref connectionStringsToUpdate[index], updatedValue);
          return true;
        }
      }
      return false;
    }
  }
}
