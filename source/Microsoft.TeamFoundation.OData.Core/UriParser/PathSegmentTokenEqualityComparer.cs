// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.PathSegmentTokenEqualityComparer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal sealed class PathSegmentTokenEqualityComparer : EqualityComparer<PathSegmentToken>
  {
    public override bool Equals(PathSegmentToken first, PathSegmentToken second)
    {
      if (first == null && second == null)
        return true;
      return first != null && second != null && this.ToHashableString(first) == this.ToHashableString(second);
    }

    public override int GetHashCode(PathSegmentToken path) => path == null ? 0 : this.ToHashableString(path).GetHashCode();

    private string ToHashableString(PathSegmentToken token) => token.NextToken == null ? token.Identifier : token.Identifier + "/" + this.ToHashableString(token.NextToken);
  }
}
