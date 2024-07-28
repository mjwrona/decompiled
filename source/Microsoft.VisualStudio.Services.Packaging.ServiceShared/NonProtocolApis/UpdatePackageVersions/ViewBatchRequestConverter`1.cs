// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions.ViewBatchRequestConverter`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions
{
  public class ViewBatchRequestConverter<TPackageId> : 
    IConverter<PackagesBatchRequest<TPackageId>, PackagesBatchRequest<TPackageId, FeedView>>,
    IHaveInputType<PackagesBatchRequest<TPackageId>>,
    IHaveOutputType<PackagesBatchRequest<TPackageId, FeedView>>
    where TPackageId : IPackageIdentity
  {
    private readonly IConverter<FeedViewRequest, FeedView> viewResolver;

    public ViewBatchRequestConverter(IConverter<FeedViewRequest, FeedView> viewResolver) => this.viewResolver = viewResolver;

    public PackagesBatchRequest<TPackageId, FeedView> Convert(PackagesBatchRequest<TPackageId> input)
    {
      if (string.IsNullOrWhiteSpace(input.BatchOperationData is BatchPromoteData batchOperationData ? batchOperationData.ViewId : (string) null))
        throw new ViewOperationException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_ArgumentRequired((object) "ViewId"));
      FeedView data = this.viewResolver.Convert(new FeedViewRequest(input.Feed, batchOperationData.ViewId));
      if (data.Type == FeedViewType.Implicit)
        throw new ViewOperationException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CantPromoteToImplicitView());
      return data.Id == new Guid() ? (PackagesBatchRequest<TPackageId, FeedView>) null : new PackagesBatchRequest<TPackageId, FeedView>(input, data);
    }
  }
}
