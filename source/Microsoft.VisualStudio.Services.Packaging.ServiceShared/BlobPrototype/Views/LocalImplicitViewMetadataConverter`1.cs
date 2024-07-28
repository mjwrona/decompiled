// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Views.LocalImplicitViewMetadataConverter`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Views
{
  public class LocalImplicitViewMetadataConverter<TMetadataEntry> : 
    IConverter<FeedRequest<TMetadataEntry>, TMetadataEntry>,
    IHaveInputType<FeedRequest<TMetadataEntry>>,
    IHaveOutputType<TMetadataEntry>
    where TMetadataEntry : IMetadataEntryWritable
  {
    private const string LocalViewName = "Local";

    public TMetadataEntry Convert(FeedRequest<TMetadataEntry> feedRequest)
    {
      TMetadataEntry additionalData = feedRequest.AdditionalData;
      FeedView view = feedRequest.Feed.View;
      FeedView feedView = view;
      if ((feedView != null ? (feedView.Type == FeedViewType.Implicit ? 1 : 0) : 0) != 0 && view.Name == "Local" && additionalData.IsLocal)
      {
        IEnumerable<Guid> views = additionalData.Views;
        List<Guid> guidList = (views != null ? views.ToList<Guid>() : (List<Guid>) null) ?? new List<Guid>();
        if (guidList.FirstOrDefault<Guid>((Func<Guid, bool>) (v => v == view.Id)) == Guid.Empty)
        {
          ref TMetadataEntry local = ref additionalData;
          if ((object) default (TMetadataEntry) == null)
          {
            TMetadataEntry metadataEntry = local;
            local = ref metadataEntry;
          }
          local.Views = (IEnumerable<Guid>) new List<Guid>((IEnumerable<Guid>) guidList)
          {
            view.Id
          };
        }
      }
      return additionalData;
    }
  }
}
