// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry.BatchOperationCiData
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry
{
  public class BatchOperationCiData : PackagingCiData
  {
    public BatchOperationCiData(
      IVssRequestContext requestContext,
      IProtocol protocol,
      FeedCore feed,
      string batchOperationType,
      string serializedBatchData,
      int batchSize)
      : base(requestContext, "BatchOperation", protocol, feed)
    {
      this.CiData.Add("BatchOperationData", serializedBatchData);
      this.CiData.Add("BatchOperationType", batchOperationType);
      this.CiData.Add("BatchSize", (double) batchSize);
    }
  }
}
