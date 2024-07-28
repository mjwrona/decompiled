// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SelectExpandPathBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal static class SelectExpandPathBinder
  {
    public static IEnumerable<ODataPathSegment> FollowTypeSegments(
      PathSegmentToken firstTypeToken,
      IEdmModel model,
      int maxDepth,
      ODataUriResolver resolver,
      ref IEdmStructuredType currentLevelType,
      out PathSegmentToken firstNonTypeToken)
    {
      ExceptionUtils.CheckArgumentNotNull<PathSegmentToken>(firstTypeToken, nameof (firstTypeToken));
      ExceptionUtils.CheckArgumentNotNull<IEdmModel>(model, nameof (model));
      if (!firstTypeToken.IsNamespaceOrContainerQualified())
        throw new ODataException(Microsoft.OData.Strings.SelectExpandPathBinder_FollowNonTypeSegment((object) firstTypeToken.Identifier));
      int num = 0;
      List<ODataPathSegment> odataPathSegmentList = new List<ODataPathSegment>();
      PathSegmentToken pathSegmentToken = firstTypeToken;
      while (pathSegmentToken.IsNamespaceOrContainerQualified() && pathSegmentToken.NextToken != null)
      {
        IEdmType parentType = (IEdmType) currentLevelType;
        currentLevelType = UriEdmHelpers.FindTypeFromModel(model, pathSegmentToken.Identifier, resolver) as IEdmStructuredType;
        if (currentLevelType == null)
          throw new ODataException(Microsoft.OData.Strings.ExpandItemBinder_CannotFindType((object) pathSegmentToken.Identifier));
        UriEdmHelpers.CheckRelatedTo(parentType, (IEdmType) currentLevelType);
        odataPathSegmentList.Add((ODataPathSegment) new TypeSegment((IEdmType) currentLevelType, (IEdmNavigationSource) null));
        ++num;
        pathSegmentToken = pathSegmentToken.NextToken;
        if (num >= maxDepth)
          throw new ODataException(Microsoft.OData.Strings.ExpandItemBinder_PathTooDeep);
      }
      firstNonTypeToken = pathSegmentToken;
      return (IEnumerable<ODataPathSegment>) odataPathSegmentList;
    }
  }
}
