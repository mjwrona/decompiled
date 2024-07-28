// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.ODataRawValueMediaTypeMapping
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
  public abstract class ODataRawValueMediaTypeMapping : MediaTypeMapping
  {
    public override double TryMatchMediaType(HttpRequestMessage request)
    {
      Microsoft.AspNet.OData.Routing.ODataPath odataPath = request != null ? request.ODataProperties().Path : throw Error.ArgumentNull(nameof (request));
      return !ODataRawValueMediaTypeMapping.IsRawValueRequest(odataPath) || !this.IsMatch(ODataRawValueMediaTypeMapping.GetProperty(odataPath)) ? 0.0 : 1.0;
    }

    protected ODataRawValueMediaTypeMapping(string mediaType)
      : base(mediaType)
    {
    }

    protected abstract bool IsMatch(PropertySegment propertySegment);

    internal static bool IsRawValueRequest(Microsoft.AspNet.OData.Routing.ODataPath path) => path != null && path.Segments.LastOrDefault<ODataPathSegment>() is ValueSegment;

    private static PropertySegment GetProperty(Microsoft.AspNet.OData.Routing.ODataPath odataPath) => odataPath == null || odataPath.Segments.Count < 2 ? (PropertySegment) null : odataPath.Segments[odataPath.Segments.Count - 2] as PropertySegment;
  }
}
