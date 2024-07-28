// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Table.AzureImpl.AzureTableLoader
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Table.AzureImpl
{
  public class AzureTableLoader : TableLoader
  {
    public ITable LoadSource(IVssRequestContext deploymentContext, StorageMigration storage) => (ITable) new AzureCloudTableAdapter(new CloudTable(new Uri(storage.Uri + storage.SasToken)));

    public ITable LoadTarget(
      IVssRequestContext deploymentContext,
      MigrateStorageInfo info,
      string tableName)
    {
      return (ITable) new AzureCloudTableAdapter(CloudStorageAccount.Parse(AzureTableLoader.GetStorageConnectionStringFromStrongbox(deploymentContext, info.Drawer, info.LookupKey)).CreateCloudTableClient().GetTableReference(tableName));
    }

    private static string GetStorageConnectionStringFromStrongbox(
      IVssRequestContext deploymentContext,
      string drawerName,
      string lookupKey)
    {
      TeamFoundationStrongBoxService service = deploymentContext.GetService<TeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(deploymentContext, drawerName, true);
      return service.GetString(deploymentContext, drawerId, lookupKey);
    }
  }
}
