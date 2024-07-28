// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.UriFunctionsHelper
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.UriParser
{
  internal static class UriFunctionsHelper
  {
    public static string BuildFunctionSignatureListDescription(
      string name,
      IEnumerable<FunctionSignatureWithReturnType> signatures)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string str1 = "";
      foreach (FunctionSignatureWithReturnType signature in signatures)
      {
        stringBuilder.Append(str1);
        str1 = "; ";
        string str2 = "";
        stringBuilder.Append(name);
        stringBuilder.Append('(');
        foreach (IEdmTypeReference argumentType in signature.ArgumentTypes)
        {
          stringBuilder.Append(str2);
          str2 = ", ";
          if (argumentType.IsODataPrimitiveTypeKind() && argumentType.IsNullable)
          {
            stringBuilder.Append(argumentType.FullName());
            stringBuilder.Append(" Nullable=true");
          }
          else
            stringBuilder.Append(argumentType.FullName());
        }
        stringBuilder.Append(')');
      }
      return stringBuilder.ToString();
    }
  }
}
