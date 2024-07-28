// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiPageIdInfo
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using System;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  internal class WikiPageIdInfo
  {
    public int PageId { get; private set; }

    public Guid WikiId { get; private set; }

    public string WikiVersion { get; private set; }

    public string GitFriendlyPagePath { get; private set; }

    public int AssociatedPushId { get; private set; }

    public WikiPageIdInfo(
      int pageId,
      Guid wikiId,
      string wikiVersion,
      string gitFriendlyPagePath,
      int associatedPushId)
    {
      this.PageId = pageId;
      this.WikiId = wikiId;
      this.WikiVersion = wikiVersion;
      this.AssociatedPushId = associatedPushId;
      this.GitFriendlyPagePath = gitFriendlyPagePath;
    }
  }
}
