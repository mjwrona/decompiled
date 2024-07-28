// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ViewsValidatingConverter`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class ViewsValidatingConverter<TIdentity, TVersionDetails> : 
    IConverter<PackageRequest<TIdentity, TVersionDetails>, IViewOperationData>,
    IHaveInputType<PackageRequest<TIdentity, TVersionDetails>>,
    IHaveOutputType<IViewOperationData>
    where TIdentity : IPackageIdentity
    where TVersionDetails : class, IPackageVersionDetails
  {
    private readonly IConverter<JsonPatchOperation, string> viewPatchToIdOrNameConverter;
    private readonly IConverter<FeedViewRequest, FeedView> viewIdResolver;

    public ViewsValidatingConverter(
      IConverter<JsonPatchOperation, string> viewPatchToIdOrNameConverter,
      IConverter<FeedViewRequest, FeedView> viewIdResolver)
    {
      this.viewPatchToIdOrNameConverter = viewPatchToIdOrNameConverter;
      this.viewIdResolver = viewIdResolver;
    }

    public IViewOperationData Convert(PackageRequest<TIdentity, TVersionDetails> input)
    {
      TVersionDetails additionalData = input.AdditionalData;
      if ((object) additionalData == null || additionalData.Views?.Value == null)
        return (IViewOperationData) null;
      string viewId = this.viewPatchToIdOrNameConverter.Convert(additionalData.Views);
      FeedView feedView = this.viewIdResolver.Convert(new FeedViewRequest(input.Feed, viewId));
      if (feedView.Type == FeedViewType.Implicit)
        throw new ViewOperationException(Resources.Error_CantPromoteToImplicitView());
      return feedView.Id == new Guid() ? (IViewOperationData) null : (IViewOperationData) new ViewOperationData((IPackageIdentity) input.PackageId, MetadataSuboperation.Add, feedView.Id);
    }
  }
}
