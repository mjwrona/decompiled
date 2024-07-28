// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Common.SRResources
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.AspNet.OData.Common
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class SRResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal SRResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (SRResources.resourceMan == null)
        {
          Assembly assembly = TypeHelper.GetAssembly(typeof (CommonWebApiResources));
          string str = ((IEnumerable<string>) assembly.GetManifestResourceNames()).Where<string>((Func<string, bool>) (s => s.EndsWith("SRResources.resources", StringComparison.OrdinalIgnoreCase))).Single<string>();
          SRResources.resourceMan = new ResourceManager(str.Substring(0, str.Length - 10), assembly);
        }
        return SRResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => SRResources.resourceCulture;
      set => SRResources.resourceCulture = value;
    }

    internal static string ActionContextMustHaveDescriptor => SRResources.ResourceManager.GetString(nameof (ActionContextMustHaveDescriptor), SRResources.resourceCulture);

    internal static string ActionContextMustHaveRequest => SRResources.ResourceManager.GetString(nameof (ActionContextMustHaveRequest), SRResources.resourceCulture);

    internal static string ActionExecutedContextMustHaveActionContext => SRResources.ResourceManager.GetString(nameof (ActionExecutedContextMustHaveActionContext), SRResources.resourceCulture);

    internal static string ActionExecutedContextMustHaveRequest => SRResources.ResourceManager.GetString(nameof (ActionExecutedContextMustHaveRequest), SRResources.resourceCulture);

    internal static string ActionNotBoundToCollectionOfEntity => SRResources.ResourceManager.GetString(nameof (ActionNotBoundToCollectionOfEntity), SRResources.resourceCulture);

    internal static string ActionNotBoundToEntity => SRResources.ResourceManager.GetString(nameof (ActionNotBoundToEntity), SRResources.resourceCulture);

    internal static string AggregateKindNotSupported => SRResources.ResourceManager.GetString(nameof (AggregateKindNotSupported), SRResources.resourceCulture);

    internal static string AggregationMethodNotSupported => SRResources.ResourceManager.GetString(nameof (AggregationMethodNotSupported), SRResources.resourceCulture);

    internal static string AggregationNotSupportedForType => SRResources.ResourceManager.GetString(nameof (AggregationNotSupportedForType), SRResources.resourceCulture);

    internal static string ApplyQueryOptionNotSupportedForLinq2SQL => SRResources.ResourceManager.GetString(nameof (ApplyQueryOptionNotSupportedForLinq2SQL), SRResources.resourceCulture);

    internal static string AggregationNotSupportedForSingleProperty => SRResources.ResourceManager.GetString(nameof (AggregationNotSupportedForSingleProperty), SRResources.resourceCulture);

    internal static string ApplyToOnUntypedQueryOption => SRResources.ResourceManager.GetString(nameof (ApplyToOnUntypedQueryOption), SRResources.resourceCulture);

    internal static string ArgumentMustBeOfType => SRResources.ResourceManager.GetString(nameof (ArgumentMustBeOfType), SRResources.resourceCulture);

    internal static string BatchRequestInvalidMediaType => SRResources.ResourceManager.GetString(nameof (BatchRequestInvalidMediaType), SRResources.resourceCulture);

    internal static string BatchRequestMissingBoundary => SRResources.ResourceManager.GetString(nameof (BatchRequestMissingBoundary), SRResources.resourceCulture);

    internal static string BatchRequestMissingContent => SRResources.ResourceManager.GetString(nameof (BatchRequestMissingContent), SRResources.resourceCulture);

    internal static string BatchRequestMissingContentType => SRResources.ResourceManager.GetString(nameof (BatchRequestMissingContentType), SRResources.resourceCulture);

    internal static string BinaryOperatorNotSupported => SRResources.ResourceManager.GetString(nameof (BinaryOperatorNotSupported), SRResources.resourceCulture);

    internal static string CannotAddToNullCollection => SRResources.ResourceManager.GetString(nameof (CannotAddToNullCollection), SRResources.resourceCulture);

    internal static string CannotApplyETagOfT => SRResources.ResourceManager.GetString(nameof (CannotApplyETagOfT), SRResources.resourceCulture);

    internal static string CannotApplyODataQueryOptionsOfT => SRResources.ResourceManager.GetString(nameof (CannotApplyODataQueryOptionsOfT), SRResources.resourceCulture);

    internal static string CannotAutoCreateMultipleCandidates => SRResources.ResourceManager.GetString(nameof (CannotAutoCreateMultipleCandidates), SRResources.resourceCulture);

    internal static string CannotCastFilter => SRResources.ResourceManager.GetString(nameof (CannotCastFilter), SRResources.resourceCulture);

    internal static string CannotDeserializeUnknownProperty => SRResources.ResourceManager.GetString(nameof (CannotDeserializeUnknownProperty), SRResources.resourceCulture);

    internal static string CannotDefineKeysOnDerivedTypes => SRResources.ResourceManager.GetString(nameof (CannotDefineKeysOnDerivedTypes), SRResources.resourceCulture);

    internal static string CannotInferEdmType => SRResources.ResourceManager.GetString(nameof (CannotInferEdmType), SRResources.resourceCulture);

    internal static string CannotInstantiateAbstractResourceType => SRResources.ResourceManager.GetString(nameof (CannotInstantiateAbstractResourceType), SRResources.resourceCulture);

    internal static string CannotPatchNavigationProperties => SRResources.ResourceManager.GetString(nameof (CannotPatchNavigationProperties), SRResources.resourceCulture);

    internal static string CannotRecognizeNodeType => SRResources.ResourceManager.GetString(nameof (CannotRecognizeNodeType), SRResources.resourceCulture);

    internal static string CannotReconfigEntityTypeAsComplexType => SRResources.ResourceManager.GetString(nameof (CannotReconfigEntityTypeAsComplexType), SRResources.resourceCulture);

    internal static string CannotRedefineBaseTypeProperty => SRResources.ResourceManager.GetString(nameof (CannotRedefineBaseTypeProperty), SRResources.resourceCulture);

    internal static string CannotReEnableDependencyInjection => SRResources.ResourceManager.GetString(nameof (CannotReEnableDependencyInjection), SRResources.resourceCulture);

    internal static string CannotSerializerNull => SRResources.ResourceManager.GetString(nameof (CannotSerializerNull), SRResources.resourceCulture);

    internal static string CannotSetDynamicPropertyDictionary => SRResources.ResourceManager.GetString(nameof (CannotSetDynamicPropertyDictionary), SRResources.resourceCulture);

    internal static string CannotWriteType => SRResources.ResourceManager.GetString(nameof (CannotWriteType), SRResources.resourceCulture);

    internal static string ClrTypeNotInModel => SRResources.ResourceManager.GetString(nameof (ClrTypeNotInModel), SRResources.resourceCulture);

    internal static string CollectionParameterShouldHaveAddMethod => SRResources.ResourceManager.GetString(nameof (CollectionParameterShouldHaveAddMethod), SRResources.resourceCulture);

    internal static string CollectionPropertiesMustReturnIEnumerable => SRResources.ResourceManager.GetString(nameof (CollectionPropertiesMustReturnIEnumerable), SRResources.resourceCulture);

    internal static string CollectionShouldHaveAddMethod => SRResources.ResourceManager.GetString(nameof (CollectionShouldHaveAddMethod), SRResources.resourceCulture);

    internal static string CollectionShouldHaveClearMethod => SRResources.ResourceManager.GetString(nameof (CollectionShouldHaveClearMethod), SRResources.resourceCulture);

    internal static string ConvertToEnumFailed => SRResources.ResourceManager.GetString(nameof (ConvertToEnumFailed), SRResources.resourceCulture);

    internal static string CreateODataValueNotSupported => SRResources.ResourceManager.GetString(nameof (CreateODataValueNotSupported), SRResources.resourceCulture);

    internal static string DeltaEntityTypeNotAssignable => SRResources.ResourceManager.GetString(nameof (DeltaEntityTypeNotAssignable), SRResources.resourceCulture);

    internal static string DeltaTypeMismatch => SRResources.ResourceManager.GetString(nameof (DeltaTypeMismatch), SRResources.resourceCulture);

    internal static string DeltaNestedResourceNameNotFound => SRResources.ResourceManager.GetString(nameof (DeltaNestedResourceNameNotFound), SRResources.resourceCulture);

    internal static string DependentAndPrincipalTypeNotMatch => SRResources.ResourceManager.GetString(nameof (DependentAndPrincipalTypeNotMatch), SRResources.resourceCulture);

    internal static string DeserializerDoesNotSupportRead => SRResources.ResourceManager.GetString(nameof (DeserializerDoesNotSupportRead), SRResources.resourceCulture);

    internal static string DoesNotSupportReadInLine => SRResources.ResourceManager.GetString(nameof (DoesNotSupportReadInLine), SRResources.resourceCulture);

    internal static string DuplicateDynamicPropertyNameFound => SRResources.ResourceManager.GetString(nameof (DuplicateDynamicPropertyNameFound), SRResources.resourceCulture);

    internal static string DuplicateKeyInSegment => SRResources.ResourceManager.GetString(nameof (DuplicateKeyInSegment), SRResources.resourceCulture);

    internal static string DynamicPropertyCannotBeSerialized => SRResources.ResourceManager.GetString(nameof (DynamicPropertyCannotBeSerialized), SRResources.resourceCulture);

    internal static string DynamicPropertyNameAlreadyUsedAsDeclaredPropertyName => SRResources.ResourceManager.GetString(nameof (DynamicPropertyNameAlreadyUsedAsDeclaredPropertyName), SRResources.resourceCulture);

    internal static string DynamicResourceSetTypeNameIsRequired => SRResources.ResourceManager.GetString(nameof (DynamicResourceSetTypeNameIsRequired), SRResources.resourceCulture);

    internal static string EditLinkNullForLocationHeader => SRResources.ResourceManager.GetString(nameof (EditLinkNullForLocationHeader), SRResources.resourceCulture);

    internal static string EdmComplexObjectNullRef => SRResources.ResourceManager.GetString(nameof (EdmComplexObjectNullRef), SRResources.resourceCulture);

    internal static string EdmObjectNull => SRResources.ResourceManager.GetString(nameof (EdmObjectNull), SRResources.resourceCulture);

    internal static string EdmTypeCannotBeNull => SRResources.ResourceManager.GetString(nameof (EdmTypeCannotBeNull), SRResources.resourceCulture);

    internal static string EdmTypeNotSupported => SRResources.ResourceManager.GetString(nameof (EdmTypeNotSupported), SRResources.resourceCulture);

    internal static string ElementClrTypeNull => SRResources.ResourceManager.GetString(nameof (ElementClrTypeNull), SRResources.resourceCulture);

    internal static string EmptyKeyTemplate => SRResources.ResourceManager.GetString(nameof (EmptyKeyTemplate), SRResources.resourceCulture);

    internal static string EmptyParameterAlias => SRResources.ResourceManager.GetString(nameof (EmptyParameterAlias), SRResources.resourceCulture);

    internal static string EntityReferenceMustHasKeySegment => SRResources.ResourceManager.GetString(nameof (EntityReferenceMustHasKeySegment), SRResources.resourceCulture);

    internal static string EntitySetAlreadyConfiguredDifferentEntityType => SRResources.ResourceManager.GetString(nameof (EntitySetAlreadyConfiguredDifferentEntityType), SRResources.resourceCulture);

    internal static string EntitySetMissingDuringSerialization => SRResources.ResourceManager.GetString(nameof (EntitySetMissingDuringSerialization), SRResources.resourceCulture);

    internal static string EntitySetNameAlreadyConfiguredAsSingleton => SRResources.ResourceManager.GetString(nameof (EntitySetNameAlreadyConfiguredAsSingleton), SRResources.resourceCulture);

    internal static string EntitySetNotFoundForName => SRResources.ResourceManager.GetString(nameof (EntitySetNotFoundForName), SRResources.resourceCulture);

    internal static string EntityTypeDoesntHaveKeyDefined => SRResources.ResourceManager.GetString(nameof (EntityTypeDoesntHaveKeyDefined), SRResources.resourceCulture);

    internal static string CollectionNavigationPropertyEntityTypeDoesntHaveKeyDefined => SRResources.ResourceManager.GetString(nameof (CollectionNavigationPropertyEntityTypeDoesntHaveKeyDefined), SRResources.resourceCulture);

    internal static string EntityTypeMismatch => SRResources.ResourceManager.GetString(nameof (EntityTypeMismatch), SRResources.resourceCulture);

    internal static string EnumTypeDoesNotExist => SRResources.ResourceManager.GetString(nameof (EnumTypeDoesNotExist), SRResources.resourceCulture);

    internal static string EnumValueCannotBeLong => SRResources.ResourceManager.GetString(nameof (EnumValueCannotBeLong), SRResources.resourceCulture);

    internal static string EqualExpressionsMustHaveSameTypes => SRResources.ResourceManager.GetString(nameof (EqualExpressionsMustHaveSameTypes), SRResources.resourceCulture);

    internal static string ErrorTypeMustBeODataErrorOrHttpError => SRResources.ResourceManager.GetString(nameof (ErrorTypeMustBeODataErrorOrHttpError), SRResources.resourceCulture);

    internal static string ETagNotWellFormed => SRResources.ResourceManager.GetString(nameof (ETagNotWellFormed), SRResources.resourceCulture);

    internal static string ExpandFilterExpressionNotLambdaExpression => SRResources.ResourceManager.GetString(nameof (ExpandFilterExpressionNotLambdaExpression), SRResources.resourceCulture);

    internal static string FailedToBuildEdmModelBecauseReturnTypeIsNull => SRResources.ResourceManager.GetString(nameof (FailedToBuildEdmModelBecauseReturnTypeIsNull), SRResources.resourceCulture);

    internal static string FailedToRetrieveTypeToBuildEdmModel => SRResources.ResourceManager.GetString(nameof (FailedToRetrieveTypeToBuildEdmModel), SRResources.resourceCulture);

    internal static string FormatterReadIsNotSupportedForType => SRResources.ResourceManager.GetString(nameof (FormatterReadIsNotSupportedForType), SRResources.resourceCulture);

    internal static string FunctionNotBoundToCollectionOfEntity => SRResources.ResourceManager.GetString(nameof (FunctionNotBoundToCollectionOfEntity), SRResources.resourceCulture);

    internal static string FunctionNotBoundToEntity => SRResources.ResourceManager.GetString(nameof (FunctionNotBoundToEntity), SRResources.resourceCulture);

    internal static string FunctionNotSupportedOnEnum => SRResources.ResourceManager.GetString(nameof (FunctionNotSupportedOnEnum), SRResources.resourceCulture);

    internal static string FunctionParameterNotFound => SRResources.ResourceManager.GetString(nameof (FunctionParameterNotFound), SRResources.resourceCulture);

    internal static string GetEdmModelCalledMoreThanOnce => SRResources.ResourceManager.GetString(nameof (GetEdmModelCalledMoreThanOnce), SRResources.resourceCulture);

    internal static string GetOnlyCollectionCannotBeArray => SRResources.ResourceManager.GetString(nameof (GetOnlyCollectionCannotBeArray), SRResources.resourceCulture);

    internal static string HasActionLinkRequiresBindToCollectionOfEntity => SRResources.ResourceManager.GetString(nameof (HasActionLinkRequiresBindToCollectionOfEntity), SRResources.resourceCulture);

    internal static string HasActionLinkRequiresBindToEntity => SRResources.ResourceManager.GetString(nameof (HasActionLinkRequiresBindToEntity), SRResources.resourceCulture);

    internal static string HasFunctionLinkRequiresBindToCollectionOfEntity => SRResources.ResourceManager.GetString(nameof (HasFunctionLinkRequiresBindToCollectionOfEntity), SRResources.resourceCulture);

    internal static string HasFunctionLinkRequiresBindToEntity => SRResources.ResourceManager.GetString(nameof (HasFunctionLinkRequiresBindToEntity), SRResources.resourceCulture);

    internal static string IdLinkNullForEntityIdHeader => SRResources.ResourceManager.GetString(nameof (IdLinkNullForEntityIdHeader), SRResources.resourceCulture);

    internal static string InvalidAttributeRoutingTemplateSegment => SRResources.ResourceManager.GetString(nameof (InvalidAttributeRoutingTemplateSegment), SRResources.resourceCulture);

    internal static string InvalidBatchReaderState => SRResources.ResourceManager.GetString(nameof (InvalidBatchReaderState), SRResources.resourceCulture);

    internal static string InvalidBindingParameterType => SRResources.ResourceManager.GetString(nameof (InvalidBindingParameterType), SRResources.resourceCulture);

    internal static string InvalidDollarId => SRResources.ResourceManager.GetString(nameof (InvalidDollarId), SRResources.resourceCulture);

    internal static string InvalidEntitySetName => SRResources.ResourceManager.GetString(nameof (InvalidEntitySetName), SRResources.resourceCulture);

    internal static string InvalidETagHandler => SRResources.ResourceManager.GetString(nameof (InvalidETagHandler), SRResources.resourceCulture);

    internal static string InvalidExpansionDepthValue => SRResources.ResourceManager.GetString(nameof (InvalidExpansionDepthValue), SRResources.resourceCulture);

    internal static string InvalidODataPathTemplate => SRResources.ResourceManager.GetString(nameof (InvalidODataPathTemplate), SRResources.resourceCulture);

    internal static string InvalidODataRouteOnAction => SRResources.ResourceManager.GetString(nameof (InvalidODataRouteOnAction), SRResources.resourceCulture);

    internal static string InvalidODataUntypedValue => SRResources.ResourceManager.GetString(nameof (InvalidODataUntypedValue), SRResources.resourceCulture);

    internal static string InvalidPathSegment => SRResources.ResourceManager.GetString(nameof (InvalidPathSegment), SRResources.resourceCulture);

    internal static string InvalidPropertyInfoForDynamicPropertyAnnotation => SRResources.ResourceManager.GetString(nameof (InvalidPropertyInfoForDynamicPropertyAnnotation), SRResources.resourceCulture);

    internal static string InvalidPropertyMapper => SRResources.ResourceManager.GetString(nameof (InvalidPropertyMapper), SRResources.resourceCulture);

    internal static string InvalidPropertyMapping => SRResources.ResourceManager.GetString(nameof (InvalidPropertyMapping), SRResources.resourceCulture);

    internal static string InvalidSingleQuoteCountForNonStringLiteral => SRResources.ResourceManager.GetString(nameof (InvalidSingleQuoteCountForNonStringLiteral), SRResources.resourceCulture);

    internal static string InvalidSingletonName => SRResources.ResourceManager.GetString(nameof (InvalidSingletonName), SRResources.resourceCulture);

    internal static string InvalidTimeZoneInfo => SRResources.ResourceManager.GetString(nameof (InvalidTimeZoneInfo), SRResources.resourceCulture);

    internal static string KeyTemplateMustBeInCurlyBraces => SRResources.ResourceManager.GetString(nameof (KeyTemplateMustBeInCurlyBraces), SRResources.resourceCulture);

    internal static string KeyValueCannotBeNull => SRResources.ResourceManager.GetString(nameof (KeyValueCannotBeNull), SRResources.resourceCulture);

    internal static string LambdaExpressionMustHaveExactlyOneParameter => SRResources.ResourceManager.GetString(nameof (LambdaExpressionMustHaveExactlyOneParameter), SRResources.resourceCulture);

    internal static string LambdaExpressionMustHaveExactlyTwoParameters => SRResources.ResourceManager.GetString(nameof (LambdaExpressionMustHaveExactlyTwoParameters), SRResources.resourceCulture);

    internal static string LiteralHasABadFormat => SRResources.ResourceManager.GetString(nameof (LiteralHasABadFormat), SRResources.resourceCulture);

    internal static string ManyNavigationPropertiesCannotBeChanged => SRResources.ResourceManager.GetString(nameof (ManyNavigationPropertiesCannotBeChanged), SRResources.resourceCulture);

    internal static string ManyToManyNavigationPropertyMustReturnCollection => SRResources.ResourceManager.GetString(nameof (ManyToManyNavigationPropertyMustReturnCollection), SRResources.resourceCulture);

    internal static string MappingDoesNotContainResourceType => SRResources.ResourceManager.GetString(nameof (MappingDoesNotContainResourceType), SRResources.resourceCulture);

    internal static string MaxAnyAllExpressionLimitExceeded => SRResources.ResourceManager.GetString(nameof (MaxAnyAllExpressionLimitExceeded), SRResources.resourceCulture);

    internal static string MaxExpandDepthExceeded => SRResources.ResourceManager.GetString(nameof (MaxExpandDepthExceeded), SRResources.resourceCulture);

    internal static string MaxNodeLimitExceeded => SRResources.ResourceManager.GetString(nameof (MaxNodeLimitExceeded), SRResources.resourceCulture);

    internal static string MemberExpressionsMustBeBoundToLambdaParameter => SRResources.ResourceManager.GetString(nameof (MemberExpressionsMustBeBoundToLambdaParameter), SRResources.resourceCulture);

    internal static string MemberExpressionsMustBeProperties => SRResources.ResourceManager.GetString(nameof (MemberExpressionsMustBeProperties), SRResources.resourceCulture);

    internal static string MissingODataServices => SRResources.ResourceManager.GetString(nameof (MissingODataServices), SRResources.resourceCulture);

    internal static string MissingODataContainer => SRResources.ResourceManager.GetString(nameof (MissingODataContainer), SRResources.resourceCulture);

    internal static string MissingNonODataContainer => SRResources.ResourceManager.GetString(nameof (MissingNonODataContainer), SRResources.resourceCulture);

    internal static string ModelBinderUtil_ModelMetadataCannotBeNull => SRResources.ResourceManager.GetString(nameof (ModelBinderUtil_ModelMetadataCannotBeNull), SRResources.resourceCulture);

    internal static string ModelBinderUtil_ValueCannotBeEnum => SRResources.ResourceManager.GetString(nameof (ModelBinderUtil_ValueCannotBeEnum), SRResources.resourceCulture);

    internal static string ModelMissingFromReadContext => SRResources.ResourceManager.GetString(nameof (ModelMissingFromReadContext), SRResources.resourceCulture);

    internal static string MoreThanOneDynamicPropertyContainerFound => SRResources.ResourceManager.GetString(nameof (MoreThanOneDynamicPropertyContainerFound), SRResources.resourceCulture);

    internal static string MoreThanOneOperationFound => SRResources.ResourceManager.GetString(nameof (MoreThanOneOperationFound), SRResources.resourceCulture);

    internal static string MoreThanOneOverloadActionBoundToSameTypeFound => SRResources.ResourceManager.GetString(nameof (MoreThanOneOverloadActionBoundToSameTypeFound), SRResources.resourceCulture);

    internal static string MoreThanOneUnboundActionFound => SRResources.ResourceManager.GetString(nameof (MoreThanOneUnboundActionFound), SRResources.resourceCulture);

    internal static string MultipleAttributesFound => SRResources.ResourceManager.GetString(nameof (MultipleAttributesFound), SRResources.resourceCulture);

    internal static string MultipleMatchingClrTypesForEdmType => SRResources.ResourceManager.GetString(nameof (MultipleMatchingClrTypesForEdmType), SRResources.resourceCulture);

    internal static string MustBeCollectionProperty => SRResources.ResourceManager.GetString(nameof (MustBeCollectionProperty), SRResources.resourceCulture);

    internal static string MustBeComplexProperty => SRResources.ResourceManager.GetString(nameof (MustBeComplexProperty), SRResources.resourceCulture);

    internal static string MustBeDateTimeProperty => SRResources.ResourceManager.GetString(nameof (MustBeDateTimeProperty), SRResources.resourceCulture);

    internal static string MustBeEnumProperty => SRResources.ResourceManager.GetString(nameof (MustBeEnumProperty), SRResources.resourceCulture);

    internal static string MustBeNavigationProperty => SRResources.ResourceManager.GetString(nameof (MustBeNavigationProperty), SRResources.resourceCulture);

    internal static string MustBePrimitiveProperty => SRResources.ResourceManager.GetString(nameof (MustBePrimitiveProperty), SRResources.resourceCulture);

    internal static string MustBePrimitiveType => SRResources.ResourceManager.GetString(nameof (MustBePrimitiveType), SRResources.resourceCulture);

    internal static string MustBeTimeSpanProperty => SRResources.ResourceManager.GetString(nameof (MustBeTimeSpanProperty), SRResources.resourceCulture);

    internal static string MustHaveMatchingMultiplicity => SRResources.ResourceManager.GetString(nameof (MustHaveMatchingMultiplicity), SRResources.resourceCulture);

    internal static string NavigationPropertyBindingPathIsNotValid => SRResources.ResourceManager.GetString(nameof (NavigationPropertyBindingPathIsNotValid), SRResources.resourceCulture);

    internal static string NavigationPropertyBindingPathNotInHierarchy => SRResources.ResourceManager.GetString(nameof (NavigationPropertyBindingPathNotInHierarchy), SRResources.resourceCulture);

    internal static string NavigationPropertyBindingPathNotSupported => SRResources.ResourceManager.GetString(nameof (NavigationPropertyBindingPathNotSupported), SRResources.resourceCulture);

    internal static string NavigationPropertyNotInHierarchy => SRResources.ResourceManager.GetString(nameof (NavigationPropertyNotInHierarchy), SRResources.resourceCulture);

    internal static string NavigationSourceMissingDuringDeserialization => SRResources.ResourceManager.GetString(nameof (NavigationSourceMissingDuringDeserialization), SRResources.resourceCulture);

    internal static string NavigationSourceMissingDuringSerialization => SRResources.ResourceManager.GetString(nameof (NavigationSourceMissingDuringSerialization), SRResources.resourceCulture);

    internal static string NavigationSourceTypeHasNoKeys => SRResources.ResourceManager.GetString(nameof (NavigationSourceTypeHasNoKeys), SRResources.resourceCulture);

    internal static string EntitySetTypeHasNoKeys => SRResources.ResourceManager.GetString(nameof (EntitySetTypeHasNoKeys), SRResources.resourceCulture);

    internal static string NestedCollectionsNotSupported => SRResources.ResourceManager.GetString(nameof (NestedCollectionsNotSupported), SRResources.resourceCulture);

    internal static string NestedPropertyNotfound => SRResources.ResourceManager.GetString(nameof (NestedPropertyNotfound), SRResources.resourceCulture);

    internal static string NoKeyNameFoundInSegment => SRResources.ResourceManager.GetString(nameof (NoKeyNameFoundInSegment), SRResources.resourceCulture);

    internal static string NoMatchingIEdmTypeFound => SRResources.ResourceManager.GetString(nameof (NoMatchingIEdmTypeFound), SRResources.resourceCulture);

    internal static string NoMatchingResource => SRResources.ResourceManager.GetString(nameof (NoMatchingResource), SRResources.resourceCulture);

    internal static string NonNullUriRequiredForMediaTypeMapping => SRResources.ResourceManager.GetString(nameof (NonNullUriRequiredForMediaTypeMapping), SRResources.resourceCulture);

    internal static string NoNonODataHttpRouteRegistered => SRResources.ResourceManager.GetString(nameof (NoNonODataHttpRouteRegistered), SRResources.resourceCulture);

    internal static string NonSelectExpandOnSingleEntity => SRResources.ResourceManager.GetString(nameof (NonSelectExpandOnSingleEntity), SRResources.resourceCulture);

    internal static string NoRoutingHandlerToSelectAction => SRResources.ResourceManager.GetString(nameof (NoRoutingHandlerToSelectAction), SRResources.resourceCulture);

    internal static string NotAllowedArithmeticOperator => SRResources.ResourceManager.GetString(nameof (NotAllowedArithmeticOperator), SRResources.resourceCulture);

    internal static string NotAllowedFunction => SRResources.ResourceManager.GetString(nameof (NotAllowedFunction), SRResources.resourceCulture);

    internal static string NotAllowedLogicalOperator => SRResources.ResourceManager.GetString(nameof (NotAllowedLogicalOperator), SRResources.resourceCulture);

    internal static string NotAllowedOrderByProperty => SRResources.ResourceManager.GetString(nameof (NotAllowedOrderByProperty), SRResources.resourceCulture);

    internal static string NotAllowedQueryOption => SRResources.ResourceManager.GetString(nameof (NotAllowedQueryOption), SRResources.resourceCulture);

    internal static string NotCountableEntitySetUsedForCount => SRResources.ResourceManager.GetString(nameof (NotCountableEntitySetUsedForCount), SRResources.resourceCulture);

    internal static string NotCountablePropertyUsedForCount => SRResources.ResourceManager.GetString(nameof (NotCountablePropertyUsedForCount), SRResources.resourceCulture);

    internal static string NotExpandablePropertyUsedInExpand => SRResources.ResourceManager.GetString(nameof (NotExpandablePropertyUsedInExpand), SRResources.resourceCulture);

    internal static string NotFilterablePropertyUsedInFilter => SRResources.ResourceManager.GetString(nameof (NotFilterablePropertyUsedInFilter), SRResources.resourceCulture);

    internal static string NotNavigablePropertyUsedInNavigation => SRResources.ResourceManager.GetString(nameof (NotNavigablePropertyUsedInNavigation), SRResources.resourceCulture);

    internal static string NotSelectablePropertyUsedInSelect => SRResources.ResourceManager.GetString(nameof (NotSelectablePropertyUsedInSelect), SRResources.resourceCulture);

    internal static string NotSortablePropertyUsedInOrderBy => SRResources.ResourceManager.GetString(nameof (NotSortablePropertyUsedInOrderBy), SRResources.resourceCulture);

    internal static string NotSupportedTransformationKind => SRResources.ResourceManager.GetString(nameof (NotSupportedTransformationKind), SRResources.resourceCulture);

    internal static string NoValueLiteralFoundInSegment => SRResources.ResourceManager.GetString(nameof (NoValueLiteralFoundInSegment), SRResources.resourceCulture);

    internal static string NullContainer => SRResources.ResourceManager.GetString(nameof (NullContainer), SRResources.resourceCulture);

    internal static string NullContainerBuilder => SRResources.ResourceManager.GetString(nameof (NullContainerBuilder), SRResources.resourceCulture);

    internal static string NullElementInCollection => SRResources.ResourceManager.GetString(nameof (NullElementInCollection), SRResources.resourceCulture);

    internal static string NullETagHandler => SRResources.ResourceManager.GetString(nameof (NullETagHandler), SRResources.resourceCulture);

    internal static string NullOnNonNullableFunctionParameter => SRResources.ResourceManager.GetString(nameof (NullOnNonNullableFunctionParameter), SRResources.resourceCulture);

    internal static string Object_NotYetInitialized => SRResources.ResourceManager.GetString(nameof (Object_NotYetInitialized), SRResources.resourceCulture);

    internal static string ODataFunctionNotSupported => SRResources.ResourceManager.GetString(nameof (ODataFunctionNotSupported), SRResources.resourceCulture);

    internal static string ODataPathMissing => SRResources.ResourceManager.GetString(nameof (ODataPathMissing), SRResources.resourceCulture);

    internal static string ODataPathNotFound => SRResources.ResourceManager.GetString(nameof (ODataPathNotFound), SRResources.resourceCulture);

    internal static string OperationHasInvalidEntitySetPath => SRResources.ResourceManager.GetString(nameof (OperationHasInvalidEntitySetPath), SRResources.resourceCulture);

    internal static string OperationImportSegmentMustBeFunction => SRResources.ResourceManager.GetString(nameof (OperationImportSegmentMustBeFunction), SRResources.resourceCulture);

    internal static string OperationSegmentMustBeFunction => SRResources.ResourceManager.GetString(nameof (OperationSegmentMustBeFunction), SRResources.resourceCulture);

    internal static string OrderByClauseNotSupported => SRResources.ResourceManager.GetString(nameof (OrderByClauseNotSupported), SRResources.resourceCulture);

    internal static string OrderByDuplicateIt => SRResources.ResourceManager.GetString(nameof (OrderByDuplicateIt), SRResources.resourceCulture);

    internal static string OrderByDuplicateProperty => SRResources.ResourceManager.GetString(nameof (OrderByDuplicateProperty), SRResources.resourceCulture);

    internal static string OrderByNodeCountExceeded => SRResources.ResourceManager.GetString(nameof (OrderByNodeCountExceeded), SRResources.resourceCulture);

    internal static string ParameterAliasMustBeInCurlyBraces => SRResources.ResourceManager.GetString(nameof (ParameterAliasMustBeInCurlyBraces), SRResources.resourceCulture);

    internal static string ParameterTypeIsNotCollection => SRResources.ResourceManager.GetString(nameof (ParameterTypeIsNotCollection), SRResources.resourceCulture);

    internal static string PropertyAlreadyDefinedInDerivedType => SRResources.ResourceManager.GetString(nameof (PropertyAlreadyDefinedInDerivedType), SRResources.resourceCulture);

    internal static string PropertyDoesNotBelongToType => SRResources.ResourceManager.GetString(nameof (PropertyDoesNotBelongToType), SRResources.resourceCulture);

    internal static string PropertyIsNotCollection => SRResources.ResourceManager.GetString(nameof (PropertyIsNotCollection), SRResources.resourceCulture);

    internal static string PropertyMustBeDateTimeOffsetOrDate => SRResources.ResourceManager.GetString(nameof (PropertyMustBeDateTimeOffsetOrDate), SRResources.resourceCulture);

    internal static string PropertyMustBeBoolean => SRResources.ResourceManager.GetString(nameof (PropertyMustBeBoolean), SRResources.resourceCulture);

    internal static string PropertyMustBeEnum => SRResources.ResourceManager.GetString(nameof (PropertyMustBeEnum), SRResources.resourceCulture);

    internal static string PropertyMustBeString => SRResources.ResourceManager.GetString(nameof (PropertyMustBeString), SRResources.resourceCulture);

    internal static string PropertyMustBeStringLengthOne => SRResources.ResourceManager.GetString(nameof (PropertyMustBeStringLengthOne), SRResources.resourceCulture);

    internal static string PropertyMustBeStringMaxLengthOne => SRResources.ResourceManager.GetString(nameof (PropertyMustBeStringMaxLengthOne), SRResources.resourceCulture);

    internal static string PropertyMustBeTimeOfDay => SRResources.ResourceManager.GetString(nameof (PropertyMustBeTimeOfDay), SRResources.resourceCulture);

    internal static string PropertyMustHavePublicGetterAndSetter => SRResources.ResourceManager.GetString(nameof (PropertyMustHavePublicGetterAndSetter), SRResources.resourceCulture);

    internal static string PropertyNotFound => SRResources.ResourceManager.GetString(nameof (PropertyNotFound), SRResources.resourceCulture);

    internal static string PropertyOrPathWasRemovedFromContext => SRResources.ResourceManager.GetString(nameof (PropertyOrPathWasRemovedFromContext), SRResources.resourceCulture);

    internal static string QueryCannotBeEmpty => SRResources.ResourceManager.GetString(nameof (QueryCannotBeEmpty), SRResources.resourceCulture);

    internal static string QueryGetModelMustNotReturnNull => SRResources.ResourceManager.GetString(nameof (QueryGetModelMustNotReturnNull), SRResources.resourceCulture);

    internal static string QueryingRequiresObjectContent => SRResources.ResourceManager.GetString(nameof (QueryingRequiresObjectContent), SRResources.resourceCulture);

    internal static string QueryNodeBindingNotSupported => SRResources.ResourceManager.GetString(nameof (QueryNodeBindingNotSupported), SRResources.resourceCulture);

    internal static string QueryNodeValidationNotSupported => SRResources.ResourceManager.GetString(nameof (QueryNodeValidationNotSupported), SRResources.resourceCulture);

    internal static string QueryParameterNotSupported => SRResources.ResourceManager.GetString(nameof (QueryParameterNotSupported), SRResources.resourceCulture);

    internal static string ReadFromStreamAsyncMustHaveRequest => SRResources.ResourceManager.GetString(nameof (ReadFromStreamAsyncMustHaveRequest), SRResources.resourceCulture);

    internal static string RebindingNotSupported => SRResources.ResourceManager.GetString(nameof (RebindingNotSupported), SRResources.resourceCulture);

    internal static string ReferenceNavigationPropertyExpandFilterVisitorUnexpectedParameter => SRResources.ResourceManager.GetString(nameof (ReferenceNavigationPropertyExpandFilterVisitorUnexpectedParameter), SRResources.resourceCulture);

    internal static string ReferentialConstraintAlreadyConfigured => SRResources.ResourceManager.GetString(nameof (ReferentialConstraintAlreadyConfigured), SRResources.resourceCulture);

    internal static string ReferentialConstraintOnManyNavigationPropertyNotSupported => SRResources.ResourceManager.GetString(nameof (ReferentialConstraintOnManyNavigationPropertyNotSupported), SRResources.resourceCulture);

    internal static string ReferentialConstraintPropertyTypeNotValid => SRResources.ResourceManager.GetString(nameof (ReferentialConstraintPropertyTypeNotValid), SRResources.resourceCulture);

    internal static string RequestContainerAlreadyExists => SRResources.ResourceManager.GetString(nameof (RequestContainerAlreadyExists), SRResources.resourceCulture);

    internal static string RequestMustContainConfiguration => SRResources.ResourceManager.GetString(nameof (RequestMustContainConfiguration), SRResources.resourceCulture);

    internal static string RequestMustHaveModel => SRResources.ResourceManager.GetString(nameof (RequestMustHaveModel), SRResources.resourceCulture);

    internal static string RequestMustHaveODataRouteName => SRResources.ResourceManager.GetString(nameof (RequestMustHaveODataRouteName), SRResources.resourceCulture);

    internal static string RequestNotActionInvocation => SRResources.ResourceManager.GetString(nameof (RequestNotActionInvocation), SRResources.resourceCulture);

    internal static string RequestUriTooShortForODataPath => SRResources.ResourceManager.GetString(nameof (RequestUriTooShortForODataPath), SRResources.resourceCulture);

    internal static string ResourceTypeNotInModel => SRResources.ResourceManager.GetString(nameof (ResourceTypeNotInModel), SRResources.resourceCulture);

    internal static string ReturnEntityCollectionWithoutEntitySet => SRResources.ResourceManager.GetString(nameof (ReturnEntityCollectionWithoutEntitySet), SRResources.resourceCulture);

    internal static string ReturnEntityWithoutEntitySet => SRResources.ResourceManager.GetString(nameof (ReturnEntityWithoutEntitySet), SRResources.resourceCulture);

    internal static string RootElementNameMissing => SRResources.ResourceManager.GetString(nameof (RootElementNameMissing), SRResources.resourceCulture);

    internal static string RoutePrefixStartsWithSlash => SRResources.ResourceManager.GetString(nameof (RoutePrefixStartsWithSlash), SRResources.resourceCulture);

    internal static string SelectExpandEmptyOrNull => SRResources.ResourceManager.GetString(nameof (SelectExpandEmptyOrNull), SRResources.resourceCulture);

    internal static string SelectionTypeNotSupported => SRResources.ResourceManager.GetString(nameof (SelectionTypeNotSupported), SRResources.resourceCulture);

    internal static string SelectNonStructured => SRResources.ResourceManager.GetString(nameof (SelectNonStructured), SRResources.resourceCulture);

    internal static string SingleResultHasMoreThanOneEntity => SRResources.ResourceManager.GetString(nameof (SingleResultHasMoreThanOneEntity), SRResources.resourceCulture);

    internal static string SingletonAlreadyConfiguredDifferentEntityType => SRResources.ResourceManager.GetString(nameof (SingletonAlreadyConfiguredDifferentEntityType), SRResources.resourceCulture);

    internal static string SingletonNameAlreadyConfiguredAsEntitySet => SRResources.ResourceManager.GetString(nameof (SingletonNameAlreadyConfiguredAsEntitySet), SRResources.resourceCulture);

    internal static string SkipTopLimitExceeded => SRResources.ResourceManager.GetString(nameof (SkipTopLimitExceeded), SRResources.resourceCulture);

    internal static string SkipTokenParseError => SRResources.ResourceManager.GetString(nameof (SkipTokenParseError), SRResources.resourceCulture);

    internal static string TargetEntityTypeMissing => SRResources.ResourceManager.GetString(nameof (TargetEntityTypeMissing), SRResources.resourceCulture);

    internal static string TargetKindNotImplemented => SRResources.ResourceManager.GetString(nameof (TargetKindNotImplemented), SRResources.resourceCulture);

    internal static string TypeCannotBeComplexWasEntity => SRResources.ResourceManager.GetString(nameof (TypeCannotBeComplexWasEntity), SRResources.resourceCulture);

    internal static string TypeCannotBeDeserialized => SRResources.ResourceManager.GetString(nameof (TypeCannotBeDeserialized), SRResources.resourceCulture);

    internal static string TypeCannotBeEntityWasComplex => SRResources.ResourceManager.GetString(nameof (TypeCannotBeEntityWasComplex), SRResources.resourceCulture);

    internal static string TypeCannotBeEnum => SRResources.ResourceManager.GetString(nameof (TypeCannotBeEnum), SRResources.resourceCulture);

    internal static string TypeCannotBeSerialized => SRResources.ResourceManager.GetString(nameof (TypeCannotBeSerialized), SRResources.resourceCulture);

    internal static string TypeDoesNotInheritFromBaseType => SRResources.ResourceManager.GetString(nameof (TypeDoesNotInheritFromBaseType), SRResources.resourceCulture);

    internal static string TypeMustBeEntity => SRResources.ResourceManager.GetString(nameof (TypeMustBeEntity), SRResources.resourceCulture);

    internal static string TypeMustBeEnumOrNullableEnum => SRResources.ResourceManager.GetString(nameof (TypeMustBeEnumOrNullableEnum), SRResources.resourceCulture);

    internal static string TypeMustBeResourceSet => SRResources.ResourceManager.GetString(nameof (TypeMustBeResourceSet), SRResources.resourceCulture);

    internal static string TypeOfDynamicPropertyNotSupported => SRResources.ResourceManager.GetString(nameof (TypeOfDynamicPropertyNotSupported), SRResources.resourceCulture);

    internal static string UnableToDetermineBaseUrl => SRResources.ResourceManager.GetString(nameof (UnableToDetermineBaseUrl), SRResources.resourceCulture);

    internal static string UnableToDetermineMetadataUrl => SRResources.ResourceManager.GetString(nameof (UnableToDetermineMetadataUrl), SRResources.resourceCulture);

    internal static string UnaryNodeValidationNotSupported => SRResources.ResourceManager.GetString(nameof (UnaryNodeValidationNotSupported), SRResources.resourceCulture);

    internal static string UnexpectedElementType => SRResources.ResourceManager.GetString(nameof (UnexpectedElementType), SRResources.resourceCulture);

    internal static string UnresolvedPathSegmentInTemplate => SRResources.ResourceManager.GetString(nameof (UnresolvedPathSegmentInTemplate), SRResources.resourceCulture);

    internal static string UnsupportedEdmType => SRResources.ResourceManager.GetString(nameof (UnsupportedEdmType), SRResources.resourceCulture);

    internal static string UnsupportedEdmTypeKind => SRResources.ResourceManager.GetString(nameof (UnsupportedEdmTypeKind), SRResources.resourceCulture);

    internal static string UnsupportedExpressionNodeType => SRResources.ResourceManager.GetString(nameof (UnsupportedExpressionNodeType), SRResources.resourceCulture);

    internal static string UnsupportedExpressionNodeTypeWithName => SRResources.ResourceManager.GetString(nameof (UnsupportedExpressionNodeTypeWithName), SRResources.resourceCulture);

    internal static string UnsupportedSelectExpandPath => SRResources.ResourceManager.GetString(nameof (UnsupportedSelectExpandPath), SRResources.resourceCulture);

    internal static string InvalidSegmentInSelectExpandPath => SRResources.ResourceManager.GetString(nameof (InvalidSegmentInSelectExpandPath), SRResources.resourceCulture);

    internal static string InvalidLastSegmentInSelectExpandPath => SRResources.ResourceManager.GetString(nameof (InvalidLastSegmentInSelectExpandPath), SRResources.resourceCulture);

    internal static string UnterminatedStringLiteral => SRResources.ResourceManager.GetString(nameof (UnterminatedStringLiteral), SRResources.resourceCulture);

    internal static string UriFunctionClrBinderAlreadyBound => SRResources.ResourceManager.GetString(nameof (UriFunctionClrBinderAlreadyBound), SRResources.resourceCulture);

    internal static string UriQueryStringInvalid => SRResources.ResourceManager.GetString(nameof (UriQueryStringInvalid), SRResources.resourceCulture);

    internal static string UrlHelperNull => SRResources.ResourceManager.GetString(nameof (UrlHelperNull), SRResources.resourceCulture);

    internal static string ValueIsInvalid => SRResources.ResourceManager.GetString(nameof (ValueIsInvalid), SRResources.resourceCulture);

    internal static string WriteObjectInlineNotSupported => SRResources.ResourceManager.GetString(nameof (WriteObjectInlineNotSupported), SRResources.resourceCulture);

    internal static string WriteObjectNotSupported => SRResources.ResourceManager.GetString(nameof (WriteObjectNotSupported), SRResources.resourceCulture);

    internal static string WriteToStreamAsyncMustHaveRequest => SRResources.ResourceManager.GetString(nameof (WriteToStreamAsyncMustHaveRequest), SRResources.resourceCulture);
  }
}
