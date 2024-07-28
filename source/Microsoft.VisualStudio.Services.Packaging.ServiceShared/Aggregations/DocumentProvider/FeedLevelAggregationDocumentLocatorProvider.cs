// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider.FeedLevelAggregationDocumentLocatorProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider
{
  public class FeedLevelAggregationDocumentLocatorProvider : 
    IAggregationDocumentLocatorProvider<NoSpecifier>
  {
    private readonly string documentName;

    public FeedLevelAggregationDocumentLocatorProvider(string documentName) => this.documentName = documentName;

    public Locator GetLocator(IFeedRequest feedRequest, NoSpecifier specifier) => new Locator(new string[2]
    {
      feedRequest.Feed.Id.ToString(),
      this.documentName
    });
  }
}
