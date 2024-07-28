// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.FeedPermissionConstants
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [Flags]
  public enum FeedPermissionConstants
  {
    None = 0,
    AdministerFeed = 1,
    ArchiveFeed = 2,
    DeleteFeed = 4,
    CreateFeed = 8,
    EditFeed = 16, // 0x00000010
    ReadPackages = 32, // 0x00000020
    AddPackage = 64, // 0x00000040
    UpdatePackage = 128, // 0x00000080
    DeletePackage = 256, // 0x00000100
    DelistPackage = 1024, // 0x00000400
    AddUpstreamPackage = 2048, // 0x00000800
  }
}
