// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.TypeExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class TypeExtensions
  {
    public static void AppendPrettyTypeName(this StringBuilder sb, Type type)
    {
      if (type.IsArray)
      {
        sb.AppendPrettyTypeName(type.GetElementType());
        sb.Append("[" + new string(',', type.GetArrayRank() - 1) + "]");
      }
      else if (type.IsPointer)
      {
        sb.AppendPrettyTypeName(type.GetElementType());
        sb.Append("*");
      }
      else if (type.IsGenericType)
      {
        if ((object) type.DeclaringType != null)
        {
          sb.AppendPrettyTypeName(type.DeclaringType);
          sb.Append("+");
        }
        sb.Append(type.Name);
        sb.Append('<');
        bool flag = true;
        foreach (Type genericArgument in type.GetGenericArguments())
        {
          if (!flag)
            sb.Append(',');
          sb.AppendPrettyTypeName(genericArgument);
          flag = false;
        }
        sb.Append(">");
      }
      else
      {
        if ((object) type.DeclaringType != null && !type.IsGenericParameter)
        {
          sb.AppendPrettyTypeName(type.DeclaringType);
          sb.Append("+");
        }
        sb.Append(type.Name);
      }
    }

    public static string GetPrettyName(this Type type)
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendPrettyTypeName(type);
      return sb.ToString();
    }
  }
}
