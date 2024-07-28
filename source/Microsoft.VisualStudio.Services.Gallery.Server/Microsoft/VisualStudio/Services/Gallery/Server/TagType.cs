// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.TagType
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public enum TagType
  {
    BasicTag = 1,
    Category = 2,
    SharedAccount = 3,
    Identity = 4,
    FullText = 5,
    ContributionType = 6,
    InstallationTarget = 7,
    SharedOrganization = 8,
    Featured = 100, // 0x00000064
    FeaturedInCategory = 101, // 0x00000065
  }
}
