// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.AnalyticsStagingTracePoints
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  public enum AnalyticsStagingTracePoints
  {
    JobStarted = 14000001, // 0x00D59F81
    JobFinished = 14000002, // 0x00D59F82
    StreamsFound = 14000003, // 0x00D59F83
    ProcessingStream = 14000004, // 0x00D59F84
    StreamProcessed = 14000005, // 0x00D59F85
    RecordsRetrieved = 14000006, // 0x00D59F86
    RecordsStaged = 14000007, // 0x00D59F87
    RecordPrepared = 14000008, // 0x00D59F88
    StreamDisabled = 14000009, // 0x00D59F89
    Retry = 14000010, // 0x00D59F8A
    RetryExhausted = 14000011, // 0x00D59F8B
    ProcessingShard = 14000012, // 0x00D59F8C
    CreateShard = 14000013, // 0x00D59F8D
    DeleteShard = 14000014, // 0x00D59F8E
    ReceivedServiceBusMessage = 14000015, // 0x00D59F8F
  }
}
