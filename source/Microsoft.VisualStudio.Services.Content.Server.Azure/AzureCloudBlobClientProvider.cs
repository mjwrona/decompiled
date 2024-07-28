// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.AzureCloudBlobClientProvider
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.RetryPolicies;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class AzureCloudBlobClientProvider : IAzureCloudBlobClientProvider, IVssFrameworkService
  {
    private Dictionary<string, CloudBlobClientAdapter> clientsByStrongBoxKey = new Dictionary<string, CloudBlobClientAdapter>();

    public AzureCloudBlobClientProvider()
    {
    }

    internal AzureCloudBlobClientProvider(IVssRequestContext systemRequestContext) => this.ServiceStart(systemRequestContext);

    public void ServiceStart(IVssRequestContext deploymentRequestContext)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      IVssRequestContext elevatedDeploymentRequestContext = deploymentRequestContext.Elevate();
      this.CreateBlobClients(elevatedDeploymentRequestContext);
      this.RegisterStrongBoxChanges(elevatedDeploymentRequestContext);
    }

    public void ServiceEnd(IVssRequestContext deploymentRequestContext) => this.UnregisterStrongBoxChanges(deploymentRequestContext.Elevate());

    private void CreateBlobClients(
      IVssRequestContext elevatedDeploymentRequestContext)
    {
      LocationMode? blobLocationMode = StorageAccountConfigurationFacade.GetBlobLocationMode(elevatedDeploymentRequestContext);
      foreach (StrongBoxConnectionString strongBoxConnectionString in StorageAccountUtilities.ReadAllStorageAccountsForDomainId(elevatedDeploymentRequestContext))
      {
        CloudBlobClientAdapter blobClient = this.CreateBlobClient(blobLocationMode, strongBoxConnectionString);
        this.clientsByStrongBoxKey.Add(strongBoxConnectionString.StrongBoxItemKey, blobClient);
      }
    }

    private CloudBlobClientAdapter CreateBlobClient(
      LocationMode? locationMode,
      StrongBoxConnectionString strongBoxConnectionString)
    {
      CloudStorageAccount account = CloudStorageAccount.Parse(strongBoxConnectionString.ConnectionString);
      ServicePoint servicePoint1 = ServicePointManager.FindServicePoint(account.BlobStorageUri.PrimaryUri);
      ServicePoint servicePoint2 = ServicePointManager.FindServicePoint(account.BlobStorageUri.SecondaryUri);
      servicePoint1.ConnectionLimit = servicePoint2.ConnectionLimit = Environment.ProcessorCount * ServicePointConstants.MaxConnectionsPerProc32;
      servicePoint1.UseNagleAlgorithm = servicePoint2.UseNagleAlgorithm = false;
      CloudBlobClient cloudBlobClient = account.CreateCloudBlobClient();
      cloudBlobClient.DefaultRequestOptions.LocationMode = locationMode;
      cloudBlobClient.DefaultRequestOptions.RetryPolicy = (IRetryPolicy) new ExponentialRetry(TimeSpan.FromMilliseconds(100.0), 10);
      cloudBlobClient.DefaultRequestOptions.MaximumExecutionTime = new TimeSpan?(TimeSpan.FromSeconds(3600.0));
      return new CloudBlobClientAdapter(cloudBlobClient);
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

    private void OnLocationModeSecretChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      foreach (StrongBoxItemName strongBoxItemName in itemNames.Where<StrongBoxItemName>((Func<StrongBoxItemName, bool>) (name => name.LookupKey.Equals("LocationMode"))))
      {
        StrongBoxItemInfo itemInfo = service.GetItemInfo(vssRequestContext, strongBoxItemName.DrawerId, strongBoxItemName.LookupKey);
        LocationMode? locationMode = StorageAccountUtilities.ParseLocationMode(service.GetString(vssRequestContext, itemInfo));
        foreach (CloudBlobClientAdapter blobClientAdapter in this.clientsByStrongBoxKey.Values)
          blobClientAdapter.LocationMode = locationMode;
      }
    }

    private void OnConnectionStringSecretChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      foreach (StrongBoxItemName itemName in itemNames)
      {
        string lookupKey = itemName.LookupKey;
        if (this.clientsByStrongBoxKey.ContainsKey(lookupKey))
        {
          StrongBoxItemInfo itemInfo = service.GetItemInfo(vssRequestContext, itemName.DrawerId, lookupKey);
          string connectionString = service.GetString(vssRequestContext, itemInfo);
          this.clientsByStrongBoxKey[lookupKey].UpdateConnectionString(connectionString);
        }
      }
    }

    public ICloudBlobClient GetBlobClient(string strongBoxKey) => (ICloudBlobClient) this.clientsByStrongBoxKey[strongBoxKey];

    public ICloudBlobClient GetBlobClient(string strongBoxKey, string containerNamePrefix) => (ICloudBlobClient) new CloudBlobClientAdapter(this.clientsByStrongBoxKey[strongBoxKey], containerNamePrefix);
  }
}
