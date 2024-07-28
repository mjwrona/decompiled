// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 141BFF87-1CDC-4DC5-9A7C-F7D6EA031F34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models
{
  [DataContract]
  public enum RuleActionType
  {
    MakeRequired = 1,
    MakeReadOnly = 2,
    SetDefaultValue = 3,
    SetDefaultFromClock = 4,
    SetDefaultFromCurrentUser = 5,
    SetDefaultFromField = 6,
    CopyValue = 7,
    CopyFromClock = 8,
    CopyFromCurrentUser = 9,
    CopyFromField = 10, // 0x0000000A
    SetValueToEmpty = 11, // 0x0000000B
    CopyFromServerClock = 12, // 0x0000000C
    CopyFromServerCurrentUser = 13, // 0x0000000D
    HideTargetField = 14, // 0x0000000E
    DisallowValue = 15, // 0x0000000F
  }
}
