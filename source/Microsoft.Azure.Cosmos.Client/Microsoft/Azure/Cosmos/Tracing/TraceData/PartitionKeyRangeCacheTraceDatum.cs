// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.TraceData.PartitionKeyRangeCacheTraceDatum
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Microsoft.Azure.Cosmos.Tracing.TraceData
{
  internal class PartitionKeyRangeCacheTraceDatum : TraceDatum
  {
    public string PreviousContinuationToken { get; }

    public string ContinuationToken { get; }

    public PartitionKeyRangeCacheTraceDatum(
      string previousContinuationToken,
      string continuationToken)
    {
      this.PreviousContinuationToken = previousContinuationToken;
      this.ContinuationToken = continuationToken;
    }

    internal override void Accept(ITraceDatumVisitor traceDatumVisitor) => traceDatumVisitor.Visit(this);
  }
}
