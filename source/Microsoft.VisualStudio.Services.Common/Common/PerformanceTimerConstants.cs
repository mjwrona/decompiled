// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.PerformanceTimerConstants
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class PerformanceTimerConstants
  {
    public const string Header = "X-VSS-PerfData";
    public const string PerfTimingKey = "PerformanceTimings";
    [Obsolete]
    public const string Aad = "AAD";
    public const string AadToken = "AadToken";
    public const string AadGraph = "AadGraph";
    public const string BlobStorage = "BlobStorage";
    public const string FinalSqlCommand = "FinalSQLCommand";
    public const string Redis = "Redis";
    public const string ServiceBus = "ServiceBus";
    public const string Sql = "SQL";
    public const string SqlReadOnly = "SQLReadOnly";
    public const string SqlRetries = "SQLRetries";
    public const string TableStorage = "TableStorage";
    public const string VssClient = "VssClient";
    public const string DocumentDB = "DocumentDB";
  }
}
