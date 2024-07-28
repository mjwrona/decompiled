// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.OData.OrderByRecognizer
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.Data.Edm;
using Microsoft.Data.OData.Query;
using Microsoft.Data.OData.Query.SemanticAst;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3.OData
{
  public class OrderByRecognizer : IOrderByRecognizer
  {
    private readonly V2FeedPackageModelInfo modelInfo;

    public OrderByRecognizer(V2FeedPackageModelInfo modelInfo) => this.modelInfo = modelInfo;

    public RecognizeResult<object> RecognizeOrderBy(string orderByString)
    {
      try
      {
        return this.RecognizeOrderBy(this.modelInfo.ODataUriParser.ParseOrderBy(orderByString, (IEdmType) this.modelInfo.V2FeedPackageType, (IEdmEntitySet) null));
      }
      catch (Exception ex)
      {
        return new RecognizeResult<object>(ex);
      }
    }

    public RecognizeResult<object> RecognizeOrderBy(OrderByClause orderByClause)
    {
      for (; orderByClause.Expression is SingleValuePropertyAccessNode expression; orderByClause = orderByClause.ThenBy)
      {
        if (expression.Property == this.modelInfo.DownloadCountProperty)
        {
          if (orderByClause.ThenBy == null)
            return new RecognizeResult<object>((object) null);
        }
        else
        {
          if (expression.Property != this.modelInfo.IdProperty)
            return new RecognizeResult<object>((Exception) new UnsupportedODataOrderByException(Resources.Error_ODataOrderByNotSupported()));
          return orderByClause.Direction != OrderByDirection.Ascending ? new RecognizeResult<object>((Exception) new UnsupportedODataOrderByException(Resources.Error_ODataOrderByNotSupported())) : new RecognizeResult<object>((object) null);
        }
      }
      return new RecognizeResult<object>((Exception) new UnsupportedODataOrderByException(Resources.Error_ODataOrderByNotSupported()));
    }
  }
}
