// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.CloudFileNtfsAttributes
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using System;

namespace Microsoft.Azure.Storage.File
{
  [Flags]
  public enum CloudFileNtfsAttributes
  {
    None = 0,
    ReadOnly = 1,
    Hidden = 2,
    System = 4,
    Normal = 128, // 0x00000080
    Directory = 16, // 0x00000010
    Archive = 32, // 0x00000020
    Temporary = 256, // 0x00000100
    Offline = 4096, // 0x00001000
    NotContentIndexed = 8192, // 0x00002000
    NoScrubData = 131072, // 0x00020000
  }
}
