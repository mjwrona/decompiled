// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion.StoredNupkgItemInfo
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion
{
  public class StoredNupkgItemInfo
  {
    public IStorageId PackageStorageId { get; set; }

    public VssNuGetPackageIdentity Identity { get; set; }

    public XDocument Nuspec { get; set; }

    public long PackageSize { get; set; }

    public NuGetPackageMetadata Metadata { get; set; }

    public byte[] NuspecBytes { get; set; }

    public IEnumerable<UpstreamSourceInfo> SourceChain { get; set; }

    public ProvenanceInfo Provenance { get; set; }

    public bool AddAsDelisted { get; set; }

    public override string ToString() => this.Identity.ToString();
  }
}
