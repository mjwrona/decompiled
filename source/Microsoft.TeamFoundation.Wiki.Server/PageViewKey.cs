// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.PageViewKey
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using System;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class PageViewKey : IEquatable<PageViewKey>
  {
    public PageViewKey(Guid projectId, string wikiId, string version, string path)
    {
      this.ProjectId = projectId;
      this.WikiId = wikiId;
      this.Version = version;
      this.Path = path;
    }

    public Guid ProjectId { get; }

    public string WikiId { get; }

    public string Version { get; }

    public string Path { get; }

    public bool Equals(PageViewKey other) => other != null && this.ProjectId == other.ProjectId && PageViewKey.StringEquals(this.WikiId, other.WikiId) && PageViewKey.StringEquals(this.Version, other.Version) && PageViewKey.StringEquals(this.Path, other.Path);

    public override bool Equals(object other) => other is PageViewKey && this.Equals(other as PageViewKey);

    public override int GetHashCode() => (this.ProjectId.ToString().ToLowerInvariant() + ":" + this.WikiId.ToLowerInvariant() + ":" + this.Version.ToLowerInvariant() + ":" + this.Path.ToLowerInvariant()).GetHashCode();

    private static bool StringEquals(string a, string b) => string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
  }
}
