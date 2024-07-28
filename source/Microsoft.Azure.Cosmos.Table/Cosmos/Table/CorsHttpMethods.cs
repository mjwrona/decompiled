// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.CorsHttpMethods
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;

namespace Microsoft.Azure.Cosmos.Table
{
  [Flags]
  public enum CorsHttpMethods
  {
    None = 0,
    Get = 1,
    Head = 2,
    Post = 4,
    Put = 8,
    Delete = 16, // 0x00000010
    Trace = 32, // 0x00000020
    Options = 64, // 0x00000040
    Connect = 128, // 0x00000080
    Merge = 256, // 0x00000100
  }
}
