// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.History.EventType
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Orchestration.History
{
  [DataContract]
  public enum EventType
  {
    [EnumMember] ExecutionStarted,
    [EnumMember] ExecutionCompleted,
    [EnumMember] ExecutionFailed,
    [EnumMember] ExecutionTerminated,
    [EnumMember] TaskScheduled,
    [EnumMember] TaskCompleted,
    [EnumMember] TaskFailed,
    [EnumMember] SubOrchestrationInstanceCreated,
    [EnumMember] SubOrchestrationInstanceCompleted,
    [EnumMember] SubOrchestrationInstanceFailed,
    [EnumMember] TimerCreated,
    [EnumMember] TimerFired,
    [EnumMember] OrchestratorStarted,
    [EnumMember] OrchestratorCompleted,
    [EnumMember] EventRaised,
    [EnumMember] ContinueAsNew,
    [EnumMember] GenericEvent,
  }
}
