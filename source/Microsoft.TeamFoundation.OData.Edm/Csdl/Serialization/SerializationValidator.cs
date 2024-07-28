// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Serialization.SerializationValidator
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.Serialization
{
  internal static class SerializationValidator
  {
    private static readonly ValidationRule<IEdmTypeReference> TypeReferenceTargetMustHaveValidName = new ValidationRule<IEdmTypeReference>((Action<ValidationContext, IEdmTypeReference>) ((context, typeReference) =>
    {
      if (!(typeReference.Definition is IEdmSchemaType definition2) || EdmUtil.IsQualifiedName(definition2.FullName()))
        return;
      context.AddError(typeReference.Location(), EdmErrorCode.ReferencedTypeMustHaveValidName, Strings.Serializer_ReferencedTypeMustHaveValidName((object) definition2.FullName()));
    }));
    private static readonly ValidationRule<IEdmEntityReferenceType> EntityReferenceTargetMustHaveValidName = new ValidationRule<IEdmEntityReferenceType>((Action<ValidationContext, IEdmEntityReferenceType>) ((context, entityReference) =>
    {
      if (EdmUtil.IsQualifiedName(entityReference.EntityType.FullName()))
        return;
      context.AddError(entityReference.Location(), EdmErrorCode.ReferencedTypeMustHaveValidName, Strings.Serializer_ReferencedTypeMustHaveValidName((object) entityReference.EntityType.FullName()));
    }));
    private static readonly ValidationRule<IEdmEntitySet> EntitySetTypeMustHaveValidName = new ValidationRule<IEdmEntitySet>((Action<ValidationContext, IEdmEntitySet>) ((context, set) =>
    {
      if (EdmUtil.IsQualifiedName(set.EntityType().FullName()))
        return;
      context.AddError(set.Location(), EdmErrorCode.ReferencedTypeMustHaveValidName, Strings.Serializer_ReferencedTypeMustHaveValidName((object) set.EntityType().FullName()));
    }));
    private static readonly ValidationRule<IEdmStructuredType> StructuredTypeBaseTypeMustHaveValidName = new ValidationRule<IEdmStructuredType>((Action<ValidationContext, IEdmStructuredType>) ((context, type) =>
    {
      if (!(type.BaseType is IEdmSchemaType baseType2) || EdmUtil.IsQualifiedName(baseType2.FullName()))
        return;
      context.AddError(type.Location(), EdmErrorCode.ReferencedTypeMustHaveValidName, Strings.Serializer_ReferencedTypeMustHaveValidName((object) baseType2.FullName()));
    }));
    private static readonly ValidationRule<IEdmVocabularyAnnotation> VocabularyAnnotationOutOfLineMustHaveValidTargetName = new ValidationRule<IEdmVocabularyAnnotation>((Action<ValidationContext, IEdmVocabularyAnnotation>) ((context, annotation) =>
    {
      EdmVocabularyAnnotationSerializationLocation? serializationLocation1 = annotation.GetSerializationLocation(context.Model);
      EdmVocabularyAnnotationSerializationLocation serializationLocation2 = EdmVocabularyAnnotationSerializationLocation.OutOfLine;
      if (!(serializationLocation1.GetValueOrDefault() == serializationLocation2 & serializationLocation1.HasValue) || EdmUtil.IsQualifiedName(annotation.TargetString()))
        return;
      context.AddError(annotation.Location(), EdmErrorCode.InvalidName, Strings.Serializer_OutOfLineAnnotationTargetMustHaveValidName((object) EdmUtil.FullyQualifiedName(annotation.Target)));
    }));
    private static readonly ValidationRule<IEdmVocabularyAnnotation> VocabularyAnnotationMustHaveValidTermName = new ValidationRule<IEdmVocabularyAnnotation>((Action<ValidationContext, IEdmVocabularyAnnotation>) ((context, annotation) =>
    {
      if (EdmUtil.IsQualifiedName(annotation.Term.FullName()))
        return;
      context.AddError(annotation.Location(), EdmErrorCode.InvalidName, Strings.Serializer_OutOfLineAnnotationTargetMustHaveValidName((object) annotation.Term.FullName()));
    }));
    private static ValidationRuleSet serializationRuleSet = new ValidationRuleSet((IEnumerable<ValidationRule>) new ValidationRule[14]
    {
      (ValidationRule) SerializationValidator.TypeReferenceTargetMustHaveValidName,
      (ValidationRule) SerializationValidator.EntityReferenceTargetMustHaveValidName,
      (ValidationRule) SerializationValidator.EntitySetTypeMustHaveValidName,
      (ValidationRule) SerializationValidator.StructuredTypeBaseTypeMustHaveValidName,
      (ValidationRule) SerializationValidator.VocabularyAnnotationOutOfLineMustHaveValidTargetName,
      (ValidationRule) SerializationValidator.VocabularyAnnotationMustHaveValidTermName,
      (ValidationRule) ValidationRules.OperationImportEntitySetExpressionIsInvalid,
      (ValidationRule) ValidationRules.TypeMustNotHaveKindOfNone,
      (ValidationRule) ValidationRules.PrimitiveTypeMustNotHaveKindOfNone,
      (ValidationRule) ValidationRules.PropertyMustNotHaveKindOfNone,
      (ValidationRule) ValidationRules.SchemaElementMustNotHaveKindOfNone,
      (ValidationRule) ValidationRules.EntityContainerElementMustNotHaveKindOfNone,
      (ValidationRule) ValidationRules.EnumMustHaveIntegerUnderlyingType,
      (ValidationRule) ValidationRules.EnumMemberValueMustHaveSameTypeAsUnderlyingType
    });

    public static IEnumerable<EdmError> GetSerializationErrors(this IEdmModel root)
    {
      IEnumerable<EdmError> errors;
      root.Validate(SerializationValidator.serializationRuleSet, out errors);
      return errors.Where<EdmError>(new Func<EdmError, bool>(SerializationValidator.SignificantToSerialization));
    }

    internal static bool SignificantToSerialization(EdmError error)
    {
      if (ValidationHelper.IsInterfaceCritical(error))
        return true;
      switch (error.ErrorCode)
      {
        case EdmErrorCode.InvalidName:
        case EdmErrorCode.NameTooLong:
        case EdmErrorCode.OperationImportEntitySetExpressionIsInvalid:
        case EdmErrorCode.SystemNamespaceEncountered:
        case EdmErrorCode.InvalidNamespaceName:
        case EdmErrorCode.OperationImportParameterIncorrectType:
        case EdmErrorCode.EnumMemberValueOutOfRange:
        case EdmErrorCode.ReferencedTypeMustHaveValidName:
        case EdmErrorCode.InvalidOperationImportParameterMode:
        case EdmErrorCode.TypeMustNotHaveKindOfNone:
        case EdmErrorCode.PrimitiveTypeMustNotHaveKindOfNone:
        case EdmErrorCode.PropertyMustNotHaveKindOfNone:
        case EdmErrorCode.SchemaElementMustNotHaveKindOfNone:
        case EdmErrorCode.EntityContainerElementMustNotHaveKindOfNone:
        case EdmErrorCode.BinaryValueCannotHaveEmptyValue:
        case EdmErrorCode.EnumMustHaveIntegerUnderlyingType:
          return true;
        default:
          return false;
      }
    }
  }
}
