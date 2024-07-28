// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Evaluation.ODataResourceMetadataContext
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Evaluation
{
  internal abstract class ODataResourceMetadataContext : IODataResourceMetadataContext
  {
    private static readonly KeyValuePair<string, object>[] EmptyProperties = new KeyValuePair<string, object>[0];
    private readonly ODataResourceBase resource;
    private readonly IODataResourceTypeContext typeContext;
    private KeyValuePair<string, object>[] keyProperties;
    private IEnumerable<KeyValuePair<string, object>> etagProperties;
    private IEnumerable<IEdmNavigationProperty> selectedNavigationProperties;
    private IDictionary<string, IEdmStructuralProperty> selectedStreamProperties;
    private IEnumerable<IEdmOperation> selectedBindableOperations;

    protected ODataResourceMetadataContext(
      ODataResourceBase resource,
      IODataResourceTypeContext typeContext)
    {
      this.resource = resource;
      this.typeContext = typeContext;
    }

    public ODataResourceBase Resource => this.resource;

    public IODataResourceTypeContext TypeContext => this.typeContext;

    public abstract string ActualResourceTypeName { get; }

    public abstract ICollection<KeyValuePair<string, object>> KeyProperties { get; }

    public abstract IEnumerable<KeyValuePair<string, object>> ETagProperties { get; }

    public abstract IEnumerable<IEdmNavigationProperty> SelectedNavigationProperties { get; }

    public abstract IDictionary<string, IEdmStructuralProperty> SelectedStreamProperties { get; }

    public abstract IEnumerable<IEdmOperation> SelectedBindableOperations { get; }

    internal static ODataResourceMetadataContext Create(
      ODataResourceBase resource,
      IODataResourceTypeContext typeContext,
      ODataResourceSerializationInfo serializationInfo,
      IEdmStructuredType actualResourceType,
      IODataMetadataContext metadataContext,
      SelectedPropertiesNode selectedProperties,
      ODataMetadataSelector metadataSelector)
    {
      return serializationInfo != null ? (ODataResourceMetadataContext) new ODataResourceMetadataContext.ODataResourceMetadataContextWithoutModel(resource, typeContext, serializationInfo) : (ODataResourceMetadataContext) new ODataResourceMetadataContext.ODataResourceMetadataContextWithModel(resource, typeContext, actualResourceType, metadataContext, selectedProperties, metadataSelector);
    }

    internal static KeyValuePair<string, object>[] GetKeyProperties(
      ODataResourceBase resource,
      ODataResourceSerializationInfo serializationInfo,
      IEdmEntityType actualEntityType)
    {
      KeyValuePair<string, object>[] keyProperties = (KeyValuePair<string, object>[]) null;
      string actualEntityTypeName = (string) null;
      if (serializationInfo != null)
      {
        actualEntityTypeName = !string.IsNullOrEmpty(resource.TypeName) ? resource.TypeName : throw new ODataException(Microsoft.OData.Strings.ODataResourceTypeContext_ODataResourceTypeNameMissing);
        keyProperties = ODataResourceMetadataContext.GetPropertiesBySerializationInfoPropertyKind(resource, ODataPropertyKind.Key, actualEntityTypeName);
      }
      else
      {
        actualEntityTypeName = actualEntityType.FullName();
        IEnumerable<IEdmStructuralProperty> source = actualEntityType.Key();
        if (source != null)
          keyProperties = source.Select<IEdmStructuralProperty, KeyValuePair<string, object>>((Func<IEdmStructuralProperty, KeyValuePair<string, object>>) (p => new KeyValuePair<string, object>(p.Name, ODataResourceMetadataContext.GetPrimitiveOrEnumPropertyValue(resource, p.Name, actualEntityTypeName, false)))).ToArray<KeyValuePair<string, object>>();
      }
      ODataResourceMetadataContext.ValidateEntityTypeHasKeyProperties(keyProperties, actualEntityTypeName);
      return keyProperties;
    }

    private static object GetPrimitiveOrEnumPropertyValue(
      ODataResourceBase resource,
      string propertyName,
      string entityTypeName,
      bool isKeyProperty)
    {
      return ODataResourceMetadataContext.GetPrimitiveOrEnumPropertyValue(entityTypeName, (resource.NonComputedProperties == null ? (ODataProperty) null : resource.NonComputedProperties.SingleOrDefault<ODataProperty>((Func<ODataProperty, bool>) (p => p.Name == propertyName))) ?? throw new ODataException(Microsoft.OData.Strings.EdmValueUtils_PropertyDoesntExist((object) entityTypeName, (object) propertyName)), isKeyProperty);
    }

    private static object GetPrimitiveOrEnumPropertyValue(
      string entityTypeName,
      ODataProperty property,
      bool isKeyProperty)
    {
      object obj = property.Value;
      if (obj == null & isKeyProperty)
        throw new ODataException(Microsoft.OData.Strings.ODataResourceMetadataContext_NullKeyValue((object) property.Name, (object) entityTypeName));
      return !(obj is ODataValue) || obj is ODataEnumValue ? obj : throw new ODataException(Microsoft.OData.Strings.ODataResourceMetadataContext_KeyOrETagValuesMustBePrimitiveValues((object) property.Name, (object) entityTypeName));
    }

    private static void ValidateEntityTypeHasKeyProperties(
      KeyValuePair<string, object>[] keyProperties,
      string actualEntityTypeName)
    {
      if (keyProperties == null || keyProperties.Length == 0)
        throw new ODataException(Microsoft.OData.Strings.ODataResourceMetadataContext_EntityTypeWithNoKeyProperties((object) actualEntityTypeName));
    }

    private static KeyValuePair<string, object>[] GetPropertiesBySerializationInfoPropertyKind(
      ODataResourceBase resource,
      ODataPropertyKind propertyKind,
      string actualEntityTypeName)
    {
      KeyValuePair<string, object>[] infoPropertyKind = ODataResourceMetadataContext.EmptyProperties;
      if (resource.NonComputedProperties != null)
        infoPropertyKind = resource.NonComputedProperties.Where<ODataProperty>((Func<ODataProperty, bool>) (p => p.SerializationInfo != null && p.SerializationInfo.PropertyKind == propertyKind)).Select<ODataProperty, KeyValuePair<string, object>>((Func<ODataProperty, KeyValuePair<string, object>>) (p => new KeyValuePair<string, object>(p.Name, ODataResourceMetadataContext.GetPrimitiveOrEnumPropertyValue(actualEntityTypeName, p, propertyKind == ODataPropertyKind.Key)))).ToArray<KeyValuePair<string, object>>();
      return infoPropertyKind;
    }

    private sealed class ODataResourceMetadataContextWithoutModel : ODataResourceMetadataContext
    {
      private static readonly IEdmNavigationProperty[] EmptyNavigationProperties = new IEdmNavigationProperty[0];
      private static readonly Dictionary<string, IEdmStructuralProperty> EmptyStreamProperties = new Dictionary<string, IEdmStructuralProperty>((IEqualityComparer<string>) StringComparer.Ordinal);
      private static readonly IEdmOperation[] EmptyOperations = new IEdmOperation[0];
      private readonly ODataResourceSerializationInfo serializationInfo;

      internal ODataResourceMetadataContextWithoutModel(
        ODataResourceBase resource,
        IODataResourceTypeContext typeContext,
        ODataResourceSerializationInfo serializationInfo)
        : base(resource, typeContext)
      {
        this.serializationInfo = serializationInfo;
      }

      public override ICollection<KeyValuePair<string, object>> KeyProperties
      {
        get
        {
          if (this.keyProperties == null)
          {
            this.keyProperties = ODataResourceMetadataContext.GetPropertiesBySerializationInfoPropertyKind(this.resource, ODataPropertyKind.Key, this.ActualResourceTypeName);
            ODataResourceMetadataContext.ValidateEntityTypeHasKeyProperties(this.keyProperties, this.ActualResourceTypeName);
          }
          return (ICollection<KeyValuePair<string, object>>) this.keyProperties;
        }
      }

      public override IEnumerable<KeyValuePair<string, object>> ETagProperties => this.etagProperties ?? (this.etagProperties = (IEnumerable<KeyValuePair<string, object>>) ODataResourceMetadataContext.GetPropertiesBySerializationInfoPropertyKind(this.resource, ODataPropertyKind.ETag, this.ActualResourceTypeName));

      public override string ActualResourceTypeName => !string.IsNullOrEmpty(this.Resource.TypeName) ? this.Resource.TypeName : throw new ODataException(Microsoft.OData.Strings.ODataResourceTypeContext_ODataResourceTypeNameMissing);

      public override IEnumerable<IEdmNavigationProperty> SelectedNavigationProperties => (IEnumerable<IEdmNavigationProperty>) ODataResourceMetadataContext.ODataResourceMetadataContextWithoutModel.EmptyNavigationProperties;

      public override IDictionary<string, IEdmStructuralProperty> SelectedStreamProperties => (IDictionary<string, IEdmStructuralProperty>) ODataResourceMetadataContext.ODataResourceMetadataContextWithoutModel.EmptyStreamProperties;

      public override IEnumerable<IEdmOperation> SelectedBindableOperations => (IEnumerable<IEdmOperation>) ODataResourceMetadataContext.ODataResourceMetadataContextWithoutModel.EmptyOperations;
    }

    private sealed class ODataResourceMetadataContextWithModel : ODataResourceMetadataContext
    {
      private readonly IEdmStructuredType actualResourceType;
      private readonly IODataMetadataContext metadataContext;
      private readonly SelectedPropertiesNode selectedProperties;
      private readonly ODataMetadataSelector metadataSelector;

      internal ODataResourceMetadataContextWithModel(
        ODataResourceBase resource,
        IODataResourceTypeContext typeContext,
        IEdmStructuredType actualResourceType,
        IODataMetadataContext metadataContext,
        SelectedPropertiesNode selectedProperties,
        ODataMetadataSelector metadataSelector)
        : base(resource, typeContext)
      {
        this.actualResourceType = actualResourceType;
        this.metadataContext = metadataContext;
        this.selectedProperties = selectedProperties;
        this.metadataSelector = metadataSelector;
      }

      public override ICollection<KeyValuePair<string, object>> KeyProperties
      {
        get
        {
          if (this.keyProperties == null)
          {
            if (this.actualResourceType is IEdmEntityType actualResourceType)
            {
              IEnumerable<IEdmStructuralProperty> source = actualResourceType.Key();
              if (source != null)
                this.keyProperties = source.Select<IEdmStructuralProperty, KeyValuePair<string, object>>((Func<IEdmStructuralProperty, KeyValuePair<string, object>>) (p => new KeyValuePair<string, object>(p.Name, ODataResourceMetadataContext.GetPrimitiveOrEnumPropertyValue(this.resource, p.Name, this.ActualResourceTypeName, true)))).ToArray<KeyValuePair<string, object>>();
              ODataResourceMetadataContext.ValidateEntityTypeHasKeyProperties(this.keyProperties, this.ActualResourceTypeName);
            }
            else
              this.keyProperties = Enumerable.Empty<KeyValuePair<string, object>>().ToArray<KeyValuePair<string, object>>();
          }
          return (ICollection<KeyValuePair<string, object>>) this.keyProperties;
        }
      }

      public override IEnumerable<KeyValuePair<string, object>> ETagProperties
      {
        get
        {
          if (this.etagProperties == null)
          {
            IEnumerable<IEdmStructuralProperty> propertiesFromAnnotation = this.ComputeETagPropertiesFromAnnotation();
            this.etagProperties = propertiesFromAnnotation.Any<IEdmStructuralProperty>() ? (IEnumerable<KeyValuePair<string, object>>) propertiesFromAnnotation.Select<IEdmStructuralProperty, KeyValuePair<string, object>>((Func<IEdmStructuralProperty, KeyValuePair<string, object>>) (p => new KeyValuePair<string, object>(p.Name, ODataResourceMetadataContext.GetPrimitiveOrEnumPropertyValue(this.resource, p.Name, this.ActualResourceTypeName, false)))).ToArray<KeyValuePair<string, object>>() : (IEnumerable<KeyValuePair<string, object>>) ODataResourceMetadataContext.EmptyProperties;
          }
          return this.etagProperties;
        }
      }

      public override string ActualResourceTypeName => this.actualResourceType.FullTypeName();

      public override IEnumerable<IEdmNavigationProperty> SelectedNavigationProperties
      {
        get
        {
          if (this.selectedNavigationProperties == null)
          {
            this.selectedNavigationProperties = this.selectedProperties.GetSelectedNavigationProperties(this.actualResourceType);
            if (this.metadataSelector != null)
              this.selectedNavigationProperties = this.metadataSelector.SelectNavigationProperties(this.actualResourceType, this.selectedNavigationProperties);
          }
          return this.selectedNavigationProperties;
        }
      }

      public override IDictionary<string, IEdmStructuralProperty> SelectedStreamProperties
      {
        get
        {
          if (this.selectedStreamProperties == null)
          {
            this.selectedStreamProperties = this.selectedProperties.GetSelectedStreamProperties(this.actualResourceType as IEdmEntityType);
            if (this.metadataSelector != null)
              this.selectedStreamProperties = (IDictionary<string, IEdmStructuralProperty>) this.metadataSelector.SelectStreamProperties(this.actualResourceType, (IEnumerable<IEdmStructuralProperty>) this.selectedStreamProperties.Values).ToDictionary<IEdmStructuralProperty, string>((Func<IEdmStructuralProperty, string>) (v => v.Name));
          }
          return this.selectedStreamProperties;
        }
      }

      public override IEnumerable<IEdmOperation> SelectedBindableOperations
      {
        get
        {
          if (this.selectedBindableOperations == null)
          {
            bool mustBeContainerQualified = this.metadataContext.OperationsBoundToStructuredTypeMustBeContainerQualified(this.actualResourceType);
            this.selectedBindableOperations = this.metadataContext.GetBindableOperationsForType((IEdmType) this.actualResourceType).Where<IEdmOperation>((Func<IEdmOperation, bool>) (operation => this.selectedProperties.IsOperationSelected(this.actualResourceType, operation, mustBeContainerQualified)));
            if (this.metadataSelector != null)
              this.selectedBindableOperations = this.metadataSelector.SelectBindableOperations(this.actualResourceType, this.selectedBindableOperations);
          }
          return this.selectedBindableOperations;
        }
      }

      private IEnumerable<IEdmStructuralProperty> ComputeETagPropertiesFromAnnotation()
      {
        ODataResourceMetadataContext.ODataResourceMetadataContextWithModel contextWithModel = this;
        IEdmModel model = contextWithModel.metadataContext.Model;
        IEdmEntitySet declaredEntitySet = model.FindDeclaredEntitySet(contextWithModel.typeContext.NavigationSourceName);
        if (declaredEntitySet != null)
        {
          IEdmVocabularyAnnotation vocabularyAnnotation = model.FindDeclaredVocabularyAnnotations((IEdmVocabularyAnnotatable) declaredEntitySet).SingleOrDefault<IEdmVocabularyAnnotation>((Func<IEdmVocabularyAnnotation, bool>) (t => t.Term.FullName().Equals("Org.OData.Core.V1.OptimisticConcurrency", StringComparison.Ordinal)));
          if (vocabularyAnnotation != null)
          {
            IEdmExpression edmExpression = vocabularyAnnotation.Value;
            if (edmExpression is IEdmCollectionExpression)
            {
              foreach (IEdmPathExpression edmPathExpression in (edmExpression as IEdmCollectionExpression).Elements.Where<IEdmExpression>((Func<IEdmExpression, bool>) (p => p is IEdmPathExpression)))
              {
                IEdmPathExpression pathExpression = edmPathExpression;
                IEdmStructuralProperty structuralProperty = contextWithModel.actualResourceType.StructuralProperties().FirstOrDefault<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (p => p.Name == pathExpression.PathSegments.LastOrDefault<string>()));
                if (structuralProperty == null)
                  throw new ODataException(Microsoft.OData.Strings.EdmValueUtils_PropertyDoesntExist((object) contextWithModel.ActualResourceTypeName, (object) pathExpression.PathSegments.LastOrDefault<string>()));
                yield return structuralProperty;
              }
            }
          }
        }
      }
    }
  }
}
