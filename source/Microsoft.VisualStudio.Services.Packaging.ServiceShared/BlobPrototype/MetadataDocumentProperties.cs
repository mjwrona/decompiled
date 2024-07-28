// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MetadataDocumentProperties
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class MetadataDocumentProperties : 
    IMetadataDocumentPropertiesWriteable,
    IMetadataDocumentProperties
  {
    public MetadataDocumentProperties(
      IPackageName packageName = null,
      string upstreamsConfigurationHash = null,
      DateTime? upstreamsLastRefreshedUtc = null,
      List<List<UpstreamSourceInfo>> sourceChainMap = null,
      object nameMetadata = null)
    {
      this.PackageName = packageName;
      this.UpstreamsConfigurationHash = upstreamsConfigurationHash;
      this.UpstreamsLastRefreshedUtc = upstreamsLastRefreshedUtc;
      this.SourceChainMap = sourceChainMap;
      this.NameMetadata = nameMetadata;
    }

    public IPackageName PackageName { get; }

    public string UpstreamsConfigurationHash { get; set; }

    public DateTime? UpstreamsLastRefreshedUtc { get; set; }

    public List<List<UpstreamSourceInfo>> SourceChainMap { get; set; }

    public object NameMetadata { get; set; }
  }
}
