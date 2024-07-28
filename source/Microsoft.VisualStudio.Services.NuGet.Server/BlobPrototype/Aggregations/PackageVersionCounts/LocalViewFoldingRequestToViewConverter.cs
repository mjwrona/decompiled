// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.LocalViewFoldingRequestToViewConverter
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public class LocalViewFoldingRequestToViewConverter : 
    IConverter<IFeedRequest, Guid>,
    IHaveInputType<IFeedRequest>,
    IHaveOutputType<Guid>
  {
    public Guid Convert(IFeedRequest request)
    {
      FeedView view = request.Feed.View;
      return view == null || (view.Type != FeedViewType.Implicit ? 0 : (view.Name == "Local" ? 1 : 0)) != 0 ? Guid.Empty : view.Id;
    }
  }
}
