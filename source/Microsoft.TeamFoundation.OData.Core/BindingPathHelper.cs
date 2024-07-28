// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.BindingPathHelper
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData
{
  internal static class BindingPathHelper
  {
    public static bool MatchBindingPath(
      IEdmPathExpression bindingPath,
      List<ODataPathSegment> parsedSegments)
    {
      List<string> list = bindingPath.PathSegments.ToList<string>();
      if (list.Count == 1)
        return true;
      int index1 = list.Count - 2;
      for (int index2 = parsedSegments.Count - 1; index2 >= 0; --index2)
      {
        ODataPathSegment parsedSegment = parsedSegments[index2];
        bool flag = parsedSegment is NavigationPropertySegment;
        if (parsedSegment is PropertySegment || flag && parsedSegment.TargetEdmNavigationSource is IEdmContainedEntitySet)
        {
          if (index1 < 0 || string.CompareOrdinal(list[index1], parsedSegment.Identifier) != 0)
            return false;
          --index1;
        }
        else
        {
          int num1;
          switch (parsedSegment)
          {
            case TypeSegment _:
              if (index1 >= 0 && list[index1].Contains("."))
              {
                if (string.CompareOrdinal(list[index1], parsedSegment.EdmType.AsElementType().FullTypeName()) != 0)
                  return false;
                --index1;
                continue;
              }
              continue;
            case EntitySetSegment _:
              num1 = 1;
              break;
            default:
              num1 = parsedSegment is SingletonSegment ? 1 : 0;
              break;
          }
          int num2 = flag ? 1 : 0;
          if ((num1 | num2) != 0)
            break;
        }
      }
      return index1 == -1;
    }
  }
}
