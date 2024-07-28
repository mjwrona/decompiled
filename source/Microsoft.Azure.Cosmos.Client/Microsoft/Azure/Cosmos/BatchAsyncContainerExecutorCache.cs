// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.BatchAsyncContainerExecutorCache
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  internal class BatchAsyncContainerExecutorCache : IDisposable
  {
    internal const int DefaultMaxBulkRequestBodySizeInBytes = 220201;
    private readonly ConcurrentDictionary<string, BatchAsyncContainerExecutor> executorsPerContainer = new ConcurrentDictionary<string, BatchAsyncContainerExecutor>();

    public BatchAsyncContainerExecutor GetExecutorForContainer(
      ContainerInternal container,
      CosmosClientContext cosmosClientContext)
    {
      if (!cosmosClientContext.ClientOptions.AllowBulkExecution)
        throw new InvalidOperationException("AllowBulkExecution is not currently enabled.");
      string key = container.LinkUri.ToString();
      BatchAsyncContainerExecutor executorForContainer;
      if (this.executorsPerContainer.TryGetValue(key, out executorForContainer))
        return executorForContainer;
      BatchAsyncContainerExecutor containerExecutor = new BatchAsyncContainerExecutor(container, cosmosClientContext, 100, 220201);
      if (!this.executorsPerContainer.TryAdd(key, containerExecutor))
        containerExecutor.Dispose();
      return this.executorsPerContainer[key];
    }

    public void Dispose()
    {
      foreach (KeyValuePair<string, BatchAsyncContainerExecutor> keyValuePair in this.executorsPerContainer)
        keyValuePair.Value.Dispose();
    }
  }
}
