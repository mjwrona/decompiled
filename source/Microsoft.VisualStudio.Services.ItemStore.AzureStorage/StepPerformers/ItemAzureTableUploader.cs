// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.AzureStorage.StepPerformers.ItemAzureTableUploader
// Assembly: Microsoft.VisualStudio.Services.ItemStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DF52255-B389-4C6F-82CF-18DDB4745F9C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.AzureStorage.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Azure.Orchestration;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ItemStore.AzureStorage.StepPerformers
{
  internal class ItemAzureTableUploader : AzureTableUploader
  {
    internal ItemAzureTableUploader(ImportSettings settings)
      : base(settings)
    {
    }

    protected override ITableClientFactory CreateTableFactory(
      IVssRequestContext deploymentContext,
      IEnumerable<StrongBoxConnectionString> connectionStrings,
      string tableName,
      string shardingStrategy,
      LocationMode? locationMode)
    {
      Func<StrongBoxConnectionString, ITableClient> getTableClient = !deploymentContext.IsFeatureEnabled("Blobstore.Features.DeploymentLifetimeAzureStorageClients") ? (Func<StrongBoxConnectionString, ITableClient>) (connectionString => (ITableClient) new AzureCloudTableClientAdapter(connectionString.ConnectionString, locationMode, (IRetryPolicy) null)) : new Func<StrongBoxConnectionString, ITableClient>(deploymentContext.GetService<IAzureCloudTableClientProvider>().GetTableClient);
      return (ITableClientFactory) new ItemShardingAzureCloudTableClientFactory(connectionStrings, getTableClient, locationMode, tableName, shardingStrategy, false);
    }
  }
}
