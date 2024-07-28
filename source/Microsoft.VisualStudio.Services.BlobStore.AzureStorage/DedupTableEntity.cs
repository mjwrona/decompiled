// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.DedupTableEntity
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  internal abstract class DedupTableEntity : TableEntity
  {
    protected DedupTableEntity()
    {
    }

    protected DedupTableEntity(DedupIdentifier dedupId, string rowKey, string etagToMatch)
    {
      this.DedupId = dedupId;
      this.PartitionKey = this.DedupId.ValueString;
      this.RowKey = rowKey;
      this.ETag = etagToMatch;
    }

    [IgnoreProperty]
    public DedupIdentifier DedupId { get; private set; }

    public override void ReadEntity(
      IDictionary<string, EntityProperty> properties,
      OperationContext operationContext)
    {
      base.ReadEntity(properties, operationContext);
      this.DedupId = DedupIdentifier.Create(this.PartitionKey);
    }
  }
}
