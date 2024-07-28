// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.FeedChangesServiceExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades
{
  public static class FeedChangesServiceExtensions
  {
    public static 
    #nullable disable
    IAsyncEnumerable<FeedChange> EnumerateFeedChangesAsyncEnumerable(
      this IFeedChangesService service,
      long initialContinuationToken = 0,
      int batchSize = 1000,
      bool includeDeleted = false)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IAsyncEnumerable<FeedChange>) new FeedChangesServiceExtensions.\u003CEnumerateFeedChangesAsyncEnumerable\u003Ed__0(-2)
      {
        \u003C\u003E3__service = service,
        \u003C\u003E3__initialContinuationToken = initialContinuationToken,
        \u003C\u003E3__batchSize = batchSize,
        \u003C\u003E3__includeDeleted = includeDeleted
      };
    }
  }
}
