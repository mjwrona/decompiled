// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.AllowedFunctions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;

namespace Microsoft.AspNet.OData.Query
{
  [Flags]
  public enum AllowedFunctions
  {
    None = 0,
    StartsWith = 1,
    EndsWith = 2,
    Contains = 4,
    Length = 8,
    IndexOf = 16, // 0x00000010
    Concat = 32, // 0x00000020
    Substring = 64, // 0x00000040
    ToLower = 128, // 0x00000080
    ToUpper = 256, // 0x00000100
    Trim = 512, // 0x00000200
    Cast = 1024, // 0x00000400
    Year = 2048, // 0x00000800
    Date = 4096, // 0x00001000
    Month = 8192, // 0x00002000
    Time = 16384, // 0x00004000
    Day = 32768, // 0x00008000
    Hour = 131072, // 0x00020000
    Minute = 524288, // 0x00080000
    Second = 2097152, // 0x00200000
    FractionalSeconds = 4194304, // 0x00400000
    Round = 8388608, // 0x00800000
    Floor = 16777216, // 0x01000000
    Ceiling = 33554432, // 0x02000000
    IsOf = 67108864, // 0x04000000
    Any = 134217728, // 0x08000000
    All = 268435456, // 0x10000000
    AllStringFunctions = Trim | ToUpper | ToLower | Substring | Concat | IndexOf | Length | Contains | EndsWith | StartsWith, // 0x000003FF
    AllDateTimeFunctions = FractionalSeconds | Second | Minute | Hour | Day | Time | Month | Date | Year, // 0x006AF800
    AllMathFunctions = Ceiling | Floor | Round, // 0x03800000
    AllFunctions = AllMathFunctions | AllDateTimeFunctions | AllStringFunctions | All | Any | IsOf | Cast, // 0x1FEAFFFF
  }
}
