// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.PathReverser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  internal static class PathReverser
  {
    public static PathSegmentToken Reverse(this PathSegmentToken head)
    {
      PathSegmentToken pathSegmentToken1 = (PathSegmentToken) null;
      PathSegmentToken nextToken;
      for (PathSegmentToken pathSegmentToken2 = head; pathSegmentToken2 != null; pathSegmentToken2 = nextToken)
      {
        nextToken = pathSegmentToken2.NextToken;
        pathSegmentToken2.NextToken = pathSegmentToken1;
        pathSegmentToken1 = pathSegmentToken2;
      }
      return pathSegmentToken1;
    }
  }
}
