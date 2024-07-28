// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenMetadataAggregationAccessor
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;


#nullable enable
namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenMetadataAggregationAccessor : 
    PackageMetadataAggregationAccessor<
    #nullable disable
    MavenPackageIdentity, IMavenMetadataEntry>,
    IMavenMetadataAggregationAccessor,
    IAggregationAccessor,
    IMavenMetadataDocumentService,
    IMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>,
    IMetadataService<
    #nullable enable
    MavenPackageIdentity, IMavenMetadataEntry>,
    IReadMetadataService<MavenPackageIdentity, IMavenMetadataEntry>,
    IReadSingleVersionMetadataService<MavenPackageIdentity, IMavenMetadataEntry>,
    IReadMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>,
    IReadMetadataDocumentService
  {
    public MavenMetadataAggregationAccessor(

      #nullable disable
      IAggregation aggregation,
      IConverter<IAggregation, Locator> aggVersionToLocatorConverter,
      IFactory<ContainerAddress, IMetadataDocumentService<MavenPackageIdentity, IMavenMetadataEntry>> metadataServiceFactory,
      IExecutionEnvironment executionEnvironment)
      : base(aggregation, aggVersionToLocatorConverter, metadataServiceFactory, executionEnvironment)
    {
    }
  }
}
