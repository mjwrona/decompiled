// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.ODataCountMediaTypeMapping
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData.UriParser;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace Microsoft.AspNet.OData.Formatter
{
  public class ODataCountMediaTypeMapping : MediaTypeMapping
  {
    public override double TryMatchMediaType(HttpRequestMessage request)
    {
      if (request == null)
        throw Error.ArgumentNull(nameof (request));
      return ODataCountMediaTypeMapping.IsCountRequest(request.ODataProperties().Path) ? 1.0 : 0.0;
    }

    public ODataCountMediaTypeMapping()
      : base("text/plain")
    {
    }

    internal static bool IsCountRequest(Microsoft.AspNet.OData.Routing.ODataPath path) => path != null && path.Segments.LastOrDefault<ODataPathSegment>() is CountSegment;
  }
}
