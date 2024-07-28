// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Strings
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm
{
  internal static class Strings
  {
    internal static string EdmPrimitive_UnexpectedKind => EntityRes.GetString(nameof (EdmPrimitive_UnexpectedKind));

    internal static string EdmPath_UnexpectedKind => EntityRes.GetString(nameof (EdmPath_UnexpectedKind));

    internal static string Annotations_TypeMismatch(object p0, object p1) => EntityRes.GetString(nameof (Annotations_TypeMismatch), p0, p1);

    internal static string Constructable_VocabularyAnnotationMustHaveTarget => EntityRes.GetString(nameof (Constructable_VocabularyAnnotationMustHaveTarget));

    internal static string Constructable_EntityTypeOrCollectionOfEntityTypeExpected => EntityRes.GetString(nameof (Constructable_EntityTypeOrCollectionOfEntityTypeExpected));

    internal static string Constructable_TargetMustBeStock(object p0) => EntityRes.GetString(nameof (Constructable_TargetMustBeStock), p0);

    internal static string TypeSemantics_CouldNotConvertTypeReference(object p0, object p1) => EntityRes.GetString(nameof (TypeSemantics_CouldNotConvertTypeReference), p0, p1);

    internal static string EdmModel_CannotUseElementWithTypeNone => EntityRes.GetString(nameof (EdmModel_CannotUseElementWithTypeNone));

    internal static string EdmModel_CannotAddMoreThanOneEntityContainerToOneEdmModel => EntityRes.GetString(nameof (EdmModel_CannotAddMoreThanOneEntityContainerToOneEdmModel));

    internal static string EdmEntityContainer_CannotUseElementWithTypeNone => EntityRes.GetString(nameof (EdmEntityContainer_CannotUseElementWithTypeNone));

    internal static string ValueWriter_NonSerializableValue(object p0) => EntityRes.GetString(nameof (ValueWriter_NonSerializableValue), p0);

    internal static string ValueHasAlreadyBeenSet => EntityRes.GetString(nameof (ValueHasAlreadyBeenSet));

    internal static string PathSegmentMustNotContainSlash => EntityRes.GetString(nameof (PathSegmentMustNotContainSlash));

    internal static string Constructable_DependentPropertyCountMustMatchNumberOfPropertiesOnPrincipalType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (Constructable_DependentPropertyCountMustMatchNumberOfPropertiesOnPrincipalType), p0, p1);
    }

    internal static string EdmType_UnexpectedEdmType => EntityRes.GetString(nameof (EdmType_UnexpectedEdmType));

    internal static string NavigationPropertyBinding_PathIsNotValid => EntityRes.GetString(nameof (NavigationPropertyBinding_PathIsNotValid));

    internal static string Edm_Evaluator_NoTermTypeAnnotationOnType(object p0, object p1) => EntityRes.GetString(nameof (Edm_Evaluator_NoTermTypeAnnotationOnType), p0, p1);

    internal static string Edm_Evaluator_NoValueAnnotationOnType(object p0, object p1) => EntityRes.GetString(nameof (Edm_Evaluator_NoValueAnnotationOnType), p0, p1);

    internal static string Edm_Evaluator_NoValueAnnotationOnElement(object p0) => EntityRes.GetString(nameof (Edm_Evaluator_NoValueAnnotationOnElement), p0);

    internal static string Edm_Evaluator_UnrecognizedExpressionKind(object p0) => EntityRes.GetString(nameof (Edm_Evaluator_UnrecognizedExpressionKind), p0);

    internal static string Edm_Evaluator_UnboundFunction(object p0) => EntityRes.GetString(nameof (Edm_Evaluator_UnboundFunction), p0);

    internal static string Edm_Evaluator_UnboundPath(object p0) => EntityRes.GetString(nameof (Edm_Evaluator_UnboundPath), p0);

    internal static string Edm_Evaluator_NoContextPath => EntityRes.GetString(nameof (Edm_Evaluator_NoContextPath));

    internal static string Edm_Evaluator_FailedTypeAssertion(object p0) => EntityRes.GetString(nameof (Edm_Evaluator_FailedTypeAssertion), p0);

    internal static string Edm_Evaluator_TypeCastNeedsEdmModel => EntityRes.GetString(nameof (Edm_Evaluator_TypeCastNeedsEdmModel));

    internal static string EdmModel_Validator_Semantic_SystemNamespaceEncountered(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_SystemNamespaceEncountered), p0);

    internal static string EdmModel_Validator_Semantic_NavigationSourceTypeHasNoKeys(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_NavigationSourceTypeHasNoKeys), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_DuplicateEndName(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_DuplicateEndName), p0);

    internal static string EdmModel_Validator_Semantic_DuplicatePropertyNameSpecifiedInEntityKey(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_DuplicatePropertyNameSpecifiedInEntityKey), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_InvalidComplexTypeAbstract(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidComplexTypeAbstract), p0);

    internal static string EdmModel_Validator_Semantic_InvalidComplexTypePolymorphic(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidComplexTypePolymorphic), p0);

    internal static string EdmModel_Validator_Semantic_InvalidKeyNullablePart(object p0, object p1) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidKeyNullablePart), p0, p1);

    internal static string EdmModel_Validator_Semantic_EntityKeyMustBeScalar(object p0, object p1) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EntityKeyMustBeScalar), p0, p1);

    internal static string EdmModel_Validator_Semantic_EntityComposableBoundEscapeFunctionMustBeLessOne(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EntityComposableBoundEscapeFunctionMustBeLessOne), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_EntityNoncomposableBoundEscapeFunctionMustBeLessOne(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EntityNoncomposableBoundEscapeFunctionMustBeLessOne), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_InvalidKeyKeyDefinedInBaseClass(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidKeyKeyDefinedInBaseClass), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_KeyMissingOnEntityType(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_KeyMissingOnEntityType), p0);

    internal static string EdmModel_Validator_Semantic_BadNavigationPropertyUndefinedRole(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_BadNavigationPropertyUndefinedRole), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_BadNavigationPropertyRolesCannotBeTheSame(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_BadNavigationPropertyRolesCannotBeTheSame), p0);
    }

    internal static string EdmModel_Validator_Semantic_BadNavigationPropertyCouldNotDetermineType(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_BadNavigationPropertyCouldNotDetermineType), p0);
    }

    internal static string EdmModel_Validator_Semantic_InvalidOperationMultipleEndsInAssociation => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidOperationMultipleEndsInAssociation));

    internal static string EdmModel_Validator_Semantic_EndWithManyMultiplicityCannotHaveOperationsSpecified(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EndWithManyMultiplicityCannotHaveOperationsSpecified), p0);
    }

    internal static string EdmModel_Validator_Semantic_EndNameAlreadyDefinedDuplicate(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EndNameAlreadyDefinedDuplicate), p0);

    internal static string EdmModel_Validator_Semantic_SameRoleReferredInReferentialConstraint(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_SameRoleReferredInReferentialConstraint), p0);
    }

    internal static string EdmModel_Validator_Semantic_NavigationPropertyPrincipalEndMultiplicityUpperBoundMustBeOne(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_NavigationPropertyPrincipalEndMultiplicityUpperBoundMustBeOne), p0);
    }

    internal static string EdmModel_Validator_Semantic_InvalidMultiplicityOfPrincipalEndDependentPropertiesAllNonnullable(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidMultiplicityOfPrincipalEndDependentPropertiesAllNonnullable), p0);
    }

    internal static string EdmModel_Validator_Semantic_InvalidMultiplicityOfPrincipalEndDependentPropertiesAllNullable(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidMultiplicityOfPrincipalEndDependentPropertiesAllNullable), p0);
    }

    internal static string EdmModel_Validator_Semantic_InvalidMultiplicityOfDependentEndMustBeZeroOneOrOne(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidMultiplicityOfDependentEndMustBeZeroOneOrOne), p0);
    }

    internal static string EdmModel_Validator_Semantic_InvalidMultiplicityOfDependentEndMustBeMany(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidMultiplicityOfDependentEndMustBeMany), p0);
    }

    internal static string EdmModel_Validator_Semantic_MismatchNumberOfPropertiesinRelationshipConstraint => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_MismatchNumberOfPropertiesinRelationshipConstraint));

    internal static string EdmModel_Validator_Semantic_TypeMismatchRelationshipConstraint(
      object p0,
      object p1,
      object p2,
      object p3,
      object p4)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_TypeMismatchRelationshipConstraint), p0, p1, p2, p3, p4);
    }

    internal static string EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraintDependentEnd(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraintDependentEnd), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraintPrimaryEnd(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraintPrimaryEnd), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_InvalidPropertyType(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidPropertyType), p0);

    internal static string EdmModel_Validator_Semantic_BoundOperationMustHaveParameters(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_BoundOperationMustHaveParameters), p0);

    internal static string EdmModel_Validator_Semantic_RequiredParametersMustPrecedeOptional(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_RequiredParametersMustPrecedeOptional), p0);
    }

    internal static string EdmModel_Validator_Semantic_OperationWithUnsupportedReturnType(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_OperationWithUnsupportedReturnType), p0);

    internal static string EdmModel_Validator_Semantic_OperationImportEntityTypeDoesNotMatchEntitySet(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_OperationImportEntityTypeDoesNotMatchEntitySet), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_OperationImportEntityTypeDoesNotMatchEntitySet2(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_OperationImportEntityTypeDoesNotMatchEntitySet2), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_OperationImportEntitySetExpressionKindIsInvalid(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_OperationImportEntitySetExpressionKindIsInvalid), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid), p0);
    }

    internal static string EdmModel_Validator_Semantic_OperationImportSpecifiesEntitySetButNotEntityType(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_OperationImportSpecifiesEntitySetButNotEntityType), p0);
    }

    internal static string EdmModel_Validator_Semantic_OperationImportCannotImportBoundOperation(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_OperationImportCannotImportBoundOperation), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_FunctionImportWithParameterShouldNotBeIncludedInServiceDocument(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_FunctionImportWithParameterShouldNotBeIncludedInServiceDocument), p0);
    }

    internal static string EdmModel_Validator_Semantic_FunctionMustHaveReturnType(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_FunctionMustHaveReturnType), p0);

    internal static string EdmModel_Validator_Semantic_UrlEscapeFunctionMustBoundFunction(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_UrlEscapeFunctionMustBoundFunction), p0);

    internal static string EdmModel_Validator_Semantic_UrlEscapeFunctionMustHaveOneStringParameter(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_UrlEscapeFunctionMustHaveOneStringParameter), p0);
    }

    internal static string EdmModel_Validator_Semantic_ParameterNameAlreadyDefinedDuplicate(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_ParameterNameAlreadyDefinedDuplicate), p0);
    }

    internal static string EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName), p0);

    internal static string EdmModel_Validator_Semantic_UnboundFunctionOverloadHasIncorrectReturnType(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_UnboundFunctionOverloadHasIncorrectReturnType), p0);
    }

    internal static string EdmModel_Validator_Semantic_OperationCannotHaveEntitySetPathWithUnBoundOperation(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_OperationCannotHaveEntitySetPathWithUnBoundOperation), p0);
    }

    internal static string EdmModel_Validator_Semantic_InvalidEntitySetPathMissingBindingParameterName(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidEntitySetPathMissingBindingParameterName), p0);
    }

    internal static string EdmModel_Validator_Semantic_InvalidEntitySetPathWithFirstPathParameterNotMatchingFirstParameterName(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidEntitySetPathWithFirstPathParameterNotMatchingFirstParameterName), p0, p1, p2, p3);
    }

    internal static string EdmModel_Validator_Semantic_InvalidEntitySetPathTypeCastSegmentMustBeEntityType(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidEntitySetPathTypeCastSegmentMustBeEntityType), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_InvalidEntitySetPathUnknownNavigationProperty(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidEntitySetPathUnknownNavigationProperty), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_InvalidEntitySetPathInvalidTypeCastSegment(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidEntitySetPathInvalidTypeCastSegment), p0, p1, p2, p3);
    }

    internal static string EdmModel_Validator_Semantic_InvalidEntitySetPathWithNonEntityBindingParameter(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidEntitySetPathWithNonEntityBindingParameter), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_InvalidEntitySetPathUnknownTypeCastSegment(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidEntitySetPathUnknownTypeCastSegment), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_OperationWithEntitySetPathReturnTypeInvalid(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_OperationWithEntitySetPathReturnTypeInvalid), p0);
    }

    internal static string EdmModel_Validator_Semantic_OperationWithEntitySetPathAndReturnTypeTypeNotAssignable(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_OperationWithEntitySetPathAndReturnTypeTypeNotAssignable), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_OperationWithEntitySetPathResolvesToCollectionEntityTypeMismatchesEntityTypeReturnType(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_OperationWithEntitySetPathResolvesToCollectionEntityTypeMismatchesEntityTypeReturnType), p0);
    }

    internal static string EdmModel_Validator_Semantic_SchemaElementNameAlreadyDefined(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_SchemaElementNameAlreadyDefined), p0);

    internal static string EdmModel_Validator_Semantic_InvalidMemberNameMatchesTypeName(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidMemberNameMatchesTypeName), p0);

    internal static string EdmModel_Validator_Semantic_PropertyNameAlreadyDefined(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_PropertyNameAlreadyDefined), p0);

    internal static string EdmModel_Validator_Semantic_BaseTypeMustHaveSameTypeKind => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_BaseTypeMustHaveSameTypeKind));

    internal static string EdmModel_Validator_Semantic_BaseTypeOfOpenTypeMustBeOpen(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_BaseTypeOfOpenTypeMustBeOpen), p0);

    internal static string EdmModel_Validator_Semantic_KeyPropertyMustBelongToEntity(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_KeyPropertyMustBelongToEntity), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_EdmPrimitiveTypeCannotBeUsedAsTypeOfKey(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EdmPrimitiveTypeCannotBeUsedAsTypeOfKey), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_EdmPrimitiveTypeCannotBeUsedAsUnderlyingType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EdmPrimitiveTypeCannotBeUsedAsUnderlyingType), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_DependentPropertiesMustBelongToDependentEntity(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_DependentPropertiesMustBelongToDependentEntity), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_DeclaringTypeMustBeCorrect(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_DeclaringTypeMustBeCorrect), p0);

    internal static string EdmModel_Validator_Semantic_InaccessibleType(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InaccessibleType), p0);

    internal static string EdmModel_Validator_Semantic_AmbiguousType(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_AmbiguousType), p0);

    internal static string EdmModel_Validator_Semantic_InvalidNavigationPropertyType(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidNavigationPropertyType), p0);

    internal static string EdmModel_Validator_Semantic_NavigationPropertyWithRecursiveContainmentTargetMustBeOptional(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_NavigationPropertyWithRecursiveContainmentTargetMustBeOptional), p0);
    }

    internal static string EdmModel_Validator_Semantic_NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne), p0);
    }

    internal static string EdmModel_Validator_Semantic_NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne), p0);
    }

    internal static string EdmModel_Validator_Semantic_ComplexTypeMustHaveProperties(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_ComplexTypeMustHaveProperties), p0);

    internal static string EdmModel_Validator_Semantic_DuplicateDependentProperty(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_DuplicateDependentProperty), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_ScaleOutOfRange => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_ScaleOutOfRange));

    internal static string EdmModel_Validator_Semantic_PrecisionOutOfRange => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_PrecisionOutOfRange));

    internal static string EdmModel_Validator_Semantic_StringMaxLengthOutOfRange => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_StringMaxLengthOutOfRange));

    internal static string EdmModel_Validator_Semantic_MaxLengthOutOfRange => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_MaxLengthOutOfRange));

    internal static string EdmModel_Validator_Semantic_EnumMemberValueOutOfRange(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EnumMemberValueOutOfRange), p0);

    internal static string EdmModel_Validator_Semantic_EnumMemberNameAlreadyDefined(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EnumMemberNameAlreadyDefined), p0);

    internal static string EdmModel_Validator_Semantic_OpenTypesSupportedForEntityTypesOnly => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_OpenTypesSupportedForEntityTypesOnly));

    internal static string EdmModel_Validator_Semantic_IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull));

    internal static string EdmModel_Validator_Semantic_InvalidElementAnnotationMismatchedTerm => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidElementAnnotationMismatchedTerm));

    internal static string EdmModel_Validator_Semantic_InvalidElementAnnotationValueInvalidXml => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidElementAnnotationValueInvalidXml));

    internal static string EdmModel_Validator_Semantic_InvalidElementAnnotationNotIEdmStringValue => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidElementAnnotationNotIEdmStringValue));

    internal static string EdmModel_Validator_Semantic_InvalidElementAnnotationNullNamespaceOrName => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InvalidElementAnnotationNullNamespaceOrName));

    internal static string EdmModel_Validator_Semantic_CannotAssertNullableTypeAsNonNullableType(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_CannotAssertNullableTypeAsNonNullableType), p0);
    }

    internal static string EdmModel_Validator_Semantic_ExpressionPrimitiveKindCannotPromoteToAssertedType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_ExpressionPrimitiveKindCannotPromoteToAssertedType), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_NullCannotBeAssertedToBeANonNullableType => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_NullCannotBeAssertedToBeANonNullableType));

    internal static string EdmModel_Validator_Semantic_ExpressionNotValidForTheAssertedType => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_ExpressionNotValidForTheAssertedType));

    internal static string EdmModel_Validator_Semantic_CollectionExpressionNotValidForNonCollectionType => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_CollectionExpressionNotValidForNonCollectionType));

    internal static string EdmModel_Validator_Semantic_PrimitiveConstantExpressionNotValidForNonPrimitiveType => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_PrimitiveConstantExpressionNotValidForNonPrimitiveType));

    internal static string EdmModel_Validator_Semantic_RecordExpressionNotValidForNonStructuredType => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_RecordExpressionNotValidForNonStructuredType));

    internal static string EdmModel_Validator_Semantic_RecordExpressionMissingProperty(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_RecordExpressionMissingProperty), p0);

    internal static string EdmModel_Validator_Semantic_RecordExpressionHasExtraProperties(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_RecordExpressionHasExtraProperties), p0);

    internal static string EdmModel_Validator_Semantic_DuplicateAnnotation(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_DuplicateAnnotation), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_IncorrectNumberOfArguments(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_IncorrectNumberOfArguments), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_DuplicateEntityContainerName(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_DuplicateEntityContainerName), p0);

    internal static string EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType));

    internal static string EdmModel_Validator_Semantic_ExpressionEnumKindNotValidForAssertedType => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_ExpressionEnumKindNotValidForAssertedType));

    internal static string EdmModel_Validator_Semantic_IntegerConstantValueOutOfRange => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_IntegerConstantValueOutOfRange));

    internal static string EdmModel_Validator_Semantic_StringConstantLengthOutOfRange(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_StringConstantLengthOutOfRange), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_BinaryConstantLengthOutOfRange(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_BinaryConstantLengthOutOfRange), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_TypeMustNotHaveKindOfNone => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_TypeMustNotHaveKindOfNone));

    internal static string EdmModel_Validator_Semantic_SchemaElementMustNotHaveKindOfNone(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_SchemaElementMustNotHaveKindOfNone), p0);

    internal static string EdmModel_Validator_Semantic_PropertyMustNotHaveKindOfNone(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_PropertyMustNotHaveKindOfNone), p0);

    internal static string EdmModel_Validator_Semantic_PrimitiveTypeMustNotHaveKindOfNone(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_PrimitiveTypeMustNotHaveKindOfNone), p0);

    internal static string EdmModel_Validator_Semantic_EntityContainerElementMustNotHaveKindOfNone(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EntityContainerElementMustNotHaveKindOfNone), p0);
    }

    internal static string EdmModel_Validator_Semantic_DuplicateNavigationPropertyMapping(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_DuplicateNavigationPropertyMapping), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_NavigationMappingMustBeBidirectional(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_NavigationMappingMustBeBidirectional), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_EntitySetCanOnlyBeContainedByASingleNavigationProperty(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EntitySetCanOnlyBeContainedByASingleNavigationProperty), p0);
    }

    internal static string EdmModel_Validator_Semantic_TypeAnnotationMissingRequiredProperty(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_TypeAnnotationMissingRequiredProperty), p0);
    }

    internal static string EdmModel_Validator_Semantic_TypeAnnotationHasExtraProperties(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_TypeAnnotationHasExtraProperties), p0);

    internal static string EdmModel_Validator_Semantic_EnumMustHaveIntegralUnderlyingType(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EnumMustHaveIntegralUnderlyingType), p0);

    internal static string EdmModel_Validator_Semantic_InaccessibleTerm(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InaccessibleTerm), p0);

    internal static string EdmModel_Validator_Semantic_InaccessibleTarget(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_InaccessibleTarget), p0);

    internal static string EdmModel_Validator_Semantic_VocabularyAnnotationApplyToNotAllowedAnnotatable(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_VocabularyAnnotationApplyToNotAllowedAnnotatable), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_ElementDirectValueAnnotationFullNameMustBeUnique(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_ElementDirectValueAnnotationFullNameMustBeUnique), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_NoEntitySetsFoundForType(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_NoEntitySetsFoundForType), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_CannotInferEntitySetWithMultipleSetsPerType(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_CannotInferEntitySetWithMultipleSetsPerType), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_NavigationPropertyEntityMustNotIndirectlyContainItself(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_NavigationPropertyEntityMustNotIndirectlyContainItself), p0);
    }

    internal static string EdmModel_Validator_Semantic_PathIsNotValidForTheGivenContext(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_PathIsNotValidForTheGivenContext), p0);

    internal static string EdmModel_Validator_Semantic_NavigationPropertyMappingMustPointToValidTargetForProperty(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_NavigationPropertyMappingMustPointToValidTargetForProperty), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterNames(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterNames), p0);
    }

    internal static string EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterTypes(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterTypes), p0);
    }

    internal static string EdmModel_Validator_Semantic_ModelDuplicateUnBoundFunctionsParameterNames(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_ModelDuplicateUnBoundFunctionsParameterNames), p0);
    }

    internal static string EdmModel_Validator_Semantic_ModelDuplicateUnBoundFunctionsParameterTypes(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_ModelDuplicateUnBoundFunctionsParameterTypes), p0);
    }

    internal static string EdmModel_Validator_Semantic_ModelDuplicateBoundActions(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_ModelDuplicateBoundActions), p0);

    internal static string EdmModel_Validator_Semantic_ModelDuplicateUnBoundActions(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Semantic_ModelDuplicateUnBoundActions), p0);

    internal static string EdmModel_Validator_Semantic_BoundFunctionOverloadsMustHaveSameReturnType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_BoundFunctionOverloadsMustHaveSameReturnType), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_EntitySetTypeMustBeCollectionOfEntityType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EntitySetTypeMustBeCollectionOfEntityType), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_SingletonTypeMustBeEntityType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_SingletonTypeMustBeEntityType), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_NavigationPropertyOfCollectionTypeMustNotTargetToSingleton(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_NavigationPropertyOfCollectionTypeMustNotTargetToSingleton), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_StructuredTypeBaseTypeCannotBeAbstractType(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_StructuredTypeBaseTypeCannotBeAbstractType), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_PropertyTypeCannotBeCollectionOfAbstractType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_PropertyTypeCannotBeCollectionOfAbstractType), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_OperationReturnTypeCannotBeCollectionOfAbstractType(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_OperationReturnTypeCannotBeCollectionOfAbstractType), p0, p1);
    }

    internal static string EdmModel_Validator_Semantic_EdmEntityTypeCannotBeTypeOfSingleton(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EdmEntityTypeCannotBeTypeOfSingleton), p0);
    }

    internal static string EdmModel_Validator_Semantic_EdmEntityTypeCannotBeTypeOfEntitySet(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_EdmEntityTypeCannotBeTypeOfEntitySet), p0);
    }

    internal static string EdmModel_Validator_Semantic_DeclaringTypeOfNavigationSourceCannotHavePathProperty(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_DeclaringTypeOfNavigationSourceCannotHavePathProperty), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Semantic_TypeOfNavigationPropertyCannotHavePathProperty(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Semantic_TypeOfNavigationPropertyCannotHavePathProperty), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Syntactic_MissingName => EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_MissingName));

    internal static string EdmModel_Validator_Syntactic_EdmModel_NameIsTooLong(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmModel_NameIsTooLong), p0);

    internal static string EdmModel_Validator_Syntactic_EdmModel_NameIsNotAllowed(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmModel_NameIsNotAllowed), p0);

    internal static string EdmModel_Validator_Syntactic_MissingNamespaceName => EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_MissingNamespaceName));

    internal static string EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsTooLong(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsTooLong), p0);

    internal static string EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsNotAllowed(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsNotAllowed), p0);

    internal static string EdmModel_Validator_Syntactic_PropertyMustNotBeNull(object p0, object p1) => EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_PropertyMustNotBeNull), p0, p1);

    internal static string EdmModel_Validator_Syntactic_EnumPropertyValueOutOfRange(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EnumPropertyValueOutOfRange), p0, p1, p2, p3);
    }

    internal static string EdmModel_Validator_Syntactic_InterfaceKindValueMismatch(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_InterfaceKindValueMismatch), p0, p1, p2, p3);
    }

    internal static string EdmModel_Validator_Syntactic_TypeRefInterfaceTypeKindValueMismatch(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_TypeRefInterfaceTypeKindValueMismatch), p0, p1);
    }

    internal static string EdmModel_Validator_Syntactic_InterfaceKindValueUnexpected(
      object p0,
      object p1,
      object p2)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_InterfaceKindValueUnexpected), p0, p1, p2);
    }

    internal static string EdmModel_Validator_Syntactic_EnumerableMustNotHaveNullElements(
      object p0,
      object p1)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_EnumerableMustNotHaveNullElements), p0, p1);
    }

    internal static string EdmModel_Validator_Syntactic_NavigationPartnerInvalid(object p0) => EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_NavigationPartnerInvalid), p0);

    internal static string EdmModel_Validator_Syntactic_InterfaceCriticalCycleInTypeHierarchy(
      object p0)
    {
      return EntityRes.GetString(nameof (EdmModel_Validator_Syntactic_InterfaceCriticalCycleInTypeHierarchy), p0);
    }

    internal static string Serializer_SingleFileExpected => EntityRes.GetString(nameof (Serializer_SingleFileExpected));

    internal static string Serializer_UnknownEdmVersion => EntityRes.GetString(nameof (Serializer_UnknownEdmVersion));

    internal static string Serializer_UnknownEdmxVersion => EntityRes.GetString(nameof (Serializer_UnknownEdmxVersion));

    internal static string Serializer_NonInlineOperationImportReturnType(object p0) => EntityRes.GetString(nameof (Serializer_NonInlineOperationImportReturnType), p0);

    internal static string Serializer_ReferencedTypeMustHaveValidName(object p0) => EntityRes.GetString(nameof (Serializer_ReferencedTypeMustHaveValidName), p0);

    internal static string Serializer_OutOfLineAnnotationTargetMustHaveValidName(object p0) => EntityRes.GetString(nameof (Serializer_OutOfLineAnnotationTargetMustHaveValidName), p0);

    internal static string Serializer_NoSchemasProduced => EntityRes.GetString(nameof (Serializer_NoSchemasProduced));

    internal static string XmlParser_EmptyFile(object p0) => EntityRes.GetString(nameof (XmlParser_EmptyFile), p0);

    internal static string XmlParser_EmptySchemaTextReader => EntityRes.GetString(nameof (XmlParser_EmptySchemaTextReader));

    internal static string XmlParser_MissingAttribute(object p0, object p1) => EntityRes.GetString(nameof (XmlParser_MissingAttribute), p0, p1);

    internal static string XmlParser_TextNotAllowed(object p0) => EntityRes.GetString(nameof (XmlParser_TextNotAllowed), p0);

    internal static string XmlParser_UnexpectedAttribute(object p0) => EntityRes.GetString(nameof (XmlParser_UnexpectedAttribute), p0);

    internal static string XmlParser_UnexpectedElement(object p0) => EntityRes.GetString(nameof (XmlParser_UnexpectedElement), p0);

    internal static string XmlParser_UnusedElement(object p0) => EntityRes.GetString(nameof (XmlParser_UnusedElement), p0);

    internal static string XmlParser_UnexpectedNodeType(object p0) => EntityRes.GetString(nameof (XmlParser_UnexpectedNodeType), p0);

    internal static string XmlParser_UnexpectedRootElement(object p0, object p1) => EntityRes.GetString(nameof (XmlParser_UnexpectedRootElement), p0, p1);

    internal static string XmlParser_UnexpectedRootElementWrongNamespace(object p0, object p1) => EntityRes.GetString(nameof (XmlParser_UnexpectedRootElementWrongNamespace), p0, p1);

    internal static string XmlParser_UnexpectedRootElementNoNamespace(object p0) => EntityRes.GetString(nameof (XmlParser_UnexpectedRootElementNoNamespace), p0);

    internal static string CsdlParser_InvalidEntitySetPathWithUnboundAction(object p0, object p1) => EntityRes.GetString(nameof (CsdlParser_InvalidEntitySetPathWithUnboundAction), p0, p1);

    internal static string CsdlParser_InvalidAlias(object p0) => EntityRes.GetString(nameof (CsdlParser_InvalidAlias), p0);

    internal static string CsdlParser_InvalidDeleteAction(object p0) => EntityRes.GetString(nameof (CsdlParser_InvalidDeleteAction), p0);

    internal static string CsdlParser_MissingTypeAttributeOrElement => EntityRes.GetString(nameof (CsdlParser_MissingTypeAttributeOrElement));

    internal static string CsdlParser_InvalidEndRoleInRelationshipConstraint(object p0, object p1) => EntityRes.GetString(nameof (CsdlParser_InvalidEndRoleInRelationshipConstraint), p0, p1);

    internal static string CsdlParser_InvalidMultiplicity(object p0) => EntityRes.GetString(nameof (CsdlParser_InvalidMultiplicity), p0);

    internal static string CsdlParser_ReferentialConstraintRequiresOneDependent => EntityRes.GetString(nameof (CsdlParser_ReferentialConstraintRequiresOneDependent));

    internal static string CsdlParser_ReferentialConstraintRequiresOnePrincipal => EntityRes.GetString(nameof (CsdlParser_ReferentialConstraintRequiresOnePrincipal));

    internal static string CsdlParser_InvalidIfExpressionIncorrectNumberOfOperands => EntityRes.GetString(nameof (CsdlParser_InvalidIfExpressionIncorrectNumberOfOperands));

    internal static string CsdlParser_InvalidIsTypeExpressionIncorrectNumberOfOperands => EntityRes.GetString(nameof (CsdlParser_InvalidIsTypeExpressionIncorrectNumberOfOperands));

    internal static string CsdlParser_InvalidCastExpressionIncorrectNumberOfOperands => EntityRes.GetString(nameof (CsdlParser_InvalidCastExpressionIncorrectNumberOfOperands));

    internal static string CsdlParser_InvalidLabeledElementExpressionIncorrectNumberOfOperands => EntityRes.GetString(nameof (CsdlParser_InvalidLabeledElementExpressionIncorrectNumberOfOperands));

    internal static string CsdlParser_InvalidTypeName(object p0) => EntityRes.GetString(nameof (CsdlParser_InvalidTypeName), p0);

    internal static string CsdlParser_InvalidQualifiedName(object p0) => EntityRes.GetString(nameof (CsdlParser_InvalidQualifiedName), p0);

    internal static string CsdlParser_NoReadersProvided => EntityRes.GetString(nameof (CsdlParser_NoReadersProvided));

    internal static string CsdlParser_NullXmlReader => EntityRes.GetString(nameof (CsdlParser_NullXmlReader));

    internal static string CsdlParser_InvalidEntitySetPath(object p0) => EntityRes.GetString(nameof (CsdlParser_InvalidEntitySetPath), p0);

    internal static string CsdlParser_InvalidEnumMemberPath(object p0) => EntityRes.GetString(nameof (CsdlParser_InvalidEnumMemberPath), p0);

    internal static string CsdlParser_CannotSpecifyNullableAttributeForNavigationPropertyWithCollectionType => EntityRes.GetString(nameof (CsdlParser_CannotSpecifyNullableAttributeForNavigationPropertyWithCollectionType));

    internal static string CsdlParser_MetadataDocumentCannotHaveMoreThanOneEntityContainer => EntityRes.GetString(nameof (CsdlParser_MetadataDocumentCannotHaveMoreThanOneEntityContainer));

    internal static string CsdlSemantics_ReferentialConstraintMismatch => EntityRes.GetString(nameof (CsdlSemantics_ReferentialConstraintMismatch));

    internal static string CsdlSemantics_EnumMemberMustHaveValue => EntityRes.GetString(nameof (CsdlSemantics_EnumMemberMustHaveValue));

    internal static string CsdlSemantics_ImpossibleAnnotationsTarget(object p0) => EntityRes.GetString(nameof (CsdlSemantics_ImpossibleAnnotationsTarget), p0);

    internal static string CsdlSemantics_DuplicateAlias(object p0, object p1) => EntityRes.GetString(nameof (CsdlSemantics_DuplicateAlias), p0, p1);

    internal static string EdmxParser_EdmxVersionMismatch => EntityRes.GetString(nameof (EdmxParser_EdmxVersionMismatch));

    internal static string EdmxParser_BodyElement(object p0) => EntityRes.GetString(nameof (EdmxParser_BodyElement), p0);

    internal static string EdmxParser_InvalidReferenceIncorrectNumberOfIncludes => EntityRes.GetString(nameof (EdmxParser_InvalidReferenceIncorrectNumberOfIncludes));

    internal static string EdmxParser_UnresolvedReferenceUriInEdmxReference => EntityRes.GetString(nameof (EdmxParser_UnresolvedReferenceUriInEdmxReference));

    internal static string EdmParseException_ErrorsEncounteredInEdmx(object p0) => EntityRes.GetString(nameof (EdmParseException_ErrorsEncounteredInEdmx), p0);

    internal static string ValueParser_InvalidBoolean(object p0) => EntityRes.GetString(nameof (ValueParser_InvalidBoolean), p0);

    internal static string ValueParser_InvalidInteger(object p0) => EntityRes.GetString(nameof (ValueParser_InvalidInteger), p0);

    internal static string ValueParser_InvalidLong(object p0) => EntityRes.GetString(nameof (ValueParser_InvalidLong), p0);

    internal static string ValueParser_InvalidFloatingPoint(object p0) => EntityRes.GetString(nameof (ValueParser_InvalidFloatingPoint), p0);

    internal static string ValueParser_InvalidMaxLength(object p0) => EntityRes.GetString(nameof (ValueParser_InvalidMaxLength), p0);

    internal static string ValueParser_InvalidSrid(object p0) => EntityRes.GetString(nameof (ValueParser_InvalidSrid), p0);

    internal static string ValueParser_InvalidScale(object p0) => EntityRes.GetString(nameof (ValueParser_InvalidScale), p0);

    internal static string ValueParser_InvalidGuid(object p0) => EntityRes.GetString(nameof (ValueParser_InvalidGuid), p0);

    internal static string ValueParser_InvalidDecimal(object p0) => EntityRes.GetString(nameof (ValueParser_InvalidDecimal), p0);

    internal static string ValueParser_InvalidDateTimeOffset(object p0) => EntityRes.GetString(nameof (ValueParser_InvalidDateTimeOffset), p0);

    internal static string ValueParser_InvalidDateTime(object p0) => EntityRes.GetString(nameof (ValueParser_InvalidDateTime), p0);

    internal static string ValueParser_InvalidDate(object p0) => EntityRes.GetString(nameof (ValueParser_InvalidDate), p0);

    internal static string ValueParser_InvalidDuration(object p0) => EntityRes.GetString(nameof (ValueParser_InvalidDuration), p0);

    internal static string ValueParser_InvalidBinary(object p0) => EntityRes.GetString(nameof (ValueParser_InvalidBinary), p0);

    internal static string ValueParser_InvalidTimeOfDay(object p0) => EntityRes.GetString(nameof (ValueParser_InvalidTimeOfDay), p0);

    internal static string UnknownEnumVal_Multiplicity(object p0) => EntityRes.GetString(nameof (UnknownEnumVal_Multiplicity), p0);

    internal static string UnknownEnumVal_SchemaElementKind(object p0) => EntityRes.GetString(nameof (UnknownEnumVal_SchemaElementKind), p0);

    internal static string UnknownEnumVal_TypeKind(object p0) => EntityRes.GetString(nameof (UnknownEnumVal_TypeKind), p0);

    internal static string UnknownEnumVal_PrimitiveKind(object p0) => EntityRes.GetString(nameof (UnknownEnumVal_PrimitiveKind), p0);

    internal static string UnknownEnumVal_ContainerElementKind(object p0) => EntityRes.GetString(nameof (UnknownEnumVal_ContainerElementKind), p0);

    internal static string UnknownEnumVal_CsdlTarget(object p0) => EntityRes.GetString(nameof (UnknownEnumVal_CsdlTarget), p0);

    internal static string UnknownEnumVal_PropertyKind(object p0) => EntityRes.GetString(nameof (UnknownEnumVal_PropertyKind), p0);

    internal static string UnknownEnumVal_ExpressionKind(object p0) => EntityRes.GetString(nameof (UnknownEnumVal_ExpressionKind), p0);

    internal static string Bad_AmbiguousElementBinding(object p0) => EntityRes.GetString(nameof (Bad_AmbiguousElementBinding), p0);

    internal static string Bad_UnresolvedType(object p0) => EntityRes.GetString(nameof (Bad_UnresolvedType), p0);

    internal static string Bad_UnresolvedComplexType(object p0) => EntityRes.GetString(nameof (Bad_UnresolvedComplexType), p0);

    internal static string Bad_UnresolvedEntityType(object p0) => EntityRes.GetString(nameof (Bad_UnresolvedEntityType), p0);

    internal static string Bad_UnresolvedPrimitiveType(object p0) => EntityRes.GetString(nameof (Bad_UnresolvedPrimitiveType), p0);

    internal static string Bad_UnresolvedOperation(object p0) => EntityRes.GetString(nameof (Bad_UnresolvedOperation), p0);

    internal static string Bad_AmbiguousOperation(object p0) => EntityRes.GetString(nameof (Bad_AmbiguousOperation), p0);

    internal static string Bad_OperationParametersDontMatch(object p0) => EntityRes.GetString(nameof (Bad_OperationParametersDontMatch), p0);

    internal static string Bad_UnresolvedEntitySet(object p0) => EntityRes.GetString(nameof (Bad_UnresolvedEntitySet), p0);

    internal static string Bad_UnresolvedEntityContainer(object p0) => EntityRes.GetString(nameof (Bad_UnresolvedEntityContainer), p0);

    internal static string Bad_UnresolvedEnumType(object p0) => EntityRes.GetString(nameof (Bad_UnresolvedEnumType), p0);

    internal static string Bad_UnresolvedEnumMember(object p0) => EntityRes.GetString(nameof (Bad_UnresolvedEnumMember), p0);

    internal static string Bad_UnresolvedProperty(object p0) => EntityRes.GetString(nameof (Bad_UnresolvedProperty), p0);

    internal static string Bad_UnresolvedParameter(object p0) => EntityRes.GetString(nameof (Bad_UnresolvedParameter), p0);

    internal static string Bad_UnresolvedReturn(object p0) => EntityRes.GetString(nameof (Bad_UnresolvedReturn), p0);

    internal static string Bad_UnresolvedLabeledElement(object p0) => EntityRes.GetString(nameof (Bad_UnresolvedLabeledElement), p0);

    internal static string Bad_CyclicEntity(object p0) => EntityRes.GetString(nameof (Bad_CyclicEntity), p0);

    internal static string Bad_CyclicComplex(object p0) => EntityRes.GetString(nameof (Bad_CyclicComplex), p0);

    internal static string Bad_CyclicEntityContainer(object p0) => EntityRes.GetString(nameof (Bad_CyclicEntityContainer), p0);

    internal static string Bad_UnresolvedNavigationPropertyPath(object p0, object p1) => EntityRes.GetString(nameof (Bad_UnresolvedNavigationPropertyPath), p0, p1);

    internal static string RuleSet_DuplicateRulesExistInRuleSet => EntityRes.GetString(nameof (RuleSet_DuplicateRulesExistInRuleSet));

    internal static string EdmToClr_UnsupportedType(object p0) => EntityRes.GetString(nameof (EdmToClr_UnsupportedType), p0);

    internal static string EdmToClr_StructuredValueMappedToNonClass => EntityRes.GetString(nameof (EdmToClr_StructuredValueMappedToNonClass));

    internal static string EdmToClr_IEnumerableOfTPropertyAlreadyHasValue(object p0, object p1) => EntityRes.GetString(nameof (EdmToClr_IEnumerableOfTPropertyAlreadyHasValue), p0, p1);

    internal static string EdmToClr_StructuredPropertyDuplicateValue(object p0) => EntityRes.GetString(nameof (EdmToClr_StructuredPropertyDuplicateValue), p0);

    internal static string EdmToClr_CannotConvertEdmValueToClrType(object p0, object p1) => EntityRes.GetString(nameof (EdmToClr_CannotConvertEdmValueToClrType), p0, p1);

    internal static string EdmToClr_CannotConvertEdmCollectionValueToClrType(object p0) => EntityRes.GetString(nameof (EdmToClr_CannotConvertEdmCollectionValueToClrType), p0);

    internal static string EdmToClr_TryCreateObjectInstanceReturnedWrongObject(object p0, object p1) => EntityRes.GetString(nameof (EdmToClr_TryCreateObjectInstanceReturnedWrongObject), p0, p1);

    internal static string EdmUtil_NullValueForMimeTypeAnnotation => EntityRes.GetString(nameof (EdmUtil_NullValueForMimeTypeAnnotation));

    internal static string EdmUtil_InvalidAnnotationValue(object p0, object p1) => EntityRes.GetString(nameof (EdmUtil_InvalidAnnotationValue), p0, p1);

    internal static string PlatformHelper_DateTimeOffsetMustContainTimeZone(object p0) => EntityRes.GetString(nameof (PlatformHelper_DateTimeOffsetMustContainTimeZone), p0);

    internal static string Date_InvalidAddedOrSubtractedResults => EntityRes.GetString(nameof (Date_InvalidAddedOrSubtractedResults));

    internal static string Date_InvalidDateParameters(object p0, object p1, object p2) => EntityRes.GetString(nameof (Date_InvalidDateParameters), p0, p1, p2);

    internal static string Date_InvalidParsingString(object p0) => EntityRes.GetString(nameof (Date_InvalidParsingString), p0);

    internal static string Date_InvalidCompareToTarget(object p0) => EntityRes.GetString(nameof (Date_InvalidCompareToTarget), p0);

    internal static string TimeOfDay_InvalidTimeOfDayParameters(
      object p0,
      object p1,
      object p2,
      object p3)
    {
      return EntityRes.GetString(nameof (TimeOfDay_InvalidTimeOfDayParameters), p0, p1, p2, p3);
    }

    internal static string TimeOfDay_TicksOutOfRange(object p0) => EntityRes.GetString(nameof (TimeOfDay_TicksOutOfRange), p0);

    internal static string TimeOfDay_ConvertErrorFromTimeSpan(object p0) => EntityRes.GetString(nameof (TimeOfDay_ConvertErrorFromTimeSpan), p0);

    internal static string TimeOfDay_InvalidParsingString(object p0) => EntityRes.GetString(nameof (TimeOfDay_InvalidParsingString), p0);

    internal static string TimeOfDay_InvalidCompareToTarget(object p0) => EntityRes.GetString(nameof (TimeOfDay_InvalidCompareToTarget), p0);
  }
}
