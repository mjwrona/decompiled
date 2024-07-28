// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.WebApi.ApprovalStatus
// Assembly: Microsoft.Azure.Pipelines.Approval.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CD73FA2-1519-4C73-96D3-90013EEE8275
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Approval.WebApi
{
  [Flags]
  [DataContract]
  public enum ApprovalStatus
  {
    [EnumMember] Undefined = 0,
    [EnumMember] Uninitiated = 1,
    [EnumMember] Pending = 2,
    [EnumMember] Approved = 4,
    [EnumMember] Rejected = 8,
    [EnumMember] Skipped = 16, // 0x00000010
    [EnumMember] Canceled = 32, // 0x00000020
    [EnumMember] TimedOut = 64, // 0x00000040
    [EnumMember] Deferred = 128, // 0x00000080
    [EnumMember] Failed = TimedOut | Canceled | Rejected, // 0x00000068
    [EnumMember] Completed = Failed | Skipped | Approved, // 0x0000007C
    [EnumMember] All = Completed | Deferred | Pending | Uninitiated, // 0x000000FF
  }
}
