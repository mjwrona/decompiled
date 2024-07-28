// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider.BlobServiceForAggregationFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider
{
  public class BlobServiceForAggregationFactory : IFactory<IBlobService>
  {
    private readonly IFactory<ContainerAddress, IBlobService> blobServiceFromContainerAddressFactory;
    private readonly IAggregation aggregation;
    private readonly IConverter<IAggregation, Locator> aggVersionToLocatorConverter;
    private readonly IExecutionEnvironment executionEnvironment;

    public BlobServiceForAggregationFactory(
      IFactory<ContainerAddress, IBlobService> blobServiceFromContainerAddressFactory,
      IAggregation aggregation,
      IConverter<IAggregation, Locator> aggVersionToLocatorConverter,
      IExecutionEnvironment executionEnvironment)
    {
      this.blobServiceFromContainerAddressFactory = blobServiceFromContainerAddressFactory;
      this.aggregation = aggregation;
      this.aggVersionToLocatorConverter = aggVersionToLocatorConverter;
      this.executionEnvironment = executionEnvironment;
    }

    public IBlobService Get() => this.blobServiceFromContainerAddressFactory.Get(new ContainerAddress((CollectionId) this.executionEnvironment.HostId, this.aggVersionToLocatorConverter.Convert(this.aggregation)));
  }
}
