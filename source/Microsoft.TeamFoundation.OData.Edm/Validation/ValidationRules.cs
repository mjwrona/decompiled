// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Validation.ValidationRules
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.OData.Edm.Validation
{
  public static class ValidationRules
  {
    public static readonly ValidationRule<IEdmElement> ElementDirectValueAnnotationFullNameMustBeUnique = new ValidationRule<IEdmElement>((Action<ValidationContext, IEdmElement>) ((context, item) =>
    {
      HashSetInternal<string> hashSetInternal = new HashSetInternal<string>();
      foreach (IEdmDirectValueAnnotation directValueAnnotation in context.Model.DirectValueAnnotationsManager.GetDirectValueAnnotations(item))
      {
        if (!hashSetInternal.Add(directValueAnnotation.NamespaceUri + ":" + directValueAnnotation.Name))
          context.AddError(directValueAnnotation.Location(), EdmErrorCode.DuplicateDirectValueAnnotationFullName, Strings.EdmModel_Validator_Semantic_ElementDirectValueAnnotationFullNameMustBeUnique((object) directValueAnnotation.NamespaceUri, (object) directValueAnnotation.Name));
      }
    }));
    public static readonly ValidationRule<IEdmNamedElement> NamedElementNameMustNotBeEmptyOrWhiteSpace = new ValidationRule<IEdmNamedElement>((Action<ValidationContext, IEdmNamedElement>) ((context, item) =>
    {
      if (!EdmUtil.IsNullOrWhiteSpaceInternal(item.Name) && item.Name.Length != 0)
        return;
      context.AddError(item.Location(), EdmErrorCode.InvalidName, Strings.EdmModel_Validator_Syntactic_MissingName);
    }));
    public static readonly ValidationRule<IEdmNamedElement> NamedElementNameIsTooLong = new ValidationRule<IEdmNamedElement>((Action<ValidationContext, IEdmNamedElement>) ((context, item) =>
    {
      if (EdmUtil.IsNullOrWhiteSpaceInternal(item.Name) || item.Name.Length <= 480)
        return;
      context.AddError(item.Location(), EdmErrorCode.NameTooLong, Strings.EdmModel_Validator_Syntactic_EdmModel_NameIsTooLong((object) item.Name));
    }));
    public static readonly ValidationRule<IEdmNamedElement> NamedElementNameIsNotAllowed = new ValidationRule<IEdmNamedElement>((Action<ValidationContext, IEdmNamedElement>) ((context, item) =>
    {
      if (item is IEdmDirectValueAnnotation || EdmUtil.IsNullOrWhiteSpaceInternal(item.Name) || item.Name.Length > 480 || item.Name.Length <= 0 || EdmUtil.IsValidUndottedName(item.Name))
        return;
      context.AddError(item.Location(), EdmErrorCode.InvalidName, Strings.EdmModel_Validator_Syntactic_EdmModel_NameIsNotAllowed((object) item.Name));
    }));
    public static readonly ValidationRule<IEdmSchemaElement> SchemaElementNamespaceMustNotBeEmptyOrWhiteSpace = new ValidationRule<IEdmSchemaElement>((Action<ValidationContext, IEdmSchemaElement>) ((context, item) =>
    {
      if (!EdmUtil.IsNullOrWhiteSpaceInternal(item.Namespace) && item.Namespace.Length != 0)
        return;
      context.AddError(item.Location(), EdmErrorCode.InvalidNamespaceName, Strings.EdmModel_Validator_Syntactic_MissingNamespaceName);
    }));
    public static readonly ValidationRule<IEdmSchemaElement> SchemaElementNamespaceIsTooLong = new ValidationRule<IEdmSchemaElement>((Action<ValidationContext, IEdmSchemaElement>) ((context, item) =>
    {
      if (item.Namespace.Length <= 512)
        return;
      context.AddError(item.Location(), EdmErrorCode.InvalidNamespaceName, Strings.EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsTooLong((object) item.Namespace));
    }));
    public static readonly ValidationRule<IEdmSchemaElement> SchemaElementNamespaceIsNotAllowed = new ValidationRule<IEdmSchemaElement>((Action<ValidationContext, IEdmSchemaElement>) ((context, item) =>
    {
      if (item.Namespace.Length > 512 || item.Namespace.Length <= 0 || EdmUtil.IsNullOrWhiteSpaceInternal(item.Namespace) || EdmUtil.IsValidDottedName(item.Namespace))
        return;
      context.AddError(item.Location(), EdmErrorCode.InvalidNamespaceName, Strings.EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsNotAllowed((object) item.Namespace));
    }));
    public static readonly ValidationRule<IEdmSchemaElement> SchemaElementSystemNamespaceEncountered = new ValidationRule<IEdmSchemaElement>((Action<ValidationContext, IEdmSchemaElement>) ((context, element) =>
    {
      if (!ValidationHelper.IsEdmSystemNamespace(element.Namespace))
        return;
      context.AddError(element.Location(), EdmErrorCode.SystemNamespaceEncountered, Strings.EdmModel_Validator_Semantic_SystemNamespaceEncountered((object) element.Namespace));
    }));
    public static readonly ValidationRule<IEdmSchemaElement> SchemaElementMustNotHaveKindOfNone = new ValidationRule<IEdmSchemaElement>((Action<ValidationContext, IEdmSchemaElement>) ((context, element) =>
    {
      if (element.SchemaElementKind != EdmSchemaElementKind.None || context.IsBad((IEdmElement) element))
        return;
      context.AddError(element.Location(), EdmErrorCode.SchemaElementMustNotHaveKindOfNone, Strings.EdmModel_Validator_Semantic_SchemaElementMustNotHaveKindOfNone((object) element.FullName()));
    }));
    public static readonly ValidationRule<IEdmEntityContainerElement> EntityContainerElementMustNotHaveKindOfNone = new ValidationRule<IEdmEntityContainerElement>((Action<ValidationContext, IEdmEntityContainerElement>) ((context, element) =>
    {
      if (element.ContainerElementKind != EdmContainerElementKind.None || context.IsBad((IEdmElement) element))
        return;
      context.AddError(element.Location(), EdmErrorCode.EntityContainerElementMustNotHaveKindOfNone, Strings.EdmModel_Validator_Semantic_EntityContainerElementMustNotHaveKindOfNone((object) (element.Container.FullName() + "/" + element.Name)));
    }));
    public static readonly ValidationRule<IEdmEntityContainer> EntityContainerDuplicateEntityContainerMemberName = new ValidationRule<IEdmEntityContainer>((Action<ValidationContext, IEdmEntityContainer>) ((context, entityContainer) =>
    {
      HashSetInternal<string> hashSetInternal1 = new HashSetInternal<string>();
      HashSetInternal<string> hashSetInternal2 = new HashSetInternal<string>();
      HashSetInternal<string> hashSetInternal3 = new HashSetInternal<string>();
      foreach (IEdmEntityContainerElement element in entityContainer.Elements)
      {
        bool flag = false;
        if (element is IEdmOperationImport edmOperationImport2)
        {
          if (!hashSetInternal3.Contains(edmOperationImport2.Name))
            hashSetInternal3.Add(edmOperationImport2.Name);
          string thingToAdd = edmOperationImport2.Name + "_" + edmOperationImport2.Operation.GetHashCode().ToString((IFormatProvider) CultureInfo.InvariantCulture);
          if (hashSetInternal2.Contains(thingToAdd))
            flag = true;
          else
            hashSetInternal2.Add(thingToAdd);
          if (hashSetInternal1.Contains(edmOperationImport2.Name))
            flag = true;
        }
        else
        {
          if (hashSetInternal1.Contains(element.Name))
            flag = true;
          else
            hashSetInternal1.Add(element.Name);
          if (hashSetInternal3.Contains(element.Name))
            flag = true;
        }
        if (flag)
          context.AddError(element.Location(), EdmErrorCode.DuplicateEntityContainerMemberName, Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName((object) element.Name));
      }
    }));
    public static readonly ValidationRule<IEdmNavigationSource> NavigationSourceTypeHasNoKeys = new ValidationRule<IEdmNavigationSource>((Action<ValidationContext, IEdmNavigationSource>) ((context, navigationSource) =>
    {
      if (navigationSource.NavigationSourceKind() == EdmNavigationSourceKind.Singleton || navigationSource.NavigationSourceKind() == EdmNavigationSourceKind.None || navigationSource.EntityType() == null || navigationSource.EntityType().Key() != null && navigationSource.EntityType().Key().Any<IEdmStructuralProperty>() || context.IsBad((IEdmElement) navigationSource.EntityType()))
        return;
      string errorMessage = Strings.EdmModel_Validator_Semantic_NavigationSourceTypeHasNoKeys((object) navigationSource.Name, (object) navigationSource.EntityType().Name);
      context.AddError(navigationSource.Location(), EdmErrorCode.NavigationSourceTypeHasNoKeys, errorMessage);
    }));
    public static readonly ValidationRule<IEdmNavigationSource> NavigationSourceDeclaringTypeCannotHavePathTypeProperty = new ValidationRule<IEdmNavigationSource>((Action<ValidationContext, IEdmNavigationSource>) ((context, navigationSource) =>
    {
      IEdmEntityType edmEntityType = navigationSource.EntityType();
      if (edmEntityType == null)
        return;
      IList<IEdmStructuredType> visited = (IList<IEdmStructuredType>) new List<IEdmStructuredType>();
      if (!ValidationRules.HasPathTypeProperty((IEdmStructuredType) edmEntityType, visited))
        return;
      string p1 = navigationSource is IEdmSingleton ? "singleton" : "entity set";
      string errorMessage = Strings.EdmModel_Validator_Semantic_DeclaringTypeOfNavigationSourceCannotHavePathProperty((object) edmEntityType.FullName(), (object) p1, (object) navigationSource.Name);
      context.AddError(navigationSource.Location(), EdmErrorCode.DeclaringTypeOfNavigationSourceCannotHavePathProperty, errorMessage);
    }));
    public static readonly ValidationRule<IEdmNavigationSource> NavigationSourceInaccessibleEntityType = new ValidationRule<IEdmNavigationSource>((Action<ValidationContext, IEdmNavigationSource>) ((context, navigationSource) =>
    {
      IEdmEntityType edmEntityType = navigationSource.EntityType();
      if (edmEntityType == null || context.IsBad((IEdmElement) edmEntityType))
        return;
      ValidationRules.CheckForUnreacheableTypeError(context, (IEdmSchemaType) edmEntityType, navigationSource.Location());
    }));
    public static readonly ValidationRule<IEdmNavigationSource> NavigationPropertyMappingsMustBeUnique = new ValidationRule<IEdmNavigationSource>((Action<ValidationContext, IEdmNavigationSource>) ((context, navigationSource) =>
    {
      HashSetInternal<KeyValuePair<IEdmNavigationProperty, string>> hashSetInternal = new HashSetInternal<KeyValuePair<IEdmNavigationProperty, string>>();
      foreach (IEdmNavigationPropertyBinding navigationPropertyBinding in navigationSource.NavigationPropertyBindings)
      {
        if (!hashSetInternal.Add(new KeyValuePair<IEdmNavigationProperty, string>(navigationPropertyBinding.NavigationProperty, navigationPropertyBinding.Path.Path)))
          context.AddError(navigationSource.Location(), EdmErrorCode.DuplicateNavigationPropertyMapping, Strings.EdmModel_Validator_Semantic_DuplicateNavigationPropertyMapping((object) navigationSource.Name, (object) navigationPropertyBinding.NavigationProperty.Name));
      }
    }));
    public static readonly ValidationRule<IEdmNavigationSource> NavigationPropertyMappingMustPointToValidTargetForProperty = new ValidationRule<IEdmNavigationSource>((Action<ValidationContext, IEdmNavigationSource>) ((context, navigationSource) =>
    {
      foreach (IEdmNavigationPropertyBinding navigationPropertyBinding in navigationSource.NavigationPropertyBindings)
      {
        if (!navigationPropertyBinding.NavigationProperty.IsBad() && !navigationPropertyBinding.Target.IsBad())
        {
          if (!navigationPropertyBinding.Target.EntityType().IsOrInheritsFrom((IEdmType) navigationPropertyBinding.NavigationProperty.ToEntityType()) && !navigationPropertyBinding.NavigationProperty.ToEntityType().IsOrInheritsFrom((IEdmType) navigationPropertyBinding.Target.EntityType()) && !context.IsBad((IEdmElement) navigationPropertyBinding.Target))
            context.AddError(navigationSource.Location(), EdmErrorCode.NavigationPropertyMappingMustPointToValidTargetForProperty, Strings.EdmModel_Validator_Semantic_NavigationPropertyMappingMustPointToValidTargetForProperty((object) navigationPropertyBinding.NavigationProperty.Name, (object) navigationPropertyBinding.Target.Name));
          if (navigationPropertyBinding.Target is IEdmSingleton && navigationPropertyBinding.NavigationProperty.Type.Definition.TypeKind == EdmTypeKind.Collection)
            context.AddError(navigationSource.Location(), EdmErrorCode.NavigationPropertyOfCollectionTypeMustNotTargetToSingleton, Strings.EdmModel_Validator_Semantic_NavigationPropertyOfCollectionTypeMustNotTargetToSingleton((object) navigationPropertyBinding.NavigationProperty.Name, (object) navigationPropertyBinding.Target.Name));
        }
      }
    }));
    public static readonly ValidationRule<IEdmNavigationSource> NavigationPropertyBindingPathMustBeResolvable = new ValidationRule<IEdmNavigationSource>((Action<ValidationContext, IEdmNavigationSource>) ((context, navigationSource) =>
    {
      foreach (IEdmNavigationPropertyBinding navigationPropertyBinding in navigationSource.NavigationPropertyBindings)
      {
        if (!navigationPropertyBinding.NavigationProperty.IsBad() && !navigationPropertyBinding.Target.IsBad() && !ValidationRules.TryResolveNavigationPropertyBindingPath(context.Model, navigationSource, navigationPropertyBinding))
          context.AddError(navigationSource.Location(), EdmErrorCode.UnresolvedNavigationPropertyBindingPath, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The binding path {0} for navigation property {1} under navigation source {2} is not valid.", new object[3]
          {
            (object) navigationPropertyBinding.Path.Path,
            (object) navigationPropertyBinding.NavigationProperty.Name,
            (object) navigationSource.Name
          }));
      }
    }));
    public static readonly ValidationRule<IEdmEntitySet> EntitySetCanOnlyBeContainedByASingleNavigationProperty = new ValidationRule<IEdmEntitySet>((Action<ValidationContext, IEdmEntitySet>) ((context, set) =>
    {
      bool flag = false;
      foreach (IEdmNavigationSource navigationSource in set.Container.Elements.OfType<IEdmNavigationSource>())
      {
        foreach (IEdmNavigationPropertyBinding navigationPropertyBinding in navigationSource.NavigationPropertyBindings)
        {
          IEdmNavigationProperty navigationProperty = navigationPropertyBinding.NavigationProperty;
          if (navigationPropertyBinding.Target == set && navigationProperty.ContainsTarget)
          {
            if (flag)
              context.AddError(set.Location(), EdmErrorCode.EntitySetCanOnlyBeContainedByASingleNavigationProperty, Strings.EdmModel_Validator_Semantic_EntitySetCanOnlyBeContainedByASingleNavigationProperty((object) (set.Container.FullName() + "." + set.Name)));
            flag = true;
          }
        }
      }
    }));
    public static readonly ValidationRule<IEdmNavigationSource> NavigationMappingMustBeBidirectional = new ValidationRule<IEdmNavigationSource>((Action<ValidationContext, IEdmNavigationSource>) ((context, navigationSource) =>
    {
      foreach (IEdmNavigationPropertyBinding navigationPropertyBinding in navigationSource.NavigationPropertyBindings)
      {
        IEdmNavigationProperty navigationProperty = navigationPropertyBinding.NavigationProperty;
        if (navigationProperty.Partner != null && !navigationProperty.IsBad())
        {
          IEdmNavigationSource navigationTarget = navigationPropertyBinding.Target.FindNavigationTarget(navigationProperty.Partner, (IEdmPathExpression) new EdmPathExpression(navigationProperty.Partner.Name));
          switch (navigationTarget)
          {
            case null:
            case IEdmUnknownEntitySet _:
            case IEdmContainedEntitySet _:
              continue;
            default:
              if (navigationTarget != navigationSource && navigationProperty.Partner.DeclaringEntityType().FindProperty(navigationProperty.Partner.Name) == navigationProperty.Partner)
              {
                context.AddError(navigationSource.Location(), EdmErrorCode.NavigationMappingMustBeBidirectional, Strings.EdmModel_Validator_Semantic_NavigationMappingMustBeBidirectional((object) navigationSource.Name, (object) navigationProperty.Name));
                continue;
              }
              continue;
          }
        }
      }
    }));
    public static readonly ValidationRule<IEdmEntitySet> EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet = new ValidationRule<IEdmEntitySet>((Action<ValidationContext, IEdmEntitySet>) ((context, set) =>
    {
      foreach (IEdmNavigationPropertyBinding navigationPropertyBinding in set.NavigationPropertyBindings)
      {
        if (navigationPropertyBinding.NavigationProperty.ContainsTarget && navigationPropertyBinding.NavigationProperty.DeclaringType.IsOrInheritsFrom((IEdmType) navigationPropertyBinding.NavigationProperty.ToEntityType()) && navigationPropertyBinding.Target != set)
          context.AddError(set.Location(), EdmErrorCode.EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet, Strings.EdmModel_Validator_Semantic_EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet((object) navigationPropertyBinding.NavigationProperty, (object) set.Name));
      }
    }));
    public static readonly ValidationRule<IEdmEntitySet> EntitySetTypeMustBeCollectionOfEntityType = new ValidationRule<IEdmEntitySet>((Action<ValidationContext, IEdmEntitySet>) ((context, entitySet) =>
    {
      bool flag = false;
      if (entitySet.Type is IEdmCollectionType type2)
        flag = type2.ElementType != null && type2.ElementType.Definition is IEdmEntityType;
      if (flag)
        return;
      string errorMessage = Strings.EdmModel_Validator_Semantic_EntitySetTypeMustBeCollectionOfEntityType((object) entitySet.Type.FullTypeName(), (object) entitySet.Name);
      context.AddError(entitySet.Location(), EdmErrorCode.EntitySetTypeMustBeCollectionOfEntityType, errorMessage);
    }));
    public static readonly ValidationRule<IEdmEntitySet> EntitySetTypeCannotBeEdmEntityType = new ValidationRule<IEdmEntitySet>((Action<ValidationContext, IEdmEntitySet>) ((context, entitySet) =>
    {
      if (entitySet.Type.AsElementType() != EdmCoreModelEntityType.Instance)
        return;
      context.AddError(entitySet.Location(), EdmErrorCode.EntityTypeOfEntitySetCannotBeEdmEntityType, Strings.EdmModel_Validator_Semantic_EdmEntityTypeCannotBeTypeOfEntitySet((object) entitySet.Name));
    }));
    public static readonly ValidationRule<IEdmSingleton> SingletonTypeMustBeEntityType = new ValidationRule<IEdmSingleton>((Action<ValidationContext, IEdmSingleton>) ((context, singleton) =>
    {
      if (singleton.Type is IEdmEntityType)
        return;
      string errorMessage = Strings.EdmModel_Validator_Semantic_SingletonTypeMustBeEntityType((object) singleton.Type.FullTypeName(), (object) singleton.Name);
      context.AddError(singleton.Location(), EdmErrorCode.SingletonTypeMustBeEntityType, errorMessage);
    }));
    public static readonly ValidationRule<IEdmSingleton> SingletonTypeCannotBeEdmEntityType = new ValidationRule<IEdmSingleton>((Action<ValidationContext, IEdmSingleton>) ((context, singleton) =>
    {
      if (singleton.Type != EdmCoreModelEntityType.Instance)
        return;
      context.AddError(singleton.Location(), EdmErrorCode.EntityTypeOfSingletonCannotBeEdmEntityType, Strings.EdmModel_Validator_Semantic_EdmEntityTypeCannotBeTypeOfSingleton((object) singleton.Name));
    }));
    public static readonly ValidationRule<IEdmStructuredType> StructuredTypeInvalidMemberNameMatchesTypeName = new ValidationRule<IEdmStructuredType>((Action<ValidationContext, IEdmStructuredType>) ((context, structuredType) =>
    {
      if (!(structuredType is IEdmSchemaType edmSchemaType2))
        return;
      List<IEdmProperty> list = structuredType.Properties().ToList<IEdmProperty>();
      if (list.Count <= 0)
        return;
      foreach (IEdmProperty edmProperty in list)
      {
        if (edmProperty != null && edmProperty.Name.EqualsOrdinal(edmSchemaType2.Name))
          context.AddError(edmProperty.Location(), EdmErrorCode.BadProperty, Strings.EdmModel_Validator_Semantic_InvalidMemberNameMatchesTypeName((object) edmProperty.Name));
      }
    }));
    public static readonly ValidationRule<IEdmStructuredType> StructuredTypePropertyNameAlreadyDefined = new ValidationRule<IEdmStructuredType>((Action<ValidationContext, IEdmStructuredType>) ((context, structuredType) =>
    {
      HashSetInternal<string> memberNameList = new HashSetInternal<string>();
      foreach (IEdmProperty property in structuredType.Properties())
      {
        if (property != null)
          ValidationHelper.AddMemberNameToHashSet((IEdmNamedElement) property, memberNameList, context, EdmErrorCode.AlreadyDefined, Strings.EdmModel_Validator_Semantic_PropertyNameAlreadyDefined((object) property.Name), !structuredType.DeclaredProperties.Contains<IEdmProperty>(property));
      }
    }));
    public static readonly ValidationRule<IEdmStructuredType> StructuredTypeBaseTypeMustBeSameKindAsDerivedKind = new ValidationRule<IEdmStructuredType>((Action<ValidationContext, IEdmStructuredType>) ((context, structuredType) =>
    {
      if (!(structuredType is IEdmSchemaType) || structuredType.BaseType == null || structuredType.BaseType.TypeKind == structuredType.TypeKind)
        return;
      context.AddError(structuredType.Location(), structuredType.TypeKind == EdmTypeKind.Entity ? EdmErrorCode.EntityMustHaveEntityBaseType : EdmErrorCode.ComplexTypeMustHaveComplexBaseType, Strings.EdmModel_Validator_Semantic_BaseTypeMustHaveSameTypeKind);
    }));
    public static readonly ValidationRule<IEdmStructuredType> StructuredTypeBaseTypeCannotBeAbstractType = new ValidationRule<IEdmStructuredType>((Action<ValidationContext, IEdmStructuredType>) ((context, structuredType) =>
    {
      if (structuredType.BaseType == null || structuredType.BaseType != EdmCoreModelComplexType.Instance && structuredType.BaseType != EdmCoreModelEntityType.Instance || context.IsBad((IEdmElement) structuredType.BaseType))
        return;
      string p1 = structuredType.TypeKind == EdmTypeKind.Entity ? "entity" : "complex";
      context.AddError(structuredType.Location(), structuredType.TypeKind == EdmTypeKind.Entity ? EdmErrorCode.EntityTypeBaseTypeCannotBeEdmEntityType : EdmErrorCode.ComplexTypeBaseTypeCannotBeEdmComplexType, Strings.EdmModel_Validator_Semantic_StructuredTypeBaseTypeCannotBeAbstractType((object) structuredType.BaseType.FullTypeName(), (object) p1, (object) structuredType.FullTypeName()));
    }));
    public static readonly ValidationRule<IEdmStructuredType> StructuredTypeInaccessibleBaseType = new ValidationRule<IEdmStructuredType>((Action<ValidationContext, IEdmStructuredType>) ((context, structuredType) =>
    {
      if (!(structuredType.BaseType is IEdmSchemaType baseType2) || context.IsBad((IEdmElement) baseType2))
        return;
      ValidationRules.CheckForUnreacheableTypeError(context, baseType2, structuredType.Location());
    }));
    public static readonly ValidationRule<IEdmStructuredType> StructuredTypePropertiesDeclaringTypeMustBeCorrect = new ValidationRule<IEdmStructuredType>((Action<ValidationContext, IEdmStructuredType>) ((context, structuredType) =>
    {
      foreach (IEdmProperty declaredProperty in structuredType.DeclaredProperties)
      {
        if (declaredProperty != null && !declaredProperty.DeclaringType.Equals((object) structuredType))
          context.AddError(declaredProperty.Location(), EdmErrorCode.DeclaringTypeMustBeCorrect, Strings.EdmModel_Validator_Semantic_DeclaringTypeMustBeCorrect((object) declaredProperty.Name));
      }
    }));
    public static readonly ValidationRule<IEdmEnumType> EnumTypeEnumMemberNameAlreadyDefined = new ValidationRule<IEdmEnumType>((Action<ValidationContext, IEdmEnumType>) ((context, enumType) =>
    {
      HashSetInternal<string> memberNameList = new HashSetInternal<string>();
      foreach (IEdmEnumMember member in enumType.Members)
      {
        if (member != null)
          ValidationHelper.AddMemberNameToHashSet((IEdmNamedElement) member, memberNameList, context, EdmErrorCode.AlreadyDefined, Strings.EdmModel_Validator_Semantic_EnumMemberNameAlreadyDefined((object) member.Name), false);
      }
    }));
    public static readonly ValidationRule<IEdmEnumType> EnumMustHaveIntegerUnderlyingType = new ValidationRule<IEdmEnumType>((Action<ValidationContext, IEdmEnumType>) ((context, enumType) =>
    {
      if (enumType.UnderlyingType.PrimitiveKind.IsIntegral() || context.IsBad((IEdmElement) enumType.UnderlyingType))
        return;
      context.AddError(enumType.Location(), EdmErrorCode.EnumMustHaveIntegerUnderlyingType, Strings.EdmModel_Validator_Semantic_EnumMustHaveIntegralUnderlyingType((object) enumType.FullName()));
    }));
    public static readonly ValidationRule<IEdmEnumType> EnumUnderlyingTypeCannotBeEdmPrimitiveType = new ValidationRule<IEdmEnumType>((Action<ValidationContext, IEdmEnumType>) ((context, enumType) =>
    {
      if (enumType.UnderlyingType.PrimitiveKind != EdmPrimitiveTypeKind.PrimitiveType || context.IsBad((IEdmElement) enumType.UnderlyingType))
        return;
      context.AddError(enumType.Location(), EdmErrorCode.TypeDefinitionUnderlyingTypeCannotBeEdmPrimitiveType, Strings.EdmModel_Validator_Semantic_EdmPrimitiveTypeCannotBeUsedAsUnderlyingType((object) "enumeration", (object) enumType.FullName()));
    }));
    public static readonly ValidationRule<IEdmEnumMember> EnumMemberValueMustHaveSameTypeAsUnderlyingType = new ValidationRule<IEdmEnumMember>((Action<ValidationContext, IEdmEnumMember>) ((context, enumMember) =>
    {
      if (context.IsBad((IEdmElement) enumMember.DeclaringType) || context.IsBad((IEdmElement) enumMember.DeclaringType.UnderlyingType) || new EdmIntegerConstant(enumMember.Value.Value).TryCastPrimitiveAsType((IEdmTypeReference) enumMember.DeclaringType.UnderlyingType.GetPrimitiveTypeReference(false), out IEnumerable<EdmError> _))
        return;
      context.AddError(enumMember.Location(), EdmErrorCode.EnumMemberValueOutOfRange, Strings.EdmModel_Validator_Semantic_EnumMemberValueOutOfRange((object) enumMember.Name));
    }));
    public static readonly ValidationRule<IEdmTypeDefinition> TypeDefinitionUnderlyingTypeCannotBeEdmPrimitiveType = new ValidationRule<IEdmTypeDefinition>((Action<ValidationContext, IEdmTypeDefinition>) ((context, typeDefinition) =>
    {
      if (typeDefinition.UnderlyingType != EdmCoreModel.Instance.GetPrimitiveType() || context.IsBad((IEdmElement) typeDefinition.UnderlyingType))
        return;
      context.AddError(typeDefinition.Location(), EdmErrorCode.TypeDefinitionUnderlyingTypeCannotBeEdmPrimitiveType, Strings.EdmModel_Validator_Semantic_EdmPrimitiveTypeCannotBeUsedAsUnderlyingType((object) "type definition", (object) typeDefinition.FullName()));
    }));
    public static readonly ValidationRule<IEdmEntityType> EntityTypeDuplicatePropertyNameSpecifiedInEntityKey = new ValidationRule<IEdmEntityType>((Action<ValidationContext, IEdmEntityType>) ((context, entityType) =>
    {
      if (entityType.DeclaredKey == null)
        return;
      HashSetInternal<string> memberNameList = new HashSetInternal<string>();
      foreach (IEdmStructuralProperty structuralProperty in entityType.DeclaredKey)
        ValidationHelper.AddMemberNameToHashSet((IEdmNamedElement) structuralProperty, memberNameList, context, EdmErrorCode.DuplicatePropertySpecifiedInEntityKey, Strings.EdmModel_Validator_Semantic_DuplicatePropertyNameSpecifiedInEntityKey((object) entityType.Name, (object) structuralProperty.Name), false);
    }));
    public static readonly ValidationRule<IEdmEntityType> EntityTypeInvalidKeyNullablePart = new ValidationRule<IEdmEntityType>((Action<ValidationContext, IEdmEntityType>) ((context, entityType) =>
    {
      if (entityType.Key() == null)
        return;
      foreach (IEdmStructuralProperty structuralProperty in entityType.Key())
      {
        if (structuralProperty.Type.IsPrimitive() && structuralProperty.Type.IsNullable)
          context.AddError(structuralProperty.Location(), EdmErrorCode.InvalidKey, Strings.EdmModel_Validator_Semantic_InvalidKeyNullablePart((object) structuralProperty.Name, (object) entityType.Name));
      }
    }));
    public static readonly ValidationRule<IEdmEntityType> EntityTypeEntityKeyMustBeScalar = new ValidationRule<IEdmEntityType>((Action<ValidationContext, IEdmEntityType>) ((context, entityType) =>
    {
      if (entityType.Key() == null)
        return;
      foreach (IEdmStructuralProperty element in entityType.Key())
      {
        if (!element.Type.IsPrimitive() && !element.Type.IsEnum() && !context.IsBad((IEdmElement) element))
          context.AddError(element.Location(), EdmErrorCode.EntityKeyMustBeScalar, Strings.EdmModel_Validator_Semantic_EntityKeyMustBeScalar((object) element.Name, (object) entityType.Name));
      }
    }));
    public static readonly ValidationRule<IEdmEntityType> EntityTypeInvalidKeyKeyDefinedInBaseClass = new ValidationRule<IEdmEntityType>((Action<ValidationContext, IEdmEntityType>) ((context, entityType) =>
    {
      if (entityType.BaseType == null || entityType.DeclaredKey == null || entityType.BaseType.TypeKind != EdmTypeKind.Entity || entityType.BaseEntityType().DeclaredKey == null)
        return;
      context.AddError(entityType.Location(), EdmErrorCode.InvalidKey, Strings.EdmModel_Validator_Semantic_InvalidKeyKeyDefinedInBaseClass((object) entityType.Name, (object) entityType.BaseEntityType().Name));
    }));
    public static readonly ValidationRule<IEdmEntityType> EntityTypeKeyMissingOnEntityType = new ValidationRule<IEdmEntityType>((Action<ValidationContext, IEdmEntityType>) ((context, entityType) =>
    {
      IEnumerable<IEdmStructuralProperty> source = entityType.Key();
      if (source != null && source.Any<IEdmStructuralProperty>() || entityType.BaseType != null || entityType.IsAbstract)
        return;
      context.AddError(entityType.Location(), EdmErrorCode.KeyMissingOnEntityType, Strings.EdmModel_Validator_Semantic_KeyMissingOnEntityType((object) entityType.Name));
    }));
    public static readonly ValidationRule<IEdmEntityType> EntityTypeKeyPropertyMustBelongToEntity = new ValidationRule<IEdmEntityType>((Action<ValidationContext, IEdmEntityType>) ((context, entityType) =>
    {
      if (entityType.DeclaredKey == null)
        return;
      foreach (IEdmStructuralProperty element in entityType.DeclaredKey)
      {
        if (element.DeclaringType != entityType && !context.IsBad((IEdmElement) element))
          context.AddError(entityType.Location(), EdmErrorCode.KeyPropertyMustBelongToEntity, Strings.EdmModel_Validator_Semantic_KeyPropertyMustBelongToEntity((object) element.Name, (object) entityType.Name));
      }
    }));
    public static readonly ValidationRule<IEdmEntityType> EntityTypeKeyTypeCannotBeEdmPrimitiveType = new ValidationRule<IEdmEntityType>((Action<ValidationContext, IEdmEntityType>) ((context, entityType) =>
    {
      if (entityType.DeclaredKey == null)
        return;
      foreach (IEdmStructuralProperty structuralProperty in entityType.DeclaredKey)
      {
        if (structuralProperty.Type.Definition == EdmCoreModel.Instance.GetPrimitiveType())
          context.AddError(entityType.Location(), EdmErrorCode.KeyPropertyTypeCannotBeEdmPrimitiveType, Strings.EdmModel_Validator_Semantic_EdmPrimitiveTypeCannotBeUsedAsTypeOfKey((object) structuralProperty.Name, (object) entityType.FullName()));
      }
    }));
    public static readonly ValidationRule<IEdmEntityType> EntityTypeBoundEscapeFunctionMustBeUnique = new ValidationRule<IEdmEntityType>((Action<ValidationContext, IEdmEntityType>) ((context, entityType) =>
    {
      IList<IEdmFunction> source1 = (IList<IEdmFunction>) new List<IEdmFunction>();
      IList<IEdmFunction> source2 = (IList<IEdmFunction>) new List<IEdmFunction>();
      foreach (IEdmFunction function in context.Model.FindBoundOperations((IEdmType) entityType).Where<IEdmOperation>((Func<IEdmOperation, bool>) (o => o.IsFunction())).OfType<IEdmFunction>())
      {
        if (context.Model.IsUrlEscapeFunction(function))
        {
          if (function.IsComposable)
            source1.Add(function);
          else
            source2.Add(function);
        }
      }
      if (source1.Count<IEdmFunction>() > 1)
      {
        string p1 = string.Join(",", source1.Select<IEdmFunction, string>((Func<IEdmFunction, string>) (c => c.Name)).ToArray<string>());
        context.AddError(entityType.Location(), EdmErrorCode.EntityComposableBoundEscapeFunctionMustBeLessOne, Strings.EdmModel_Validator_Semantic_EntityComposableBoundEscapeFunctionMustBeLessOne((object) entityType.FullName(), (object) p1));
      }
      if (source2.Count<IEdmFunction>() <= 1)
        return;
      string p1_1 = string.Join(",", source2.Select<IEdmFunction, string>((Func<IEdmFunction, string>) (c => c.Name)).ToArray<string>());
      context.AddError(entityType.Location(), EdmErrorCode.EntityNoncomposableBoundEscapeFunctionMustBeLessOne, Strings.EdmModel_Validator_Semantic_EntityNoncomposableBoundEscapeFunctionMustBeLessOne((object) entityType.FullName(), (object) p1_1));
    }));
    public static readonly ValidationRule<IEdmEntityReferenceType> EntityReferenceTypeInaccessibleEntityType = new ValidationRule<IEdmEntityReferenceType>((Action<ValidationContext, IEdmEntityReferenceType>) ((context, entityReferenceType) =>
    {
      if (context.IsBad((IEdmElement) entityReferenceType.EntityType))
        return;
      ValidationRules.CheckForUnreacheableTypeError(context, (IEdmSchemaType) entityReferenceType.EntityType, entityReferenceType.Location());
    }));
    public static readonly ValidationRule<IEdmType> TypeMustNotHaveKindOfNone = new ValidationRule<IEdmType>((Action<ValidationContext, IEdmType>) ((context, type) =>
    {
      if (type.TypeKind != EdmTypeKind.None || context.IsBad((IEdmElement) type))
        return;
      context.AddError(type.Location(), EdmErrorCode.TypeMustNotHaveKindOfNone, Strings.EdmModel_Validator_Semantic_TypeMustNotHaveKindOfNone);
    }));
    public static readonly ValidationRule<IEdmPrimitiveType> PrimitiveTypeMustNotHaveKindOfNone = new ValidationRule<IEdmPrimitiveType>((Action<ValidationContext, IEdmPrimitiveType>) ((context, type) =>
    {
      if (type.PrimitiveKind != EdmPrimitiveTypeKind.None || context.IsBad((IEdmElement) type))
        return;
      context.AddError(type.Location(), EdmErrorCode.PrimitiveTypeMustNotHaveKindOfNone, Strings.EdmModel_Validator_Semantic_PrimitiveTypeMustNotHaveKindOfNone((object) type.FullName()));
    }));
    public static readonly ValidationRule<IEdmComplexType> OpenComplexTypeCannotHaveClosedDerivedComplexType = new ValidationRule<IEdmComplexType>((Action<ValidationContext, IEdmComplexType>) ((context, complexType) =>
    {
      if (complexType.BaseType == null || !complexType.BaseType.IsOpen || complexType.IsOpen)
        return;
      context.AddError(complexType.Location(), EdmErrorCode.InvalidAbstractComplexType, Strings.EdmModel_Validator_Semantic_BaseTypeOfOpenTypeMustBeOpen((object) complexType.FullName()));
    }));
    public static readonly ValidationRule<IEdmStructuralProperty> StructuralPropertyInvalidPropertyType = new ValidationRule<IEdmStructuralProperty>((Action<ValidationContext, IEdmStructuralProperty>) ((context, property) =>
    {
      IEdmType element = !property.Type.IsCollection() ? property.Type.Definition : property.Type.AsCollection().ElementType().Definition;
      if (element.TypeKind == EdmTypeKind.Primitive || element.TypeKind == EdmTypeKind.Enum || element.TypeKind == EdmTypeKind.Untyped || element.TypeKind == EdmTypeKind.Complex || element.TypeKind == EdmTypeKind.Path || element.TypeKind == EdmTypeKind.TypeDefinition || context.IsBad((IEdmElement) element))
        return;
      context.AddError(property.Location(), EdmErrorCode.InvalidPropertyType, Strings.EdmModel_Validator_Semantic_InvalidPropertyType((object) property.Type.TypeKind().ToString()));
    }));
    public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyInvalidOperationMultipleEndsInAssociatedNavigationProperties = new ValidationRule<IEdmNavigationProperty>((Action<ValidationContext, IEdmNavigationProperty>) ((context, navigationProperty) =>
    {
      if (navigationProperty.OnDelete == EdmOnDeleteAction.None || navigationProperty.Partner == null || navigationProperty.Partner.OnDelete == EdmOnDeleteAction.None)
        return;
      context.AddError(navigationProperty.Location(), EdmErrorCode.InvalidAction, Strings.EdmModel_Validator_Semantic_InvalidOperationMultipleEndsInAssociation);
    }));
    public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyCorrectType = new ValidationRule<IEdmNavigationProperty>((Action<ValidationContext, IEdmNavigationProperty>) ((context, property) =>
    {
      if (property.ToEntityType() == null)
      {
        context.AddError(property.Location(), EdmErrorCode.InvalidNavigationPropertyType, Strings.EdmModel_Validator_Semantic_InvalidNavigationPropertyType((object) property.Name));
      }
      else
      {
        if (property.Partner == null || property.Partner is BadNavigationProperty || property.Partner.DeclaringType is IEdmComplexType || property.ToEntityType() == property.Partner.DeclaringEntityType())
          return;
        context.AddError(property.Location(), EdmErrorCode.InvalidNavigationPropertyType, Strings.EdmModel_Validator_Semantic_InvalidNavigationPropertyType((object) property.Name));
      }
    }));
    public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyDuplicateDependentProperty = new ValidationRule<IEdmNavigationProperty>((Action<ValidationContext, IEdmNavigationProperty>) ((context, navigationProperty) =>
    {
      if (navigationProperty.DependentProperties() == null)
        return;
      HashSetInternal<string> memberNameList = new HashSetInternal<string>();
      foreach (IEdmStructuralProperty dependentProperty in navigationProperty.DependentProperties())
      {
        if (dependentProperty != null)
          ValidationHelper.AddMemberNameToHashSet((IEdmNamedElement) dependentProperty, memberNameList, context, EdmErrorCode.DuplicateDependentProperty, Strings.EdmModel_Validator_Semantic_DuplicateDependentProperty((object) dependentProperty.Name, (object) navigationProperty.Name), false);
      }
    }));
    public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyPrincipalEndMultiplicity = new ValidationRule<IEdmNavigationProperty>((Action<ValidationContext, IEdmNavigationProperty>) ((context, navigationProperty) =>
    {
      IEnumerable<IEdmStructuralProperty> properties = navigationProperty.DependentProperties();
      if (properties == null)
        return;
      if (ValidationHelper.AllPropertiesAreNullable(properties))
      {
        if (navigationProperty.TargetMultiplicity() == EdmMultiplicity.ZeroOrOne)
          return;
        context.AddError(navigationProperty.Location(), EdmErrorCode.InvalidMultiplicityOfPrincipalEnd, Strings.EdmModel_Validator_Semantic_InvalidMultiplicityOfPrincipalEndDependentPropertiesAllNullable((object) navigationProperty.Name));
      }
      else if (!ValidationHelper.HasNullableProperty(properties))
      {
        if (navigationProperty.TargetMultiplicity() == EdmMultiplicity.One)
          return;
        context.AddError(navigationProperty.Location(), EdmErrorCode.InvalidMultiplicityOfPrincipalEnd, Strings.EdmModel_Validator_Semantic_InvalidMultiplicityOfPrincipalEndDependentPropertiesAllNonnullable((object) navigationProperty.Name));
      }
      else
      {
        if (navigationProperty.TargetMultiplicity() == EdmMultiplicity.One || navigationProperty.TargetMultiplicity() == EdmMultiplicity.ZeroOrOne)
          return;
        context.AddError(navigationProperty.Location(), EdmErrorCode.InvalidMultiplicityOfPrincipalEnd, Strings.EdmModel_Validator_Semantic_NavigationPropertyPrincipalEndMultiplicityUpperBoundMustBeOne((object) navigationProperty.Name));
      }
    }));
    public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyDependentEndMultiplicity = new ValidationRule<IEdmNavigationProperty>((Action<ValidationContext, IEdmNavigationProperty>) ((context, navigationProperty) =>
    {
      if (navigationProperty.Partner == null)
        return;
      IEnumerable<IEdmStructuralProperty> set2 = navigationProperty.DependentProperties();
      if (set2 == null)
        return;
      if (ValidationHelper.PropertySetsAreEquivalent(navigationProperty.DeclaringEntityType().Key(), set2))
      {
        if (!navigationProperty.Type.IsCollection())
          return;
        context.AddError(navigationProperty.Location(), EdmErrorCode.InvalidMultiplicityOfDependentEnd, Strings.EdmModel_Validator_Semantic_InvalidMultiplicityOfDependentEndMustBeZeroOneOrOne((object) navigationProperty.Name));
      }
      else
      {
        if (navigationProperty.Partner.Type.IsCollection())
          return;
        context.AddError(navigationProperty.Location(), EdmErrorCode.InvalidMultiplicityOfDependentEnd, Strings.EdmModel_Validator_Semantic_InvalidMultiplicityOfDependentEndMustBeMany((object) navigationProperty.Name));
      }
    }));
    public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyDependentPropertiesMustBelongToDependentEntity = new ValidationRule<IEdmNavigationProperty>((Action<ValidationContext, IEdmNavigationProperty>) ((context, navigationProperty) =>
    {
      IEnumerable<IEdmStructuralProperty> structuralProperties = navigationProperty.DependentProperties();
      if (structuralProperties == null)
        return;
      IEdmEntityType edmEntityType = navigationProperty.DeclaringEntityType();
      foreach (IEdmStructuralProperty element in structuralProperties)
      {
        if (!context.IsBad((IEdmElement) element) && !element.IsBad() && edmEntityType.FindProperty(element.Name) != element)
          context.AddError(navigationProperty.Location(), EdmErrorCode.DependentPropertiesMustBelongToDependentEntity, Strings.EdmModel_Validator_Semantic_DependentPropertiesMustBelongToDependentEntity((object) element.Name, (object) edmEntityType.Name));
      }
    }));
    public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyEndWithManyMultiplicityCannotHaveOperationsSpecified = new ValidationRule<IEdmNavigationProperty>((Action<ValidationContext, IEdmNavigationProperty>) ((context, end) =>
    {
      if (end.Partner == null || !end.Partner.Type.IsCollection() || end.OnDelete == EdmOnDeleteAction.None)
        return;
      string errorMessage = Strings.EdmModel_Validator_Semantic_EndWithManyMultiplicityCannotHaveOperationsSpecified((object) end.Name);
      context.AddError(end.Location(), EdmErrorCode.EndWithManyMultiplicityCannotHaveOperationsSpecified, errorMessage);
    }));
    public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyPartnerPathShouldBeResolvable = new ValidationRule<IEdmNavigationProperty>((Action<ValidationContext, IEdmNavigationProperty>) ((context, property) =>
    {
      IEdmPathExpression partnerPath = property.GetPartnerPath();
      if (partnerPath == null || !(property.Type.Definition.AsElementType() is IEdmEntityType) || CsdlSemanticsNavigationProperty.ResolvePartnerPath((IEdmEntityType) property.Type.Definition.AsElementType(), partnerPath, context.Model) != null)
        return;
      context.AddError(property.Location(), EdmErrorCode.UnresolvedNavigationPropertyPartnerPath, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot resolve partner path for navigation property '{0}'.", new object[1]
      {
        (object) property.Name
      }));
    }));
    public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyWithRecursiveContainmentTargetMustBeOptional = new ValidationRule<IEdmNavigationProperty>((Action<ValidationContext, IEdmNavigationProperty>) ((context, property) =>
    {
      if (!property.ContainsTarget || !property.DeclaringType.IsOrInheritsFrom((IEdmType) property.ToEntityType()) || property.Type.IsCollection() || property.Type.IsNullable)
        return;
      context.AddError(property.Location(), EdmErrorCode.NavigationPropertyWithRecursiveContainmentTargetMustBeOptional, Strings.EdmModel_Validator_Semantic_NavigationPropertyWithRecursiveContainmentTargetMustBeOptional((object) property.Name));
    }));
    public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne = new ValidationRule<IEdmNavigationProperty>((Action<ValidationContext, IEdmNavigationProperty>) ((context, property) =>
    {
      if (property.Partner == null || !property.ContainsTarget || !property.DeclaringType.IsOrInheritsFrom((IEdmType) property.ToEntityType()) || !property.Partner.Type.IsCollection() && property.Partner.Type.IsNullable)
        return;
      context.AddError(property.Location(), EdmErrorCode.NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne, Strings.EdmModel_Validator_Semantic_NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne((object) property.Name));
    }));
    public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne = new ValidationRule<IEdmNavigationProperty>((Action<ValidationContext, IEdmNavigationProperty>) ((context, property) =>
    {
      if (property.Partner == null || !property.ContainsTarget || property.DeclaringType.IsOrInheritsFrom((IEdmType) property.ToEntityType()) || !property.Partner.Type.IsCollection() && !property.Partner.Type.IsNullable)
        return;
      context.AddError(property.Location(), EdmErrorCode.NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne, Strings.EdmModel_Validator_Semantic_NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne((object) property.Name));
    }));
    public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyEntityMustNotIndirectlyContainItself = new ValidationRule<IEdmNavigationProperty>((Action<ValidationContext, IEdmNavigationProperty>) ((context, property) =>
    {
      if (!property.ContainsTarget || property.DeclaringType.IsOrInheritsFrom((IEdmType) property.ToEntityType()) || !ValidationHelper.TypeIndirectlyContainsTarget(property.ToEntityType(), property.DeclaringEntityType(), new HashSetInternal<IEdmEntityType>(), context.Model))
        return;
      context.AddError(property.Location(), EdmErrorCode.NavigationPropertyEntityMustNotIndirectlyContainItself, Strings.EdmModel_Validator_Semantic_NavigationPropertyEntityMustNotIndirectlyContainItself((object) property.Name));
    }));
    public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyTypeCannotHavePathTypeProperty = new ValidationRule<IEdmNavigationProperty>((Action<ValidationContext, IEdmNavigationProperty>) ((context, property) =>
    {
      IEdmTypeReference edmTypeReference = property.Type;
      if (edmTypeReference.IsCollection())
        edmTypeReference = edmTypeReference.AsCollection().ElementType();
      IEdmStructuredType structuredType = edmTypeReference.ToStructuredType();
      if (structuredType == null)
        return;
      IList<IEdmStructuredType> visited = (IList<IEdmStructuredType>) new List<IEdmStructuredType>();
      if (!ValidationRules.HasPathTypeProperty(structuredType, visited))
        return;
      string errorMessage = Strings.EdmModel_Validator_Semantic_TypeOfNavigationPropertyCannotHavePathProperty((object) property.Type.FullName(), (object) property.Name, (object) property.DeclaringType.FullTypeName());
      context.AddError(property.Location(), EdmErrorCode.TypeOfNavigationPropertyCannotHavePathProperty, errorMessage);
    }));
    public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyTypeMismatchRelationshipConstraint = new ValidationRule<IEdmNavigationProperty>((Action<ValidationContext, IEdmNavigationProperty>) ((context, navigationProperty) =>
    {
      IEnumerable<IEdmStructuralProperty> source3 = navigationProperty.DependentProperties();
      if (source3 == null)
        return;
      int num = source3.Count<IEdmStructuralProperty>();
      IEdmEntityType entityType = navigationProperty.ToEntityType();
      IEnumerable<IEdmStructuralProperty> source4 = navigationProperty.PrincipalProperties();
      if (num != source4.Count<IEdmStructuralProperty>())
        return;
      for (int index = 0; index < num; ++index)
      {
        IEdmType definition1 = source3.ElementAtOrDefault<IEdmStructuralProperty>(index).Type.Definition;
        IEdmType definition2 = source4.ElementAtOrDefault<IEdmStructuralProperty>(index).Type.Definition;
        if (!(definition1 is BadType) && !(definition2 is BadType) && !definition1.IsEquivalentTo(definition2))
        {
          string errorMessage = Strings.EdmModel_Validator_Semantic_TypeMismatchRelationshipConstraint((object) navigationProperty.DependentProperties().ToList<IEdmStructuralProperty>()[index].Name, (object) navigationProperty.DeclaringEntityType().FullName(), (object) source4.ToList<IEdmStructuralProperty>()[index].Name, (object) entityType.Name, (object) "Fred");
          context.AddError(navigationProperty.Location(), EdmErrorCode.TypeMismatchRelationshipConstraint, errorMessage);
        }
      }
    }));
    public static readonly ValidationRule<IEdmProperty> PropertyMustNotHaveKindOfNone = new ValidationRule<IEdmProperty>((Action<ValidationContext, IEdmProperty>) ((context, property) =>
    {
      if (property.PropertyKind != EdmPropertyKind.None || context.IsBad((IEdmElement) property))
        return;
      context.AddError(property.Location(), EdmErrorCode.PropertyMustNotHaveKindOfNone, Strings.EdmModel_Validator_Semantic_PropertyMustNotHaveKindOfNone((object) property.Name));
    }));
    public static readonly ValidationRule<IEdmProperty> PropertyTypeCannotBeCollectionOfAbstractType = new ValidationRule<IEdmProperty>((Action<ValidationContext, IEdmProperty>) ((context, property) =>
    {
      if (!property.Type.IsCollection())
        return;
      IEdmTypeReference edmTypeReference = property.Type.AsCollection().ElementType();
      if (edmTypeReference.Definition != EdmCoreModelComplexType.Instance && edmTypeReference.Definition != EdmCoreModel.Instance.GetPrimitiveType())
        return;
      context.AddError(property.Location(), EdmErrorCode.PropertyTypeCannotBeCollectionOfAbstractType, Strings.EdmModel_Validator_Semantic_PropertyTypeCannotBeCollectionOfAbstractType((object) property.Type.FullName(), (object) property.Name));
    }));
    public static readonly ValidationRule<IEdmOperationImport> OperationImportCannotImportBoundOperation = new ValidationRule<IEdmOperationImport>((Action<ValidationContext, IEdmOperationImport>) ((context, operationImport) =>
    {
      if (!operationImport.Operation.IsBound)
        return;
      context.AddError(operationImport.Location(), EdmErrorCode.OperationImportCannotImportBoundOperation, Strings.EdmModel_Validator_Semantic_OperationImportCannotImportBoundOperation((object) operationImport.Name, (object) operationImport.Operation.Name));
    }));
    public static readonly ValidationRule<IEdmOperationImport> OperationImportEntitySetExpressionIsInvalid = new ValidationRule<IEdmOperationImport>((Action<ValidationContext, IEdmOperationImport>) ((context, operationImport) =>
    {
      if (operationImport.EntitySet == null)
        return;
      if (operationImport.EntitySet.ExpressionKind != EdmExpressionKind.Path)
      {
        context.AddError(operationImport.Location(), EdmErrorCode.OperationImportEntitySetExpressionIsInvalid, Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionKindIsInvalid((object) operationImport.Name, (object) operationImport.EntitySet.ExpressionKind));
      }
      else
      {
        IEdmEntitySetBase entitySet;
        if (!operationImport.TryGetStaticEntitySet(context.Model, out entitySet))
        {
          context.AddError(operationImport.Location(), EdmErrorCode.OperationImportEntitySetExpressionIsInvalid, Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid((object) operationImport.Name));
        }
        else
        {
          if (context.IsBad((IEdmElement) entitySet) || operationImport.Container.FindEntitySetExtended(entitySet.Name) != null)
            return;
          context.AddError(operationImport.Location(), EdmErrorCode.OperationImportEntitySetExpressionIsInvalid, Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid((object) operationImport.Name));
        }
      }
    }));
    public static readonly ValidationRule<IEdmOperationImport> OperationImportEntityTypeDoesNotMatchEntitySet = new ValidationRule<IEdmOperationImport>((Action<ValidationContext, IEdmOperationImport>) ((context, operationImport) =>
    {
      if (operationImport.EntitySet == null || operationImport.Operation.ReturnType == null)
        return;
      IEdmTypeReference type3 = operationImport.Operation.ReturnType.IsCollection() ? operationImport.Operation.ReturnType.AsCollection().ElementType() : operationImport.Operation.ReturnType;
      if (type3.IsEntity())
      {
        IEdmEntityType edmEntityType1 = type3.AsEntity().EntityDefinition();
        IEdmEntitySetBase entitySet;
        if (operationImport.TryGetStaticEntitySet(context.Model, out entitySet))
        {
          IEdmEntityType edmEntityType2 = entitySet.EntityType();
          if (edmEntityType1.IsOrInheritsFrom((IEdmType) edmEntityType2) || context.IsBad((IEdmElement) edmEntityType1) || context.IsBad((IEdmElement) entitySet) || context.IsBad((IEdmElement) edmEntityType2))
            return;
          string errorMessage = Strings.EdmModel_Validator_Semantic_OperationImportEntityTypeDoesNotMatchEntitySet((object) operationImport.Name, (object) edmEntityType1.FullName(), (object) entitySet.Name);
          context.AddError(operationImport.Location(), EdmErrorCode.OperationImportEntityTypeDoesNotMatchEntitySet, errorMessage);
        }
        else
        {
          IEdmOperationParameter parameter;
          Dictionary<IEdmNavigationProperty, IEdmPathExpression> relativeNavigations;
          if (!operationImport.TryGetRelativeEntitySetPath(context.Model, out parameter, out relativeNavigations, out IEnumerable<EdmError> _))
            return;
          IEdmTypeReference type4 = relativeNavigations.Select<KeyValuePair<IEdmNavigationProperty, IEdmPathExpression>, IEdmNavigationProperty>((Func<KeyValuePair<IEdmNavigationProperty, IEdmPathExpression>, IEdmNavigationProperty>) (s => s.Key)).ToList<IEdmNavigationProperty>().Count == 0 ? parameter.Type : relativeNavigations.Last<KeyValuePair<IEdmNavigationProperty, IEdmPathExpression>>().Key.Type;
          IEdmTypeReference edmTypeReference = type4.IsCollection() ? type4.AsCollection().ElementType() : type4;
          if (edmEntityType1.IsOrInheritsFrom(edmTypeReference.Definition) || context.IsBad((IEdmElement) edmEntityType1) || context.IsBad((IEdmElement) edmTypeReference.Definition))
            return;
          context.AddError(operationImport.Location(), EdmErrorCode.OperationImportEntityTypeDoesNotMatchEntitySet, Strings.EdmModel_Validator_Semantic_OperationImportEntityTypeDoesNotMatchEntitySet2((object) operationImport.Name, (object) type3.FullName()));
        }
      }
      else
      {
        if (context.IsBad((IEdmElement) type3.Definition))
          return;
        context.AddError(operationImport.Location(), EdmErrorCode.OperationImportSpecifiesEntitySetButDoesNotReturnEntityType, Strings.EdmModel_Validator_Semantic_OperationImportSpecifiesEntitySetButNotEntityType((object) operationImport.Name));
      }
    }));
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly ValidationRule<IEdmFunctionImport> FunctionImportWithParameterShouldNotBeIncludedInServiceDocument = new ValidationRule<IEdmFunctionImport>((Action<ValidationContext, IEdmFunctionImport>) ((context, functionImport) =>
    {
      if (!functionImport.IncludeInServiceDocument || !functionImport.Function.Parameters.Any<IEdmOperationParameter>())
        return;
      context.AddError(functionImport.Location(), EdmErrorCode.FunctionImportWithParameterShouldNotBeIncludedInServiceDocument, Strings.EdmModel_Validator_Semantic_FunctionImportWithParameterShouldNotBeIncludedInServiceDocument((object) functionImport.Name));
    }));
    public static readonly ValidationRule<IEdmFunction> FunctionMustHaveReturnType = new ValidationRule<IEdmFunction>((Action<ValidationContext, IEdmFunction>) ((context, function) =>
    {
      if (function.ReturnType != null)
        return;
      context.AddError(function.Location(), EdmErrorCode.FunctionMustHaveReturnType, Strings.EdmModel_Validator_Semantic_FunctionMustHaveReturnType((object) function.Name));
    }));
    public static readonly ValidationRule<IEdmFunction> FunctionWithUrlEscapeFunctionMustBeBound = new ValidationRule<IEdmFunction>((Action<ValidationContext, IEdmFunction>) ((context, function) =>
    {
      if (!context.Model.IsUrlEscapeFunction(function) || function.IsBound)
        return;
      context.AddError(function.Location(), EdmErrorCode.UrlEscapeFunctionMustBeBoundFunction, Strings.EdmModel_Validator_Semantic_UrlEscapeFunctionMustBoundFunction((object) function.Name));
    }));
    public static readonly ValidationRule<IEdmFunction> FunctionWithUrlEscapeFunctionMustHaveOneStringParameter = new ValidationRule<IEdmFunction>((Action<ValidationContext, IEdmFunction>) ((context, function) =>
    {
      if (!context.Model.IsUrlEscapeFunction(function) || function.Parameters != null && function.Parameters.Count<IEdmOperationParameter>() == 2 && function.Parameters.ElementAt<IEdmOperationParameter>(1).Type.IsString())
        return;
      context.AddError(function.Location(), EdmErrorCode.UrlEscapeFunctionMustHaveOnlyOneEdmStringParameter, Strings.EdmModel_Validator_Semantic_UrlEscapeFunctionMustHaveOneStringParameter((object) function.Name));
    }));
    public static readonly ValidationRule<IEdmOperation> OperationUnsupportedReturnType = new ValidationRule<IEdmOperation>((Action<ValidationContext, IEdmOperation>) ((context, operation) =>
    {
      if (operation.ReturnType == null)
        return;
      IEdmTypeReference edmTypeReference = operation.ReturnType.IsCollection() ? operation.ReturnType.AsCollection().ElementType() : operation.ReturnType;
      if (edmTypeReference.Definition is IUnresolvedElement || !context.IsBad((IEdmElement) edmTypeReference.Definition))
        return;
      context.AddError(operation.Location(), EdmErrorCode.OperationImportUnsupportedReturnType, Strings.EdmModel_Validator_Semantic_OperationWithUnsupportedReturnType((object) operation.Name));
    }));
    public static readonly ValidationRule<IEdmOperation> OperationParameterNameAlreadyDefinedDuplicate = new ValidationRule<IEdmOperation>((Action<ValidationContext, IEdmOperation>) ((context, operation) =>
    {
      HashSetInternal<string> memberNameList = new HashSetInternal<string>();
      if (operation.Parameters == null)
        return;
      foreach (IEdmOperationParameter parameter in operation.Parameters)
        ValidationHelper.AddMemberNameToHashSet((IEdmNamedElement) parameter, memberNameList, context, EdmErrorCode.AlreadyDefined, Strings.EdmModel_Validator_Semantic_ParameterNameAlreadyDefinedDuplicate((object) parameter.Name), false);
    }));
    public static readonly ValidationRule<IEdmOperation> BoundOperationMustHaveParameters = new ValidationRule<IEdmOperation>((Action<ValidationContext, IEdmOperation>) ((context, operation) =>
    {
      if (!operation.IsBound || operation.Parameters.Any<IEdmOperationParameter>((Func<IEdmOperationParameter, bool>) (p => !(p is IEdmOptionalParameter))))
        return;
      context.AddError(operation.Location(), EdmErrorCode.BoundOperationMustHaveParameters, Strings.EdmModel_Validator_Semantic_BoundOperationMustHaveParameters((object) operation.Name));
    }));
    public static readonly ValidationRule<IEdmOperation> OptionalParametersMustComeAfterRequiredParameters = new ValidationRule<IEdmOperation>((Action<ValidationContext, IEdmOperation>) ((context, operation) =>
    {
      bool flag = false;
      foreach (IEdmOperationParameter parameter in operation.Parameters)
      {
        if (parameter is IEdmOptionalParameter)
          flag = true;
        else if (flag)
          context.AddError(operation.Location(), EdmErrorCode.RequiredParametersMustPrecedeOptional, Strings.EdmModel_Validator_Semantic_RequiredParametersMustPrecedeOptional((object) parameter.Name));
      }
    }));
    public static readonly ValidationRule<IEdmOperation> OperationEntitySetPathMustBeValid = new ValidationRule<IEdmOperation>((Action<ValidationContext, IEdmOperation>) ((context, operation) =>
    {
      IEdmOperationParameter parameter = (IEdmOperationParameter) null;
      Dictionary<IEdmNavigationProperty, IEdmPathExpression> relativeNavigations = (Dictionary<IEdmNavigationProperty, IEdmPathExpression>) null;
      IEdmEntityType lastEntityType = (IEdmEntityType) null;
      IEnumerable<EdmError> errors = (IEnumerable<EdmError>) null;
      operation.TryGetRelativeEntitySetPath(context.Model, out parameter, out relativeNavigations, out lastEntityType, out errors);
      foreach (EdmError error in errors)
        context.AddError(error);
    }));
    public static readonly ValidationRule<IEdmOperation> OperationReturnTypeEntityTypeMustBeValid = new ValidationRule<IEdmOperation>((Action<ValidationContext, IEdmOperation>) ((context, operation) =>
    {
      IEdmOperationParameter parameter = (IEdmOperationParameter) null;
      Dictionary<IEdmNavigationProperty, IEdmPathExpression> relativeNavigations = (Dictionary<IEdmNavigationProperty, IEdmPathExpression>) null;
      IEdmEntityType lastEntityType = (IEdmEntityType) null;
      IEnumerable<EdmError> errors = (IEnumerable<EdmError>) null;
      if (!operation.TryGetRelativeEntitySetPath(context.Model, out parameter, out relativeNavigations, out lastEntityType, out errors) || operation.ReturnType == null)
        return;
      IEdmEntityType definition3 = operation.ReturnType.Definition as IEdmEntityType;
      IEdmCollectionType definition4 = operation.ReturnType.Definition as IEdmCollectionType;
      if (definition3 == null && definition4 != null)
        definition3 = definition4.ElementType.Definition as IEdmEntityType;
      bool flag = operation.ReturnType.IsEntity();
      if (definition4 != null)
        flag = definition4.ElementType.IsEntity();
      if (!flag || context.IsBad((IEdmElement) definition3))
        context.AddError(operation.Location(), EdmErrorCode.OperationWithEntitySetPathReturnTypeInvalid, Strings.EdmModel_Validator_Semantic_OperationWithEntitySetPathReturnTypeInvalid((object) operation.Name));
      IEdmNavigationProperty property = (IEdmNavigationProperty) null;
      if (relativeNavigations != null)
        property = relativeNavigations.LastOrDefault<KeyValuePair<IEdmNavigationProperty, IEdmPathExpression>>().Key;
      if (property != null && property.TargetMultiplicity() != EdmMultiplicity.Many && definition4 != null)
        context.AddError(operation.Location(), EdmErrorCode.OperationWithEntitySetPathResolvesToCollectionEntityTypeMismatchesEntityTypeReturnType, Strings.EdmModel_Validator_Semantic_OperationWithEntitySetPathResolvesToCollectionEntityTypeMismatchesEntityTypeReturnType((object) operation.Name));
      if (lastEntityType == null || definition3 == null || definition3.IsOrInheritsFrom((IEdmType) lastEntityType))
        return;
      context.AddError(operation.Location(), EdmErrorCode.OperationWithEntitySetPathAndReturnTypeTypeNotAssignable, Strings.EdmModel_Validator_Semantic_OperationWithEntitySetPathAndReturnTypeTypeNotAssignable((object) operation.Name, (object) definition3.FullName(), (object) lastEntityType.FullName()));
    }));
    public static readonly ValidationRule<IEdmOperation> OperationReturnTypeCannotBeCollectionOfAbstractType = new ValidationRule<IEdmOperation>((Action<ValidationContext, IEdmOperation>) ((context, operation) =>
    {
      if (operation.ReturnType == null || !operation.ReturnType.IsCollection())
        return;
      IEdmTypeReference edmTypeReference = operation.ReturnType.AsCollection().ElementType();
      if (edmTypeReference.Definition != EdmCoreModelComplexType.Instance && edmTypeReference.Definition != EdmCoreModel.Instance.GetPrimitiveType())
        return;
      context.AddError(operation.Location(), EdmErrorCode.OperationWithCollectionOfAbstractReturnTypeInvalid, Strings.EdmModel_Validator_Semantic_OperationReturnTypeCannotBeCollectionOfAbstractType((object) operation.ReturnType.FullName(), (object) operation.FullName()));
    }));
    public static readonly ValidationRule<IEdmTypeReference> TypeReferenceInaccessibleSchemaType = new ValidationRule<IEdmTypeReference>((Action<ValidationContext, IEdmTypeReference>) ((context, typeReference) =>
    {
      if (!(typeReference.Definition is IEdmSchemaType definition6) || context.IsBad((IEdmElement) definition6))
        return;
      ValidationRules.CheckForUnreacheableTypeError(context, definition6, typeReference.Location());
    }));
    public static readonly ValidationRule<IEdmDecimalTypeReference> DecimalTypeReferenceScaleOutOfRange = new ValidationRule<IEdmDecimalTypeReference>((Action<ValidationContext, IEdmDecimalTypeReference>) ((context, type) =>
    {
      int? scale1 = type.Scale;
      int? precision = type.Precision;
      if (!(scale1.GetValueOrDefault() > precision.GetValueOrDefault() & scale1.HasValue & precision.HasValue))
      {
        int? scale2 = type.Scale;
        int num = 0;
        if (!(scale2.GetValueOrDefault() < num & scale2.HasValue))
          return;
      }
      context.AddError(type.Location(), EdmErrorCode.ScaleOutOfRange, Strings.EdmModel_Validator_Semantic_ScaleOutOfRange);
    }));
    public static readonly ValidationRule<IEdmDecimalTypeReference> DecimalTypeReferencePrecisionOutOfRange = new ValidationRule<IEdmDecimalTypeReference>((Action<ValidationContext, IEdmDecimalTypeReference>) ((context, type) =>
    {
      int? precision1 = type.Precision;
      int maxValue = int.MaxValue;
      if (!(precision1.GetValueOrDefault() > maxValue & precision1.HasValue))
      {
        int? precision2 = type.Precision;
        int num = 0;
        if (!(precision2.GetValueOrDefault() < num & precision2.HasValue))
          return;
      }
      context.AddError(type.Location(), EdmErrorCode.PrecisionOutOfRange, Strings.EdmModel_Validator_Semantic_PrecisionOutOfRange);
    }));
    public static readonly ValidationRule<IEdmStringTypeReference> StringTypeReferenceStringMaxLengthNegative = new ValidationRule<IEdmStringTypeReference>((Action<ValidationContext, IEdmStringTypeReference>) ((context, type) =>
    {
      int? maxLength = type.MaxLength;
      int num = 0;
      if (!(maxLength.GetValueOrDefault() < num & maxLength.HasValue))
        return;
      context.AddError(type.Location(), EdmErrorCode.MaxLengthOutOfRange, Strings.EdmModel_Validator_Semantic_StringMaxLengthOutOfRange);
    }));
    public static readonly ValidationRule<IEdmStringTypeReference> StringTypeReferenceStringUnboundedNotValidForMaxLength = new ValidationRule<IEdmStringTypeReference>((Action<ValidationContext, IEdmStringTypeReference>) ((context, type) =>
    {
      if (!type.MaxLength.HasValue || !type.IsUnbounded)
        return;
      context.AddError(type.Location(), EdmErrorCode.IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull, Strings.EdmModel_Validator_Semantic_IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull);
    }));
    public static readonly ValidationRule<IEdmBinaryTypeReference> BinaryTypeReferenceBinaryMaxLengthNegative = new ValidationRule<IEdmBinaryTypeReference>((Action<ValidationContext, IEdmBinaryTypeReference>) ((context, type) =>
    {
      int? maxLength = type.MaxLength;
      int num = 0;
      if (!(maxLength.GetValueOrDefault() < num & maxLength.HasValue))
        return;
      context.AddError(type.Location(), EdmErrorCode.MaxLengthOutOfRange, Strings.EdmModel_Validator_Semantic_MaxLengthOutOfRange);
    }));
    public static readonly ValidationRule<IEdmBinaryTypeReference> BinaryTypeReferenceBinaryUnboundedNotValidForMaxLength = new ValidationRule<IEdmBinaryTypeReference>((Action<ValidationContext, IEdmBinaryTypeReference>) ((context, type) =>
    {
      if (!type.MaxLength.HasValue || !type.IsUnbounded)
        return;
      context.AddError(type.Location(), EdmErrorCode.IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull, Strings.EdmModel_Validator_Semantic_IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull);
    }));
    public static readonly ValidationRule<IEdmTemporalTypeReference> TemporalTypeReferencePrecisionOutOfRange = new ValidationRule<IEdmTemporalTypeReference>((Action<ValidationContext, IEdmTemporalTypeReference>) ((context, type) =>
    {
      int? precision3 = type.Precision;
      int maxValue = int.MaxValue;
      if (!(precision3.GetValueOrDefault() > maxValue & precision3.HasValue))
      {
        int? precision4 = type.Precision;
        int num = 0;
        if (!(precision4.GetValueOrDefault() < num & precision4.HasValue))
          return;
      }
      context.AddError(type.Location(), EdmErrorCode.PrecisionOutOfRange, Strings.EdmModel_Validator_Semantic_PrecisionOutOfRange);
    }));
    public static readonly ValidationRule<IEdmModel> ModelDuplicateSchemaElementName = new ValidationRule<IEdmModel>((Action<ValidationContext, IEdmModel>) ((context, model) =>
    {
      HashSetInternal<string> hashSetInternal4 = new HashSetInternal<string>();
      DuplicateOperationValidator operationValidator = new DuplicateOperationValidator(context);
      HashSetInternal<string> hashSetInternal5 = new HashSetInternal<string>();
      foreach (IEdmSchemaElement schemaElement in model.SchemaElements)
      {
        bool flag = false;
        string str = schemaElement.FullName();
        if (schemaElement is IEdmOperation edmOperation2)
        {
          if (!hashSetInternal5.Contains(edmOperation2.FullName()))
            hashSetInternal5.Add(edmOperation2.FullName());
          if (hashSetInternal4.Contains(str))
            flag = true;
          operationValidator.ValidateNotDuplicate(edmOperation2, false);
          if (!flag)
            flag = model.OperationOrNameExistsInReferencedModel(edmOperation2, str);
        }
        else
          flag = !hashSetInternal4.Add(str) || hashSetInternal5.Contains(str) || model.ItemExistsInReferencedModel(str, true);
        if (flag)
          context.AddError(schemaElement.Location(), EdmErrorCode.AlreadyDefined, Strings.EdmModel_Validator_Semantic_SchemaElementNameAlreadyDefined((object) str));
      }
    }));
    public static readonly ValidationRule<IEdmModel> ModelDuplicateEntityContainerName = new ValidationRule<IEdmModel>((Action<ValidationContext, IEdmModel>) ((context, model) =>
    {
      HashSetInternal<string> memberNameList = new HashSetInternal<string>();
      IEdmEntityContainer entityContainer = model.EntityContainer;
      if (entityContainer == null)
        return;
      ValidationHelper.AddMemberNameToHashSet((IEdmNamedElement) entityContainer, memberNameList, context, EdmErrorCode.DuplicateEntityContainerName, Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerName((object) entityContainer.Name), false);
    }));
    public static readonly ValidationRule<IEdmModel> ModelBoundFunctionOverloadsMustHaveSameReturnType = new ValidationRule<IEdmModel>((Action<ValidationContext, IEdmModel>) ((context, model) =>
    {
      foreach (IGrouping<string, IEdmFunction> grouping in model.SchemaElements.OfType<IEdmFunction>().Where<IEdmFunction>((Func<IEdmFunction, bool>) (f => f.IsBound)).GroupBy<IEdmFunction, string>((Func<IEdmFunction, string>) (f2 => f2.FullName())))
      {
        Dictionary<IEdmTypeReference, IEdmTypeReference> dictionary = new Dictionary<IEdmTypeReference, IEdmTypeReference>((IEqualityComparer<IEdmTypeReference>) new ValidationRules.EdmTypeReferenceComparer());
        foreach (IEdmFunction edmFunction in (IEnumerable<IEdmFunction>) grouping)
        {
          if (edmFunction.Parameters.Any<IEdmOperationParameter>() && edmFunction.ReturnType != null)
          {
            IEdmOperationParameter operationParameter = edmFunction.Parameters.First<IEdmOperationParameter>();
            if (!dictionary.ContainsKey(operationParameter.Type))
            {
              dictionary.Add(operationParameter.Type, edmFunction.ReturnType);
            }
            else
            {
              IEdmTypeReference edmTypeReference = dictionary[operationParameter.Type];
              if (!edmFunction.ReturnType.IsEquivalentTo(edmTypeReference))
                context.AddError(edmFunction.Location(), EdmErrorCode.BoundFunctionOverloadsMustHaveSameReturnType, Strings.EdmModel_Validator_Semantic_BoundFunctionOverloadsMustHaveSameReturnType((object) edmFunction.Name, (object) edmTypeReference.FullName()));
            }
          }
        }
      }
    }));
    public static readonly ValidationRule<IEdmModel> UnBoundFunctionOverloadsMustHaveIdenticalReturnTypes = new ValidationRule<IEdmModel>((Action<ValidationContext, IEdmModel>) ((context, model) =>
    {
      Dictionary<string, IEdmTypeReference> dictionary = new Dictionary<string, IEdmTypeReference>();
      foreach (IEdmFunction edmFunction in model.SchemaElements.OfType<IEdmFunction>().Where<IEdmFunction>((Func<IEdmFunction, bool>) (f => !f.IsBound)))
      {
        if (!dictionary.ContainsKey(edmFunction.Name))
          dictionary.Add(edmFunction.Name, edmFunction.ReturnType);
        else if (!edmFunction.ReturnType.IsEquivalentTo(dictionary[edmFunction.Name]))
          context.AddError(edmFunction.Location(), EdmErrorCode.UnboundFunctionOverloadHasIncorrectReturnType, Strings.EdmModel_Validator_Semantic_UnboundFunctionOverloadHasIncorrectReturnType((object) edmFunction.Name));
      }
    }));
    public static readonly ValidationRule<IEdmDirectValueAnnotation> ImmediateValueAnnotationElementAnnotationIsValid = new ValidationRule<IEdmDirectValueAnnotation>((Action<ValidationContext, IEdmDirectValueAnnotation>) ((context, annotation) =>
    {
      if (!(annotation.Value is IEdmStringValue edmStringValue2) || !edmStringValue2.IsSerializedAsElement(context.Model) || !EdmUtil.IsNullOrWhiteSpaceInternal(annotation.NamespaceUri) && !EdmUtil.IsNullOrWhiteSpaceInternal(annotation.Name))
        return;
      context.AddError(annotation.Location(), EdmErrorCode.InvalidElementAnnotation, Strings.EdmModel_Validator_Semantic_InvalidElementAnnotationMismatchedTerm);
    }));
    public static readonly ValidationRule<IEdmDirectValueAnnotation> ImmediateValueAnnotationElementAnnotationHasNameAndNamespace = new ValidationRule<IEdmDirectValueAnnotation>((Action<ValidationContext, IEdmDirectValueAnnotation>) ((context, annotation) =>
    {
      EdmError error;
      if (!(annotation.Value is IEdmStringValue edmStringValue4) || !edmStringValue4.IsSerializedAsElement(context.Model) || ValidationHelper.ValidateValueCanBeWrittenAsXmlElementAnnotation((IEdmValue) edmStringValue4, annotation.NamespaceUri, annotation.Name, out error))
        return;
      context.AddError(error);
    }));
    public static readonly ValidationRule<IEdmDirectValueAnnotation> DirectValueAnnotationHasXmlSerializableName = new ValidationRule<IEdmDirectValueAnnotation>((Action<ValidationContext, IEdmDirectValueAnnotation>) ((context, annotation) =>
    {
      string name = annotation.Name;
      if (EdmUtil.IsNullOrWhiteSpaceInternal(name) || name.Length > 480)
        return;
      if (name.Length <= 0)
        return;
      try
      {
        XmlConvert.VerifyNCName(annotation.Name);
      }
      catch (XmlException ex)
      {
        EdmLocation errorLocation = !(annotation.Value is IEdmValue edmValue2) ? (EdmLocation) null : edmValue2.Location();
        context.AddError(new EdmError(errorLocation, EdmErrorCode.InvalidName, Strings.EdmModel_Validator_Syntactic_EdmModel_NameIsNotAllowed((object) annotation.Name)));
      }
    }));
    public static readonly ValidationRule<IEdmVocabularyAnnotation> VocabularyAnnotationInaccessibleTarget = new ValidationRule<IEdmVocabularyAnnotation>((Action<ValidationContext, IEdmVocabularyAnnotation>) ((context, annotation) =>
    {
      IEdmVocabularyAnnotatable target = annotation.Target;
      bool flag = false;
      switch (target)
      {
        case IEdmEntityContainer element5:
          flag = context.Model.FindEntityContainer(element5.FullName()) != null;
          break;
        case IEdmEntitySet edmEntitySet2:
          IEdmEntityContainer container = edmEntitySet2.Container;
          if (container != null)
          {
            flag = container.FindEntitySetExtended(edmEntitySet2.Name) != null;
            break;
          }
          break;
        case IEdmSchemaType element6:
          flag = context.Model.FindType(element6.FullName()) != null;
          break;
        case IEdmTerm element7:
          flag = context.Model.FindTerm(element7.FullName()) != null;
          break;
        case IEdmOperation element8:
          flag = context.Model.FindOperations(element8.FullName()).Any<IEdmOperation>();
          break;
        case IEdmOperationImport edmOperationImport4:
          flag = edmOperationImport4.Container.FindOperationImportsExtended(edmOperationImport4.Name).Any<IEdmOperationImport>();
          break;
        case IEdmProperty edmProperty2:
          string qualifiedName = EdmUtil.FullyQualifiedName((IEdmVocabularyAnnotatable) (edmProperty2.DeclaringType as IEdmSchemaType));
          if (context.Model.FindType(qualifiedName) is IEdmStructuredType type6)
          {
            flag = type6.FindProperty(edmProperty2.Name) != null;
            break;
          }
          break;
        case IEdmOperationParameter operationParameter2:
          IEdmOperation declaringOperation2 = operationParameter2.DeclaringOperation;
          if (declaringOperation2 != null)
          {
            using (IEnumerator<IEdmOperation> enumerator = context.Model.FindOperations(declaringOperation2.FullName()).GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                if (enumerator.Current.FindParameter(operationParameter2.Name) != null)
                {
                  flag = true;
                  break;
                }
              }
              break;
            }
          }
          else if (operationParameter2.DeclaringOperation is IEdmOperationImport declaringOperation3)
          {
            using (IEnumerator<IEdmOperationImport> enumerator = declaringOperation3.Container.FindOperationImportsExtended(declaringOperation3.Name).GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                if (enumerator.Current.Operation.FindParameter(operationParameter2.Name) != null)
                {
                  flag = true;
                  break;
                }
              }
              break;
            }
          }
          else
            break;
        default:
          flag = true;
          break;
      }
      if (flag)
        return;
      context.AddError(annotation.Location(), EdmErrorCode.BadUnresolvedTarget, Strings.EdmModel_Validator_Semantic_InaccessibleTarget((object) EdmUtil.FullyQualifiedName(target)));
    }));
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly ValidationRule<IEdmVocabularyAnnotation> VocabularyAnnotationAssertCorrectExpressionType = new ValidationRule<IEdmVocabularyAnnotation>((Action<ValidationContext, IEdmVocabularyAnnotation>) ((context, annotation) =>
    {
      IEnumerable<EdmError> discoveredErrors;
      if (annotation.Value.TryCast(annotation.Term.Type, out discoveredErrors))
        return;
      foreach (EdmError error in discoveredErrors)
      {
        if (error.ErrorCode != EdmErrorCode.RecordExpressionMissingRequiredProperty)
          context.AddError(error);
      }
    }));
    public static readonly ValidationRule<IEdmVocabularyAnnotation> AnnotationInaccessibleTerm = new ValidationRule<IEdmVocabularyAnnotation>((Action<ValidationContext, IEdmVocabularyAnnotation>) ((context, annotation) =>
    {
      IEdmTerm term = annotation.Term;
      if (term is IUnresolvedElement || context.Model.FindTerm(term.FullName()) != null)
        return;
      context.AddError(annotation.Location(), EdmErrorCode.BadUnresolvedTerm, Strings.EdmModel_Validator_Semantic_InaccessibleTerm((object) annotation.Term.FullName()));
    }));
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
    public static readonly ValidationRule<IEdmVocabularyAnnotation> VocabularyAnnotationTargetAllowedApplyToElement = new ValidationRule<IEdmVocabularyAnnotation>((Action<ValidationContext, IEdmVocabularyAnnotation>) ((context, annotation) =>
    {
      IEdmTerm term = annotation.Term;
      if (term.AppliesTo == null)
        return;
      if (new HashSet<string>(((IEnumerable<string>) term.AppliesTo.Split(' ')).Select<string, string>((Func<string, string>) (e => e.Trim()))).Contains(annotation.Target.GetSymbolicString()))
        return;
      context.AddError(annotation.Location(), EdmErrorCode.AnnotationApplyToNotAllowedAnnotatable, Strings.EdmModel_Validator_Semantic_VocabularyAnnotationApplyToNotAllowedAnnotatable((object) EdmUtil.FullyQualifiedName(annotation.Target), (object) term.AppliesTo, (object) term.FullName()));
    }));
    public static readonly ValidationRule<IEdmPropertyValueBinding> PropertyValueBindingValueIsCorrectType = new ValidationRule<IEdmPropertyValueBinding>((Action<ValidationContext, IEdmPropertyValueBinding>) ((context, binding) =>
    {
      IEnumerable<EdmError> discoveredErrors;
      if (binding.Value.TryCast(binding.BoundProperty.Type, out discoveredErrors) || context.IsBad((IEdmElement) binding) || context.IsBad((IEdmElement) binding.BoundProperty))
        return;
      foreach (EdmError error in discoveredErrors)
        context.AddError(error);
    }));
    public static readonly ValidationRule<IEdmIfExpression> IfExpressionAssertCorrectTestType = new ValidationRule<IEdmIfExpression>((Action<ValidationContext, IEdmIfExpression>) ((context, expression) =>
    {
      IEnumerable<EdmError> discoveredErrors;
      if (expression.TestExpression.TryCast((IEdmTypeReference) EdmCoreModel.Instance.GetBoolean(false), out discoveredErrors))
        return;
      foreach (EdmError error in discoveredErrors)
        context.AddError(error);
    }));
    public static readonly ValidationRule<IEdmCollectionExpression> CollectionExpressionAllElementsCorrectType = new ValidationRule<IEdmCollectionExpression>((Action<ValidationContext, IEdmCollectionExpression>) ((context, expression) =>
    {
      if (expression.DeclaredType == null || context.IsBad((IEdmElement) expression) || context.IsBad((IEdmElement) expression.DeclaredType))
        return;
      IEnumerable<EdmError> discoveredErrors;
      expression.TryCastCollectionAsType(expression.DeclaredType, (IEdmType) null, false, out discoveredErrors);
      foreach (EdmError error in discoveredErrors)
        context.AddError(error);
    }));
    public static readonly ValidationRule<IEdmRecordExpression> RecordExpressionPropertiesMatchType = new ValidationRule<IEdmRecordExpression>((Action<ValidationContext, IEdmRecordExpression>) ((context, expression) =>
    {
      if (expression.DeclaredType == null || context.IsBad((IEdmElement) expression) || context.IsBad((IEdmElement) expression.DeclaredType))
        return;
      IEnumerable<EdmError> discoveredErrors;
      expression.TryCastRecordAsType((IEdmTypeReference) expression.DeclaredType, (IEdmType) null, false, out discoveredErrors);
      foreach (EdmError error in discoveredErrors)
        context.AddError(error);
    }));
    public static readonly ValidationRule<IEdmApplyExpression> FunctionApplicationExpressionParametersMatchAppliedFunction = new ValidationRule<IEdmApplyExpression>((Action<ValidationContext, IEdmApplyExpression>) ((context, expression) =>
    {
      IEdmFunction appliedFunction = expression.AppliedFunction;
      if (appliedFunction == null || context.IsBad((IEdmElement) appliedFunction))
        return;
      if (appliedFunction.Parameters.Count<IEdmOperationParameter>() != expression.Arguments.Count<IEdmExpression>())
        context.AddError(new EdmError(expression.Location(), EdmErrorCode.IncorrectNumberOfArguments, Strings.EdmModel_Validator_Semantic_IncorrectNumberOfArguments((object) expression.Arguments.Count<IEdmExpression>(), (object) appliedFunction.FullName(), (object) appliedFunction.Parameters.Count<IEdmOperationParameter>())));
      IEnumerator<IEdmExpression> enumerator = expression.Arguments.GetEnumerator();
      foreach (IEdmOperationParameter parameter in appliedFunction.Parameters)
      {
        enumerator.MoveNext();
        IEnumerable<EdmError> discoveredErrors;
        if (!enumerator.Current.TryCast(parameter.Type, out discoveredErrors))
        {
          foreach (EdmError error in discoveredErrors)
            context.AddError(error);
        }
      }
    }));
    public static readonly ValidationRule<IEdmVocabularyAnnotatable> VocabularyAnnotatableNoDuplicateAnnotations = new ValidationRule<IEdmVocabularyAnnotatable>((Action<ValidationContext, IEdmVocabularyAnnotatable>) ((context, annotatable) =>
    {
      HashSetInternal<string> hashSetInternal = new HashSetInternal<string>();
      foreach (IEdmVocabularyAnnotation vocabularyAnnotation in context.Model.FindDeclaredVocabularyAnnotations(annotatable))
      {
        if (!hashSetInternal.Add(vocabularyAnnotation.Term.FullName() + ":" + vocabularyAnnotation.Qualifier))
          context.AddError(new EdmError(vocabularyAnnotation.Location(), EdmErrorCode.DuplicateAnnotation, Strings.EdmModel_Validator_Semantic_DuplicateAnnotation((object) EdmUtil.FullyQualifiedName(annotatable), (object) vocabularyAnnotation.Term.FullName(), (object) vocabularyAnnotation.Qualifier)));
      }
    }));
    public static readonly ValidationRule<IEdmPrimitiveValue> PrimitiveValueValidForType = new ValidationRule<IEdmPrimitiveValue>((Action<ValidationContext, IEdmPrimitiveValue>) ((context, value) =>
    {
      if (value.Type == null || context.IsBad((IEdmElement) value) || context.IsBad((IEdmElement) value.Type))
        return;
      IEnumerable<EdmError> discoveredErrors;
      value.TryCastPrimitiveAsType(value.Type, out discoveredErrors);
      foreach (EdmError error in discoveredErrors)
        context.AddError(error);
    }));

    private static void CheckForUnreacheableTypeError(
      ValidationContext context,
      IEdmSchemaType type,
      EdmLocation location)
    {
      IEdmType type1 = (IEdmType) context.Model.FindType(type.FullName());
      if (type1 is AmbiguousTypeBinding)
      {
        context.AddError(location, EdmErrorCode.BadAmbiguousElementBinding, Strings.EdmModel_Validator_Semantic_AmbiguousType((object) type.FullName()));
      }
      else
      {
        if (type1.IsEquivalentTo((IEdmType) type))
          return;
        context.AddError(location, EdmErrorCode.BadUnresolvedType, Strings.EdmModel_Validator_Semantic_InaccessibleType((object) type.FullName()));
      }
    }

    private static bool TryResolveNavigationPropertyBindingPath(
      IEdmModel model,
      IEdmNavigationSource navigationSource,
      IEdmNavigationPropertyBinding binding)
    {
      string[] array = binding.Path.PathSegments.ToArray<string>();
      otherType = (IEdmStructuredType) navigationSource.EntityType();
      for (int index = 0; index < array.Length - 1; ++index)
      {
        string str = array[index];
        if (str.IndexOf('.') < 0)
        {
          IEdmProperty property = otherType.FindProperty(str);
          if (property == null || property is IEdmNavigationProperty navigationProperty && !navigationProperty.ContainsTarget || !(property.Type.Definition.AsElementType() is IEdmStructuredType otherType))
            return false;
        }
        else
        {
          if (!(model.FindType(str) is IEdmStructuredType type) || !type.IsOrInheritsFrom((IEdmType) otherType))
            return false;
          otherType = type;
        }
      }
      return otherType.FindProperty(((IEnumerable<string>) array).Last<string>()) is IEdmNavigationProperty;
    }

    private static bool HasPathTypeProperty(
      IEdmStructuredType structuredType,
      IList<IEdmStructuredType> visited)
    {
      if (structuredType == null || visited == null || visited.Any<IEdmStructuredType>((Func<IEdmStructuredType, bool>) (c => c == structuredType)))
        return false;
      visited.Add(structuredType);
      IEdmStructuredType baseType = structuredType.BaseType;
      if (baseType != null && ValidationRules.HasPathTypeProperty(baseType, visited))
        return true;
      foreach (IEdmProperty declaredProperty in structuredType.DeclaredProperties)
      {
        IEdmTypeReference type = declaredProperty.Type;
        if (type.IsCollection())
          type = type.AsCollection().ElementType();
        if (type.IsStructured())
        {
          if (ValidationRules.HasPathTypeProperty(type.AsStructured().StructuredDefinition(), visited))
            return true;
        }
        else if (type.IsPath())
          return true;
      }
      return false;
    }

    internal class EdmTypeReferenceComparer : IEqualityComparer<IEdmTypeReference>
    {
      public bool Equals(IEdmTypeReference x, IEdmTypeReference y) => x.IsEquivalentTo(y);

      public int GetHashCode(IEdmTypeReference obj)
      {
        string str = obj.FullName();
        return str == null ? 0 : str.GetHashCode();
      }
    }
  }
}
