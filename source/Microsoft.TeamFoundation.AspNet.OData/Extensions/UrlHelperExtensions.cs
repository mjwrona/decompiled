// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Extensions.UrlHelperExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Routing;
using Microsoft.OData.UriParser;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Microsoft.AspNet.OData.Extensions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class UrlHelperExtensions
  {
    public static string CreateODataLink(
      this UrlHelper urlHelper,
      params ODataPathSegment[] segments)
    {
      return urlHelper.CreateODataLink((IList<ODataPathSegment>) segments);
    }

    public static string CreateODataLink(this UrlHelper urlHelper, IList<ODataPathSegment> segments)
    {
      HttpRequestMessage request = urlHelper != null ? urlHelper.Request : throw Error.ArgumentNull(nameof (urlHelper));
      string routeName = request.ODataProperties().RouteName;
      if (string.IsNullOrEmpty(routeName))
        throw Error.InvalidOperation(SRResources.RequestMustHaveODataRouteName);
      IODataPathHandler pathHandler = request.GetPathHandler();
      return urlHelper.CreateODataLink(routeName, pathHandler, segments);
    }

    public static string CreateODataLink(
      this UrlHelper urlHelper,
      string routeName,
      IODataPathHandler pathHandler,
      IList<ODataPathSegment> segments)
    {
      if (urlHelper == null)
        throw Error.ArgumentNull(nameof (urlHelper));
      string str = pathHandler != null ? pathHandler.Link(new Microsoft.AspNet.OData.Routing.ODataPath((IEnumerable<ODataPathSegment>) segments)) : throw Error.ArgumentNull(nameof (pathHandler));
      UrlHelper urlHelper1 = urlHelper;
      string routeName1 = routeName;
      HttpRouteValueDictionary routeValues = new HttpRouteValueDictionary();
      routeValues.Add(ODataRouteConstants.ODataPath, (object) str);
      return urlHelper1.Link(routeName1, (IDictionary<string, object>) routeValues);
    }
  }
}
