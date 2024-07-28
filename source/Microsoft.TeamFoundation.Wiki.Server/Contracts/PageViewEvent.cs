// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.Contracts.PageViewEvent
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Wiki.Server.Contracts
{
  public class PageViewEvent
  {
    [DataMember]
    public Guid WikiId { get; set; }

    [DataMember]
    public int PageId { get; set; }

    public PageViewEvent(Guid wikiId, int pageId)
    {
      this.WikiId = wikiId;
      this.PageId = pageId;
    }
  }
}
