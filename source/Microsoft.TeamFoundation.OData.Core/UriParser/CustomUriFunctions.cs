// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.CustomUriFunctions
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  public static class CustomUriFunctions
  {
    private static readonly Dictionary<string, FunctionSignatureWithReturnType[]> CustomFunctions = new Dictionary<string, FunctionSignatureWithReturnType[]>((IEqualityComparer<string>) StringComparer.Ordinal);
    private static readonly object Locker = new object();

    public static void AddCustomUriFunction(
      string functionName,
      FunctionSignatureWithReturnType functionSignature)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(functionName, nameof (functionName));
      ExceptionUtils.CheckArgumentNotNull<FunctionSignatureWithReturnType>(functionSignature, nameof (functionSignature));
      CustomUriFunctions.ValidateFunctionWithReturnType(functionSignature);
      lock (CustomUriFunctions.Locker)
      {
        FunctionSignatureWithReturnType[] signatures;
        if (BuiltInUriFunctions.TryGetBuiltInFunction(functionName, out signatures) && ((IEnumerable<FunctionSignatureWithReturnType>) signatures).Any<FunctionSignatureWithReturnType>((Func<FunctionSignatureWithReturnType, bool>) (builtInFunction => CustomUriFunctions.AreFunctionsSignatureEqual(functionSignature, builtInFunction))))
          throw new ODataException(Microsoft.OData.Strings.CustomUriFunctions_AddCustomUriFunction_BuiltInExistsFullSignature((object) functionName));
        CustomUriFunctions.AddCustomFunction(functionName, functionSignature);
      }
    }

    public static bool RemoveCustomUriFunction(
      string functionName,
      FunctionSignatureWithReturnType functionSignature)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(functionName, nameof (functionName));
      ExceptionUtils.CheckArgumentNotNull<FunctionSignatureWithReturnType>(functionSignature, nameof (functionSignature));
      CustomUriFunctions.ValidateFunctionWithReturnType(functionSignature);
      lock (CustomUriFunctions.Locker)
      {
        FunctionSignatureWithReturnType[] source;
        if (!CustomUriFunctions.CustomFunctions.TryGetValue(functionName, out source))
          return false;
        FunctionSignatureWithReturnType[] array = ((IEnumerable<FunctionSignatureWithReturnType>) source).SkipWhile<FunctionSignatureWithReturnType>((Func<FunctionSignatureWithReturnType, bool>) (funcOverload => CustomUriFunctions.AreFunctionsSignatureEqual(funcOverload, functionSignature))).ToArray<FunctionSignatureWithReturnType>();
        if (array.Length == source.Length)
          return false;
        if (array.Length == 0)
          return CustomUriFunctions.CustomFunctions.Remove(functionName);
        CustomUriFunctions.CustomFunctions[functionName] = array;
        return true;
      }
    }

    public static bool RemoveCustomUriFunction(string functionName)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(functionName, nameof (functionName));
      lock (CustomUriFunctions.Locker)
        return CustomUriFunctions.CustomFunctions.Remove(functionName);
    }

    internal static bool TryGetCustomFunction(
      string functionCallToken,
      out IList<KeyValuePair<string, FunctionSignatureWithReturnType>> nameSignatures,
      bool enableCaseInsensitive = false)
    {
      lock (CustomUriFunctions.Locker)
      {
        IList<KeyValuePair<string, FunctionSignatureWithReturnType>> keyValuePairList = (IList<KeyValuePair<string, FunctionSignatureWithReturnType>>) new List<KeyValuePair<string, FunctionSignatureWithReturnType>>();
        foreach (KeyValuePair<string, FunctionSignatureWithReturnType[]> customFunction in CustomUriFunctions.CustomFunctions)
        {
          if (customFunction.Key.Equals(functionCallToken, enableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
          {
            foreach (FunctionSignatureWithReturnType signatureWithReturnType in customFunction.Value)
              keyValuePairList.Add(new KeyValuePair<string, FunctionSignatureWithReturnType>(customFunction.Key, signatureWithReturnType));
          }
        }
        nameSignatures = keyValuePairList.Count != 0 ? keyValuePairList : (IList<KeyValuePair<string, FunctionSignatureWithReturnType>>) null;
        return nameSignatures != null;
      }
    }

    private static void AddCustomFunction(
      string customFunctionName,
      FunctionSignatureWithReturnType newCustomFunctionSignature)
    {
      FunctionSignatureWithReturnType[] signatureWithReturnTypeArray;
      if (!CustomUriFunctions.CustomFunctions.TryGetValue(customFunctionName, out signatureWithReturnTypeArray))
      {
        CustomUriFunctions.CustomFunctions.Add(customFunctionName, new FunctionSignatureWithReturnType[1]
        {
          newCustomFunctionSignature
        });
      }
      else
      {
        if (((IEnumerable<FunctionSignatureWithReturnType>) signatureWithReturnTypeArray).Any<FunctionSignatureWithReturnType>((Func<FunctionSignatureWithReturnType, bool>) (existingFunction => CustomUriFunctions.AreFunctionsSignatureEqual(existingFunction, newCustomFunctionSignature))))
          throw new ODataException(Microsoft.OData.Strings.CustomUriFunctions_AddCustomUriFunction_CustomFunctionOverloadExists((object) customFunctionName));
        CustomUriFunctions.CustomFunctions[customFunctionName] = ((IEnumerable<FunctionSignatureWithReturnType>) signatureWithReturnTypeArray).Concat<FunctionSignatureWithReturnType>((IEnumerable<FunctionSignatureWithReturnType>) new FunctionSignatureWithReturnType[1]
        {
          newCustomFunctionSignature
        }).ToArray<FunctionSignatureWithReturnType>();
      }
    }

    private static bool AreFunctionsSignatureEqual(
      FunctionSignatureWithReturnType functionOne,
      FunctionSignatureWithReturnType functionTwo)
    {
      if (!functionOne.ReturnType.IsEquivalentTo(functionTwo.ReturnType) || functionOne.ArgumentTypes.Length != functionTwo.ArgumentTypes.Length)
        return false;
      for (int index = 0; index < functionOne.ArgumentTypes.Length; ++index)
      {
        if (!functionOne.ArgumentTypes[index].IsEquivalentTo(functionTwo.ArgumentTypes[index]))
          return false;
      }
      return true;
    }

    private static void ValidateFunctionWithReturnType(
      FunctionSignatureWithReturnType functionSignature)
    {
      if (functionSignature == null)
        return;
      ExceptionUtils.CheckArgumentNotNull<IEdmTypeReference>(functionSignature.ReturnType, "functionSignatureWithReturnType must contain a return type");
    }
  }
}
