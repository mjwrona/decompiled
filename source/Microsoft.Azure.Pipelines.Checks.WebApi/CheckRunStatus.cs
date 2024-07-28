// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.CheckRunStatus
// Assembly: Microsoft.Azure.Pipelines.Checks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 381241F9-9196-42AF-BB4C-5187E3EFE32E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Checks.WebApi
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Flags]
  public enum CheckRunStatus
  {
    [EnumMember] None = 0,
    [EnumMember] Queued = 1,
    [EnumMember] Running = 2,
    [EnumMember] Approved = 4,
    [EnumMember] Rejected = 8,
    [EnumMember] Canceled = 16, // 0x00000010
    [EnumMember] TimedOut = 32, // 0x00000020
    [EnumMember] Rerunning = 64, // 0x00000040
    [EnumMember] Bypassed = 128, // 0x00000080
    [EnumMember] Deferred = 256, // 0x00000100
    [EnumMember] Failed = TimedOut | Canceled | Rejected, // 0x00000038
    [EnumMember] Completed = Failed | Bypassed | Approved, // 0x000000BC
    [EnumMember] All = Completed | Deferred | Rerunning | Running | Queued, // 0x000001FF
  }
}
