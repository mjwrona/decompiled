// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2.UpstreamPackagesToRefreshInformation
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2
{
  [DataContract]
  public class UpstreamPackagesToRefreshInformation
  {
    public UpstreamPackagesToRefreshInformation()
    {
    }

    public UpstreamPackagesToRefreshInformation(
      Guid feed,
      IPackageName firstPackage,
      IPackageName lastPackage)
    {
      this.FeedId = feed;
      this.FirstPackageDisplayName = firstPackage?.DisplayName;
      this.LastPackageDisplayName = lastPackage?.DisplayName;
    }

    [DataMember]
    public Guid FeedId { get; set; }

    [DataMember]
    public string FirstPackageDisplayName { get; set; }

    [DataMember]
    public string LastPackageDisplayName { get; set; }
  }
}
