// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.PathToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  public abstract class PathToken : QueryToken
  {
    public abstract QueryToken NextToken { get; set; }

    public abstract string Identifier { get; }

    public override bool Equals(object obj)
    {
      if (!(obj is PathToken pathToken) || !this.Identifier.Equals(pathToken.Identifier))
        return false;
      return this.NextToken == null && pathToken.NextToken == null || this.NextToken.Equals((object) pathToken.NextToken);
    }

    public override int GetHashCode()
    {
      int h1 = this.Identifier.GetHashCode();
      if (this.NextToken != null)
        h1 = PathToken.Combine(h1, this.NextToken.GetHashCode());
      return h1;
    }

    private static int Combine(int h1, int h2) => (h1 << 5 | h1 >>> 27) + h1 ^ h2;
  }
}
