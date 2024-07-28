// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsNavigationProperty
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsNavigationProperty : 
    CsdlSemanticsElement,
    IEdmNavigationProperty,
    IEdmProperty,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmCheckable
  {
    private readonly CsdlNavigationProperty navigationProperty;
    private readonly CsdlSemanticsStructuredTypeDefinition declaringType;
    private readonly Cache<CsdlSemanticsNavigationProperty, IEdmTypeReference> typeCache = new Cache<CsdlSemanticsNavigationProperty, IEdmTypeReference>();
    private static readonly Func<CsdlSemanticsNavigationProperty, IEdmTypeReference> ComputeTypeFunc = (Func<CsdlSemanticsNavigationProperty, IEdmTypeReference>) (me => me.ComputeType());
    private readonly Cache<CsdlSemanticsNavigationProperty, IEdmNavigationProperty> partnerCache = new Cache<CsdlSemanticsNavigationProperty, IEdmNavigationProperty>();
    private static readonly Func<CsdlSemanticsNavigationProperty, IEdmNavigationProperty> ComputePartnerFunc = (Func<CsdlSemanticsNavigationProperty, IEdmNavigationProperty>) (me => me.ComputePartner());
    private readonly Cache<CsdlSemanticsNavigationProperty, IEdmReferentialConstraint> referentialConstraintCache = new Cache<CsdlSemanticsNavigationProperty, IEdmReferentialConstraint>();
    private static readonly Func<CsdlSemanticsNavigationProperty, IEdmReferentialConstraint> ComputeReferentialConstraintFunc = (Func<CsdlSemanticsNavigationProperty, IEdmReferentialConstraint>) (me => me.ComputeReferentialConstraint());
    private readonly Cache<CsdlSemanticsNavigationProperty, IEdmEntityType> targetEntityTypeCache = new Cache<CsdlSemanticsNavigationProperty, IEdmEntityType>();
    private static readonly Func<CsdlSemanticsNavigationProperty, IEdmEntityType> ComputeTargetEntityTypeFunc = (Func<CsdlSemanticsNavigationProperty, IEdmEntityType>) (me => me.ComputeTargetEntityType());
    private readonly Cache<CsdlSemanticsNavigationProperty, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsNavigationProperty, IEnumerable<EdmError>>();
    private static readonly Func<CsdlSemanticsNavigationProperty, IEnumerable<EdmError>> ComputeErrorsFunc = (Func<CsdlSemanticsNavigationProperty, IEnumerable<EdmError>>) (me => me.ComputeErrors());

    public CsdlSemanticsNavigationProperty(
      CsdlSemanticsStructuredTypeDefinition declaringType,
      CsdlNavigationProperty navigationProperty)
      : base((CsdlElement) navigationProperty)
    {
      this.declaringType = declaringType;
      this.navigationProperty = navigationProperty;
    }

    public override CsdlSemanticsModel Model => this.declaringType.Model;

    public override CsdlElement Element => (CsdlElement) this.navigationProperty;

    public string Name => this.navigationProperty.Name;

    public EdmOnDeleteAction OnDelete => this.navigationProperty.OnDelete == null ? EdmOnDeleteAction.None : this.navigationProperty.OnDelete.Action;

    public IEdmStructuredType DeclaringType => (IEdmStructuredType) this.declaringType;

    public bool ContainsTarget => this.navigationProperty.ContainsTarget;

    public IEdmTypeReference Type => this.typeCache.GetValue(this, CsdlSemanticsNavigationProperty.ComputeTypeFunc, (Func<CsdlSemanticsNavigationProperty, IEdmTypeReference>) null);

    public EdmPropertyKind PropertyKind => EdmPropertyKind.Navigation;

    public IEdmNavigationProperty Partner => this.partnerCache.GetValue(this, CsdlSemanticsNavigationProperty.ComputePartnerFunc, (Func<CsdlSemanticsNavigationProperty, IEdmNavigationProperty>) (cycle => (IEdmNavigationProperty) null));

    public IEnumerable<EdmError> Errors => this.errorsCache.GetValue(this, CsdlSemanticsNavigationProperty.ComputeErrorsFunc, (Func<CsdlSemanticsNavigationProperty, IEnumerable<EdmError>>) null);

    public IEdmReferentialConstraint ReferentialConstraint => this.referentialConstraintCache.GetValue(this, CsdlSemanticsNavigationProperty.ComputeReferentialConstraintFunc, (Func<CsdlSemanticsNavigationProperty, IEdmReferentialConstraint>) null);

    private IEdmEntityType TargetEntityType => this.targetEntityTypeCache.GetValue(this, CsdlSemanticsNavigationProperty.ComputeTargetEntityTypeFunc, (Func<CsdlSemanticsNavigationProperty, IEdmEntityType>) null);

    internal static IEdmNavigationProperty ResolvePartnerPath(
      IEdmEntityType type,
      IEdmPathExpression path,
      IEdmModel model)
    {
      IEdmStructuredType otherType = (IEdmStructuredType) type;
      IEdmProperty edmProperty = (IEdmProperty) null;
      foreach (string pathSegment in path.PathSegments)
      {
        if (otherType == null)
          return (IEdmNavigationProperty) null;
        if (pathSegment.IndexOf('.') < 0)
        {
          edmProperty = otherType.FindProperty(pathSegment);
          if (edmProperty == null)
            return (IEdmNavigationProperty) null;
          otherType = edmProperty.Type.Definition.AsElementType() as IEdmStructuredType;
        }
        else
        {
          IEdmSchemaType declaredType = model.FindDeclaredType(pathSegment);
          if (declaredType == null || !declaredType.IsOrInheritsFrom((IEdmType) otherType))
            return (IEdmNavigationProperty) null;
          otherType = declaredType as IEdmStructuredType;
          edmProperty = (IEdmProperty) null;
        }
      }
      return edmProperty == null ? (IEdmNavigationProperty) null : edmProperty as IEdmNavigationProperty;
    }

    protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations() => this.Model.WrapInlineVocabularyAnnotations((CsdlSemanticsElement) this, this.declaringType.Context);

    private IEdmEntityType ComputeTargetEntityType()
    {
      IEdmType definition = this.Type.Definition;
      if (definition.TypeKind == EdmTypeKind.Collection)
        definition = ((IEdmCollectionType) definition).ElementType.Definition;
      return (IEdmEntityType) definition;
    }

    private IEdmNavigationProperty ComputePartner()
    {
      IEdmPathExpression partnerPath = this.navigationProperty.PartnerPath;
      IEdmEntityType targetEntityType = this.TargetEntityType;
      if (partnerPath != null)
        return CsdlSemanticsNavigationProperty.ResolvePartnerPath(targetEntityType, partnerPath, (IEdmModel) this.Model) ?? (IEdmNavigationProperty) new UnresolvedNavigationPropertyPath((IEdmStructuredType) targetEntityType, partnerPath.Path, this.Location);
      foreach (IEdmNavigationProperty navigationProperty in targetEntityType.NavigationProperties())
      {
        if (navigationProperty != this && navigationProperty.Partner == this)
          return navigationProperty;
      }
      return (IEdmNavigationProperty) null;
    }

    private IEdmTypeReference ComputeType()
    {
      string str = this.navigationProperty.Type;
      bool flag;
      if (str.StartsWith("Collection(", StringComparison.Ordinal) && str.EndsWith(")", StringComparison.Ordinal))
      {
        flag = true;
        str = str.Substring("Collection(".Length, str.Length - "Collection(".Length - 1);
      }
      else
        flag = false;
      if (!(this.declaringType.Context.FindType(str) is IEdmEntityType entityType))
        entityType = (IEdmEntityType) new UnresolvedEntityType(str, this.Location);
      bool isNullable = !flag && ((int) this.navigationProperty.Nullable ?? 1) != 0;
      IEdmEntityTypeReference elementType = (IEdmEntityTypeReference) new EdmEntityTypeReference(entityType, isNullable);
      return flag ? (IEdmTypeReference) new EdmCollectionTypeReference((IEdmCollectionType) new EdmCollectionType((IEdmTypeReference) elementType)) : (IEdmTypeReference) elementType;
    }

    private IEdmReferentialConstraint ComputeReferentialConstraint() => this.navigationProperty.ReferentialConstraints.Any<CsdlReferentialConstraint>() ? (IEdmReferentialConstraint) new EdmReferentialConstraint(this.navigationProperty.ReferentialConstraints.Select<CsdlReferentialConstraint, EdmReferentialConstraintPropertyPair>(new Func<CsdlReferentialConstraint, EdmReferentialConstraintPropertyPair>(this.ComputeReferentialConstraintPropertyPair))) : (IEdmReferentialConstraint) null;

    private EdmReferentialConstraintPropertyPair ComputeReferentialConstraintPropertyPair(
      CsdlReferentialConstraint csdlConstraint)
    {
      if (!(this.declaringType.FindProperty(csdlConstraint.PropertyName) is IEdmStructuralProperty structuralProperty1))
        structuralProperty1 = (IEdmStructuralProperty) new UnresolvedProperty((IEdmStructuredType) this.declaringType, csdlConstraint.PropertyName, csdlConstraint.Location);
      IEdmStructuralProperty dependentProperty = structuralProperty1;
      if (!(this.TargetEntityType.FindProperty(csdlConstraint.ReferencedPropertyName) is IEdmStructuralProperty structuralProperty2))
        structuralProperty2 = (IEdmStructuralProperty) new UnresolvedProperty((IEdmStructuredType) this.ToEntityType(), csdlConstraint.ReferencedPropertyName, csdlConstraint.Location);
      IEdmStructuralProperty principalProperty = structuralProperty2;
      return new EdmReferentialConstraintPropertyPair(dependentProperty, principalProperty);
    }

    private IEnumerable<EdmError> ComputeErrors()
    {
      List<EdmError> list = (List<EdmError>) null;
      if (this.Type.IsCollection() && this.navigationProperty.Nullable.HasValue)
        list = CsdlSemanticsElement.AllocateAndAdd<EdmError>(list, new EdmError(this.Location, EdmErrorCode.NavigationPropertyWithCollectionTypeCannotHaveNullableAttribute, Strings.CsdlParser_CannotSpecifyNullableAttributeForNavigationPropertyWithCollectionType));
      if (this.TargetEntityType is BadEntityType targetEntityType)
        list = CsdlSemanticsElement.AllocateAndAdd<EdmError>(list, targetEntityType.Errors);
      return (IEnumerable<EdmError>) list ?? Enumerable.Empty<EdmError>();
    }
  }
}
