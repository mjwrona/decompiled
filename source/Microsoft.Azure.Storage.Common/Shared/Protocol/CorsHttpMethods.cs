// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.CorsHttpMethods
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;

namespace Microsoft.Azure.Storage.Shared.Protocol
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
    Patch = 512, // 0x00000200
  }
}
