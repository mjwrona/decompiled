// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.PerRequestContentNegotiator
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace Microsoft.AspNet.OData
{
  internal class PerRequestContentNegotiator : IContentNegotiator
  {
    private IContentNegotiator _innerContentNegotiator;

    public PerRequestContentNegotiator(IContentNegotiator innerContentNegotiator) => this._innerContentNegotiator = innerContentNegotiator != null ? innerContentNegotiator : throw Error.ArgumentNull(nameof (innerContentNegotiator));

    public ContentNegotiationResult Negotiate(
      Type type,
      HttpRequestMessage request,
      IEnumerable<MediaTypeFormatter> formatters)
    {
      MediaTypeHeaderValue contentType = request.Content == null ? (MediaTypeHeaderValue) null : request.Content.Headers.ContentType;
      List<MediaTypeFormatter> formatters1 = new List<MediaTypeFormatter>();
      foreach (MediaTypeFormatter formatter in formatters)
      {
        if (formatter != null)
          formatters1.Add(formatter.GetPerRequestFormatterInstance(type, request, contentType));
      }
      return this._innerContentNegotiator.Negotiate(type, request, (IEnumerable<MediaTypeFormatter>) formatters1);
    }
  }
}
