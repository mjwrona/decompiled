// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Validation.ValidationRuleSet
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Validation
{
  public sealed class ValidationRuleSet : IEnumerable<ValidationRule>, IEnumerable
  {
    private readonly Dictionary<Type, List<ValidationRule>> rules;
    private static readonly ValidationRuleSet BaseRuleSet = new ValidationRuleSet((IEnumerable<ValidationRule>) new ValidationRule[99]
    {
      (ValidationRule) ValidationRules.EntityTypeKeyPropertyMustBelongToEntity,
      (ValidationRule) ValidationRules.StructuredTypePropertiesDeclaringTypeMustBeCorrect,
      (ValidationRule) ValidationRules.NamedElementNameMustNotBeEmptyOrWhiteSpace,
      (ValidationRule) ValidationRules.NamedElementNameIsTooLong,
      (ValidationRule) ValidationRules.NamedElementNameIsNotAllowed,
      (ValidationRule) ValidationRules.SchemaElementNamespaceIsNotAllowed,
      (ValidationRule) ValidationRules.SchemaElementNamespaceIsTooLong,
      (ValidationRule) ValidationRules.SchemaElementNamespaceMustNotBeEmptyOrWhiteSpace,
      (ValidationRule) ValidationRules.SchemaElementSystemNamespaceEncountered,
      (ValidationRule) ValidationRules.EntityContainerDuplicateEntityContainerMemberName,
      (ValidationRule) ValidationRules.EntityTypeDuplicatePropertyNameSpecifiedInEntityKey,
      (ValidationRule) ValidationRules.EntityTypeInvalidKeyNullablePart,
      (ValidationRule) ValidationRules.EntityTypeEntityKeyMustBeScalar,
      (ValidationRule) ValidationRules.EntityTypeInvalidKeyKeyDefinedInBaseClass,
      (ValidationRule) ValidationRules.EntityTypeBoundEscapeFunctionMustBeUnique,
      (ValidationRule) ValidationRules.StructuredTypeInvalidMemberNameMatchesTypeName,
      (ValidationRule) ValidationRules.StructuredTypePropertyNameAlreadyDefined,
      (ValidationRule) ValidationRules.StructuralPropertyInvalidPropertyType,
      (ValidationRule) ValidationRules.OperationParameterNameAlreadyDefinedDuplicate,
      (ValidationRule) ValidationRules.OperationImportEntityTypeDoesNotMatchEntitySet,
      (ValidationRule) ValidationRules.OperationImportCannotImportBoundOperation,
      (ValidationRule) ValidationRules.StructuredTypeBaseTypeMustBeSameKindAsDerivedKind,
      (ValidationRule) ValidationRules.NavigationPropertyWithRecursiveContainmentTargetMustBeOptional,
      (ValidationRule) ValidationRules.NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne,
      (ValidationRule) ValidationRules.NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne,
      (ValidationRule) ValidationRules.EntitySetTypeMustBeCollectionOfEntityType,
      (ValidationRule) ValidationRules.NavigationSourceInaccessibleEntityType,
      (ValidationRule) ValidationRules.StructuredTypeInaccessibleBaseType,
      (ValidationRule) ValidationRules.EntityReferenceTypeInaccessibleEntityType,
      (ValidationRule) ValidationRules.TypeReferenceInaccessibleSchemaType,
      (ValidationRule) ValidationRules.NavigationSourceTypeHasNoKeys,
      (ValidationRule) ValidationRules.DecimalTypeReferenceScaleOutOfRange,
      (ValidationRule) ValidationRules.BinaryTypeReferenceBinaryMaxLengthNegative,
      (ValidationRule) ValidationRules.StringTypeReferenceStringMaxLengthNegative,
      (ValidationRule) ValidationRules.EnumMemberValueMustHaveSameTypeAsUnderlyingType,
      (ValidationRule) ValidationRules.EnumTypeEnumMemberNameAlreadyDefined,
      (ValidationRule) ValidationRules.BoundOperationMustHaveParameters,
      (ValidationRule) ValidationRules.OptionalParametersMustComeAfterRequiredParameters,
      (ValidationRule) ValidationRules.OperationEntitySetPathMustBeValid,
      (ValidationRule) ValidationRules.OperationReturnTypeEntityTypeMustBeValid,
      (ValidationRule) ValidationRules.OperationImportEntitySetExpressionIsInvalid,
      (ValidationRule) ValidationRules.FunctionImportWithParameterShouldNotBeIncludedInServiceDocument,
      (ValidationRule) ValidationRules.BinaryTypeReferenceBinaryUnboundedNotValidForMaxLength,
      (ValidationRule) ValidationRules.StringTypeReferenceStringUnboundedNotValidForMaxLength,
      (ValidationRule) ValidationRules.ImmediateValueAnnotationElementAnnotationIsValid,
      (ValidationRule) ValidationRules.VocabularyAnnotationAssertCorrectExpressionType,
      (ValidationRule) ValidationRules.IfExpressionAssertCorrectTestType,
      (ValidationRule) ValidationRules.CollectionExpressionAllElementsCorrectType,
      (ValidationRule) ValidationRules.RecordExpressionPropertiesMatchType,
      (ValidationRule) ValidationRules.NavigationPropertyDependentPropertiesMustBelongToDependentEntity,
      (ValidationRule) ValidationRules.NavigationPropertyInvalidOperationMultipleEndsInAssociatedNavigationProperties,
      (ValidationRule) ValidationRules.NavigationPropertyEndWithManyMultiplicityCannotHaveOperationsSpecified,
      (ValidationRule) ValidationRules.NavigationPropertyPartnerPathShouldBeResolvable,
      (ValidationRule) ValidationRules.NavigationPropertyTypeMismatchRelationshipConstraint,
      (ValidationRule) ValidationRules.NavigationPropertyDuplicateDependentProperty,
      (ValidationRule) ValidationRules.NavigationPropertyPrincipalEndMultiplicity,
      (ValidationRule) ValidationRules.NavigationPropertyDependentEndMultiplicity,
      (ValidationRule) ValidationRules.NavigationPropertyCorrectType,
      (ValidationRule) ValidationRules.NavigationPropertyBindingPathMustBeResolvable,
      (ValidationRule) ValidationRules.ImmediateValueAnnotationElementAnnotationHasNameAndNamespace,
      (ValidationRule) ValidationRules.OpenComplexTypeCannotHaveClosedDerivedComplexType,
      (ValidationRule) ValidationRules.FunctionApplicationExpressionParametersMatchAppliedFunction,
      (ValidationRule) ValidationRules.VocabularyAnnotatableNoDuplicateAnnotations,
      (ValidationRule) ValidationRules.TemporalTypeReferencePrecisionOutOfRange,
      (ValidationRule) ValidationRules.DecimalTypeReferencePrecisionOutOfRange,
      (ValidationRule) ValidationRules.ModelDuplicateEntityContainerName,
      (ValidationRule) ValidationRules.ModelBoundFunctionOverloadsMustHaveSameReturnType,
      (ValidationRule) ValidationRules.UnBoundFunctionOverloadsMustHaveIdenticalReturnTypes,
      (ValidationRule) ValidationRules.TypeMustNotHaveKindOfNone,
      (ValidationRule) ValidationRules.PrimitiveTypeMustNotHaveKindOfNone,
      (ValidationRule) ValidationRules.PropertyMustNotHaveKindOfNone,
      (ValidationRule) ValidationRules.SchemaElementMustNotHaveKindOfNone,
      (ValidationRule) ValidationRules.EntityContainerElementMustNotHaveKindOfNone,
      (ValidationRule) ValidationRules.PrimitiveValueValidForType,
      (ValidationRule) ValidationRules.EntitySetCanOnlyBeContainedByASingleNavigationProperty,
      (ValidationRule) ValidationRules.NavigationMappingMustBeBidirectional,
      (ValidationRule) ValidationRules.SingletonTypeMustBeEntityType,
      (ValidationRule) ValidationRules.NavigationPropertyMappingsMustBeUnique,
      (ValidationRule) ValidationRules.PropertyValueBindingValueIsCorrectType,
      (ValidationRule) ValidationRules.EnumMustHaveIntegerUnderlyingType,
      (ValidationRule) ValidationRules.AnnotationInaccessibleTerm,
      (ValidationRule) ValidationRules.ElementDirectValueAnnotationFullNameMustBeUnique,
      (ValidationRule) ValidationRules.VocabularyAnnotationInaccessibleTarget,
      (ValidationRule) ValidationRules.EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet,
      (ValidationRule) ValidationRules.NavigationPropertyMappingMustPointToValidTargetForProperty,
      (ValidationRule) ValidationRules.DirectValueAnnotationHasXmlSerializableName,
      (ValidationRule) ValidationRules.FunctionMustHaveReturnType,
      (ValidationRule) ValidationRules.FunctionWithUrlEscapeFunctionMustBeBound,
      (ValidationRule) ValidationRules.FunctionWithUrlEscapeFunctionMustHaveOneStringParameter,
      (ValidationRule) ValidationRules.EntitySetTypeCannotBeEdmEntityType,
      (ValidationRule) ValidationRules.SingletonTypeCannotBeEdmEntityType,
      (ValidationRule) ValidationRules.OperationReturnTypeCannotBeCollectionOfAbstractType,
      (ValidationRule) ValidationRules.PropertyTypeCannotBeCollectionOfAbstractType,
      (ValidationRule) ValidationRules.EntityTypeKeyTypeCannotBeEdmPrimitiveType,
      (ValidationRule) ValidationRules.TypeDefinitionUnderlyingTypeCannotBeEdmPrimitiveType,
      (ValidationRule) ValidationRules.EnumUnderlyingTypeCannotBeEdmPrimitiveType,
      (ValidationRule) ValidationRules.StructuredTypeBaseTypeCannotBeAbstractType,
      (ValidationRule) ValidationRules.NavigationSourceDeclaringTypeCannotHavePathTypeProperty,
      (ValidationRule) ValidationRules.NavigationPropertyTypeCannotHavePathTypeProperty
    });
    private static readonly ValidationRuleSet V4RuleSet = new ValidationRuleSet((IEnumerable<ValidationRule>) ValidationRuleSet.BaseRuleSet, (IEnumerable<ValidationRule>) new ValidationRule[2]
    {
      (ValidationRule) ValidationRules.OperationUnsupportedReturnType,
      (ValidationRule) ValidationRules.ModelDuplicateSchemaElementName
    });

    public ValidationRuleSet(
      IEnumerable<ValidationRule> baseSet,
      IEnumerable<ValidationRule> newRules)
      : this(EdmUtil.CheckArgumentNull<IEnumerable<ValidationRule>>(baseSet, nameof (baseSet)).Concat<ValidationRule>(EdmUtil.CheckArgumentNull<IEnumerable<ValidationRule>>(newRules, nameof (newRules))))
    {
    }

    public ValidationRuleSet(IEnumerable<ValidationRule> rules)
    {
      EdmUtil.CheckArgumentNull<IEnumerable<ValidationRule>>(rules, nameof (rules));
      this.rules = new Dictionary<Type, List<ValidationRule>>();
      foreach (ValidationRule rule in rules)
        this.AddRule(rule);
    }

    public static ValidationRuleSet GetEdmModelRuleSet(Version version)
    {
      if (version == EdmConstants.EdmVersion4 || version == EdmConstants.EdmVersion401)
        return ValidationRuleSet.V4RuleSet;
      throw new InvalidOperationException(Strings.Serializer_UnknownEdmVersion);
    }

    public IEnumerator<ValidationRule> GetEnumerator()
    {
      foreach (List<ValidationRule> validationRuleList in this.rules.Values)
      {
        foreach (ValidationRule validationRule in validationRuleList)
          yield return validationRule;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    internal IEnumerable<ValidationRule> GetRules(Type t)
    {
      List<ValidationRule> validationRuleList;
      return !this.rules.TryGetValue(t, out validationRuleList) ? Enumerable.Empty<ValidationRule>() : (IEnumerable<ValidationRule>) validationRuleList;
    }

    private void AddRule(ValidationRule rule)
    {
      List<ValidationRule> validationRuleList;
      if (!this.rules.TryGetValue(rule.ValidatedType, out validationRuleList))
      {
        validationRuleList = new List<ValidationRule>();
        this.rules[rule.ValidatedType] = validationRuleList;
      }
      if (validationRuleList.Contains(rule))
        throw new InvalidOperationException(Strings.RuleSet_DuplicateRulesExistInRuleSet);
      validationRuleList.Add(rule);
    }
  }
}
