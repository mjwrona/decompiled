// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.AzureTableStorageHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal static class AzureTableStorageHelper
  {
    private const int c_maxEntitiesPerBatch = 100;

    internal static void ExecuteBatchInsertOrReplaceOperation(
      IVssRequestContext requestContext,
      IAzureTableStorageProvider tableProvider,
      string tableName,
      IEnumerable<ITableEntity> entities)
    {
      AzureTableStorageHelper.ExecuteBatchOperation(requestContext, tableName, entities, new Func<IVssRequestContext, string, IEnumerable<ITableEntity>, IList<TableResult>>(((AzureTableStorageProviderExtensions) tableProvider).Upsert));
    }

    internal static void ExecuteBatchDeleteOperation(
      IVssRequestContext requestContext,
      IAzureTableStorageProvider tableProvider,
      string tableName,
      IEnumerable<ITableEntity> entities)
    {
      AzureTableStorageHelper.ExecuteBatchOperation(requestContext, tableName, entities, new Func<IVssRequestContext, string, IEnumerable<ITableEntity>, IList<TableResult>>(((AzureTableStorageProviderExtensions) tableProvider).Delete));
    }

    internal static void ExecuteBatchOperation(
      IVssRequestContext requestContext,
      string tableName,
      IEnumerable<ITableEntity> entities,
      Func<IVssRequestContext, string, IEnumerable<ITableEntity>, IList<TableResult>> operation,
      int maxEntitiesPerBatch = 100)
    {
      foreach (IList<ITableEntity> tableEntityList in entities.Batch<ITableEntity>(maxEntitiesPerBatch))
      {
        IList<TableResult> tableResultList = operation(requestContext, tableName, (IEnumerable<ITableEntity>) tableEntityList);
      }
    }
  }
}
