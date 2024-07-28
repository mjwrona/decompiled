// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.IPyPiVersionMetadataAggregationAccessor
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server
{
  public interface IPyPiVersionMetadataAggregationAccessor : 
    IAggregationAccessor,
    IMetadataDocumentService<
    #nullable disable
    PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>,
    IMetadataService<
    #nullable enable
    PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>,
    IReadMetadataService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>,
    IReadSingleVersionMetadataService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>,
    IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntryWithRawMetadata>,
    IReadMetadataDocumentService
  {
  }
}
