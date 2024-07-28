// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.PublisherFlags
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [Flags]
  public enum PublisherFlags
  {
    UnChanged = 1073741824, // 0x40000000
    None = 0,
    Disabled = 1,
    Verified = 2,
    Certified = 4,
    ServiceFlags = Certified | Verified | Disabled, // 0x00000007
  }
}
