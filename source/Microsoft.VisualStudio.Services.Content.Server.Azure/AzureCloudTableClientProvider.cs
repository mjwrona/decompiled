// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.AzureCloudTableClientProvider
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class AzureCloudTableClientProvider : IAzureCloudTableClientProvider, IVssFrameworkService
  {
    private Dictionary<string, ITableClient> clientsByStrongBoxKey = new Dictionary<string, ITableClient>();

    public AzureCloudTableClientProvider()
    {
    }

    internal AzureCloudTableClientProvider(IVssRequestContext systemRequestContext) => this.ServiceStart(systemRequestContext);

    public void ServiceStart(IVssRequestContext deploymentRequestContext)
    {
      deploymentRequestContext.CheckDeploymentRequestContext();
      IVssRequestContext elevatedDeploymentRequestContext = deploymentRequestContext.Elevate();
      this.CreateTableClients(elevatedDeploymentRequestContext);
      this.RegisterStrongBoxChanges(elevatedDeploymentRequestContext);
    }

    public void ServiceEnd(IVssRequestContext deploymentRequestContext) => this.UnregisterStrongBoxChanges(deploymentRequestContext.Elevate());

    private void CreateTableClients(
      IVssRequestContext elevatedDeploymentRequestContext)
    {
      LocationMode? tableLocationMode = StorageAccountConfigurationFacade.GetTableLocationMode(elevatedDeploymentRequestContext);
      foreach (StrongBoxConnectionString strongBoxConnectionString in StorageAccountUtilities.ReadAllStorageAccountsForDomainId(elevatedDeploymentRequestContext))
      {
        ITableClient tableClient = this.CreateTableClient(tableLocationMode, strongBoxConnectionString);
        this.clientsByStrongBoxKey.Add(strongBoxConnectionString.StrongBoxItemKey, tableClient);
      }
    }

    private ITableClient CreateTableClient(
      LocationMode? locationMode,
      StrongBoxConnectionString strongBoxConnectionString)
    {
      return (ITableClient) new AzureCloudTableClientAdapter(strongBoxConnectionString.ConnectionString, locationMode, (IRetryPolicy) null);
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
        LocationMode? tableLocationMode = StorageAccountUtilities.ParseTableLocationMode(service.GetString(vssRequestContext, itemInfo));
        foreach (ITableClient tableClient in this.clientsByStrongBoxKey.Values)
          tableClient.LocationMode = tableLocationMode;
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
          string newConnectionString = service.GetString(vssRequestContext, itemInfo);
          this.clientsByStrongBoxKey[lookupKey].UpdateConnectionString(newConnectionString);
        }
      }
    }

    public ITableClient GetTableClient(string strongBoxKey) => this.clientsByStrongBoxKey[strongBoxKey];

    public ITableClient GetTableClient(StrongBoxConnectionString connectionString) => this.GetTableClient(connectionString.StrongBoxItemKey);
  }
}
