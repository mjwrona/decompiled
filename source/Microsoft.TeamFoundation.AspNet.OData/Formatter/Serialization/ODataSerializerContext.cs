// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Serialization.ODataSerializerContext
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
  public class ODataSerializerContext
  {
    private HttpRequestMessage _request;
    private UrlHelper _urlHelper;
    private ClrTypeCache _typeMappingCache;
    private IDictionary<object, object> _items;
    private ODataQueryContext _queryContext;
    private SelectExpandClause _selectExpandClause;
    private bool _isSelectExpandClauseSet;

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

    private void CopyPlatformSpecificProperties(ODataSerializerContext context)
    {
      this.Request = context.Request;
      this.Url = context.Url;
    }

    public ODataSerializerContext()
    {
    }

    public ODataSerializerContext(
      ResourceContext resource,
      SelectExpandClause selectExpandClause,
      IEdmProperty edmProperty)
      : this(resource, edmProperty, (ODataQueryContext) null, (SelectItem) null)
    {
      this.SelectExpandClause = selectExpandClause;
    }

    internal ODataSerializerContext(
      ResourceContext resource,
      IEdmProperty edmProperty,
      ODataQueryContext queryContext,
      SelectItem currentSelectItem)
    {
      ODataSerializerContext context = resource != null ? resource.SerializerContext : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resource));
      this.CopyPlatformSpecificProperties(context);
      this.Model = context.Model;
      this.Path = context.Path;
      this.RootElementName = context.RootElementName;
      this.SkipExpensiveAvailabilityChecks = context.SkipExpensiveAvailabilityChecks;
      this.MetadataLevel = context.MetadataLevel;
      this.Items = context.Items;
      this.ExpandReference = context.ExpandReference;
      this.QueryContext = queryContext;
      this.ExpandedResource = resource;
      this.CurrentSelectItem = currentSelectItem;
      if (currentSelectItem is ExpandedNavigationSelectItem navigationSelectItem)
      {
        this.SelectExpandClause = navigationSelectItem.SelectAndExpand;
        this.NavigationSource = navigationSelectItem.NavigationSource;
      }
      else
      {
        if (currentSelectItem is PathSelectItem pathSelectItem)
        {
          this.SelectExpandClause = pathSelectItem.SelectAndExpand;
          this.NavigationSource = resource.NavigationSource;
        }
        if (currentSelectItem is ExpandedReferenceSelectItem referenceSelectItem)
        {
          this.ExpandReference = true;
          this.NavigationSource = referenceSelectItem.NavigationSource;
        }
      }
      this.EdmProperty = edmProperty;
      if (currentSelectItem != null && !(this.NavigationSource is IEdmUnknownEntitySet))
        return;
      if (edmProperty is IEdmNavigationProperty && context.NavigationSource != null)
        this.NavigationSource = context.NavigationSource.FindNavigationTarget(this.NavigationProperty);
      else
        this.NavigationSource = resource.NavigationSource;
    }

    internal IWebApiRequestMessage InternalRequest { get; private set; }

    internal IWebApiUrlHelper InternalUrlHelper { get; private set; }

    internal ODataQueryContext QueryContext
    {
      get => this.QueryOptions != null ? this.QueryOptions.Context : this._queryContext;
      private set => this._queryContext = value;
    }

    public IEdmNavigationSource NavigationSource { get; set; }

    public IEdmModel Model { get; set; }

    public Microsoft.AspNet.OData.Routing.ODataPath Path { get; set; }

    public string RootElementName { get; set; }

    public bool SkipExpensiveAvailabilityChecks { get; set; }

    public ODataMetadataLevel MetadataLevel { get; set; }

    public SelectExpandClause SelectExpandClause
    {
      get
      {
        if (this._isSelectExpandClauseSet)
          return this._selectExpandClause;
        return this.QueryOptions != null ? (this.QueryOptions.SelectExpand != null ? this.QueryOptions.SelectExpand.ProcessedSelectExpandClause : (SelectExpandClause) null) : (this.CurrentSelectItem is ExpandedNavigationSelectItem currentSelectItem ? currentSelectItem.SelectAndExpand : (SelectExpandClause) null);
      }
      set
      {
        this._isSelectExpandClauseSet = true;
        this._selectExpandClause = value;
      }
    }

    internal ExpandedReferenceSelectItem CurrentExpandedSelectItem => this.CurrentSelectItem as ExpandedReferenceSelectItem;

    internal SelectItem CurrentSelectItem { get; set; }

    public ODataQueryOptions QueryOptions { get; internal set; }

    internal Queue<IEdmProperty> PropertiesInPath { get; private set; }

    public ResourceContext ExpandedResource { get; set; }

    public bool ExpandReference { get; set; }

    public IEdmProperty EdmProperty { get; set; }

    public IEdmNavigationProperty NavigationProperty => this.EdmProperty as IEdmNavigationProperty;

    public IDictionary<object, object> Items
    {
      get
      {
        this._items = this._items ?? (IDictionary<object, object>) new Dictionary<object, object>();
        return this._items;
      }
      private set => this._items = value;
    }

    internal IEdmTypeReference GetEdmType(object instance, Type type)
    {
      IEdmTypeReference edmType1;
      if (instance is IEdmObject edmObject)
      {
        if (edmObject is IEdmStructuredObject structuredObject)
          structuredObject.SetModel(this.Model);
        edmType1 = edmObject.GetEdmType();
        if (edmType1 == null)
          throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.EdmTypeCannotBeNull, (object) edmObject.GetType().FullName, (object) typeof (IEdmObject).Name);
      }
      else
      {
        if (this.Model == null)
          throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.RequestMustHaveModel);
        this._typeMappingCache = this._typeMappingCache ?? this.Model.GetTypeMappingCache();
        edmType1 = this._typeMappingCache.GetEdmType(type, this.Model);
        if (edmType1 == null)
        {
          if (instance != null)
            edmType1 = this._typeMappingCache.GetEdmType(instance.GetType(), this.Model);
          if (edmType1 == null)
            throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.ClrTypeNotInModel, (object) type);
        }
        else if (instance != null)
        {
          IEdmTypeReference edmType2 = this._typeMappingCache.GetEdmType(instance.GetType(), this.Model);
          if (edmType2 != null && edmType2 != edmType1)
            edmType1 = edmType2;
        }
      }
      return edmType1;
    }
  }
}
