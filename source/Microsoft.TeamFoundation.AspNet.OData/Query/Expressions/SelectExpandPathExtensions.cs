// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.SelectExpandPathExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  internal static class SelectExpandPathExtensions
  {
    public static ODataPathSegment GetFirstNonTypeCastSegment(
      this ODataSelectPath selectPath,
      out IList<ODataPathSegment> remainingSegments)
    {
      if (selectPath == null)
        throw new ArgumentNullException(nameof (selectPath));
      return SelectExpandPathExtensions.GetFirstNonTypeCastSegment((ODataPath) selectPath, (Func<ODataPathSegment, bool>) (m => m is PropertySegment || m is TypeSegment), (Func<ODataPathSegment, bool>) (s =>
      {
        switch (s)
        {
          case NavigationPropertySegment _:
          case PropertySegment _:
          case OperationSegment _:
            return true;
          default:
            return s is DynamicPathSegment;
        }
      }), out remainingSegments);
    }

    public static ODataPathSegment GetFirstNonTypeCastSegment(
      this ODataExpandPath expandPath,
      out IList<ODataPathSegment> remainingSegments)
    {
      if (expandPath == null)
        throw new ArgumentNullException(nameof (expandPath));
      return SelectExpandPathExtensions.GetFirstNonTypeCastSegment((ODataPath) expandPath, (Func<ODataPathSegment, bool>) (m => m is PropertySegment || m is TypeSegment), (Func<ODataPathSegment, bool>) (s => s is NavigationPropertySegment), out remainingSegments);
    }

    private static ODataPathSegment GetFirstNonTypeCastSegment(
      ODataPath path,
      Func<ODataPathSegment, bool> middleSegmentPredicte,
      Func<ODataPathSegment, bool> lastSegmentPredicte,
      out IList<ODataPathSegment> remainingSegments)
    {
      remainingSegments = (IList<ODataPathSegment>) null;
      ODataPathSegment nonTypeCastSegment = (ODataPathSegment) null;
      int num1 = path.Count - 1;
      int num2 = 0;
      foreach (ODataPathSegment odataPathSegment in path)
      {
        if (num2 == num1)
        {
          if (!lastSegmentPredicte(odataPathSegment))
            throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.InvalidLastSegmentInSelectExpandPath, (object) odataPathSegment.GetType().Name));
        }
        else if (!middleSegmentPredicte(odataPathSegment))
          throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.InvalidSegmentInSelectExpandPath, (object) odataPathSegment.GetType().Name));
        ++num2;
        if (nonTypeCastSegment != null)
        {
          if (remainingSegments == null)
            remainingSegments = (IList<ODataPathSegment>) new List<ODataPathSegment>();
          remainingSegments.Add(odataPathSegment);
        }
        else if (!(odataPathSegment is TypeSegment))
          nonTypeCastSegment = odataPathSegment;
      }
      return nonTypeCastSegment;
    }
  }
}
