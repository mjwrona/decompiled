// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmNavigationProperty
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  public sealed class EdmNavigationProperty : 
    EdmProperty,
    IEdmNavigationProperty,
    IEdmProperty,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    private readonly IEdmReferentialConstraint referentialConstraint;
    private readonly bool containsTarget;
    private readonly EdmOnDeleteAction onDelete;
    private IEdmNavigationProperty partner;

    private EdmNavigationProperty(
      IEdmStructuredType declaringType,
      string name,
      IEdmTypeReference type,
      IEnumerable<IEdmStructuralProperty> dependentProperties,
      IEnumerable<IEdmStructuralProperty> principalProperties,
      bool containsTarget,
      EdmOnDeleteAction onDelete)
      : base(declaringType, name, type)
    {
      this.containsTarget = containsTarget;
      this.onDelete = onDelete;
      if (dependentProperties == null)
        return;
      this.referentialConstraint = (IEdmReferentialConstraint) EdmReferentialConstraint.Create(dependentProperties, principalProperties);
    }

    public override EdmPropertyKind PropertyKind => EdmPropertyKind.Navigation;

    public bool ContainsTarget => this.containsTarget;

    public IEdmReferentialConstraint ReferentialConstraint => this.referentialConstraint;

    public EdmOnDeleteAction OnDelete => this.onDelete;

    public IEdmNavigationProperty Partner => this.partner;

    internal IEdmPathExpression PartnerPath { get; private set; }

    public static EdmNavigationProperty CreateNavigationProperty(
      IEdmStructuredType declaringType,
      EdmNavigationPropertyInfo propertyInfo)
    {
      EdmUtil.CheckArgumentNull<EdmNavigationPropertyInfo>(propertyInfo, nameof (propertyInfo));
      EdmUtil.CheckArgumentNull<string>(propertyInfo.Name, "propertyInfo.Name");
      EdmUtil.CheckArgumentNull<IEdmEntityType>(propertyInfo.Target, "propertyInfo.Target");
      return new EdmNavigationProperty(declaringType, propertyInfo.Name, EdmNavigationProperty.CreateNavigationPropertyType(propertyInfo.Target, propertyInfo.TargetMultiplicity, "propertyInfo.TargetMultiplicity"), propertyInfo.DependentProperties, propertyInfo.PrincipalProperties, propertyInfo.ContainsTarget, propertyInfo.OnDelete);
    }

    public static EdmNavigationProperty CreateNavigationPropertyWithPartner(
      EdmNavigationPropertyInfo propertyInfo,
      EdmNavigationPropertyInfo partnerInfo)
    {
      EdmUtil.CheckArgumentNull<EdmNavigationPropertyInfo>(propertyInfo, nameof (propertyInfo));
      EdmUtil.CheckArgumentNull<string>(propertyInfo.Name, "propertyInfo.Name");
      EdmUtil.CheckArgumentNull<IEdmEntityType>(propertyInfo.Target, "propertyInfo.Target");
      EdmUtil.CheckArgumentNull<EdmNavigationPropertyInfo>(partnerInfo, nameof (partnerInfo));
      EdmUtil.CheckArgumentNull<string>(partnerInfo.Name, "partnerInfo.Name");
      EdmUtil.CheckArgumentNull<IEdmEntityType>(partnerInfo.Target, "partnerInfo.Target");
      EdmNavigationProperty propertyWithPartner = new EdmNavigationProperty((IEdmStructuredType) partnerInfo.Target, propertyInfo.Name, EdmNavigationProperty.CreateNavigationPropertyType(propertyInfo.Target, propertyInfo.TargetMultiplicity, "propertyInfo.TargetMultiplicity"), propertyInfo.DependentProperties, propertyInfo.PrincipalProperties, propertyInfo.ContainsTarget, propertyInfo.OnDelete);
      EdmNavigationProperty navigationProperty = new EdmNavigationProperty((IEdmStructuredType) propertyInfo.Target, partnerInfo.Name, EdmNavigationProperty.CreateNavigationPropertyType(partnerInfo.Target, partnerInfo.TargetMultiplicity, "partnerInfo.TargetMultiplicity"), partnerInfo.DependentProperties, partnerInfo.PrincipalProperties, partnerInfo.ContainsTarget, partnerInfo.OnDelete);
      propertyWithPartner.SetPartner((IEdmNavigationProperty) navigationProperty, (IEdmPathExpression) new EdmPathExpression(navigationProperty.Name));
      navigationProperty.SetPartner((IEdmNavigationProperty) propertyWithPartner, (IEdmPathExpression) new EdmPathExpression(propertyWithPartner.Name));
      return propertyWithPartner;
    }

    public static EdmNavigationProperty CreateNavigationPropertyWithPartner(
      string propertyName,
      IEdmTypeReference propertyType,
      IEnumerable<IEdmStructuralProperty> dependentProperties,
      IEnumerable<IEdmStructuralProperty> principalProperties,
      bool containsTarget,
      EdmOnDeleteAction onDelete,
      string partnerPropertyName,
      IEdmTypeReference partnerPropertyType,
      IEnumerable<IEdmStructuralProperty> partnerDependentProperties,
      IEnumerable<IEdmStructuralProperty> partnerPrincipalProperties,
      bool partnerContainsTarget,
      EdmOnDeleteAction partnerOnDelete)
    {
      EdmUtil.CheckArgumentNull<string>(propertyName, nameof (propertyName));
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(propertyType, nameof (propertyType));
      EdmUtil.CheckArgumentNull<string>(partnerPropertyName, nameof (partnerPropertyName));
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(partnerPropertyType, nameof (partnerPropertyType));
      IEdmEntityType entityType1 = EdmNavigationProperty.GetEntityType(partnerPropertyType);
      if (entityType1 == null)
        throw new ArgumentException(Strings.Constructable_EntityTypeOrCollectionOfEntityTypeExpected, nameof (partnerPropertyType));
      IEdmEntityType entityType2 = EdmNavigationProperty.GetEntityType(propertyType);
      if (entityType2 == null)
        throw new ArgumentException(Strings.Constructable_EntityTypeOrCollectionOfEntityTypeExpected, nameof (propertyType));
      EdmNavigationProperty propertyWithPartner = new EdmNavigationProperty((IEdmStructuredType) entityType1, propertyName, propertyType, dependentProperties, principalProperties, containsTarget, onDelete);
      EdmNavigationProperty navigationProperty = new EdmNavigationProperty((IEdmStructuredType) entityType2, partnerPropertyName, partnerPropertyType, partnerDependentProperties, partnerPrincipalProperties, partnerContainsTarget, partnerOnDelete);
      propertyWithPartner.SetPartner((IEdmNavigationProperty) navigationProperty, (IEdmPathExpression) new EdmPathExpression(navigationProperty.Name));
      navigationProperty.SetPartner((IEdmNavigationProperty) propertyWithPartner, (IEdmPathExpression) new EdmPathExpression(propertyWithPartner.Name));
      return propertyWithPartner;
    }

    internal void SetPartner(
      IEdmNavigationProperty navigationProperty,
      IEdmPathExpression navigationPropertyPath)
    {
      this.partner = navigationProperty;
      this.PartnerPath = navigationPropertyPath;
    }

    private static IEdmEntityType GetEntityType(IEdmTypeReference type)
    {
      IEdmEntityType entityType = (IEdmEntityType) null;
      if (type.IsEntity())
        entityType = (IEdmEntityType) type.Definition;
      else if (type.IsCollection())
      {
        type = ((IEdmCollectionType) type.Definition).ElementType;
        if (type.IsEntity())
          entityType = (IEdmEntityType) type.Definition;
      }
      return entityType;
    }

    private static IEdmTypeReference CreateNavigationPropertyType(
      IEdmEntityType entityType,
      EdmMultiplicity multiplicity,
      string multiplicityParameterName)
    {
      switch (multiplicity)
      {
        case EdmMultiplicity.ZeroOrOne:
          return (IEdmTypeReference) new EdmEntityTypeReference(entityType, true);
        case EdmMultiplicity.One:
          return (IEdmTypeReference) new EdmEntityTypeReference(entityType, false);
        case EdmMultiplicity.Many:
          return (IEdmTypeReference) EdmCoreModel.GetCollection((IEdmTypeReference) new EdmEntityTypeReference(entityType, false));
        default:
          throw new ArgumentOutOfRangeException(multiplicityParameterName, Strings.UnknownEnumVal_Multiplicity((object) multiplicity));
      }
    }
  }
}
