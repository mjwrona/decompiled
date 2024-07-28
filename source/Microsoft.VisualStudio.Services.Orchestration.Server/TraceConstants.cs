// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.TraceConstants
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal static class TraceConstants
  {
    public const string Area = "Orchestration";
    public const string Dispatcher = "Dispatcher";
    public const string Service = "Service";
    public const int StartingOrchestrationActivityExecution = 15010000;
    public const int CompletedOrchestrationActivityExecution = 15010001;
    public const int ProcessingActivityMessages = 15010002;
    public const int MaximumActivityCountMet = 15010003;
    public const int ProcessingActivityForSession = 15010004;
    public const int ProcessingActivityMessagesResultedInOrchestrationMessages = 15010005;
    public const int QueueingOrchestrationDispatcher = 15010006;
    public const int QueueingActivityDispatcher = 15010007;
    public const int StartingActivityDispatcher = 15010008;
    public const int FinishedActivityDispatcher = 15010009;
    public const int ScheduledDispatcherJobs = 15010010;
    public const int DispatcherJobTimeout = 15010011;
    public const int DispatcherJobRequeueError = 15010012;
    public const int ActivityDispatcherException = 15010013;
    public const int ActivityDispatcherShutdown = 15010014;
    public const int ActivityDispatcherShutdownTimeout = 15010015;
    public const int ActivityDispatcherCanceled = 15010016;
    public const int ActivityDispatcherNotDefined = 15010017;
    public const int OrchestrationMessageDeliveryLag = 15010018;
    public const int ADThreadInfo = 15010019;
    public const int OrchestrationMultiThreading = 15010020;
    public const int OrchestrationMultiThreadingDispatcherStalling = 15010021;
    public const int OrchestrationMultiThreadingComplketedTask = 15010022;
    public const int OrchestrationMultiThreadingDipatchersInfo = 15010023;
    public const int OrchestrationDispatcherMessagesPerSession = 15010024;
    public const int ActivityInvokerUnhandledException = 15010025;
    public const int OrchestrationMultiThreadingThreadsInfo = 15010026;
    public const int OrchestrationDispatcherFlowTrace = 15010027;
    public const int OrchestrationDispatcherThreadTrace = 15010028;
    public const int OrchestrationDispatcherAwaitingDispatchersIdle = 15010029;
    public const int SessionSizeCloseToALimit = 15010030;
    public const int OrchestrationDIspatcherSessionGroupsInfo = 15010031;
    public const int OrchestrationDispatcherNoMessageContent = 15010032;
    public const int OrchestrationDispatcherMessagesStats = 15010033;
    public const int OrchestrationDispatcherTerminated = 15010034;
  }
}
