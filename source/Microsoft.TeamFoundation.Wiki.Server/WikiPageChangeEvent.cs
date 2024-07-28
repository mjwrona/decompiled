// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiPageChangeEvent
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Git.Server;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  [DataContract]
  [ServiceEventObject]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-wiki-web.wiki-page-changed-event")]
  public class WikiPageChangeEvent
  {
    [DataMember]
    public Guid ProjectId { get; set; }

    [DataMember]
    public Guid WikiId { get; set; }

    [DataMember]
    public string WikiName { get; set; }

    [DataMember]
    public int PageId { get; set; }

    [DataMember]
    public string RemoteUrl { get; set; }

    [DataMember]
    public string PagePath { get; set; }

    [DataMember]
    public string PageTitle { get; set; }

    [DataMember]
    public TfsGitChangeType ChangeType { get; set; }

    [DataMember]
    public Guid PusherId { get; set; }

    [DataMember]
    public string UnFollowUrl { get; set; }

    [DataMember]
    public List<Microsoft.TeamFoundation.Wiki.Server.CommitDetails> CommitDetails { get; set; }

    public WikiPageChangeEvent()
    {
    }

    public WikiPageChangeEvent(WikiPageChangeEvent changeEvent, Microsoft.TeamFoundation.Wiki.Server.CommitDetails commitDetails)
    {
      this.ProjectId = changeEvent.ProjectId;
      this.WikiId = changeEvent.WikiId;
      this.WikiName = changeEvent.WikiName;
      this.PageId = changeEvent.PageId;
      this.RemoteUrl = changeEvent.RemoteUrl;
      this.PagePath = changeEvent.PagePath;
      this.PageTitle = changeEvent.PageTitle;
      this.ChangeType = changeEvent.ChangeType;
      this.PusherId = changeEvent.PusherId;
      this.UnFollowUrl = changeEvent.UnFollowUrl;
      this.CommitDetails = new List<Microsoft.TeamFoundation.Wiki.Server.CommitDetails>();
      this.CommitDetails.Add(commitDetails);
    }
  }
}
