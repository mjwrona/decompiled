// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Extensions.HttpRequestMessageProperties
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.AspNet.OData.Extensions
{
  public class HttpRequestMessageProperties
  {
    private const string DeltaLinkKey = "Microsoft.AspNet.OData.DeltaLink";
    private const string NextLinkKey = "Microsoft.AspNet.OData.NextLink";
    private const string PathKey = "Microsoft.AspNet.OData.Path";
    private const string RouteNameKey = "Microsoft.AspNet.OData.RouteName";
    private const string RoutingConventionsStoreKey = "Microsoft.AspNet.OData.RoutingConventionsStore";
    private const string SelectExpandClauseKey = "Microsoft.AspNet.OData.SelectExpandClause";
    private const string ApplyClauseKey = "Microsoft.AspNet.OData.ApplyClause";
    private const string TotalCountKey = "Microsoft.AspNet.OData.TotalCount";
    private const string TotalCountFuncKey = "Microsoft.AspNet.OData.TotalCountFunc";
    private const string PageSizeKey = "Microsoft.AspNet.OData.PageSize";
    private const string QueryOptionsKey = "Microsoft.AspNet.OData.QueryOptions";
    private HttpRequestMessage _request;

    internal HttpRequestMessageProperties(HttpRequestMessage request) => this._request = request;

    internal Func<long> TotalCountFunc
    {
      get
      {
        object obj;
        return this._request.Properties.TryGetValue("Microsoft.AspNet.OData.TotalCountFunc", out obj) ? (Func<long>) obj : (Func<long>) null;
      }
      set => this._request.Properties["Microsoft.AspNet.OData.TotalCountFunc"] = (object) value;
    }

    internal int PageSize
    {
      get
      {
        object obj;
        return this._request.Properties.TryGetValue("Microsoft.AspNet.OData.PageSize", out obj) ? (int) obj : -1;
      }
      set => this._request.Properties["Microsoft.AspNet.OData.PageSize"] = (object) value;
    }

    internal ODataQueryOptions QueryOptions
    {
      get => this.GetValueOrNull<ODataQueryOptions>("Microsoft.AspNet.OData.QueryOptions");
      set => this._request.Properties["Microsoft.AspNet.OData.QueryOptions"] = value != null ? (object) value : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (value));
    }

    public string RouteName
    {
      get => this.GetValueOrNull<string>("Microsoft.AspNet.OData.RouteName");
      set => this._request.Properties["Microsoft.AspNet.OData.RouteName"] = (object) value;
    }

    public Microsoft.AspNet.OData.Routing.ODataPath Path
    {
      get => this.GetValueOrNull<Microsoft.AspNet.OData.Routing.ODataPath>("Microsoft.AspNet.OData.Path");
      set => this._request.Properties["Microsoft.AspNet.OData.Path"] = (object) value;
    }

    public long? TotalCount
    {
      get
      {
        object totalCount;
        if (this._request.Properties.TryGetValue("Microsoft.AspNet.OData.TotalCount", out totalCount))
          return (long?) totalCount;
        if (this.TotalCountFunc == null)
          return new long?();
        long num = this.TotalCountFunc();
        this._request.Properties["Microsoft.AspNet.OData.TotalCount"] = (object) num;
        return new long?(num);
      }
      set => this._request.Properties["Microsoft.AspNet.OData.TotalCount"] = (object) value;
    }

    public Uri NextLink
    {
      get => this.GetValueOrNull<Uri>("Microsoft.AspNet.OData.NextLink");
      set => this._request.Properties["Microsoft.AspNet.OData.NextLink"] = (object) value;
    }

    public Uri DeltaLink
    {
      get => this.GetValueOrNull<Uri>("Microsoft.AspNet.OData.DeltaLink");
      set => this._request.Properties["Microsoft.AspNet.OData.DeltaLink"] = (object) value;
    }

    public SelectExpandClause SelectExpandClause
    {
      get => this.GetValueOrNull<SelectExpandClause>("Microsoft.AspNet.OData.SelectExpandClause");
      set => this._request.Properties["Microsoft.AspNet.OData.SelectExpandClause"] = value != null ? (object) value : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (value));
    }

    public ApplyClause ApplyClause
    {
      get => this.GetValueOrNull<ApplyClause>("Microsoft.AspNet.OData.ApplyClause");
      set => this._request.Properties["Microsoft.AspNet.OData.ApplyClause"] = value != null ? (object) value : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (value));
    }

    public IDictionary<string, object> RoutingConventionsStore
    {
      get
      {
        IDictionary<string, object> conventionsStore = this.GetValueOrNull<IDictionary<string, object>>("Microsoft.AspNet.OData.RoutingConventionsStore");
        if (conventionsStore == null)
        {
          conventionsStore = (IDictionary<string, object>) new Dictionary<string, object>();
          this.RoutingConventionsStore = conventionsStore;
        }
        return conventionsStore;
      }
      private set => this._request.Properties["Microsoft.AspNet.OData.RoutingConventionsStore"] = (object) value;
    }

    internal ODataVersion? ODataServiceVersion => HttpRequestMessageProperties.GetODataVersionFromHeader((HttpHeaders) this._request.Headers, "OData-Version");

    internal ODataVersion? ODataMaxServiceVersion => HttpRequestMessageProperties.GetODataVersionFromHeader((HttpHeaders) this._request.Headers, "OData-MaxVersion");

    internal ODataVersion? ODataMinServiceVersion => HttpRequestMessageProperties.GetODataVersionFromHeader((HttpHeaders) this._request.Headers, "OData-MinVersion");

    private static ODataVersion? GetODataVersionFromHeader(HttpHeaders headers, string headerName)
    {
      IEnumerable<string> values;
      if (headers.TryGetValues(headerName, out values))
      {
        string str = values.FirstOrDefault<string>();
        if (str != null)
        {
          string version = str.Trim(' ', ';');
          try
          {
            return new ODataVersion?(ODataUtils.StringToODataVersion(version));
          }
          catch (ODataException ex)
          {
          }
        }
      }
      return new ODataVersion?();
    }

    private T GetValueOrNull<T>(string propertyName) where T : class
    {
      object obj;
      return this._request.Properties.TryGetValue(propertyName, out obj) ? (T) obj : default (T);
    }
  }
}
