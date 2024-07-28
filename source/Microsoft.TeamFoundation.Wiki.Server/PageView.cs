// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.PageView
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using System;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class PageView
  {
    public PageView(int dataspaceId, string wikiId, string version, string pagePath)
      : this(dataspaceId, wikiId, version, pagePath, 0, DateTime.UtcNow)
    {
    }

    public PageView(
      int dataspaceId,
      string wikiId,
      string version,
      string pagePath,
      int viewCount,
      DateTime lastViewed)
    {
      this.DataspaceId = dataspaceId;
      this.WikiId = wikiId;
      this.Version = version;
      this.PagePath = pagePath;
      this.ViewCount = viewCount;
      this.LastViewed = lastViewed;
    }

    public int DataspaceId { get; }

    public string WikiId { get; }

    public string Version { get; }

    public string PagePath { get; }

    public int ViewCount { get; }

    public DateTime LastViewed { get; }
  }
}
