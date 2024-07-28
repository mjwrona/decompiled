// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldStatusFlags
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [Flags]
  public enum FieldStatusFlags
  {
    None = 0,
    Required = 1,
    ReadOnly = 2,
    HasValues = 4,
    LimitedToValues = 8,
    HasFormats = 16, // 0x00000010
    LimitedToFormats = 32, // 0x00000020
    AllowsOldValue = 64, // 0x00000040
    SetByRule = 128, // 0x00000080
    SetByDefaultRule = 256, // 0x00000100
    SetByComputedRule = 512, // 0x00000200
    PendingListCheck = 2048, // 0x00000800
    InvalidType = 4096, // 0x00001000
    InvalidDate = 8192, // 0x00002000
    InvalidID = 16384, // 0x00004000
    InvalidPath = 32768, // 0x00008000
    InvalidTooLong = 65536, // 0x00010000
    InvalidSpecialChars = 131072, // 0x00020000
    InvalidRule = 262144, // 0x00040000
    InvalidEmpty = 524288, // 0x00080000
    InvalidNotEmpty = 1048576, // 0x00100000
    InvalidFormat = 2097152, // 0x00200000
    InvalidListValue = 4194304, // 0x00400000
    InvalidOldValue = 8388608, // 0x00800000
    InvalidNotOldValue = 16777216, // 0x01000000
    InvalidEmptyOrOldValue = 33554432, // 0x02000000
    InvalidNotEmptyOrOldValue = 67108864, // 0x04000000
    InvalidValueInOtherField = 134217728, // 0x08000000
    InvalidValueNotInOtherField = 268435456, // 0x10000000
    InvalidComputedField = 536870912, // 0x20000000
    InvalidIdentityField = 1073741824, // 0x40000000
    InvalidMask = InvalidIdentityField | InvalidComputedField | InvalidValueNotInOtherField | InvalidValueInOtherField | InvalidNotEmptyOrOldValue | InvalidEmptyOrOldValue | InvalidNotOldValue | InvalidOldValue | InvalidListValue | InvalidFormat | InvalidNotEmpty | InvalidEmpty | InvalidRule | InvalidSpecialChars | InvalidTooLong | InvalidPath | InvalidID | InvalidDate | InvalidType, // 0x7FFFF000
  }
}
