// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Data.Model.EnvironmentJobStatus
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Data.Model
{
  [Flags]
  [DataContract]
  public enum EnvironmentJobStatus
  {
    [EnumMember] None = 0,
    [EnumMember] Succeeded = 1,
    [EnumMember] SucceededWithIssues = 2,
    [EnumMember] Abandoned = 4,
    [EnumMember] Canceled = 8,
    [EnumMember] Failed = 16, // 0x00000010
    [EnumMember] Skipped = 32, // 0x00000020
    [EnumMember] InProgress = 64, // 0x00000040
    [EnumMember] All = InProgress | Skipped | Failed | Canceled | Abandoned | SucceededWithIssues | Succeeded, // 0x0000007F
  }
}
