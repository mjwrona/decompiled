// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.ITraceDatumVisitor
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing.TraceData;

namespace Microsoft.Azure.Cosmos.Tracing
{
  internal interface ITraceDatumVisitor
  {
    void Visit(QueryMetricsTraceDatum queryMetricsTraceDatum);

    void Visit(
      PointOperationStatisticsTraceDatum pointOperationStatisticsTraceDatum);

    void Visit(
      ClientSideRequestStatisticsTraceDatum clientSideRequestStatisticsTraceDatum);

    void Visit(CpuHistoryTraceDatum cpuHistoryTraceDatum);

    void Visit(
      ClientConfigurationTraceDatum clientConfigurationTraceDatum);

    void Visit(
      PartitionKeyRangeCacheTraceDatum partitionKeyRangeCacheTraceDatum);
  }
}
