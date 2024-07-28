// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.BuildParameterType
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

namespace Microsoft.TeamFoundation.Build.Common
{
  public enum BuildParameterType
  {
    None = 0,
    Object = 1,
    Array = 2,
    Integer = 6,
    Float = 7,
    String = 8,
    Boolean = 9,
    Null = 10, // 0x0000000A
    Date = 12, // 0x0000000C
    Guid = 15, // 0x0000000F
    Uri = 16, // 0x00000010
    TimeSpan = 17, // 0x00000011
  }
}
