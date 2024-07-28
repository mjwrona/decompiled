// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.QueryEngineTimes
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents
{
  public sealed class QueryEngineTimes
  {
    private readonly TimeSpan indexLookupTime;
    private readonly TimeSpan documentLoadTime;
    private readonly TimeSpan vmExecutionTime;
    private readonly TimeSpan writeOutputTime;
    private readonly RuntimeExecutionTimes runtimeExecutionTimes;

    internal QueryEngineTimes(
      TimeSpan indexLookupTime,
      TimeSpan documentLoadTime,
      TimeSpan vmExecutionTime,
      TimeSpan writeOutputTime,
      RuntimeExecutionTimes runtimeExecutionTimes)
    {
      this.indexLookupTime = indexLookupTime;
      this.documentLoadTime = documentLoadTime;
      this.vmExecutionTime = vmExecutionTime;
      this.writeOutputTime = writeOutputTime;
      this.runtimeExecutionTimes = runtimeExecutionTimes;
    }

    public TimeSpan IndexLookupTime => this.indexLookupTime;

    public TimeSpan DocumentLoadTime => this.documentLoadTime;

    public TimeSpan WriteOutputTime => this.writeOutputTime;

    public RuntimeExecutionTimes RuntimeExecutionTimes => this.runtimeExecutionTimes;

    internal TimeSpan VMExecutionTime => this.vmExecutionTime;
  }
}
