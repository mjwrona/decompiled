// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ODataUriFunctions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Query.Expressions;
using Microsoft.OData.UriParser;
using System.Reflection;

namespace Microsoft.AspNet.OData
{
  public static class ODataUriFunctions
  {
    public static void AddCustomUriFunction(
      string functionName,
      FunctionSignatureWithReturnType functionSignature,
      MethodInfo methodInfo)
    {
      try
      {
        CustomUriFunctions.AddCustomUriFunction(functionName, functionSignature);
        UriFunctionsBinder.BindUriFunctionName(functionName, methodInfo);
      }
      catch
      {
        ODataUriFunctions.RemoveCustomUriFunction(functionName, functionSignature, methodInfo);
        throw;
      }
    }

    public static bool RemoveCustomUriFunction(
      string functionName,
      FunctionSignatureWithReturnType functionSignature,
      MethodInfo methodInfo)
    {
      return CustomUriFunctions.RemoveCustomUriFunction(functionName, functionSignature) && UriFunctionsBinder.UnbindUriFunctionName(functionName, methodInfo);
    }
  }
}
