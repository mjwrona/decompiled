// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Deserialization.ODataResourceDeserializer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.Edm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.AspNet.OData.Formatter.Deserialization
{
  public class ODataResourceDeserializer : ODataEdmTypeDeserializer
  {
    public ODataResourceDeserializer(ODataDeserializerProvider deserializerProvider)
      : base(ODataPayloadKind.Resource, deserializerProvider)
    {
    }

    public override object Read(
      ODataMessageReader messageReader,
      Type type,
      ODataDeserializerContext readContext)
    {
      if (messageReader == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (messageReader));
      IEdmTypeReference type1 = readContext != null ? readContext.GetEdmType(type) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (readContext));
      IEdmStructuredTypeReference structuredTypeReference = type1.IsStructured() ? type1.AsStructured() : throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (type), SRResources.ArgumentMustBeOfType, (object) "Structured");
      IEdmNavigationSource navigationSource = (IEdmNavigationSource) null;
      if (structuredTypeReference.IsEntity())
      {
        if (readContext.Path == null)
          throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (readContext), SRResources.ODataPathMissing);
        navigationSource = readContext.Path.NavigationSource;
        if (navigationSource == null)
          throw new SerializationException(SRResources.NavigationSourceMissingDuringDeserialization);
      }
      return this.ReadInline((object) (messageReader.CreateODataResourceReader(navigationSource, structuredTypeReference.StructuredDefinition()).ReadResourceOrResourceSet() as ODataResourceWrapper), (IEdmTypeReference) structuredTypeReference, readContext);
    }

    public override sealed object ReadInline(
      object item,
      IEdmTypeReference edmType,
      ODataDeserializerContext readContext)
    {
      if (edmType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmType));
      if (edmType.IsComplex() && item == null)
        return (object) null;
      if (item == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (item));
      if (!edmType.IsStructured())
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (edmType), SRResources.ArgumentMustBeOfType, (object) "Entity or Complex");
      if (!(item is ODataResourceWrapper resourceWrapper))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (item), SRResources.ArgumentMustBeOfType, (object) typeof (ODataResource).Name);
      RuntimeHelpers.EnsureSufficientExecutionStack();
      return this.ReadResource(resourceWrapper, edmType.AsStructured(), readContext);
    }

    public virtual object ReadResource(
      ODataResourceWrapper resourceWrapper,
      IEdmStructuredTypeReference structuredType,
      ODataDeserializerContext readContext)
    {
      if (resourceWrapper == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceWrapper));
      if (readContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (readContext));
      if (!string.IsNullOrEmpty(resourceWrapper.Resource.TypeName) && structuredType.FullName() != resourceWrapper.Resource.TypeName)
      {
        IEdmModel model = readContext.Model;
        if (model == null)
          throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (readContext), SRResources.ModelMissingFromReadContext);
        if (!(model.FindType(resourceWrapper.Resource.TypeName) is IEdmStructuredType type))
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.ResourceTypeNotInModel, (object) resourceWrapper.Resource.TypeName));
        if (type.IsAbstract)
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotInstantiateAbstractResourceType, (object) resourceWrapper.Resource.TypeName));
        IEdmTypeReference edmType = !(type is IEdmEntityType edmEntityType) ? (IEdmTypeReference) new EdmComplexTypeReference(type as IEdmComplexType, false) : (IEdmTypeReference) new EdmEntityTypeReference(edmEntityType, false);
        object obj = (this.DeserializerProvider.GetEdmTypeDeserializer(edmType) ?? throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeCannotBeDeserialized, (object) edmEntityType.FullName()))).ReadInline((object) resourceWrapper, edmType, readContext);
        if (!(obj is EdmStructuredObject structuredObject))
          return obj;
        structuredObject.ExpectedEdmType = structuredType.StructuredDefinition();
        return obj;
      }
      object resourceInstance = this.CreateResourceInstance(structuredType, readContext);
      this.ApplyResourceProperties(resourceInstance, resourceWrapper, structuredType, readContext);
      return resourceInstance;
    }

    public virtual object CreateResourceInstance(
      IEdmStructuredTypeReference structuredType,
      ODataDeserializerContext readContext)
    {
      if (readContext == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (readContext));
      if (structuredType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (structuredType));
      IEdmModel model = readContext.Model;
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (readContext), SRResources.ModelMissingFromReadContext);
      if (readContext.IsUntyped)
        return structuredType.IsEntity() ? (object) new EdmEntityObject(structuredType.AsEntity()) : (object) new EdmComplexObject(structuredType.AsComplex());
      Type clrType = EdmLibHelpers.GetClrType((IEdmTypeReference) structuredType, model);
      if (clrType == (Type) null)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.MappingDoesNotContainResourceType, (object) structuredType.FullName()));
      if (!readContext.IsDeltaOfT)
        return Activator.CreateInstance(clrType);
      IEnumerable<string> strings = structuredType.StructuralProperties().Select<IEdmStructuralProperty, string>((Func<IEdmStructuralProperty, string>) (edmProperty => EdmLibHelpers.GetClrPropertyName((IEdmProperty) edmProperty, model)));
      if (structuredType.IsOpen())
      {
        PropertyInfo propertyDictionary = EdmLibHelpers.GetDynamicPropertyDictionary(structuredType.StructuredDefinition(), model);
        return Activator.CreateInstance(readContext.ResourceType, (object) clrType, (object) strings, (object) propertyDictionary);
      }
      return Activator.CreateInstance(readContext.ResourceType, (object) clrType, (object) strings);
    }

    public virtual void ApplyNestedProperties(
      object resource,
      ODataResourceWrapper resourceWrapper,
      IEdmStructuredTypeReference structuredType,
      ODataDeserializerContext readContext)
    {
      if (resourceWrapper == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceWrapper));
      foreach (ODataNestedResourceInfoWrapper nestedResourceInfo in (IEnumerable<ODataNestedResourceInfoWrapper>) resourceWrapper.NestedResourceInfos)
        this.ApplyNestedProperty(resource, nestedResourceInfo, structuredType, readContext);
    }

    public virtual void ApplyNestedProperty(
      object resource,
      ODataNestedResourceInfoWrapper resourceInfoWrapper,
      IEdmStructuredTypeReference structuredType,
      ODataDeserializerContext readContext)
    {
      if (resource == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resource));
      if (resourceInfoWrapper == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceInfoWrapper));
      IEdmProperty property = structuredType.FindProperty(resourceInfoWrapper.NestedResourceInfo.Name);
      if (property == null && !structuredType.IsOpen())
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.NestedPropertyNotfound, (object) resourceInfoWrapper.NestedResourceInfo.Name, (object) structuredType.FullName()));
      foreach (ODataItemBase nestedItem in (IEnumerable<ODataItemBase>) resourceInfoWrapper.NestedItems)
      {
        if (nestedItem == null)
        {
          if (property == null)
            this.ApplyDynamicResourceInNestedProperty(resourceInfoWrapper.NestedResourceInfo.Name, resource, structuredType, (ODataResourceWrapper) null, readContext);
          else
            this.ApplyResourceInNestedProperty(property, resource, (ODataResourceWrapper) null, readContext);
        }
        if (!(nestedItem is ODataEntityReferenceLinkBase))
        {
          if (nestedItem is ODataResourceSetWrapper resourceSetWrapper)
          {
            if (property == null)
              this.ApplyDynamicResourceSetInNestedProperty(resourceInfoWrapper.NestedResourceInfo.Name, resource, structuredType, resourceSetWrapper, readContext);
            else
              this.ApplyResourceSetInNestedProperty(property, resource, resourceSetWrapper, readContext);
          }
          else
          {
            ODataResourceWrapper resourceWrapper = (ODataResourceWrapper) nestedItem;
            if (resourceWrapper != null)
            {
              if (property == null)
                this.ApplyDynamicResourceInNestedProperty(resourceInfoWrapper.NestedResourceInfo.Name, resource, structuredType, resourceWrapper, readContext);
              else
                this.ApplyResourceInNestedProperty(property, resource, resourceWrapper, readContext);
            }
          }
        }
      }
    }

    public virtual void ApplyStructuralProperties(
      object resource,
      ODataResourceWrapper resourceWrapper,
      IEdmStructuredTypeReference structuredType,
      ODataDeserializerContext readContext)
    {
      if (resourceWrapper == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resourceWrapper));
      foreach (ODataProperty property in resourceWrapper.Resource.Properties)
        this.ApplyStructuralProperty(resource, property, structuredType, readContext);
    }

    public virtual void ApplyStructuralProperty(
      object resource,
      ODataProperty structuralProperty,
      IEdmStructuredTypeReference structuredType,
      ODataDeserializerContext readContext)
    {
      if (resource == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (resource));
      if (structuralProperty == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (structuralProperty));
      DeserializationHelpers.ApplyProperty(structuralProperty, structuredType, resource, this.DeserializerProvider, readContext);
    }

    private void ApplyResourceProperties(
      object resource,
      ODataResourceWrapper resourceWrapper,
      IEdmStructuredTypeReference structuredType,
      ODataDeserializerContext readContext)
    {
      this.ApplyStructuralProperties(resource, resourceWrapper, structuredType, readContext);
      this.ApplyNestedProperties(resource, resourceWrapper, structuredType, readContext);
    }

    private void ApplyResourceInNestedProperty(
      IEdmProperty nestedProperty,
      object resource,
      ODataResourceWrapper resourceWrapper,
      ODataDeserializerContext readContext)
    {
      if (readContext.IsDeltaOfT && nestedProperty is IEdmNavigationProperty property)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotPatchNavigationProperties, (object) property.Name, (object) property.DeclaringEntityType().FullName()));
      object obj = this.ReadNestedResourceInline(resourceWrapper, nestedProperty.Type, readContext);
      string clrPropertyName = EdmLibHelpers.GetClrPropertyName(nestedProperty, readContext.Model);
      DeserializationHelpers.SetProperty(resource, clrPropertyName, obj);
    }

    private void ApplyDynamicResourceInNestedProperty(
      string propertyName,
      object resource,
      IEdmStructuredTypeReference resourceStructuredType,
      ODataResourceWrapper resourceWrapper,
      ODataDeserializerContext readContext)
    {
      object obj = (object) null;
      if (resourceWrapper != null)
      {
        IEdmTypeReference edmTypeReference = readContext.Model.FindDeclaredType(resourceWrapper.Resource.TypeName).ToEdmTypeReference(true);
        obj = this.ReadNestedResourceInline(resourceWrapper, edmTypeReference, readContext);
      }
      DeserializationHelpers.SetDynamicProperty(resource, propertyName, obj, resourceStructuredType.StructuredDefinition(), readContext.Model);
    }

    private object ReadNestedResourceInline(
      ODataResourceWrapper resourceWrapper,
      IEdmTypeReference edmType,
      ODataDeserializerContext readContext)
    {
      if (resourceWrapper == null)
        return (object) null;
      ODataEdmTypeDeserializer typeDeserializer = this.DeserializerProvider.GetEdmTypeDeserializer(edmType);
      if (typeDeserializer == null)
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeCannotBeDeserialized, (object) edmType.FullName()));
      IEdmStructuredTypeReference type1 = edmType.AsStructured();
      ODataDeserializerContext readContext1 = new ODataDeserializerContext()
      {
        Path = readContext.Path,
        Model = readContext.Model
      };
      Type type2;
      if (readContext.IsUntyped)
      {
        type2 = type1.IsEntity() ? typeof (EdmEntityObject) : typeof (EdmComplexObject);
      }
      else
      {
        type2 = EdmLibHelpers.GetClrType((IEdmTypeReference) type1, readContext.Model);
        if (type2 == (Type) null)
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.MappingDoesNotContainResourceType, (object) type1.FullName()));
      }
      ODataDeserializerContext deserializerContext = readContext1;
      Type type3;
      if (!readContext.IsDeltaOfT)
        type3 = type2;
      else
        type3 = typeof (Delta<>).MakeGenericType(type2);
      deserializerContext.ResourceType = type3;
      return typeDeserializer.ReadInline((object) resourceWrapper, edmType, readContext1);
    }

    private void ApplyResourceSetInNestedProperty(
      IEdmProperty nestedProperty,
      object resource,
      ODataResourceSetWrapper resourceSetWrapper,
      ODataDeserializerContext readContext)
    {
      if (readContext.IsDeltaOfT && nestedProperty is IEdmNavigationProperty property)
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.CannotPatchNavigationProperties, (object) property.Name, (object) property.DeclaringEntityType().FullName()));
      object obj = this.ReadNestedResourceSetInline(resourceSetWrapper, nestedProperty.Type, readContext);
      string clrPropertyName = EdmLibHelpers.GetClrPropertyName(nestedProperty, readContext.Model);
      DeserializationHelpers.SetCollectionProperty(resource, nestedProperty, obj, clrPropertyName);
    }

    private void ApplyDynamicResourceSetInNestedProperty(
      string propertyName,
      object resource,
      IEdmStructuredTypeReference structuredType,
      ODataResourceSetWrapper resourceSetWrapper,
      ODataDeserializerContext readContext)
    {
      if (string.IsNullOrEmpty(resourceSetWrapper.ResourceSet.TypeName))
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.DynamicResourceSetTypeNameIsRequired, (object) propertyName));
      string collectionElementTypeName = DeserializationHelpers.GetCollectionElementTypeName(resourceSetWrapper.ResourceSet.TypeName, false);
      EdmCollectionTypeReference collectionTypeReference = new EdmCollectionTypeReference((IEdmCollectionType) new EdmCollectionType(readContext.Model.FindDeclaredType(collectionElementTypeName).ToEdmTypeReference(true)));
      if (this.DeserializerProvider.GetEdmTypeDeserializer((IEdmTypeReference) collectionTypeReference) == null)
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeCannotBeDeserialized, (object) collectionTypeReference.FullName()));
      IEnumerable enumerable = this.ReadNestedResourceSetInline(resourceSetWrapper, (IEdmTypeReference) collectionTypeReference, readContext) as IEnumerable;
      object propertyValue = (object) enumerable;
      if (enumerable != null && readContext.IsUntyped)
        propertyValue = (object) enumerable.ConvertToEdmObject((IEdmCollectionTypeReference) collectionTypeReference);
      DeserializationHelpers.SetDynamicProperty(resource, structuredType, EdmTypeKind.Collection, propertyName, propertyValue, (IEdmTypeReference) collectionTypeReference, readContext.Model);
    }

    private object ReadNestedResourceSetInline(
      ODataResourceSetWrapper resourceSetWrapper,
      IEdmTypeReference edmType,
      ODataDeserializerContext readContext)
    {
      ODataEdmTypeDeserializer typeDeserializer = this.DeserializerProvider.GetEdmTypeDeserializer(edmType);
      if (typeDeserializer == null)
        throw new SerializationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeCannotBeDeserialized, (object) edmType.FullName()));
      IEdmStructuredTypeReference type = edmType.AsCollection().ElementType().AsStructured();
      ODataDeserializerContext readContext1 = new ODataDeserializerContext()
      {
        Path = readContext.Path,
        Model = readContext.Model
      };
      if (readContext.IsUntyped)
      {
        readContext1.ResourceType = !type.IsEntity() ? typeof (EdmComplexObjectCollection) : typeof (EdmEntityObjectCollection);
      }
      else
      {
        Type clrType = EdmLibHelpers.GetClrType((IEdmTypeReference) type, readContext.Model);
        if (clrType == (Type) null)
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.MappingDoesNotContainResourceType, (object) type.FullName()));
        readContext1.ResourceType = typeof (List<>).MakeGenericType(clrType);
      }
      return typeDeserializer.ReadInline((object) resourceSetWrapper, edmType, readContext1);
    }
  }
}
