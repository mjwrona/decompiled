// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ResourceContext
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Query.Expressions;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Microsoft.AspNet.OData
{
  public class ResourceContext
  {
    private object _resourceInstance;
    private IEdmStructuredObject edmObject;

    public HttpRequestMessage Request
    {
      get => this.SerializerContext.Request;
      set => this.SerializerContext.Request = value;
    }

    public UrlHelper Url
    {
      get => this.SerializerContext.Url;
      set => this.SerializerContext.Url = value;
    }

    public ResourceContext() => this.SerializerContext = new ODataSerializerContext();

    public ResourceContext(
      ODataSerializerContext serializerContext,
      IEdmStructuredTypeReference structuredType,
      object resourceInstance)
      : this(serializerContext, structuredType, ResourceContext.AsEdmResourceObject(resourceInstance, structuredType, serializerContext.Model))
    {
    }

    private ResourceContext(
      ODataSerializerContext serializerContext,
      IEdmStructuredTypeReference structuredType,
      IEdmStructuredObject edmObject)
    {
      this.SerializerContext = serializerContext != null ? serializerContext : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (serializerContext));
      this.StructuredType = structuredType.StructuredDefinition();
      this.EdmObject = edmObject;
    }

    public ODataSerializerContext SerializerContext { get; set; }

    internal IWebApiRequestMessage InternalRequest => this.SerializerContext.InternalRequest;

    public IEdmModel EdmModel
    {
      get => this.SerializerContext.Model;
      set
      {
        this.SerializerContext.Model = value;
        this.EnsureModel(this.edmObject);
      }
    }

    public IEdmNavigationSource NavigationSource
    {
      get => this.SerializerContext.NavigationSource;
      set => this.SerializerContext.NavigationSource = value;
    }

    public IEdmStructuredType StructuredType { get; set; }

    public IEdmStructuredObject EdmObject
    {
      get => this.edmObject;
      set => this.edmObject = this.EnsureModel(value);
    }

    private IEdmStructuredObject EnsureModel(IEdmStructuredObject obj)
    {
      obj?.SetModel(this.EdmModel);
      return obj;
    }

    public object ResourceInstance
    {
      get
      {
        if (this._resourceInstance == null)
          this._resourceInstance = this.BuildResourceInstance();
        return this._resourceInstance;
      }
      set => this._resourceInstance = value;
    }

    internal IWebApiUrlHelper InternalUrlHelper => this.SerializerContext.InternalUrlHelper;

    public bool SkipExpensiveAvailabilityChecks
    {
      get => this.SerializerContext.SkipExpensiveAvailabilityChecks;
      set => this.SerializerContext.SkipExpensiveAvailabilityChecks = value;
    }

    public IDictionary<string, object> DynamicComplexProperties { get; set; }

    public object GetPropertyValue(string propertyName)
    {
      if (this.EdmObject == null)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.EdmObjectNull, (object) typeof (ResourceContext).Name);
      object propertyValue;
      if (this.EdmObject.TryGetPropertyValue(propertyName, out propertyValue))
        return propertyValue;
      IEdmTypeReference edmType = this.EdmObject.GetEdmType();
      if (edmType == null)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.EdmTypeCannotBeNull, (object) this.EdmObject.GetType().FullName, (object) typeof (IEdmObject).Name);
      throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.PropertyNotFound, (object) edmType.ToTraceString(), (object) propertyName);
    }

    private object BuildResourceInstance()
    {
      if (this.EdmObject == null)
        return (object) null;
      if (this.EdmObject is TypedEdmStructuredObject edmObject1)
        return edmObject1.Instance;
      if (this.EdmObject is SelectExpandWrapper edmObject2 && edmObject2.UntypedInstance != null)
        return edmObject2.UntypedInstance;
      Type clrType = EdmLibHelpers.GetClrType((IEdmType) this.StructuredType, this.EdmModel);
      object resource = !(clrType == (Type) null) ? Activator.CreateInstance(clrType) : throw new InvalidOperationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.MappingDoesNotContainResourceType, (object) this.StructuredType.FullTypeName()));
      foreach (IEdmStructuralProperty structuralProperty in this.StructuredType.StructuralProperties())
      {
        object obj;
        if (this.EdmObject.TryGetPropertyValue(structuralProperty.Name, out obj) && obj != null)
        {
          string clrPropertyName = EdmLibHelpers.GetClrPropertyName((IEdmProperty) structuralProperty, this.EdmModel);
          if (TypeHelper.IsCollection(obj.GetType()))
            DeserializationHelpers.SetCollectionProperty(resource, (IEdmProperty) structuralProperty, obj, clrPropertyName);
          else
            DeserializationHelpers.SetProperty(resource, clrPropertyName, obj);
        }
      }
      return resource;
    }

    private static IEdmStructuredObject AsEdmResourceObject(
      object resourceInstance,
      IEdmStructuredTypeReference structuredType,
      IEdmModel model)
    {
      if (structuredType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (structuredType));
      if (resourceInstance is IEdmStructuredObject structuredObject)
        return structuredObject;
      return structuredType.IsEntity() ? (IEdmStructuredObject) new TypedEdmEntityObject(resourceInstance, structuredType.AsEntity(), model) : (IEdmStructuredObject) new TypedEdmComplexObject(resourceInstance, structuredType.AsComplex(), model);
    }
  }
}
