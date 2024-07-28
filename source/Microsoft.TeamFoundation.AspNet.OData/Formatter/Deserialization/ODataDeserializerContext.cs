// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Deserialization.ODataDeserializerContext
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Routing;
using Microsoft.OData.Edm;
using System;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace Microsoft.AspNet.OData.Formatter.Deserialization
{
  public class ODataDeserializerContext
  {
    private HttpRequestMessage _request;
    private bool? _isDeltaOfT;
    private bool? _isUntyped;

    public HttpRequestMessage Request
    {
      get => this._request;
      set
      {
        this._request = value;
        this.InternalRequest = this._request != null ? (IWebApiRequestMessage) new WebApiRequestMessage(this._request) : (IWebApiRequestMessage) null;
        this.InternalUrlHelper = this._request != null ? (IWebApiUrlHelper) new WebApiUrlHelper(this._request.GetUrlHelper()) : (IWebApiUrlHelper) null;
      }
    }

    public HttpRequestContext RequestContext { get; set; }

    public Type ResourceType { get; set; }

    public IEdmTypeReference ResourceEdmType { get; set; }

    public ODataPath Path { get; set; }

    public IEdmModel Model { get; set; }

    internal IWebApiRequestMessage InternalRequest { get; private set; }

    internal IWebApiUrlHelper InternalUrlHelper { get; private set; }

    internal bool IsDeltaOfT
    {
      get
      {
        if (!this._isDeltaOfT.HasValue)
          this._isDeltaOfT = new bool?(this.ResourceType != (Type) null && this.ResourceType.IsGenericType() && this.ResourceType.GetGenericTypeDefinition() == typeof (Delta<>));
        return this._isDeltaOfT.Value;
      }
    }

    internal bool IsUntyped
    {
      get
      {
        if (!this._isUntyped.HasValue)
          this._isUntyped = new bool?(TypeHelper.IsTypeAssignableFrom(typeof (IEdmObject), this.ResourceType) || typeof (ODataUntypedActionParameters) == this.ResourceType);
        return this._isUntyped.Value;
      }
    }

    internal IEdmTypeReference GetEdmType(Type type) => this.ResourceEdmType != null ? this.ResourceEdmType : EdmLibHelpers.GetExpectedPayloadType(type, this.Path, this.Model);
  }
}
