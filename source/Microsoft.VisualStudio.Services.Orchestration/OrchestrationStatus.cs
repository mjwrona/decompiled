// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.OrchestrationStatus
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  [DataContract]
  public enum OrchestrationStatus
  {
    [EnumMember] Running,
    [EnumMember] Completed,
    [EnumMember] ContinuedAsNew,
    [EnumMember] Failed,
    [EnumMember] Canceled,
    [EnumMember] Terminated,
  }
}
