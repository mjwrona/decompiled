// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ResourceSetContext
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.OData.Edm;
using System.Collections;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Microsoft.AspNet.OData
{
  public class ResourceSetContext
  {
    private HttpRequestMessage _request;
    private UrlHelper _urlHelper;

    public HttpRequestMessage Request
    {
      get => this._request;
      set
      {
        this._request = value;
        this.InternalRequest = this._request != null ? (IWebApiRequestMessage) new WebApiRequestMessage(this._request) : (IWebApiRequestMessage) null;
      }
    }

    public HttpRequestContext RequestContext { get; set; }

    public UrlHelper Url
    {
      get => this._urlHelper;
      set
      {
        this._urlHelper = value;
        this.InternalUrlHelper = this._urlHelper != null ? (IWebApiUrlHelper) new WebApiUrlHelper(this._urlHelper) : (IWebApiUrlHelper) null;
      }
    }

    public IEdmModel EdmModel => this.Request.GetModel();

    internal static ResourceSetContext Create(
      ODataSerializerContext writeContext,
      IEnumerable resourceSetInstance)
    {
      return new ResourceSetContext()
      {
        Request = writeContext.Request,
        EntitySetBase = writeContext.NavigationSource as IEdmEntitySetBase,
        Url = writeContext.Url,
        ResourceSetInstance = (object) resourceSetInstance
      };
    }

    public IEdmEntitySetBase EntitySetBase { get; set; }

    public object ResourceSetInstance { get; set; }

    internal IWebApiRequestMessage InternalRequest { get; private set; }

    internal IWebApiUrlHelper InternalUrlHelper { get; private set; }
  }
}
