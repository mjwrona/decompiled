// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Serialization.ODataResourceSerializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Query.Expressions;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
  public class ODataResourceSerializer : ODataEdmTypeSerializer
  {
    private const string Resource = "Resource";

    public ODataResourceSerializer(ODataSerializerProvider serializerProvider)
      : base(ODataPayloadKind.Resource, serializerProvider)
    {
    }

    public override void WriteObject(
      object graph,
      Type type,
      ODataMessageWriter messageWriter,
      ODataSerializerContext writeContext)
    {
      if (messageWriter == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (messageWriter));
      if (writeContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writeContext));
      IEdmTypeReference edmType = writeContext.GetEdmType(graph, type);
      IEdmNavigationSource navigationSource = writeContext.NavigationSource;
      ODataWriter odataResourceWriter = messageWriter.CreateODataResourceWriter(navigationSource, edmType.ToStructuredType());
      this.WriteObjectInline(graph, edmType, odataResourceWriter, writeContext);
    }

    public override void WriteObjectInline(
      object graph,
      IEdmTypeReference expectedType,
      ODataWriter writer,
      ODataSerializerContext writeContext)
    {
      if (writer == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writer));
      if (writeContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writeContext));
      switch (graph)
      {
        case null:
        case NullEdmComplexObject _:
          throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotSerializerNull, (object) "Resource"));
        default:
          this.WriteResource(graph, writer, writeContext, expectedType);
          break;
      }
    }

    public virtual void WriteDeltaObjectInline(
      object graph,
      IEdmTypeReference expectedType,
      ODataWriter writer,
      ODataSerializerContext writeContext)
    {
      if (writer == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writer));
      if (writeContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writeContext));
      if (graph == null)
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotSerializerNull, (object) "Resource"));
      this.WriteDeltaResource(graph, writer, writeContext);
    }

    private void WriteDeltaResource(
      object graph,
      ODataWriter writer,
      ODataSerializerContext writeContext)
    {
      IEdmStructuredTypeReference resourceType = this.GetResourceType(graph, writeContext);
      ResourceContext resourceContext = new ResourceContext(writeContext, resourceType, graph);
      if (graph is EdmDeltaEntityObject deltaEntityObject && deltaEntityObject.NavigationSource != null)
        resourceContext.NavigationSource = deltaEntityObject.NavigationSource;
      SelectExpandNode selectExpandNode = this.CreateSelectExpandNode(resourceContext);
      if (selectExpandNode == null)
        return;
      ODataResource resource = this.CreateResource(selectExpandNode, resourceContext);
      if (resource == null)
        return;
      writer.WriteStart(resource);
      this.WriteDeltaComplexProperties(selectExpandNode, resourceContext, writer);
      writer.WriteEnd();
    }

    private void WriteDeltaComplexProperties(
      SelectExpandNode selectExpandNode,
      ResourceContext resourceContext,
      ODataWriter writer)
    {
      if (selectExpandNode.SelectedComplexTypeProperties == null)
        return;
      IEnumerable<IEdmStructuralProperty> source = (IEnumerable<IEdmStructuralProperty>) selectExpandNode.SelectedComplexTypeProperties.Keys;
      if (resourceContext.EdmObject != null && resourceContext.EdmObject.IsDeltaResource())
      {
        IEnumerable<string> changedProperties = (resourceContext.EdmObject as IDelta).GetChangedPropertyNames();
        source = source.Where<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (p => changedProperties.Contains<string>(p.Name)));
      }
      foreach (IEdmStructuralProperty structuralProperty in source)
      {
        ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
        {
          IsCollection = new bool?(structuralProperty.Type.IsCollection()),
          Name = structuralProperty.Name
        };
        writer.WriteStart(nestedResourceInfo);
        this.WriteDeltaComplexAndExpandedNavigationProperty((IEdmProperty) structuralProperty, (SelectExpandClause) null, resourceContext, writer);
        writer.WriteEnd();
      }
    }

    private void WriteDeltaComplexAndExpandedNavigationProperty(
      IEdmProperty edmProperty,
      SelectExpandClause selectExpandClause,
      ResourceContext resourceContext,
      ODataWriter writer)
    {
      object propertyValue = resourceContext.GetPropertyValue(edmProperty.Name);
      switch (propertyValue)
      {
        case null:
        case NullEdmComplexObject _:
          if (edmProperty.Type.IsCollection())
          {
            ODataWriter odataWriter = writer;
            ODataResourceSet resourceSet = new ODataResourceSet();
            resourceSet.TypeName = edmProperty.Type.FullName();
            odataWriter.WriteStart(resourceSet);
          }
          else
            writer.WriteStart((ODataResource) null);
          writer.WriteEnd();
          break;
        default:
          ODataSerializerContext writeContext = new ODataSerializerContext(resourceContext, selectExpandClause, edmProperty);
          if (edmProperty.Type.IsCollection())
          {
            new ODataDeltaFeedSerializer(this.SerializerProvider).WriteDeltaFeedInline(propertyValue, edmProperty.Type, writer, writeContext);
            break;
          }
          new ODataResourceSerializer(this.SerializerProvider).WriteDeltaObjectInline(propertyValue, edmProperty.Type, writer, writeContext);
          break;
      }
    }

    private static Dictionary<string, object> ExtractObjectProperties(
      object graph,
      ODataSerializerContext writeContext)
    {
      switch (graph)
      {
        case DynamicTypeWrapper dynamicTypeWrapper:
          if (dynamicTypeWrapper is IEdmStructuredObject structuredObject)
            structuredObject.SetModel(writeContext.Model);
          return dynamicTypeWrapper.Values;
        case IEnumerable<DynamicTypeWrapper> source:
          return ODataResourceSerializer.ExtractObjectProperties((object) source.SingleOrDefault<DynamicTypeWrapper>(), writeContext);
        case SelectExpandWrapper selectExpandWrapper:
          return selectExpandWrapper.Container.ToDictionary((IPropertyMapper) new IdentityPropertyMapper());
        default:
          return (Dictionary<string, object>) null;
      }
    }

    private static IEnumerable<ODataProperty> CreateODataPropertiesFromDynamicType(
      EdmEntityType entityType,
      object graph,
      Dictionary<IEdmProperty, object> dynamicTypeProperties,
      ODataSerializerContext writeContext)
    {
      Dictionary<string, object> objectProperties = ODataResourceSerializer.ExtractObjectProperties(graph, writeContext);
      List<ODataProperty> propertiesFromDynamicType = new List<ODataProperty>();
      if (objectProperties != null)
      {
        foreach (KeyValuePair<string, object> keyValuePair in objectProperties)
        {
          KeyValuePair<string, object> prop = keyValuePair;
          if (prop.Value != null && (prop.Value is DynamicTypeWrapper || prop.Value is IEnumerable<DynamicTypeWrapper>))
          {
            IEdmProperty key = entityType.Properties().FirstOrDefault<IEdmProperty>((Func<IEdmProperty, bool>) (p => p.Name.Equals(prop.Key)));
            if (key != null)
              dynamicTypeProperties.Add(key, prop.Value);
          }
          else
          {
            ODataProperty odataProperty1 = new ODataProperty();
            odataProperty1.Name = prop.Key;
            odataProperty1.Value = prop.Value;
            ODataProperty odataProperty2 = odataProperty1;
            propertiesFromDynamicType.Add(odataProperty2);
          }
        }
      }
      return (IEnumerable<ODataProperty>) propertiesFromDynamicType;
    }

    private void WriteDynamicTypeResource(
      object graph,
      ODataWriter writer,
      IEdmTypeReference expectedType,
      ODataSerializerContext writeContext)
    {
      Dictionary<IEdmProperty, object> dynamicTypeProperties = new Dictionary<IEdmProperty, object>();
      EdmEntityType definition = expectedType.Definition as EdmEntityType;
      ODataResource odataResource = new ODataResource();
      odataResource.TypeName = expectedType.FullName();
      odataResource.Properties = ODataResourceSerializer.CreateODataPropertiesFromDynamicType(definition, graph, dynamicTypeProperties, writeContext);
      ODataResource resource = odataResource;
      resource.IsTransient = true;
      writer.WriteStart(resource);
      foreach (IEdmProperty key in dynamicTypeProperties.Keys)
      {
        IEdmProperty property = key;
        ResourceContext resourceContext = new ResourceContext(writeContext, (IEdmStructuredTypeReference) expectedType.AsEntity(), graph);
        if (definition.NavigationProperties().Any<IEdmNavigationProperty>((Func<IEdmNavigationProperty, bool>) (p => p.Type.Equals((object) property.Type))) && !(property.Type is EdmCollectionTypeReference))
        {
          ODataNestedResourceInfo navigationLink = this.CreateNavigationLink(definition.NavigationProperties().FirstOrDefault<IEdmNavigationProperty>((Func<IEdmNavigationProperty, bool>) (p => p.Type.Equals((object) property.Type))), resourceContext);
          if (navigationLink != null)
          {
            writer.WriteStart(navigationLink);
            this.WriteDynamicTypeResource(dynamicTypeProperties[property], writer, property.Type, writeContext);
            writer.WriteEnd();
          }
        }
        else
        {
          ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
          {
            IsCollection = new bool?(property.Type.IsCollection()),
            Name = property.Name
          };
          writer.WriteStart(nestedResourceInfo);
          this.WriteDynamicComplexProperty(dynamicTypeProperties[property], property.Type, resourceContext, writer);
          writer.WriteEnd();
        }
      }
      writer.WriteEnd();
    }

    private void WriteResource(
      object graph,
      ODataWriter writer,
      ODataSerializerContext writeContext,
      IEdmTypeReference expectedType)
    {
      if (EdmLibHelpers.IsAggregatedTypeWrapper(graph.GetType()))
      {
        this.WriteDynamicTypeResource(graph, writer, expectedType, writeContext);
      }
      else
      {
        IEdmStructuredTypeReference resourceType = this.GetResourceType(graph, writeContext);
        ResourceContext resourceContext = new ResourceContext(writeContext, resourceType, graph);
        SelectExpandNode selectExpandNode = this.CreateSelectExpandNode(resourceContext);
        if (selectExpandNode == null)
          return;
        ODataResource resource = this.CreateResource(selectExpandNode, resourceContext);
        if (resource == null)
          return;
        if (resourceContext.SerializerContext.ExpandReference)
        {
          writer.WriteEntityReferenceLink(new ODataEntityReferenceLink()
          {
            Url = resource.Id
          });
        }
        else
        {
          writer.WriteStart(resource);
          this.WriteComplexProperties(selectExpandNode, resourceContext, writer);
          this.WriteDynamicComplexProperties(resourceContext, writer);
          this.WriteNavigationLinks(selectExpandNode, resourceContext, writer);
          this.WriteExpandedNavigationProperties(selectExpandNode, resourceContext, writer);
          this.WriteReferencedNavigationProperties(selectExpandNode, resourceContext, writer);
          writer.WriteEnd();
        }
      }
    }

    public virtual SelectExpandNode CreateSelectExpandNode(ResourceContext resourceContext)
    {
      ODataSerializerContext writeContext = resourceContext != null ? resourceContext.SerializerContext : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceContext));
      IEdmStructuredType structuredType = resourceContext.StructuredType;
      Tuple<SelectExpandClause, IEdmStructuredType> key = Tuple.Create<SelectExpandClause, IEdmStructuredType>(writeContext.SelectExpandClause, structuredType);
      object selectExpandNode;
      if (!writeContext.Items.TryGetValue((object) key, out selectExpandNode))
      {
        selectExpandNode = (object) new SelectExpandNode(structuredType, writeContext);
        writeContext.Items[(object) key] = selectExpandNode;
      }
      return selectExpandNode as SelectExpandNode;
    }

    public virtual ODataResource CreateResource(
      SelectExpandNode selectExpandNode,
      ResourceContext resourceContext)
    {
      if (selectExpandNode == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (selectExpandNode));
      if (resourceContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceContext));
      if (resourceContext.SerializerContext.ExpandReference)
      {
        ODataResource resource = new ODataResource();
        resource.Id = resourceContext.GenerateSelfLink(false);
        return resource;
      }
      string str = resourceContext.StructuredType.FullTypeName();
      ODataResource odataResource = new ODataResource();
      odataResource.TypeName = str;
      odataResource.Properties = this.CreateStructuralPropertyBag(selectExpandNode, resourceContext);
      ODataResource resource1 = odataResource;
      if (resourceContext.EdmObject is EdmDeltaEntityObject && resourceContext.NavigationSource != null)
      {
        ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo();
        serializationInfo.NavigationSourceName = resourceContext.NavigationSource.Name;
        serializationInfo.NavigationSourceKind = resourceContext.NavigationSource.NavigationSourceKind();
        IEdmEntityType edmEntityType = resourceContext.NavigationSource.EntityType();
        if (edmEntityType != null)
          serializationInfo.NavigationSourceEntityTypeName = edmEntityType.Name;
        ODataObjectModelExtensions.SetSerializationInfo(resource1, serializationInfo);
      }
      this.AppendDynamicProperties(resource1, selectExpandNode, resourceContext);
      if (selectExpandNode.SelectedActions != null)
      {
        foreach (ODataAction odataAction in this.CreateODataActions((IEnumerable<IEdmAction>) selectExpandNode.SelectedActions, resourceContext))
          resource1.AddAction(odataAction);
      }
      if (selectExpandNode.SelectedFunctions != null)
      {
        foreach (ODataFunction odataFunction in this.CreateODataFunctions((IEnumerable<IEdmFunction>) selectExpandNode.SelectedFunctions, resourceContext))
          resource1.AddFunction(odataFunction);
      }
      IEdmStructuredType odataPathType = ODataResourceSerializer.GetODataPathType(resourceContext.SerializerContext);
      if (resourceContext.StructuredType.TypeKind == EdmTypeKind.Complex)
        ODataResourceSerializer.AddTypeNameAnnotationAsNeededForComplex(resource1, resourceContext.SerializerContext.MetadataLevel);
      else
        ODataResourceSerializer.AddTypeNameAnnotationAsNeeded(resource1, odataPathType, resourceContext.SerializerContext.MetadataLevel);
      if (resourceContext.StructuredType.TypeKind == EdmTypeKind.Entity && resourceContext.NavigationSource != null)
      {
        if (!(resourceContext.NavigationSource is IEdmContainedEntitySet))
        {
          EntitySelfLinks entitySelfLinks = resourceContext.SerializerContext.Model.GetNavigationSourceLinkBuilder(resourceContext.NavigationSource).BuildEntitySelfLinks(resourceContext, resourceContext.SerializerContext.MetadataLevel);
          if (entitySelfLinks.IdLink != (Uri) null)
            resource1.Id = entitySelfLinks.IdLink;
          if (entitySelfLinks.ReadLink != (Uri) null)
            resource1.ReadLink = entitySelfLinks.ReadLink;
          if (entitySelfLinks.EditLink != (Uri) null)
            resource1.EditLink = entitySelfLinks.EditLink;
        }
        string etag = this.CreateETag(resourceContext);
        if (etag != null)
          resource1.ETag = etag;
      }
      return resource1;
    }

    public virtual void AppendDynamicProperties(
      ODataResource resource,
      SelectExpandNode selectExpandNode,
      ResourceContext resourceContext)
    {
      IEdmStructuredObject edmObject = resourceContext.EdmObject;
      bool flag1 = EdmLibHelpers.HasDynamicTypeWrapper(edmObject?.GetType());
      if (!resourceContext.StructuredType.IsOpen && !flag1 || !selectExpandNode.SelectAllDynamicProperties && (selectExpandNode.SelectedDynamicProperties == null || !selectExpandNode.SelectedDynamicProperties.Any<string>()))
        return;
      bool flag2 = false;
      if (resourceContext.EdmObject is EdmDeltaComplexObject || resourceContext.EdmObject is EdmDeltaEntityObject)
        flag2 = true;
      else if (resourceContext.InternalRequest != null)
        flag2 = resourceContext.InternalRequest.Options.NullDynamicPropertyIsEnabled;
      IEdmStructuredType structuredType = resourceContext.StructuredType;
      object obj;
      if (!(edmObject is IDelta))
      {
        PropertyInfo propertyDictionary = EdmLibHelpers.GetDynamicPropertyDictionary(structuredType, resourceContext.EdmModel);
        if (propertyDictionary == (PropertyInfo) null || edmObject == null || !edmObject.TryGetPropertyValue(propertyDictionary.Name, out obj) || obj == null)
        {
          if (!flag1)
            return;
          obj = edmObject is ISelectExpandWrapper selectExpandWrapper ? (object) selectExpandWrapper.ToDictionary() : (object) (IDictionary<string, object>) null;
        }
      }
      else
        obj = (object) ((EdmStructuredObject) edmObject).TryGetDynamicProperties();
      IDictionary<string, object> source = (IDictionary<string, object>) obj;
      HashSet<string> stringSet = new HashSet<string>(resource.Properties.Select<ODataProperty, string>((Func<ODataProperty, string>) (p => p.Name)));
      List<ODataProperty> odataPropertyList1 = new List<ODataProperty>();
      Func<KeyValuePair<string, object>, bool> predicate = (Func<KeyValuePair<string, object>, bool>) (x => selectExpandNode.SelectedDynamicProperties == null || selectExpandNode.SelectedDynamicProperties.Contains(x.Key));
      foreach (KeyValuePair<string, object> keyValuePair in source.Where<KeyValuePair<string, object>>(predicate))
      {
        if (!string.IsNullOrEmpty(keyValuePair.Key))
        {
          if (keyValuePair.Value == null)
          {
            if (flag2)
            {
              List<ODataProperty> odataPropertyList2 = odataPropertyList1;
              ODataProperty odataProperty = new ODataProperty();
              odataProperty.Name = keyValuePair.Key;
              odataProperty.Value = (object) new ODataNullValue();
              odataPropertyList2.Add(odataProperty);
            }
          }
          else if (stringSet.Contains(keyValuePair.Key))
          {
            if (!flag1)
              throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.DynamicPropertyNameAlreadyUsedAsDeclaredPropertyName, (object) keyValuePair.Key, (object) structuredType.FullTypeName());
          }
          else
          {
            IEdmTypeReference edmType = resourceContext.SerializerContext.GetEdmType(keyValuePair.Value, keyValuePair.Value.GetType());
            if (edmType == null)
              throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.TypeOfDynamicPropertyNotSupported, (object) keyValuePair.Value.GetType().FullName, (object) keyValuePair.Key);
            if (edmType.IsStructured() || edmType.IsCollection() && edmType.AsCollection().ElementType().IsStructured())
            {
              if (resourceContext.DynamicComplexProperties == null)
                resourceContext.DynamicComplexProperties = (IDictionary<string, object>) new ConcurrentDictionary<string, object>();
              resourceContext.DynamicComplexProperties.Add(keyValuePair);
            }
            else
              odataPropertyList1.Add((this.SerializerProvider.GetEdmTypeSerializer(edmType) ?? throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.DynamicPropertyCannotBeSerialized, (object) keyValuePair.Key, (object) edmType.FullName())).CreateProperty(keyValuePair.Value, edmType, keyValuePair.Key, resourceContext.SerializerContext));
          }
        }
      }
      if (!odataPropertyList1.Any<ODataProperty>())
        return;
      resource.Properties = resource.Properties.Concat<ODataProperty>((IEnumerable<ODataProperty>) odataPropertyList1);
    }

    public virtual string CreateETag(ResourceContext resourceContext)
    {
      if (resourceContext.InternalRequest == null)
        return (string) null;
      IEdmModel edmModel = resourceContext.EdmModel;
      IEdmNavigationSource navigationSource = resourceContext.NavigationSource;
      IEnumerable<IEdmStructuralProperty> structuralProperties = edmModel == null || navigationSource == null ? Enumerable.Empty<IEdmStructuralProperty>() : (IEnumerable<IEdmStructuralProperty>) edmModel.GetConcurrencyProperties(navigationSource).OrderBy<IEdmStructuralProperty, string>((Func<IEdmStructuralProperty, string>) (c => c.Name));
      IDictionary<string, object> properties = (IDictionary<string, object>) new Dictionary<string, object>();
      foreach (IEdmStructuralProperty structuralProperty in structuralProperties)
        properties.Add(structuralProperty.Name, resourceContext.GetPropertyValue(structuralProperty.Name));
      return resourceContext.InternalRequest.CreateETag(properties);
    }

    private void WriteNavigationLinks(
      SelectExpandNode selectExpandNode,
      ResourceContext resourceContext,
      ODataWriter writer)
    {
      if (selectExpandNode.SelectedNavigationProperties == null)
        return;
      foreach (ODataNestedResourceInfo navigationLink in this.CreateNavigationLinks((IEnumerable<IEdmNavigationProperty>) selectExpandNode.SelectedNavigationProperties, resourceContext))
      {
        writer.WriteStart(navigationLink);
        writer.WriteEnd();
      }
    }

    private void WriteDynamicComplexProperties(ResourceContext resourceContext, ODataWriter writer)
    {
      if (resourceContext.DynamicComplexProperties == null)
        return;
      foreach (KeyValuePair<string, object> dynamicComplexProperty in (IEnumerable<KeyValuePair<string, object>>) resourceContext.DynamicComplexProperties)
      {
        if (!string.IsNullOrEmpty(dynamicComplexProperty.Key) && dynamicComplexProperty.Value != null)
        {
          IEdmTypeReference edmType = resourceContext.SerializerContext.GetEdmType(dynamicComplexProperty.Value, dynamicComplexProperty.Value.GetType());
          if (edmType.IsStructured() || edmType.IsCollection() && edmType.AsCollection().ElementType().IsStructured())
          {
            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
              IsCollection = new bool?(edmType.IsCollection()),
              Name = dynamicComplexProperty.Key
            };
            writer.WriteStart(nestedResourceInfo);
            this.WriteDynamicComplexProperty(dynamicComplexProperty.Value, edmType, resourceContext, writer);
            writer.WriteEnd();
          }
        }
      }
    }

    private void WriteDynamicComplexProperty(
      object propertyValue,
      IEdmTypeReference edmType,
      ResourceContext resourceContext,
      ODataWriter writer)
    {
      ODataSerializerContext writeContext = new ODataSerializerContext(resourceContext, (SelectExpandClause) null, (IEdmProperty) null);
      if (edmType.IsCollection() && edmType.AsCollection().ElementType().IsEntity())
      {
        IEdmEntitySet edmEntitySet = resourceContext.EdmModel.EntityContainer.EntitySets().FirstOrDefault<IEdmEntitySet>((Func<IEdmEntitySet, bool>) (e => e.EntityType() == edmType.AsCollection().ElementType().AsEntity().Definition));
        if (edmEntitySet != null)
          writeContext.NavigationSource = (IEdmNavigationSource) edmEntitySet;
      }
      (this.SerializerProvider.GetEdmTypeSerializer(edmType) ?? throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeCannotBeSerialized, (object) edmType.ToTraceString()))).WriteObjectInline(propertyValue, edmType, writer, writeContext);
    }

    private void WriteComplexProperties(
      SelectExpandNode selectExpandNode,
      ResourceContext resourceContext,
      ODataWriter writer)
    {
      IDictionary<IEdmStructuralProperty, PathSelectItem> source = selectExpandNode.SelectedComplexTypeProperties;
      if (source == null)
        return;
      if (resourceContext.EdmObject != null && resourceContext.EdmObject.IsDeltaResource())
      {
        IEnumerable<string> changedProperties = (resourceContext.EdmObject as IDelta).GetChangedPropertyNames();
        source = (IDictionary<IEdmStructuralProperty, PathSelectItem>) source.Where<KeyValuePair<IEdmStructuralProperty, PathSelectItem>>((Func<KeyValuePair<IEdmStructuralProperty, PathSelectItem>, bool>) (p => changedProperties.Contains<string>(p.Key.Name))).ToDictionary<KeyValuePair<IEdmStructuralProperty, PathSelectItem>, IEdmStructuralProperty, PathSelectItem>((Func<KeyValuePair<IEdmStructuralProperty, PathSelectItem>, IEdmStructuralProperty>) (a => a.Key), (Func<KeyValuePair<IEdmStructuralProperty, PathSelectItem>, PathSelectItem>) (a => a.Value));
      }
      foreach (KeyValuePair<IEdmStructuralProperty, PathSelectItem> keyValuePair in (IEnumerable<KeyValuePair<IEdmStructuralProperty, PathSelectItem>>) source)
      {
        IEdmStructuralProperty key = keyValuePair.Key;
        ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
        {
          IsCollection = new bool?(key.Type.IsCollection()),
          Name = key.Name
        };
        writer.WriteStart(nestedResourceInfo);
        this.WriteComplexAndExpandedNavigationProperty((IEdmProperty) key, (SelectItem) keyValuePair.Value, resourceContext, writer);
        writer.WriteEnd();
      }
    }

    private void WriteExpandedNavigationProperties(
      SelectExpandNode selectExpandNode,
      ResourceContext resourceContext,
      ODataWriter writer)
    {
      IDictionary<IEdmNavigationProperty, ExpandedNavigationSelectItem> expandedProperties = selectExpandNode.ExpandedProperties;
      if (expandedProperties == null)
        return;
      foreach (KeyValuePair<IEdmNavigationProperty, ExpandedNavigationSelectItem> keyValuePair in (IEnumerable<KeyValuePair<IEdmNavigationProperty, ExpandedNavigationSelectItem>>) expandedProperties)
      {
        IEdmNavigationProperty key = keyValuePair.Key;
        ODataNestedResourceInfo navigationLink = this.CreateNavigationLink(key, resourceContext);
        if (navigationLink != null)
        {
          writer.WriteStart(navigationLink);
          this.WriteComplexAndExpandedNavigationProperty((IEdmProperty) key, (SelectItem) keyValuePair.Value, resourceContext, writer);
          writer.WriteEnd();
        }
      }
    }

    private void WriteReferencedNavigationProperties(
      SelectExpandNode selectExpandNode,
      ResourceContext resourceContext,
      ODataWriter writer)
    {
      IDictionary<IEdmNavigationProperty, ExpandedReferenceSelectItem> referencedProperties = selectExpandNode.ReferencedProperties;
      if (referencedProperties == null)
        return;
      foreach (KeyValuePair<IEdmNavigationProperty, ExpandedReferenceSelectItem> keyValuePair in (IEnumerable<KeyValuePair<IEdmNavigationProperty, ExpandedReferenceSelectItem>>) referencedProperties)
      {
        IEdmNavigationProperty key = keyValuePair.Key;
        ODataNestedResourceInfo navigationLink = this.CreateNavigationLink(key, resourceContext);
        if (navigationLink != null)
        {
          writer.WriteStart(navigationLink);
          this.WriteComplexAndExpandedNavigationProperty((IEdmProperty) key, (SelectItem) keyValuePair.Value, resourceContext, writer);
          writer.WriteEnd();
        }
      }
    }

    private void WriteComplexAndExpandedNavigationProperty(
      IEdmProperty edmProperty,
      SelectItem selectItem,
      ResourceContext resourceContext,
      ODataWriter writer)
    {
      object propertyValue = resourceContext.GetPropertyValue(edmProperty.Name);
      switch (propertyValue)
      {
        case null:
        case NullEdmComplexObject _:
          if (edmProperty.Type.IsCollection())
          {
            ODataWriter odataWriter = writer;
            ODataResourceSet resourceSet = new ODataResourceSet();
            resourceSet.TypeName = edmProperty.Type.FullName();
            odataWriter.WriteStart(resourceSet);
          }
          else
            writer.WriteStart((ODataResource) null);
          writer.WriteEnd();
          break;
        default:
          ODataSerializerContext writeContext = new ODataSerializerContext(resourceContext, edmProperty, resourceContext.SerializerContext.QueryContext, selectItem);
          (this.SerializerProvider.GetEdmTypeSerializer(edmProperty.Type) ?? throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeCannotBeSerialized, (object) edmProperty.Type.ToTraceString()))).WriteObjectInline(propertyValue, edmProperty.Type, writer, writeContext);
          break;
      }
    }

    private IEnumerable<ODataNestedResourceInfo> CreateNavigationLinks(
      IEnumerable<IEdmNavigationProperty> navigationProperties,
      ResourceContext resourceContext)
    {
      foreach (IEdmNavigationProperty navigationProperty in navigationProperties)
      {
        ODataNestedResourceInfo navigationLink = this.CreateNavigationLink(navigationProperty, resourceContext);
        if (navigationLink != null)
          yield return navigationLink;
      }
    }

    public virtual ODataNestedResourceInfo CreateNavigationLink(
      IEdmNavigationProperty navigationProperty,
      ResourceContext resourceContext)
    {
      if (navigationProperty == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (navigationProperty));
      ODataSerializerContext serializerContext = resourceContext != null ? resourceContext.SerializerContext : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceContext));
      IEdmNavigationSource navigationSource = serializerContext.NavigationSource;
      ODataNestedResourceInfo navigationLink = (ODataNestedResourceInfo) null;
      if (navigationSource != null)
      {
        IEdmTypeReference type = navigationProperty.Type;
        Uri uri = serializerContext.Model.GetNavigationSourceLinkBuilder(navigationSource).BuildNavigationLink(resourceContext, navigationProperty, serializerContext.MetadataLevel);
        navigationLink = new ODataNestedResourceInfo()
        {
          IsCollection = new bool?(type.IsCollection()),
          Name = navigationProperty.Name
        };
        if (uri != (Uri) null)
          navigationLink.Url = uri;
      }
      return navigationLink;
    }

    private IEnumerable<ODataProperty> CreateStructuralPropertyBag(
      SelectExpandNode selectExpandNode,
      ResourceContext resourceContext)
    {
      List<ODataProperty> structuralPropertyBag = new List<ODataProperty>();
      if (selectExpandNode.SelectedStructuralProperties != null)
      {
        IEnumerable<IEdmStructuralProperty> source = (IEnumerable<IEdmStructuralProperty>) selectExpandNode.SelectedStructuralProperties;
        if (resourceContext.EdmObject != null && resourceContext.EdmObject.IsDeltaResource())
        {
          IEnumerable<string> changedProperties = (resourceContext.EdmObject as IDelta).GetChangedPropertyNames();
          source = source.Where<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (p => changedProperties.Contains<string>(p.Name)));
        }
        foreach (IEdmStructuralProperty structuralProperty1 in source)
        {
          ODataProperty structuralProperty2 = this.CreateStructuralProperty(structuralProperty1, resourceContext);
          if (structuralProperty2 != null)
            structuralPropertyBag.Add(structuralProperty2);
        }
      }
      return (IEnumerable<ODataProperty>) structuralPropertyBag;
    }

    public virtual ODataProperty CreateStructuralProperty(
      IEdmStructuralProperty structuralProperty,
      ResourceContext resourceContext)
    {
      if (structuralProperty == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (structuralProperty));
      ODataSerializerContext writeContext = resourceContext != null ? resourceContext.SerializerContext : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceContext));
      ODataEdmTypeSerializer edmTypeSerializer = this.SerializerProvider.GetEdmTypeSerializer(structuralProperty.Type);
      if (edmTypeSerializer == null)
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeCannotBeSerialized, (object) structuralProperty.Type.FullName()));
      object propertyValue = resourceContext.GetPropertyValue(structuralProperty.Name);
      IEdmTypeReference edmTypeReference = structuralProperty.Type;
      if (propertyValue != null && !edmTypeReference.IsPrimitive() && !edmTypeReference.IsEnum())
      {
        IEdmTypeReference edmType = writeContext.GetEdmType(propertyValue, propertyValue.GetType());
        if (edmTypeReference != null && edmTypeReference != edmType)
          edmTypeReference = edmType;
      }
      return edmTypeSerializer.CreateProperty(propertyValue, edmTypeReference, structuralProperty.Name, writeContext);
    }

    private IEnumerable<ODataAction> CreateODataActions(
      IEnumerable<IEdmAction> actions,
      ResourceContext resourceContext)
    {
      foreach (IEdmAction action in actions)
      {
        ODataAction odataAction = this.CreateODataAction(action, resourceContext);
        if (odataAction != null)
          yield return odataAction;
      }
    }

    private IEnumerable<ODataFunction> CreateODataFunctions(
      IEnumerable<IEdmFunction> functions,
      ResourceContext resourceContext)
    {
      foreach (IEdmFunction function in functions)
      {
        ODataFunction odataFunction = this.CreateODataFunction(function, resourceContext);
        if (odataFunction != null)
          yield return odataFunction;
      }
    }

    public virtual ODataAction CreateODataAction(IEdmAction action, ResourceContext resourceContext)
    {
      if (action == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (action));
      if (resourceContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceContext));
      OperationLinkBuilder operationLinkBuilder = resourceContext.EdmModel.GetOperationLinkBuilder((IEdmOperation) action);
      return operationLinkBuilder == null ? (ODataAction) null : ODataResourceSerializer.CreateODataOperation((IEdmOperation) action, operationLinkBuilder, resourceContext) as ODataAction;
    }

    public virtual ODataFunction CreateODataFunction(
      IEdmFunction function,
      ResourceContext resourceContext)
    {
      if (function == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (function));
      if (resourceContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceContext));
      OperationLinkBuilder operationLinkBuilder = resourceContext.EdmModel.GetOperationLinkBuilder((IEdmOperation) function);
      return operationLinkBuilder == null ? (ODataFunction) null : ODataResourceSerializer.CreateODataOperation((IEdmOperation) function, operationLinkBuilder, resourceContext) as ODataFunction;
    }

    private static ODataOperation CreateODataOperation(
      IEdmOperation operation,
      OperationLinkBuilder builder,
      ResourceContext resourceContext)
    {
      ODataMetadataLevel metadataLevel = resourceContext.SerializerContext.MetadataLevel;
      IEdmModel edmModel = resourceContext.EdmModel;
      if (ODataResourceSerializer.ShouldOmitOperation(operation, builder, metadataLevel))
        return (ODataOperation) null;
      Uri uri1 = builder.BuildLink(resourceContext);
      if (uri1 == (Uri) null)
        return (ODataOperation) null;
      Uri uri2 = new Uri(new Uri(resourceContext.InternalUrlHelper.CreateODataLink((ODataPathSegment) MetadataSegment.Instance)), "#" + ODataResourceSerializer.CreateMetadataFragment(operation));
      ODataOperation odataOperation = !(operation is IEdmAction) ? (ODataOperation) new ODataFunction() : (ODataOperation) new ODataAction();
      odataOperation.Metadata = uri2;
      if (metadataLevel == ODataMetadataLevel.FullMetadata)
        ODataResourceSerializer.EmitTitle(edmModel, operation, odataOperation);
      if (!builder.FollowsConventions || metadataLevel == ODataMetadataLevel.FullMetadata)
        odataOperation.Target = uri1;
      return odataOperation;
    }

    internal static void EmitTitle(
      IEdmModel model,
      IEdmOperation operation,
      ODataOperation odataOperation)
    {
      OperationTitleAnnotation operationTitleAnnotation = model.GetOperationTitleAnnotation(operation);
      if (operationTitleAnnotation != null)
        odataOperation.Title = operationTitleAnnotation.Title;
      else
        odataOperation.Title = operation.Name;
    }

    internal static string CreateMetadataFragment(IEdmOperation operation)
    {
      string name = operation.Name;
      return operation.Namespace + "." + name;
    }

    private static IEdmStructuredType GetODataPathType(ODataSerializerContext serializerContext)
    {
      if (serializerContext.EdmProperty != null)
        return serializerContext.EdmProperty.Type.IsCollection() ? serializerContext.EdmProperty.Type.AsCollection().ElementType().ToStructuredType() : serializerContext.EdmProperty.Type.AsStructured().StructuredDefinition();
      if (serializerContext.ExpandedResource != null)
        return (IEdmStructuredType) null;
      IEdmType odataPathType = (IEdmType) null;
      if (serializerContext.NavigationSource != null)
      {
        odataPathType = (IEdmType) serializerContext.NavigationSource.EntityType();
        if (odataPathType.TypeKind == EdmTypeKind.Collection)
          odataPathType = (odataPathType as IEdmCollectionType).ElementType.Definition;
      }
      if (serializerContext.Path != null && (serializerContext.NavigationSource == null || serializerContext.NavigationSource == serializerContext.Path.NavigationSource))
      {
        odataPathType = serializerContext.Path.EdmType;
        if (odataPathType != null && odataPathType.TypeKind == EdmTypeKind.Collection)
          odataPathType = (odataPathType as IEdmCollectionType).ElementType.Definition;
      }
      return odataPathType as IEdmStructuredType;
    }

    internal static void AddTypeNameAnnotationAsNeeded(
      ODataResource resource,
      IEdmStructuredType odataPathType,
      ODataMetadataLevel metadataLevel)
    {
      string typeName = (string) null;
      if (!ODataResourceSerializer.ShouldSuppressTypeNameSerialization(resource, odataPathType, metadataLevel))
        typeName = resource.TypeName;
      resource.TypeAnnotation = new ODataTypeAnnotation(typeName);
    }

    internal static void AddTypeNameAnnotationAsNeededForComplex(
      ODataResource resource,
      ODataMetadataLevel metadataLevel)
    {
      if (!ODataResourceSerializer.ShouldAddTypeNameAnnotationForComplex(metadataLevel))
        return;
      string typeName = !ODataResourceSerializer.ShouldSuppressTypeNameSerializationForComplex(metadataLevel) ? resource.TypeName : (string) null;
      resource.TypeAnnotation = new ODataTypeAnnotation(typeName);
    }

    internal static bool ShouldAddTypeNameAnnotationForComplex(ODataMetadataLevel metadataLevel)
    {
      switch (metadataLevel)
      {
        case ODataMetadataLevel.MinimalMetadata:
          return false;
        default:
          return true;
      }
    }

    internal static bool ShouldSuppressTypeNameSerializationForComplex(
      ODataMetadataLevel metadataLevel)
    {
      return metadataLevel != ODataMetadataLevel.FullMetadata && metadataLevel == ODataMetadataLevel.NoMetadata;
    }

    internal static bool ShouldOmitOperation(
      IEdmOperation operation,
      OperationLinkBuilder builder,
      ODataMetadataLevel metadataLevel)
    {
      switch (metadataLevel)
      {
        case ODataMetadataLevel.MinimalMetadata:
        case ODataMetadataLevel.NoMetadata:
          return operation.IsBound && builder.FollowsConventions;
        default:
          return false;
      }
    }

    internal static bool ShouldSuppressTypeNameSerialization(
      ODataResource resource,
      IEdmStructuredType edmType,
      ODataMetadataLevel metadataLevel)
    {
      switch (metadataLevel)
      {
        case ODataMetadataLevel.FullMetadata:
          return false;
        case ODataMetadataLevel.NoMetadata:
          return true;
        default:
          string b = (string) null;
          if (edmType != null)
            b = edmType.FullTypeName();
          return string.Equals(resource.TypeName, b, StringComparison.Ordinal);
      }
    }

    private IEdmStructuredTypeReference GetResourceType(
      object graph,
      ODataSerializerContext writeContext)
    {
      IEdmTypeReference edmType = writeContext.GetEdmType(graph, graph.GetType());
      return edmType.IsStructured() ? edmType.AsStructured() : throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotWriteType, (object) this.GetType().Name, (object) edmType.FullName()));
    }
  }
}
