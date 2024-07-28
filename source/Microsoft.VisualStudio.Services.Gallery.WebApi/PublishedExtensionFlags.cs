// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.PublishedExtensionFlags
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [Flags]
  public enum PublishedExtensionFlags
  {
    None = 0,
    Disabled = 1,
    BuiltIn = 2,
    Validated = 4,
    Trusted = 8,
    Paid = 16, // 0x00000010
    Public = 256, // 0x00000100
    MultiVersion = 512, // 0x00000200
    System = 1024, // 0x00000400
    Preview = 2048, // 0x00000800
    Unpublished = 4096, // 0x00001000
    Trial = 8192, // 0x00002000
    Locked = 16384, // 0x00004000
    Hidden = 32768, // 0x00008000
  }
}
