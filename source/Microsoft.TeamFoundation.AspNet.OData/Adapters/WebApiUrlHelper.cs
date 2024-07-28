// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Adapters.WebApiUrlHelper
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Routing;
using Microsoft.OData.UriParser;
using System.Collections.Generic;
using System.Web.Http.Routing;

namespace Microsoft.AspNet.OData.Adapters
{
  internal class WebApiUrlHelper : IWebApiUrlHelper
  {
    private UrlHelper innerHelper;

    public WebApiUrlHelper(UrlHelper helper) => this.innerHelper = helper != null ? helper : throw Error.ArgumentNull(nameof (helper));

    public string CreateODataLink(params ODataPathSegment[] segments) => this.innerHelper.CreateODataLink(segments);

    public string CreateODataLink(IList<ODataPathSegment> segments) => this.innerHelper.CreateODataLink(segments);

    public string CreateODataLink(
      string routeName,
      IODataPathHandler pathHandler,
      IList<ODataPathSegment> segments)
    {
      return this.innerHelper.CreateODataLink(routeName, pathHandler, segments);
    }
  }
}
