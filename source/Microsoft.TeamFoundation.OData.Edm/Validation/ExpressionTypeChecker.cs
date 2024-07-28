// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Validation.ExpressionTypeChecker
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.OData.Edm.Validation
{
  public static class ExpressionTypeChecker
  {
    private static readonly bool[,] promotionMap = ExpressionTypeChecker.InitializePromotionMap();

    public static bool TryCast(
      this IEdmExpression expression,
      IEdmTypeReference type,
      out IEnumerable<EdmError> discoveredErrors)
    {
      return expression.TryCast(type, (IEdmType) null, false, out discoveredErrors);
    }

    public static bool TryCast(
      this IEdmExpression expression,
      IEdmTypeReference type,
      IEdmType context,
      bool matchExactly,
      out IEnumerable<EdmError> discoveredErrors)
    {
      EdmUtil.CheckArgumentNull<IEdmExpression>(expression, nameof (expression));
      type = type.AsActualTypeReference();
      if (type == null || type.TypeKind() == EdmTypeKind.None)
      {
        discoveredErrors = Enumerable.Empty<EdmError>();
        return true;
      }
      switch (expression.ExpressionKind)
      {
        case EdmExpressionKind.BinaryConstant:
        case EdmExpressionKind.BooleanConstant:
        case EdmExpressionKind.DateTimeOffsetConstant:
        case EdmExpressionKind.DecimalConstant:
        case EdmExpressionKind.FloatingConstant:
        case EdmExpressionKind.GuidConstant:
        case EdmExpressionKind.IntegerConstant:
        case EdmExpressionKind.StringConstant:
        case EdmExpressionKind.DurationConstant:
        case EdmExpressionKind.DateConstant:
        case EdmExpressionKind.TimeOfDayConstant:
          IEdmPrimitiveValue expression1 = (IEdmPrimitiveValue) expression;
          return expression1.Type != null ? expression1.Type.TestTypeReferenceMatch(type, expression.Location(), matchExactly, out discoveredErrors) : expression1.TryCastPrimitiveAsType(type, out discoveredErrors);
        case EdmExpressionKind.Null:
          return ((IEdmNullExpression) expression).TryCastNullAsType(type, out discoveredErrors);
        case EdmExpressionKind.Record:
          IEdmRecordExpression expression2 = (IEdmRecordExpression) expression;
          return expression2.DeclaredType != null ? expression2.DeclaredType.TestTypeReferenceMatch(type, expression.Location(), matchExactly, out discoveredErrors) : expression2.TryCastRecordAsType(type, context, matchExactly, out discoveredErrors);
        case EdmExpressionKind.Collection:
          IEdmCollectionExpression expression3 = (IEdmCollectionExpression) expression;
          return expression3.DeclaredType != null ? expression3.DeclaredType.TestTypeReferenceMatch(type, expression.Location(), matchExactly, out discoveredErrors) : expression3.TryCastCollectionAsType(type, context, matchExactly, out discoveredErrors);
        case EdmExpressionKind.Path:
        case EdmExpressionKind.PropertyPath:
        case EdmExpressionKind.NavigationPropertyPath:
          return ((IEdmPathExpression) expression).TryCastPathAsType(type, context, matchExactly, out discoveredErrors);
        case EdmExpressionKind.If:
          return ((IEdmIfExpression) expression).TryCastIfAsType(type, context, matchExactly, out discoveredErrors);
        case EdmExpressionKind.Cast:
          return ((IEdmCastExpression) expression).Type.TestTypeReferenceMatch(type, expression.Location(), matchExactly, out discoveredErrors);
        case EdmExpressionKind.IsType:
          return EdmCoreModel.Instance.GetBoolean(false).TestTypeReferenceMatch(type, expression.Location(), matchExactly, out discoveredErrors);
        case EdmExpressionKind.FunctionApplication:
          IEdmApplyExpression edmApplyExpression = (IEdmApplyExpression) expression;
          if (edmApplyExpression.AppliedFunction != null)
          {
            IEdmOperation appliedFunction = (IEdmOperation) edmApplyExpression.AppliedFunction;
            if (appliedFunction != null)
              return appliedFunction.ReturnType.TestTypeReferenceMatch(type, expression.Location(), matchExactly, out discoveredErrors);
          }
          discoveredErrors = Enumerable.Empty<EdmError>();
          return true;
        case EdmExpressionKind.LabeledExpressionReference:
          return ((IEdmLabeledExpressionReferenceExpression) expression).ReferencedLabeledExpression.TryCast(type, out discoveredErrors);
        case EdmExpressionKind.Labeled:
          return ((IEdmLabeledExpression) expression).Expression.TryCast(type, context, matchExactly, out discoveredErrors);
        case EdmExpressionKind.EnumMember:
          return ExpressionTypeChecker.TryCastEnumConstantAsType((IEdmEnumMemberExpression) expression, type, matchExactly, out discoveredErrors);
        default:
          discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
          {
            new EdmError(expression.Location(), EdmErrorCode.ExpressionNotValidForTheAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionNotValidForTheAssertedType)
          };
          return false;
      }
    }

    internal static bool TryCastPrimitiveAsType(
      this IEdmPrimitiveValue expression,
      IEdmTypeReference type,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!type.IsPrimitive())
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.PrimitiveConstantExpressionNotValidForNonPrimitiveType, Strings.EdmModel_Validator_Semantic_PrimitiveConstantExpressionNotValidForNonPrimitiveType)
        };
        return false;
      }
      switch (expression.ValueKind)
      {
        case EdmValueKind.Binary:
          return ExpressionTypeChecker.TryCastBinaryConstantAsType((IEdmBinaryConstantExpression) expression, type, out discoveredErrors);
        case EdmValueKind.Boolean:
          return ExpressionTypeChecker.TryCastBooleanConstantAsType((IEdmBooleanConstantExpression) expression, type, out discoveredErrors);
        case EdmValueKind.DateTimeOffset:
          return ExpressionTypeChecker.TryCastDateTimeOffsetConstantAsType((IEdmDateTimeOffsetConstantExpression) expression, type, out discoveredErrors);
        case EdmValueKind.Decimal:
          return ExpressionTypeChecker.TryCastDecimalConstantAsType((IEdmDecimalConstantExpression) expression, type, out discoveredErrors);
        case EdmValueKind.Floating:
          return ExpressionTypeChecker.TryCastFloatingConstantAsType((IEdmFloatingConstantExpression) expression, type, out discoveredErrors);
        case EdmValueKind.Guid:
          return ExpressionTypeChecker.TryCastGuidConstantAsType((IEdmGuidConstantExpression) expression, type, out discoveredErrors);
        case EdmValueKind.Integer:
          return ExpressionTypeChecker.TryCastIntegerConstantAsType((IEdmIntegerConstantExpression) expression, type, out discoveredErrors);
        case EdmValueKind.String:
          return ExpressionTypeChecker.TryCastStringConstantAsType((IEdmStringConstantExpression) expression, type, out discoveredErrors);
        case EdmValueKind.Duration:
          return ExpressionTypeChecker.TryCastDurationConstantAsType((IEdmDurationConstantExpression) expression, type, out discoveredErrors);
        case EdmValueKind.Date:
          return ExpressionTypeChecker.TryCastDateConstantAsType((IEdmDateConstantExpression) expression, type, out discoveredErrors);
        case EdmValueKind.TimeOfDay:
          return ExpressionTypeChecker.TryCastTimeOfDayConstantAsType((IEdmTimeOfDayConstantExpression) expression, type, out discoveredErrors);
        default:
          discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
          {
            new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType)
          };
          return false;
      }
    }

    internal static bool TryCastNullAsType(
      this IEdmNullExpression expression,
      IEdmTypeReference type,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!type.IsNullable)
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.NullCannotBeAssertedToBeANonNullableType, Strings.EdmModel_Validator_Semantic_NullCannotBeAssertedToBeANonNullableType)
        };
        return false;
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    internal static bool TryCastPathAsType(
      this IEdmPathExpression expression,
      IEdmTypeReference type,
      IEdmType context,
      bool matchExactly,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (context is IEdmStructuredType)
      {
        IEdmType expressionType = context;
        foreach (string pathSegment in expression.PathSegments)
        {
          if (!(expressionType is IEdmStructuredType edmStructuredType))
          {
            discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
            {
              new EdmError(expression.Location(), EdmErrorCode.PathIsNotValidForTheGivenContext, Strings.EdmModel_Validator_Semantic_PathIsNotValidForTheGivenContext((object) pathSegment))
            };
            return false;
          }
          expressionType = edmStructuredType.FindProperty(pathSegment)?.Type.Definition;
          if (expressionType == null)
          {
            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
          }
        }
        return expressionType.TestTypeMatch(type.Definition, expression.Location(), matchExactly, out discoveredErrors);
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    internal static bool TryCastIfAsType(
      this IEdmIfExpression expression,
      IEdmTypeReference type,
      IEdmType context,
      bool matchExactly,
      out IEnumerable<EdmError> discoveredErrors)
    {
      IEnumerable<EdmError> discoveredErrors1;
      bool flag1 = expression.TrueExpression.TryCast(type, context, matchExactly, out discoveredErrors1);
      IEnumerable<EdmError> discoveredErrors2;
      bool flag2 = expression.FalseExpression.TryCast(type, context, matchExactly, out discoveredErrors2) & flag1;
      if (!flag2)
      {
        List<EdmError> edmErrorList = new List<EdmError>(discoveredErrors1);
        edmErrorList.AddRange(discoveredErrors2);
        discoveredErrors = (IEnumerable<EdmError>) edmErrorList;
      }
      else
        discoveredErrors = Enumerable.Empty<EdmError>();
      return flag2;
    }

    internal static bool TryCastRecordAsType(
      this IEdmRecordExpression expression,
      IEdmTypeReference type,
      IEdmType context,
      bool matchExactly,
      out IEnumerable<EdmError> discoveredErrors)
    {
      EdmUtil.CheckArgumentNull<IEdmRecordExpression>(expression, nameof (expression));
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      if (!type.IsStructured())
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.RecordExpressionNotValidForNonStructuredType, Strings.EdmModel_Validator_Semantic_RecordExpressionNotValidForNonStructuredType)
        };
        return false;
      }
      HashSetInternal<string> hashSetInternal = new HashSetInternal<string>();
      List<EdmError> source = new List<EdmError>();
      IEdmStructuredTypeReference type1 = type.AsStructured();
      foreach (IEdmProperty property in type1.StructuredDefinition().Properties())
      {
        IEdmProperty typeProperty = property;
        IEdmPropertyConstructor propertyConstructor = expression.Properties.FirstOrDefault<IEdmPropertyConstructor>((Func<IEdmPropertyConstructor, bool>) (p => p.Name == typeProperty.Name));
        if (propertyConstructor == null)
        {
          source.Add(new EdmError(expression.Location(), EdmErrorCode.RecordExpressionMissingRequiredProperty, Strings.EdmModel_Validator_Semantic_RecordExpressionMissingProperty((object) typeProperty.Name)));
        }
        else
        {
          IEnumerable<EdmError> discoveredErrors1;
          if (!propertyConstructor.Value.TryCast(typeProperty.Type, context, matchExactly, out discoveredErrors1))
          {
            foreach (EdmError edmError in discoveredErrors1)
              source.Add(edmError);
          }
          hashSetInternal.Add(typeProperty.Name);
        }
      }
      if (!type1.IsOpen())
      {
        foreach (IEdmPropertyConstructor property in expression.Properties)
        {
          if (!hashSetInternal.Contains(property.Name))
            source.Add(new EdmError(expression.Location(), EdmErrorCode.RecordExpressionHasExtraProperties, Strings.EdmModel_Validator_Semantic_RecordExpressionHasExtraProperties((object) property.Name)));
        }
      }
      if (source.FirstOrDefault<EdmError>() != null)
      {
        discoveredErrors = (IEnumerable<EdmError>) source;
        return false;
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    internal static bool TryCastCollectionAsType(
      this IEdmCollectionExpression expression,
      IEdmTypeReference type,
      IEdmType context,
      bool matchExactly,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!type.IsCollection())
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.CollectionExpressionNotValidForNonCollectionType, Strings.EdmModel_Validator_Semantic_CollectionExpressionNotValidForNonCollectionType)
        };
        return false;
      }
      IEdmTypeReference type1 = type.AsCollection().ElementType();
      bool flag = true;
      List<EdmError> edmErrorList = new List<EdmError>();
      foreach (IEdmExpression element in expression.Elements)
      {
        IEnumerable<EdmError> discoveredErrors1;
        flag = element.TryCast(type1, context, matchExactly, out discoveredErrors1) & flag;
        edmErrorList.AddRange(discoveredErrors1);
      }
      discoveredErrors = (IEnumerable<EdmError>) edmErrorList;
      return flag;
    }

    private static bool TryCastGuidConstantAsType(
      IEdmGuidConstantExpression expression,
      IEdmTypeReference type,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!type.IsGuid())
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType)
        };
        return false;
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    private static bool TryCastFloatingConstantAsType(
      IEdmFloatingConstantExpression expression,
      IEdmTypeReference type,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!type.IsFloating())
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType)
        };
        return false;
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    private static bool TryCastDecimalConstantAsType(
      IEdmDecimalConstantExpression expression,
      IEdmTypeReference type,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!type.IsDecimal())
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType)
        };
        return false;
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    private static bool TryCastDateTimeOffsetConstantAsType(
      IEdmDateTimeOffsetConstantExpression expression,
      IEdmTypeReference type,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!type.IsDateTimeOffset())
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType)
        };
        return false;
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    private static bool TryCastDurationConstantAsType(
      IEdmDurationConstantExpression expression,
      IEdmTypeReference type,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!type.IsDuration())
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType)
        };
        return false;
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    private static bool TryCastDateConstantAsType(
      IEdmDateConstantExpression expression,
      IEdmTypeReference type,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!type.IsDate())
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType)
        };
        return false;
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    private static bool TryCastTimeOfDayConstantAsType(
      IEdmTimeOfDayConstantExpression expression,
      IEdmTypeReference type,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!type.IsTimeOfDay())
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType)
        };
        return false;
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    private static bool TryCastBooleanConstantAsType(
      IEdmBooleanConstantExpression expression,
      IEdmTypeReference type,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!type.IsBoolean())
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType)
        };
        return false;
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    private static bool TryCastStringConstantAsType(
      IEdmStringConstantExpression expression,
      IEdmTypeReference type,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!type.IsString())
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType)
        };
        return false;
      }
      IEdmStringTypeReference stringTypeReference = type.AsString();
      int? maxLength = stringTypeReference.MaxLength;
      if (maxLength.HasValue)
      {
        int length1 = expression.Value.Length;
        maxLength = stringTypeReference.MaxLength;
        int num = maxLength.Value;
        if (length1 > num)
        {
          ref IEnumerable<EdmError> local = ref discoveredErrors;
          EdmError[] edmErrorArray = new EdmError[1];
          EdmLocation errorLocation = expression.Location();
          // ISSUE: variable of a boxed type
          __Boxed<int> length2 = (ValueType) expression.Value.Length;
          maxLength = stringTypeReference.MaxLength;
          // ISSUE: variable of a boxed type
          __Boxed<int> p1 = (ValueType) maxLength.Value;
          string errorMessage = Strings.EdmModel_Validator_Semantic_StringConstantLengthOutOfRange((object) length2, (object) p1);
          edmErrorArray[0] = new EdmError(errorLocation, EdmErrorCode.StringConstantLengthOutOfRange, errorMessage);
          local = (IEnumerable<EdmError>) edmErrorArray;
          return false;
        }
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    private static bool TryCastIntegerConstantAsType(
      IEdmIntegerConstantExpression expression,
      IEdmTypeReference type,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!type.IsIntegral())
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType)
        };
        return false;
      }
      switch (type.PrimitiveKind())
      {
        case EdmPrimitiveTypeKind.Byte:
          return ExpressionTypeChecker.TryCastIntegerConstantInRange(expression, 0L, (long) byte.MaxValue, out discoveredErrors);
        case EdmPrimitiveTypeKind.Int16:
          return ExpressionTypeChecker.TryCastIntegerConstantInRange(expression, (long) short.MinValue, (long) short.MaxValue, out discoveredErrors);
        case EdmPrimitiveTypeKind.Int32:
          return ExpressionTypeChecker.TryCastIntegerConstantInRange(expression, (long) int.MinValue, (long) int.MaxValue, out discoveredErrors);
        case EdmPrimitiveTypeKind.Int64:
          return ExpressionTypeChecker.TryCastIntegerConstantInRange(expression, long.MinValue, long.MaxValue, out discoveredErrors);
        case EdmPrimitiveTypeKind.SByte:
          return ExpressionTypeChecker.TryCastIntegerConstantInRange(expression, (long) sbyte.MinValue, (long) sbyte.MaxValue, out discoveredErrors);
        default:
          discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
          {
            new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType)
          };
          return false;
      }
    }

    private static bool TryCastIntegerConstantInRange(
      IEdmIntegerConstantExpression expression,
      long min,
      long max,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (expression.Value < min || expression.Value > max)
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.IntegerConstantValueOutOfRange, Strings.EdmModel_Validator_Semantic_IntegerConstantValueOutOfRange)
        };
        return false;
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    private static bool TryCastBinaryConstantAsType(
      IEdmBinaryConstantExpression expression,
      IEdmTypeReference type,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!type.IsBinary())
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType)
        };
        return false;
      }
      IEdmBinaryTypeReference binaryTypeReference = type.AsBinary();
      int? maxLength = binaryTypeReference.MaxLength;
      if (maxLength.HasValue)
      {
        int length1 = expression.Value.Length;
        maxLength = binaryTypeReference.MaxLength;
        int num = maxLength.Value;
        if (length1 > num)
        {
          ref IEnumerable<EdmError> local = ref discoveredErrors;
          EdmError[] edmErrorArray = new EdmError[1];
          EdmLocation errorLocation = expression.Location();
          // ISSUE: variable of a boxed type
          __Boxed<int> length2 = (ValueType) expression.Value.Length;
          maxLength = binaryTypeReference.MaxLength;
          // ISSUE: variable of a boxed type
          __Boxed<int> p1 = (ValueType) maxLength.Value;
          string errorMessage = Strings.EdmModel_Validator_Semantic_BinaryConstantLengthOutOfRange((object) length2, (object) p1);
          edmErrorArray[0] = new EdmError(errorLocation, EdmErrorCode.BinaryConstantLengthOutOfRange, errorMessage);
          local = (IEnumerable<EdmError>) edmErrorArray;
          return false;
        }
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    private static bool TryCastEnumConstantAsType(
      IEdmEnumMemberExpression expression,
      IEdmTypeReference type,
      bool matchExactly,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!type.IsEnum())
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(expression.Location(), EdmErrorCode.ExpressionEnumKindNotValidForAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionEnumKindNotValidForAssertedType)
        };
        return false;
      }
      foreach (IEdmEnumMember enumMember in expression.EnumMembers)
      {
        if (!enumMember.DeclaringType.TestTypeMatch(type.Definition, expression.Location(), matchExactly, out discoveredErrors))
          return false;
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    private static bool TestTypeReferenceMatch(
      this IEdmTypeReference expressionType,
      IEdmTypeReference assertedType,
      EdmLocation location,
      bool matchExactly,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!expressionType.TestNullabilityMatch(assertedType, location, out discoveredErrors))
        return false;
      if (!expressionType.IsBad())
        return expressionType.Definition.TestTypeMatch(assertedType.Definition, location, matchExactly, out discoveredErrors);
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    private static bool TestTypeMatch(
      this IEdmType expressionType,
      IEdmType assertedType,
      EdmLocation location,
      bool matchExactly,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (matchExactly)
      {
        if (!expressionType.IsEquivalentTo(assertedType))
        {
          discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
          {
            new EdmError(location, EdmErrorCode.ExpressionNotValidForTheAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionNotValidForTheAssertedType)
          };
          return false;
        }
      }
      else
      {
        if (expressionType.TypeKind == EdmTypeKind.None || expressionType.IsBad())
        {
          discoveredErrors = Enumerable.Empty<EdmError>();
          return true;
        }
        if (expressionType.TypeKind == EdmTypeKind.Primitive && assertedType.TypeKind == EdmTypeKind.Primitive)
        {
          if (!(expressionType as IEdmPrimitiveType).PrimitiveKind.PromotesTo((assertedType as IEdmPrimitiveType).PrimitiveKind))
          {
            discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
            {
              new EdmError(location, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindCannotPromoteToAssertedType((object) expressionType.ToTraceString(), (object) assertedType.ToTraceString()))
            };
            return false;
          }
        }
        else if (!expressionType.IsOrInheritsFrom(assertedType))
        {
          discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
          {
            new EdmError(location, EdmErrorCode.ExpressionNotValidForTheAssertedType, Strings.EdmModel_Validator_Semantic_ExpressionNotValidForTheAssertedType)
          };
          return false;
        }
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    private static bool TestNullabilityMatch(
      this IEdmTypeReference expressionType,
      IEdmTypeReference assertedType,
      EdmLocation location,
      out IEnumerable<EdmError> discoveredErrors)
    {
      if (!assertedType.IsNullable && expressionType.IsNullable)
      {
        discoveredErrors = (IEnumerable<EdmError>) new EdmError[1]
        {
          new EdmError(location, EdmErrorCode.CannotAssertNullableTypeAsNonNullableType, Strings.EdmModel_Validator_Semantic_CannotAssertNullableTypeAsNonNullableType((object) expressionType.FullName()))
        };
        return false;
      }
      discoveredErrors = Enumerable.Empty<EdmError>();
      return true;
    }

    private static bool PromotesTo(
      this EdmPrimitiveTypeKind startingKind,
      EdmPrimitiveTypeKind target)
    {
      return startingKind == target || ExpressionTypeChecker.promotionMap[(int) startingKind, (int) target];
    }

    private static bool[,] InitializePromotionMap()
    {
      int length = ((IEnumerable<FieldInfo>) typeof (EdmPrimitiveTypeKind).GetFields()).Where<FieldInfo>((Func<FieldInfo, bool>) (f => f.IsLiteral)).Count<FieldInfo>();
      bool[,] flagArray = new bool[length, length];
      flagArray[3, 8] = true;
      flagArray[3, 9] = true;
      flagArray[3, 10] = true;
      flagArray[11, 8] = true;
      flagArray[11, 9] = true;
      flagArray[11, 10] = true;
      flagArray[8, 9] = true;
      flagArray[8, 10] = true;
      flagArray[9, 10] = true;
      flagArray[12, 6] = true;
      flagArray[20, 16] = true;
      flagArray[18, 16] = true;
      flagArray[22, 16] = true;
      flagArray[23, 16] = true;
      flagArray[21, 16] = true;
      flagArray[17, 16] = true;
      flagArray[19, 16] = true;
      flagArray[28, 24] = true;
      flagArray[26, 24] = true;
      flagArray[30, 24] = true;
      flagArray[31, 24] = true;
      flagArray[29, 24] = true;
      flagArray[25, 24] = true;
      flagArray[27, 24] = true;
      return flagArray;
    }
  }
}
