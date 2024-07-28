// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.QueryNodeUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal static class QueryNodeUtils
  {
    private static Dictionary<Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>, EdmPrimitiveTypeKind> additionalMap = new Dictionary<Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>, EdmPrimitiveTypeKind>((IEqualityComparer<Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>>) EqualityComparer<Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>>.Default)
    {
      {
        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(BinaryOperatorKind.Add, EdmPrimitiveTypeKind.DateTimeOffset, EdmPrimitiveTypeKind.Duration),
        EdmPrimitiveTypeKind.DateTimeOffset
      },
      {
        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(BinaryOperatorKind.Add, EdmPrimitiveTypeKind.Duration, EdmPrimitiveTypeKind.DateTimeOffset),
        EdmPrimitiveTypeKind.DateTimeOffset
      },
      {
        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(BinaryOperatorKind.Add, EdmPrimitiveTypeKind.Date, EdmPrimitiveTypeKind.Duration),
        EdmPrimitiveTypeKind.Date
      },
      {
        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(BinaryOperatorKind.Add, EdmPrimitiveTypeKind.Duration, EdmPrimitiveTypeKind.Date),
        EdmPrimitiveTypeKind.Date
      },
      {
        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(BinaryOperatorKind.Add, EdmPrimitiveTypeKind.Duration, EdmPrimitiveTypeKind.Duration),
        EdmPrimitiveTypeKind.Duration
      },
      {
        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(BinaryOperatorKind.Subtract, EdmPrimitiveTypeKind.DateTimeOffset, EdmPrimitiveTypeKind.Duration),
        EdmPrimitiveTypeKind.DateTimeOffset
      },
      {
        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(BinaryOperatorKind.Subtract, EdmPrimitiveTypeKind.DateTimeOffset, EdmPrimitiveTypeKind.DateTimeOffset),
        EdmPrimitiveTypeKind.Duration
      },
      {
        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(BinaryOperatorKind.Subtract, EdmPrimitiveTypeKind.Date, EdmPrimitiveTypeKind.Duration),
        EdmPrimitiveTypeKind.Date
      },
      {
        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(BinaryOperatorKind.Subtract, EdmPrimitiveTypeKind.Date, EdmPrimitiveTypeKind.Date),
        EdmPrimitiveTypeKind.Duration
      },
      {
        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(BinaryOperatorKind.Subtract, EdmPrimitiveTypeKind.Duration, EdmPrimitiveTypeKind.Duration),
        EdmPrimitiveTypeKind.Duration
      }
    };

    internal static IEdmPrimitiveTypeReference GetBinaryOperatorResultType(
      IEdmPrimitiveTypeReference left,
      IEdmPrimitiveTypeReference right,
      BinaryOperatorKind operatorKind)
    {
      EdmPrimitiveTypeKind kind;
      if (QueryNodeUtils.additionalMap.TryGetValue(new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(operatorKind, ExtensionMethods.PrimitiveKind(left), ExtensionMethods.PrimitiveKind(right)), out kind))
        return EdmCoreModel.Instance.GetPrimitive(kind, left.IsNullable);
      switch (operatorKind)
      {
        case BinaryOperatorKind.Or:
        case BinaryOperatorKind.And:
        case BinaryOperatorKind.Equal:
        case BinaryOperatorKind.NotEqual:
        case BinaryOperatorKind.GreaterThan:
        case BinaryOperatorKind.GreaterThanOrEqual:
        case BinaryOperatorKind.LessThan:
        case BinaryOperatorKind.LessThanOrEqual:
        case BinaryOperatorKind.Has:
          return EdmCoreModel.Instance.GetBoolean(left.IsNullable);
        case BinaryOperatorKind.Add:
        case BinaryOperatorKind.Subtract:
        case BinaryOperatorKind.Multiply:
        case BinaryOperatorKind.Divide:
        case BinaryOperatorKind.Modulo:
          return left;
        default:
          throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodes.QueryNodeUtils_BinaryOperatorResultType_UnreachableCodepath));
      }
    }
  }
}
