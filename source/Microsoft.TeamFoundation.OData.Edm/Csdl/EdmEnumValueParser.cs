// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.EdmEnumValueParser
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl
{
  internal static class EdmEnumValueParser
  {
    internal static bool TryParseEnumMember(
      string value,
      IEdmModel model,
      EdmLocation location,
      out IEnumerable<IEdmEnumMember> result)
    {
      result = (IEnumerable<IEdmEnumMember>) null;
      if (value == null || model == null)
        return false;
      bool flag = false;
      string[] source = value.Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
      if (!((IEnumerable<string>) source).Any<string>())
        return false;
      string qualifiedName = ((IEnumerable<string>) source[0].Split('/')).FirstOrDefault<string>();
      if (string.IsNullOrEmpty(qualifiedName))
        return false;
      if (!(model.FindType(qualifiedName) is IEdmEnumType edmEnumType))
      {
        edmEnumType = (IEdmEnumType) new UnresolvedEnumType(qualifiedName, location);
        flag = true;
      }
      else if (((IEnumerable<string>) source).Count<string>() > 1 && (!edmEnumType.IsFlags || !EdmEnumValueParser.IsEnumIntegerType(edmEnumType)))
        return false;
      List<IEdmEnumMember> edmEnumMemberList = new List<IEdmEnumMember>();
      foreach (string str in source)
      {
        string[] path = str.Split('/');
        if (((IEnumerable<string>) path).Count<string>() != 2 || path[0] != qualifiedName)
          return false;
        if (!flag)
        {
          IEdmEnumMember edmEnumMember = edmEnumType.Members.SingleOrDefault<IEdmEnumMember>((Func<IEdmEnumMember, bool>) (m => m.Name == path[1]));
          if (edmEnumMember == null)
            return false;
          edmEnumMemberList.Add(edmEnumMember);
        }
        else
          edmEnumMemberList.Add((IEdmEnumMember) new UnresolvedEnumMember(path[1], edmEnumType, location));
      }
      result = (IEnumerable<IEdmEnumMember>) edmEnumMemberList;
      return true;
    }

    internal static bool IsEnumIntegerType(IEdmEnumType enumType) => enumType.UnderlyingType.PrimitiveKind == EdmPrimitiveTypeKind.Byte || enumType.UnderlyingType.PrimitiveKind == EdmPrimitiveTypeKind.SByte || enumType.UnderlyingType.PrimitiveKind == EdmPrimitiveTypeKind.Int16 || enumType.UnderlyingType.PrimitiveKind == EdmPrimitiveTypeKind.Int32 || enumType.UnderlyingType.PrimitiveKind == EdmPrimitiveTypeKind.Int64;
  }
}
