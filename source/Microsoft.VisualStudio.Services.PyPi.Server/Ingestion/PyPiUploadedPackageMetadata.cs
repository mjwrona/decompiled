// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.PyPiUploadedPackageMetadata
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion
{
  public class PyPiUploadedPackageMetadata
  {
    public IReadOnlyDictionary<string, string[]> MetadataFields { get; set; }

    public IPyPiResolvedMetadata Metadata { get; set; }

    public PackageFileStream PackageFileStream { get; set; }

    public string ComputedMd5 { get; set; }

    public string ComputedSha256 { get; set; }

    public DeflateCompressibleBytes GpgSignature { get; set; }
  }
}
