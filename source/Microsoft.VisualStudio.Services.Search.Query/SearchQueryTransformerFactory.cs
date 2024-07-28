// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SearchQueryTransformerFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  internal static class SearchQueryTransformerFactory
  {
    internal static ISearchQueryTransformer GetTransformerInstance(
      IVssRequestContext requestContext,
      IEntityType entityType)
    {
      switch (entityType.Name)
      {
        case "Code":
          DocumentContractType documentContractType = new IndexMapper((IEntityType) CodeEntityType.GetInstance()).GetDocumentContractType(requestContext);
          switch (documentContractType)
          {
            case DocumentContractType.SourceNoDedupeFileContractV5:
            case DocumentContractType.DedupeFileContractV5:
              return (ISearchQueryTransformer) new CodeSearchQueryTransformerV4(requestContext);
            default:
              if (documentContractType.IsNoPayloadContract())
                return (ISearchQueryTransformer) new CodeSearchQueryTransformerV3(requestContext);
              return requestContext.IsFTSEnabled() ? (ISearchQueryTransformer) new CodeSearchQueryTransformerV2(requestContext) : (ISearchQueryTransformer) new CodeSearchQueryTransformer();
          }
        case "Wiki":
          return (ISearchQueryTransformer) new WikiSearchQueryTransformer();
        case "WorkItem":
          return (ISearchQueryTransformer) new WorkItemSearchQueryTransformer();
        default:
          throw new SearchException(FormattableString.Invariant(FormattableStringFactory.Create("Couldn't create instance for unsupported type:{0}", (object) entityType.Name)));
      }
    }
  }
}
