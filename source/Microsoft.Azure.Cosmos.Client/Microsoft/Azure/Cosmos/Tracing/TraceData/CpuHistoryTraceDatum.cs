// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.TraceData.CpuHistoryTraceDatum
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents.Rntbd;
using System;

namespace Microsoft.Azure.Cosmos.Tracing.TraceData
{
  internal sealed class CpuHistoryTraceDatum : TraceDatum
  {
    public CpuHistoryTraceDatum(SystemUsageHistory cpuLoadHistory) => this.Value = cpuLoadHistory ?? throw new ArgumentNullException(nameof (cpuLoadHistory));

    public SystemUsageHistory Value { get; }

    internal override void Accept(ITraceDatumVisitor traceDatumVisitor) => traceDatumVisitor.Visit(this);
  }
}
