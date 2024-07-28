// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.FeedCapabilities
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [Flags]
  public enum FeedCapabilities
  {
    None = 0,
    UpstreamV2 = 1,
    UnderMaintenance = -2147483648, // 0x80000000
    DefaultCapabilities = UpstreamV2, // 0x00000001
  }
}
