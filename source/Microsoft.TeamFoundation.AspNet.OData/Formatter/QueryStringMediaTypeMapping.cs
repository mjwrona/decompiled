// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.QueryStringMediaTypeMapping
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace Microsoft.AspNet.OData.Formatter
{
  public class QueryStringMediaTypeMapping : MediaTypeMapping
  {
    public QueryStringMediaTypeMapping(
      string queryStringParameterName,
      MediaTypeHeaderValue mediaType)
      : base(mediaType)
    {
      this.QueryStringParameterName = queryStringParameterName != null ? queryStringParameterName : throw Error.ArgumentNull(nameof (queryStringParameterName));
    }

    public override double TryMatchMediaType(HttpRequestMessage request)
    {
      if (request == null)
        throw Error.ArgumentNull(nameof (request));
      return this.DoesQueryStringMatch((IEnumerable<KeyValuePair<string, string>>) QueryStringMediaTypeMapping.GetQueryString(request.RequestUri)) ? 1.0 : 0.0;
    }

    private static FormDataCollection GetQueryString(Uri uri) => !(uri == (Uri) null) ? new FormDataCollection(uri) : throw Error.InvalidOperation(SRResources.NonNullUriRequiredForMediaTypeMapping, (object) typeof (QueryStringMediaTypeMapping).Name);

    public QueryStringMediaTypeMapping(string queryStringParameterName, string mediaType)
      : base(mediaType)
    {
      this.QueryStringParameterName = queryStringParameterName != null ? queryStringParameterName : throw Error.ArgumentNull(nameof (queryStringParameterName));
    }

    public string QueryStringParameterName { get; private set; }

    private bool DoesQueryStringMatch(
      IEnumerable<KeyValuePair<string, string>> queryString)
    {
      if (queryString != null)
      {
        string input = queryString.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (kvp => kvp.Key == this.QueryStringParameterName)).FirstOrDefault<KeyValuePair<string, string>>().Value;
        MediaTypeHeaderValue parsedValue;
        if (input != null && MediaTypeHeaderValue.TryParse(input, out parsedValue) && this.MediaType.Equals((object) parsedValue))
          return true;
      }
      return false;
    }
  }
}
