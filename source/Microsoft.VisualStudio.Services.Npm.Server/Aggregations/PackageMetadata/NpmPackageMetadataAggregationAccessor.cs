// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata.NpmPackageMetadataAggregationAccessor
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.Aggregations.PackageMetadata
{
  public class NpmPackageMetadataAggregationAccessor : 
    PackageMetadataAggregationAccessor<
    #nullable disable
    NpmPackageIdentity, INpmMetadataEntry>,
    INpmPackageMetadataAggregationAccessor,
    IAggregationAccessor<NpmPackageMetadataAggregation>,
    IAggregationAccessor,
    INpmMetadataService,
    IMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>,
    IMetadataService<
    #nullable enable
    NpmPackageIdentity, INpmMetadataEntry>,
    IReadMetadataService<NpmPackageIdentity, INpmMetadataEntry>,
    IReadSingleVersionMetadataService<NpmPackageIdentity, INpmMetadataEntry>,
    IReadMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>,
    IReadMetadataDocumentService
  {
    public NpmPackageMetadataAggregationAccessor(

      #nullable disable
      IAggregation aggregation,
      IConverter<IAggregation, Locator> aggVersionToLocatorConverter,
      IFactory<ContainerAddress, IMetadataDocumentService<NpmPackageIdentity, INpmMetadataEntry>> metadataServiceFactory,
      IExecutionEnvironment executionEnvironment)
      : base(aggregation, aggVersionToLocatorConverter, metadataServiceFactory, executionEnvironment)
    {
    }
  }
}
