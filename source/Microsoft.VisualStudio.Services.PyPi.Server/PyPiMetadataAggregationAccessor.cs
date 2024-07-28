// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PyPiMetadataAggregationAccessor
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server
{
  public class PyPiMetadataAggregationAccessor : 
    PackageMetadataAggregationAccessor<
    #nullable disable
    PyPiPackageIdentity, IPyPiMetadataEntry>,
    IPyPiMetadataAggregationAccessor,
    IAggregationAccessor,
    IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>,
    IMetadataService<
    #nullable enable
    PyPiPackageIdentity, IPyPiMetadataEntry>,
    IReadMetadataService<PyPiPackageIdentity, IPyPiMetadataEntry>,
    IReadSingleVersionMetadataService<PyPiPackageIdentity, IPyPiMetadataEntry>,
    IReadMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>,
    IReadMetadataDocumentService
  {
    public PyPiMetadataAggregationAccessor(

      #nullable disable
      IAggregation aggregation,
      IConverter<IAggregation, Locator> aggVersionToLocatorConverter,
      IFactory<ContainerAddress, IMetadataDocumentService<PyPiPackageIdentity, IPyPiMetadataEntry>> metadataServiceFactory,
      IExecutionEnvironment executionEnvironment)
      : base(aggregation, aggVersionToLocatorConverter, metadataServiceFactory, executionEnvironment)
    {
    }
  }
}
