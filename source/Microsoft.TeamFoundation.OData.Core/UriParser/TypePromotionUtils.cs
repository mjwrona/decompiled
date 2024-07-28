// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.TypePromotionUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal static class TypePromotionUtils
  {
    internal static readonly KeyValuePair<string, FunctionSignatureWithReturnType> NotFoundKeyValuePair = new KeyValuePair<string, FunctionSignatureWithReturnType>();
    private static readonly FunctionSignature[] logicalSignatures = new FunctionSignature[2]
    {
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetBoolean(false),
        EdmCoreModel.Instance.GetBoolean(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetBoolean(true),
        EdmCoreModel.Instance.GetBoolean(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null)
    };
    private static readonly FunctionSignature[] notSignatures = new FunctionSignature[2]
    {
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[1]
      {
        EdmCoreModel.Instance.GetBoolean(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[1]
      {
        EdmCoreModel.Instance.GetBoolean(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null)
    };
    private static readonly FunctionSignature[] arithmeticSignatures = new FunctionSignature[10]
    {
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetInt32(false),
        EdmCoreModel.Instance.GetInt32(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetInt32(true),
        EdmCoreModel.Instance.GetInt32(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetInt64(false),
        EdmCoreModel.Instance.GetInt64(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetInt64(true),
        EdmCoreModel.Instance.GetInt64(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetSingle(false),
        EdmCoreModel.Instance.GetSingle(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetSingle(true),
        EdmCoreModel.Instance.GetSingle(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetDouble(false),
        EdmCoreModel.Instance.GetDouble(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetDouble(true),
        EdmCoreModel.Instance.GetDouble(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmDecimalTypeReference[2]
      {
        EdmCoreModel.Instance.GetDecimal(false),
        EdmCoreModel.Instance.GetDecimal(false)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(p, s, false)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(p, s, false))
      }),
      new FunctionSignature((IEdmTypeReference[]) new IEdmDecimalTypeReference[2]
      {
        EdmCoreModel.Instance.GetDecimal(true),
        EdmCoreModel.Instance.GetDecimal(true)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(p, s, true)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(p, s, true))
      })
    };
    private static readonly FunctionSignature[] AdditionSignatures = ((IEnumerable<FunctionSignature>) TypePromotionUtils.arithmeticSignatures).Concat<FunctionSignature>(TypePromotionUtils.GetAdditionTermporalSignatures()).ToArray<FunctionSignature>();
    private static readonly FunctionSignature[] SubtractionSignatures = ((IEnumerable<FunctionSignature>) TypePromotionUtils.arithmeticSignatures).Concat<FunctionSignature>(TypePromotionUtils.GetSubtractionTermporalSignatures()).ToArray<FunctionSignature>();
    private static readonly FunctionSignature[] relationalSignatures = new FunctionSignature[24]
    {
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetInt32(false),
        EdmCoreModel.Instance.GetInt32(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetInt32(true),
        EdmCoreModel.Instance.GetInt32(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetInt64(false),
        EdmCoreModel.Instance.GetInt64(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetInt64(true),
        EdmCoreModel.Instance.GetInt64(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetSingle(false),
        EdmCoreModel.Instance.GetSingle(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetSingle(true),
        EdmCoreModel.Instance.GetSingle(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetDouble(false),
        EdmCoreModel.Instance.GetDouble(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetDouble(true),
        EdmCoreModel.Instance.GetDouble(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmDecimalTypeReference[2]
      {
        EdmCoreModel.Instance.GetDecimal(false),
        EdmCoreModel.Instance.GetDecimal(false)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(p, s, false)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(p, s, false))
      }),
      new FunctionSignature((IEdmTypeReference[]) new IEdmDecimalTypeReference[2]
      {
        EdmCoreModel.Instance.GetDecimal(true),
        EdmCoreModel.Instance.GetDecimal(true)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(p, s, true)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(p, s, true))
      }),
      new FunctionSignature((IEdmTypeReference[]) new IEdmStringTypeReference[2]
      {
        EdmCoreModel.Instance.GetString(true),
        EdmCoreModel.Instance.GetString(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmBinaryTypeReference[2]
      {
        EdmCoreModel.Instance.GetBinary(true),
        EdmCoreModel.Instance.GetBinary(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetBoolean(false),
        EdmCoreModel.Instance.GetBoolean(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetBoolean(true),
        EdmCoreModel.Instance.GetBoolean(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetGuid(false),
        EdmCoreModel.Instance.GetGuid(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetGuid(true),
        EdmCoreModel.Instance.GetGuid(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetDate(false),
        EdmCoreModel.Instance.GetDate(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetDate(true),
        EdmCoreModel.Instance.GetDate(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false),
        EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, false)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, p, false)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, p, false))
      }),
      new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, true),
        EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, true)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, p, true)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, p, true))
      }),
      new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, false),
        EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, false)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, false)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, false))
      }),
      new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, true),
        EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, true)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, true)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, true))
      }),
      new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, false),
        EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, false)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, p, false)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, p, false))
      }),
      new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, true),
        EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, true)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, p, true)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.TimeOfDay, p, true))
      })
    };
    private static readonly FunctionSignature[] negationSignatures = new FunctionSignature[12]
    {
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[1]
      {
        EdmCoreModel.Instance.GetInt32(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[1]
      {
        EdmCoreModel.Instance.GetInt32(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[1]
      {
        EdmCoreModel.Instance.GetInt64(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[1]
      {
        EdmCoreModel.Instance.GetInt64(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[1]
      {
        EdmCoreModel.Instance.GetSingle(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[1]
      {
        EdmCoreModel.Instance.GetSingle(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[1]
      {
        EdmCoreModel.Instance.GetDouble(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[1]
      {
        EdmCoreModel.Instance.GetDouble(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null),
      new FunctionSignature((IEdmTypeReference[]) new IEdmDecimalTypeReference[1]
      {
        EdmCoreModel.Instance.GetDecimal(false)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[1]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(p, s, false))
      }),
      new FunctionSignature((IEdmTypeReference[]) new IEdmDecimalTypeReference[1]
      {
        EdmCoreModel.Instance.GetDecimal(true)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[1]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetDecimal(p, s, true))
      }),
      new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[1]
      {
        EdmCoreModel.Instance.GetDuration(false)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[1]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, false))
      }),
      new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[1]
      {
        EdmCoreModel.Instance.GetDuration(true)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[1]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, true))
      })
    };

    internal static void GetTypeFacets(IEdmTypeReference type, out int? precision, out int? scale)
    {
      precision = new int?();
      scale = new int?();
      switch (type)
      {
        case IEdmDecimalTypeReference decimalTypeReference:
          precision = decimalTypeReference.Precision;
          scale = decimalTypeReference.Scale;
          break;
        case IEdmTemporalTypeReference temporalTypeReference:
          precision = temporalTypeReference.Precision;
          break;
      }
    }

    internal static bool PromoteOperandTypes(
      BinaryOperatorKind operatorKind,
      SingleValueNode leftNode,
      SingleValueNode rightNode,
      out IEdmTypeReference left,
      out IEdmTypeReference right,
      TypeFacetsPromotionRules facetsPromotionRules)
    {
      left = leftNode.TypeReference;
      right = rightNode.TypeReference;
      if (left == null && right == null)
        return true;
      if (operatorKind == BinaryOperatorKind.NotEqual || operatorKind == BinaryOperatorKind.Equal)
      {
        if (TypePromotionUtils.TryHandleEqualityOperatorForEntityOrComplexTypes(ref left, ref right))
          return true;
        if (left != null && right != null && left.IsEnum() && right.IsString())
        {
          right = left;
          return true;
        }
        if (left != null && right != null && right.IsEnum() && left.IsString())
        {
          left = right;
          return true;
        }
        if (left == null && right != null && (right.IsEnum() || right is IEdmSpatialTypeReference))
        {
          left = right;
          return true;
        }
        if (right == null && left != null && (left.IsEnum() || left is IEdmSpatialTypeReference))
        {
          right = left;
          return true;
        }
      }
      if (left != null && right != null && left.IsEnum() && right.IsEnum())
        return string.Equals(left.FullName(), right.FullName(), StringComparison.Ordinal);
      if (left != null && left.IsTypeDefinition())
        left = (IEdmTypeReference) left.AsPrimitive();
      if (right != null && right.IsTypeDefinition())
        right = (IEdmTypeReference) right.AsPrimitive();
      if (left != null && !left.IsODataPrimitiveTypeKind() || right != null && !right.IsODataPrimitiveTypeKind())
        return false;
      FunctionSignature bestMatch;
      bool flag = TypePromotionUtils.FindBestSignature(TypePromotionUtils.GetFunctionSignatures(operatorKind), new SingleValueNode[2]
      {
        leftNode,
        rightNode
      }, new IEdmTypeReference[2]{ left, right }, out bestMatch) == 1;
      if (flag)
      {
        int? precision1;
        int? scale1;
        TypePromotionUtils.GetTypeFacets(left, out precision1, out scale1);
        int? precision2;
        int? scale2;
        TypePromotionUtils.GetTypeFacets(right, out precision2, out scale2);
        int? promotedPrecision = facetsPromotionRules.GetPromotedPrecision(precision1, precision2);
        int? promotedScale = facetsPromotionRules.GetPromotedScale(scale1, scale2);
        left = bestMatch.GetArgumentTypeWithFacets(0, promotedPrecision, promotedScale);
        right = bestMatch.GetArgumentTypeWithFacets(1, promotedPrecision, promotedScale);
      }
      return flag;
    }

    internal static bool PromoteOperandType(
      UnaryOperatorKind operatorKind,
      ref IEdmTypeReference typeReference)
    {
      if (typeReference == null)
        return true;
      FunctionSignature bestMatch;
      bool flag = TypePromotionUtils.FindBestSignature(TypePromotionUtils.GetFunctionSignatures(operatorKind), new SingleValueNode[1], new IEdmTypeReference[1]
      {
        typeReference
      }, out bestMatch) == 1;
      if (flag)
      {
        int? precision;
        int? scale;
        TypePromotionUtils.GetTypeFacets(typeReference, out precision, out scale);
        typeReference = bestMatch.GetArgumentTypeWithFacets(0, precision, scale);
      }
      return flag;
    }

    internal static KeyValuePair<string, FunctionSignatureWithReturnType> FindBestFunctionSignature(
      IList<KeyValuePair<string, FunctionSignatureWithReturnType>> nameFunctions,
      SingleValueNode[] argumentNodes,
      string functionCallToken)
    {
      IEdmTypeReference[] array = ((IEnumerable<SingleValueNode>) argumentNodes).Select<SingleValueNode, IEdmTypeReference>((Func<SingleValueNode, IEdmTypeReference>) (s => s.TypeReference)).ToArray<IEdmTypeReference>();
      IList<KeyValuePair<string, FunctionSignatureWithReturnType>> keyValuePairList1 = (IList<KeyValuePair<string, FunctionSignatureWithReturnType>>) new List<KeyValuePair<string, FunctionSignatureWithReturnType>>(nameFunctions.Count);
      foreach (KeyValuePair<string, FunctionSignatureWithReturnType> nameFunction in (IEnumerable<KeyValuePair<string, FunctionSignatureWithReturnType>>) nameFunctions)
      {
        if (nameFunction.Value.ArgumentTypes.Length == array.Length)
        {
          bool flag = true;
          for (int index = 0; index < nameFunction.Value.ArgumentTypes.Length; ++index)
          {
            if (!TypePromotionUtils.CanPromoteNodeTo(argumentNodes[index], array[index], nameFunction.Value.ArgumentTypes[index]))
            {
              flag = false;
              break;
            }
          }
          if (flag)
            keyValuePairList1.Add(nameFunction);
        }
      }
      if (keyValuePairList1.Count == 0)
        return TypePromotionUtils.NotFoundKeyValuePair;
      if (keyValuePairList1.Count == 1)
        return keyValuePairList1[0];
      IList<KeyValuePair<string, FunctionSignatureWithReturnType>> keyValuePairList2 = (IList<KeyValuePair<string, FunctionSignatureWithReturnType>>) new List<KeyValuePair<string, FunctionSignatureWithReturnType>>();
      for (int index1 = 0; index1 < keyValuePairList1.Count; ++index1)
      {
        bool flag = true;
        for (int index2 = 0; index2 < keyValuePairList1.Count; ++index2)
        {
          if (index1 != index2)
          {
            IEdmTypeReference[] argumentTypes1 = array;
            KeyValuePair<string, FunctionSignatureWithReturnType> keyValuePair = keyValuePairList1[index2];
            IEdmTypeReference[] argumentTypes2 = keyValuePair.Value.ArgumentTypes;
            keyValuePair = keyValuePairList1[index1];
            IEdmTypeReference[] argumentTypes3 = keyValuePair.Value.ArgumentTypes;
            if (TypePromotionUtils.MatchesArgumentTypesBetterThan(argumentTypes1, argumentTypes2, argumentTypes3))
            {
              flag = false;
              break;
            }
          }
        }
        if (flag)
          keyValuePairList2.Add(keyValuePairList1[index1]);
      }
      KeyValuePair<string, FunctionSignatureWithReturnType> functionSignature = TypePromotionUtils.NotFoundKeyValuePair;
      if (keyValuePairList2.Count == 1)
      {
        functionSignature = keyValuePairList2[0];
      }
      else
      {
        foreach (KeyValuePair<string, FunctionSignatureWithReturnType> keyValuePair in (IEnumerable<KeyValuePair<string, FunctionSignatureWithReturnType>>) keyValuePairList2)
        {
          if (keyValuePair.Key.Equals(functionCallToken, StringComparison.Ordinal))
          {
            functionSignature = keyValuePair;
            break;
          }
        }
      }
      return functionSignature;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "One method to describe all rules around converts.")]
    internal static bool CanConvertTo(
      SingleValueNode sourceNodeOrNull,
      IEdmTypeReference sourceReference,
      IEdmTypeReference targetReference)
    {
      if (sourceReference.IsEquivalentTo(targetReference) || sourceReference.IsUntyped() || targetReference.IsUntyped())
        return true;
      if (targetReference.IsStructured())
        return (sourceReference.IsODataComplexTypeKind() || sourceReference.IsODataEntityTypeKind()) && EdmLibraryExtensions.IsAssignableFrom((IEdmStructuredType) targetReference.Definition, (IEdmStructuredType) sourceReference.Definition);
      if (sourceReference.IsEnum() && targetReference.IsEnum())
      {
        if (!sourceReference.Definition.IsEquivalentTo(targetReference.Definition))
          return false;
        return targetReference.IsNullable() || !sourceReference.IsNullable();
      }
      if (targetReference.IsEnum() && sourceReference.IsString())
        return true;
      IEdmPrimitiveTypeReference type1 = sourceReference.AsPrimitiveOrNull();
      IEdmPrimitiveTypeReference type2 = targetReference.AsPrimitiveOrNull();
      return type1 != null && type2 != null && MetadataUtilsCommon.CanConvertPrimitiveTypeTo(sourceNodeOrNull, type1.PrimitiveDefinition(), type2.PrimitiveDefinition());
    }

    private static IEnumerable<FunctionSignature> GetAdditionTermporalSignatures()
    {
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetDateTimeOffset(false),
        EdmCoreModel.Instance.GetDuration(false)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, p, false)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, false))
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetDateTimeOffset(true),
        EdmCoreModel.Instance.GetDuration(true)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, p, true)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, true))
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetDuration(false),
        EdmCoreModel.Instance.GetDateTimeOffset(false)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, false)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, p, false))
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetDuration(true),
        EdmCoreModel.Instance.GetDateTimeOffset(true)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, true)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, p, true))
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetDuration(false),
        EdmCoreModel.Instance.GetDuration(false)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, false)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, false))
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetDuration(true),
        EdmCoreModel.Instance.GetDuration(true)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, true)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, true))
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetDate(false),
        (IEdmPrimitiveTypeReference) EdmCoreModel.Instance.GetDuration(false)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        null,
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, false))
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetDate(true),
        (IEdmPrimitiveTypeReference) EdmCoreModel.Instance.GetDuration(true)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        null,
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, true))
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        (IEdmPrimitiveTypeReference) EdmCoreModel.Instance.GetDuration(false),
        EdmCoreModel.Instance.GetDate(false)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, false)),
        null
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        (IEdmPrimitiveTypeReference) EdmCoreModel.Instance.GetDuration(true),
        EdmCoreModel.Instance.GetDate(true)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, true)),
        null
      });
    }

    private static IEnumerable<FunctionSignature> GetSubtractionTermporalSignatures()
    {
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetDateTimeOffset(false),
        EdmCoreModel.Instance.GetDuration(false)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, p, false)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, false))
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetDateTimeOffset(true),
        EdmCoreModel.Instance.GetDuration(true)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, p, true)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, true))
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetDuration(false),
        EdmCoreModel.Instance.GetDuration(false)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, false)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, false))
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetDuration(true),
        EdmCoreModel.Instance.GetDuration(true)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, true)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, true))
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetDateTimeOffset(false),
        EdmCoreModel.Instance.GetDateTimeOffset(false)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, p, false)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, p, false))
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmTemporalTypeReference[2]
      {
        EdmCoreModel.Instance.GetDateTimeOffset(true),
        EdmCoreModel.Instance.GetDateTimeOffset(true)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, p, true)),
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.DateTimeOffset, p, true))
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetDate(false),
        (IEdmPrimitiveTypeReference) EdmCoreModel.Instance.GetDuration(false)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        null,
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, false))
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetDate(true),
        (IEdmPrimitiveTypeReference) EdmCoreModel.Instance.GetDuration(true)
      }, new FunctionSignature.CreateArgumentTypeWithFacets[2]
      {
        null,
        (FunctionSignature.CreateArgumentTypeWithFacets) ((p, s) => (IEdmTypeReference) EdmCoreModel.Instance.GetTemporal(EdmPrimitiveTypeKind.Duration, p, true))
      });
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetDate(false),
        EdmCoreModel.Instance.GetDate(false)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null);
      yield return new FunctionSignature((IEdmTypeReference[]) new IEdmPrimitiveTypeReference[2]
      {
        EdmCoreModel.Instance.GetDate(true),
        EdmCoreModel.Instance.GetDate(true)
      }, (FunctionSignature.CreateArgumentTypeWithFacets[]) null);
    }

    private static FunctionSignature[] GetFunctionSignatures(BinaryOperatorKind operatorKind)
    {
      switch (operatorKind)
      {
        case BinaryOperatorKind.Or:
        case BinaryOperatorKind.And:
          return TypePromotionUtils.logicalSignatures;
        case BinaryOperatorKind.Equal:
        case BinaryOperatorKind.NotEqual:
        case BinaryOperatorKind.GreaterThan:
        case BinaryOperatorKind.GreaterThanOrEqual:
        case BinaryOperatorKind.LessThan:
        case BinaryOperatorKind.LessThanOrEqual:
          return TypePromotionUtils.relationalSignatures;
        case BinaryOperatorKind.Add:
          return TypePromotionUtils.AdditionSignatures;
        case BinaryOperatorKind.Subtract:
          return TypePromotionUtils.SubtractionSignatures;
        case BinaryOperatorKind.Multiply:
        case BinaryOperatorKind.Divide:
        case BinaryOperatorKind.Modulo:
          return TypePromotionUtils.arithmeticSignatures;
        default:
          throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodes.TypePromotionUtils_GetFunctionSignatures_Binary_UnreachableCodepath));
      }
    }

    private static FunctionSignature[] GetFunctionSignatures(UnaryOperatorKind operatorKind)
    {
      if (operatorKind == UnaryOperatorKind.Negate)
        return TypePromotionUtils.negationSignatures;
      if (operatorKind == UnaryOperatorKind.Not)
        return TypePromotionUtils.notSignatures;
      throw new ODataException(Microsoft.OData.Strings.General_InternalError((object) InternalErrorCodes.TypePromotionUtils_GetFunctionSignatures_Unary_UnreachableCodepath));
    }

    private static int FindBestSignature(
      FunctionSignature[] signatures,
      SingleValueNode[] argumentNodes,
      IEdmTypeReference[] argumentTypes,
      out FunctionSignature bestMatch)
    {
      bestMatch = (FunctionSignature) null;
      List<FunctionSignature> signatures1 = ((IEnumerable<FunctionSignature>) signatures).Where<FunctionSignature>((Func<FunctionSignature, bool>) (signature => TypePromotionUtils.IsApplicable(signature, argumentNodes, argumentTypes))).ToList<FunctionSignature>();
      if (signatures1.Count > 1)
        signatures1 = TypePromotionUtils.FindBestApplicableSignatures(signatures1, argumentTypes);
      int count = signatures1.Count;
      switch (count)
      {
        case 1:
          bestMatch = signatures1[0];
          for (int index = 0; index < argumentTypes.Length; ++index)
            argumentTypes[index] = bestMatch.ArgumentTypes[index];
          return count;
        case 2:
          if (argumentTypes.Length == 2 && signatures1[0].ArgumentTypes[0].Definition.IsEquivalentTo(signatures1[1].ArgumentTypes[0].Definition) && signatures1[0].ArgumentTypes[1].Definition.IsEquivalentTo(signatures1[1].ArgumentTypes[1].Definition))
          {
            bestMatch = signatures1[0].ArgumentTypes[0].IsNullable ? signatures1[0] : signatures1[1];
            argumentTypes[0] = bestMatch.ArgumentTypes[0];
            argumentTypes[1] = bestMatch.ArgumentTypes[1];
            return 1;
          }
          break;
      }
      return count;
    }

    private static bool IsApplicable(
      FunctionSignature signature,
      SingleValueNode[] argumentNodes,
      IEdmTypeReference[] argumentTypes)
    {
      if (signature.ArgumentTypes.Length != argumentTypes.Length)
        return false;
      for (int index = 0; index < argumentTypes.Length; ++index)
      {
        if (!TypePromotionUtils.CanPromoteNodeTo(argumentNodes[index], argumentTypes[index], signature.ArgumentTypes[index]))
          return false;
      }
      return true;
    }

    private static bool CanPromoteNodeTo(
      SingleValueNode sourceNodeOrNull,
      IEdmTypeReference sourceType,
      IEdmTypeReference targetType)
    {
      if (sourceType == null)
        return targetType.IsNullable;
      if (sourceType.IsEquivalentTo(targetType) || TypePromotionUtils.CanConvertTo(sourceNodeOrNull, sourceType, targetType))
        return true;
      if (sourceType.IsNullable && targetType.IsODataValueType())
      {
        IEdmTypeReference typeReference = sourceType.Definition.ToTypeReference(false);
        if (TypePromotionUtils.CanConvertTo(sourceNodeOrNull, typeReference, targetType))
          return true;
      }
      return false;
    }

    private static List<FunctionSignature> FindBestApplicableSignatures(
      List<FunctionSignature> signatures,
      IEdmTypeReference[] argumentTypes)
    {
      List<FunctionSignature> applicableSignatures = new List<FunctionSignature>();
      foreach (FunctionSignature signature1 in signatures)
      {
        bool flag = true;
        foreach (FunctionSignature signature2 in signatures)
        {
          if (signature2 != signature1 && TypePromotionUtils.MatchesArgumentTypesBetterThan(argumentTypes, signature2.ArgumentTypes, signature1.ArgumentTypes))
          {
            flag = false;
            break;
          }
        }
        if (flag)
          applicableSignatures.Add(signature1);
      }
      return applicableSignatures;
    }

    private static bool MatchesArgumentTypesBetterThan(
      IEdmTypeReference[] argumentTypes,
      IEdmTypeReference[] firstCandidate,
      IEdmTypeReference[] secondCandidate)
    {
      bool flag = false;
      for (int index = 0; index < argumentTypes.Length; ++index)
      {
        if (argumentTypes[index] != null)
        {
          int num = TypePromotionUtils.CompareConversions(argumentTypes[index], firstCandidate[index], secondCandidate[index]);
          if (num < 0)
            return false;
          if (num > 0)
            flag = true;
        }
      }
      return flag;
    }

    private static int CompareConversions(
      IEdmTypeReference source,
      IEdmTypeReference targetA,
      IEdmTypeReference targetB)
    {
      if (targetA.IsEquivalentTo(targetB))
        return 0;
      if (source.IsEquivalentTo(targetA))
        return 1;
      if (source.IsEquivalentTo(targetB))
        return -1;
      bool flag1 = TypePromotionUtils.CanConvertTo((SingleValueNode) null, targetA, targetB);
      bool flag2 = TypePromotionUtils.CanConvertTo((SingleValueNode) null, targetB, targetA);
      if (flag1 && !flag2)
        return 1;
      if (flag2 && !flag1)
        return -1;
      bool isNullable1 = source.IsNullable;
      bool isNullable2 = targetA.IsNullable;
      bool isNullable3 = targetB.IsNullable;
      if (isNullable1 == isNullable2 && isNullable1 != isNullable3)
        return 1;
      if (isNullable1 != isNullable2 && isNullable1 == isNullable3)
        return -1;
      if (TypePromotionUtils.IsSignedIntegralType(targetA) && TypePromotionUtils.IsUnsignedIntegralType(targetB))
        return 1;
      if (TypePromotionUtils.IsSignedIntegralType(targetB) && TypePromotionUtils.IsUnsignedIntegralType(targetA) || TypePromotionUtils.IsDecimalType(targetA) && TypePromotionUtils.IsDoubleOrSingle(targetB))
        return -1;
      if (TypePromotionUtils.IsDecimalType(targetB) && TypePromotionUtils.IsDoubleOrSingle(targetA) || TypePromotionUtils.IsDateTimeOffset(targetA) && TypePromotionUtils.IsDate(targetB))
        return 1;
      return TypePromotionUtils.IsDateTimeOffset(targetB) && TypePromotionUtils.IsDate(targetA) ? -1 : 0;
    }

    private static bool TryHandleEqualityOperatorForEntityOrComplexTypes(
      ref IEdmTypeReference left,
      ref IEdmTypeReference right)
    {
      if (left != null && left.IsStructured())
      {
        if (right == null)
        {
          right = left;
          return true;
        }
        if (!right.IsStructured())
          return false;
        if (left.Definition.IsEquivalentTo(right.Definition))
        {
          if (left.IsNullable && !right.IsNullable)
            right = left;
          else
            left = right;
          return true;
        }
        if (TypePromotionUtils.CanConvertTo((SingleValueNode) null, left, right))
        {
          left = right;
          return true;
        }
        if (!TypePromotionUtils.CanConvertTo((SingleValueNode) null, right, left))
          return false;
        right = left;
        return true;
      }
      if (right == null || !right.IsStructured() || left != null)
        return false;
      left = right;
      return true;
    }

    private static bool IsSignedIntegralType(IEdmTypeReference typeReference) => TypePromotionUtils.GetNumericTypeKind(typeReference) == TypePromotionUtils.NumericTypeKind.SignedIntegral;

    private static bool IsUnsignedIntegralType(IEdmTypeReference typeReference) => TypePromotionUtils.GetNumericTypeKind(typeReference) == TypePromotionUtils.NumericTypeKind.UnsignedIntegral;

    private static bool IsDate(IEdmTypeReference typeReference)
    {
      IEdmPrimitiveTypeReference type = typeReference.AsPrimitiveOrNull();
      return type != null && ExtensionMethods.PrimitiveKind(type) == EdmPrimitiveTypeKind.Date;
    }

    private static bool IsDateTimeOffset(IEdmTypeReference typeReference)
    {
      IEdmPrimitiveTypeReference type = typeReference.AsPrimitiveOrNull();
      return type != null && ExtensionMethods.PrimitiveKind(type) == EdmPrimitiveTypeKind.DateTimeOffset;
    }

    private static bool IsDecimalType(IEdmTypeReference typeReference)
    {
      IEdmPrimitiveTypeReference type = typeReference.AsPrimitiveOrNull();
      return type != null && ExtensionMethods.PrimitiveKind(type) == EdmPrimitiveTypeKind.Decimal;
    }

    private static bool IsDoubleOrSingle(IEdmTypeReference typeReference)
    {
      IEdmPrimitiveTypeReference type = typeReference.AsPrimitiveOrNull();
      if (type == null)
        return false;
      EdmPrimitiveTypeKind primitiveTypeKind = ExtensionMethods.PrimitiveKind(type);
      return primitiveTypeKind == EdmPrimitiveTypeKind.Double || primitiveTypeKind == EdmPrimitiveTypeKind.Single;
    }

    private static TypePromotionUtils.NumericTypeKind GetNumericTypeKind(
      IEdmTypeReference typeReference)
    {
      IEdmPrimitiveTypeReference type = typeReference.AsPrimitiveOrNull();
      if (type == null)
        return TypePromotionUtils.NumericTypeKind.NotNumeric;
      switch (type.PrimitiveDefinition().PrimitiveKind)
      {
        case EdmPrimitiveTypeKind.Byte:
          return TypePromotionUtils.NumericTypeKind.UnsignedIntegral;
        case EdmPrimitiveTypeKind.Decimal:
        case EdmPrimitiveTypeKind.Double:
        case EdmPrimitiveTypeKind.Single:
          return TypePromotionUtils.NumericTypeKind.NotIntegral;
        case EdmPrimitiveTypeKind.Int16:
        case EdmPrimitiveTypeKind.Int32:
        case EdmPrimitiveTypeKind.Int64:
        case EdmPrimitiveTypeKind.SByte:
          return TypePromotionUtils.NumericTypeKind.SignedIntegral;
        default:
          return TypePromotionUtils.NumericTypeKind.NotNumeric;
      }
    }

    private enum NumericTypeKind
    {
      NotNumeric,
      NotIntegral,
      SignedIntegral,
      UnsignedIntegral,
    }
  }
}
