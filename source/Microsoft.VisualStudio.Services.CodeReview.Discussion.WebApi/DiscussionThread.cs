// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.DiscussionThread
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 443E2621-CB19-4319-96B1-AE621A0F5B5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi
{
  [DataContract]
  [JsonConverter(typeof (DiscussionThreadConverter))]
  public abstract class DiscussionThread
  {
    [DataMember(Name = "Id")]
    public int DiscussionId { get; set; }

    [DataMember]
    public string ArtifactUri { get; set; }

    [DataMember]
    public DateTime PublishedDate { get; set; }

    [DataMember]
    public DateTime LastUpdatedDate { get; set; }

    [DataMember]
    public DiscussionComment[] Comments { get; set; }

    [XmlIgnore]
    [DataMember]
    public PropertiesCollection Properties { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int CommentsCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int WorkItemId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DiscussionStatus Status { get; set; }

    [XmlIgnore]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public DiscussionSeverity Severity { get; set; }

    [XmlIgnore]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int Revision { get; set; }

    [XmlIgnore]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsDirty { get; set; }

    [XmlIgnore]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public string VersionId { get; set; }

    [XmlIgnore]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int PropertyId { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public string ItemPath
    {
      get
      {
        if (this.Properties != null)
        {
          string itemPath = (string) null;
          if (this.Properties.TryGetValue<string>("Microsoft.TeamFoundation.Discussion.ItemPath", out itemPath) || this.Properties.TryGetValue<string>("Microsoft.VisualStudio.Services.CodeReview.ItemPath", out itemPath))
            return itemPath;
        }
        return (string) null;
      }
    }

    [DataMember]
    public bool IsDeleted => this.Comments != null && this.Comments.Length != 0 && ((IEnumerable<DiscussionComment>) this.Comments).All<DiscussionComment>((Func<DiscussionComment, bool>) (comment => comment.IsDeleted));

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    internal DateTime PriorLastUpdatedDate { get; set; }

    public T ToSubclass<T>() where T : DiscussionThread, new()
    {
      T subclass = new T();
      subclass.Comments = this.Comments;
      subclass.CommentsCount = DiscussionThread.GetCommentCount(this);
      subclass.DiscussionId = this.DiscussionId;
      subclass.Status = this.Status;
      subclass.Severity = this.Severity;
      subclass.WorkItemId = this.WorkItemId;
      subclass.ArtifactUri = this.ArtifactUri;
      subclass.PublishedDate = this.PublishedDate;
      subclass.LastUpdatedDate = this.LastUpdatedDate;
      subclass.Revision = this.Revision;
      subclass.IsDirty = this.IsDirty;
      subclass.PropertyId = this.PropertyId;
      subclass.Properties = this.Properties;
      subclass.VersionId = this.VersionId;
      return subclass;
    }

    public static int GetCommentCount(DiscussionThread discussionThread)
    {
      int commentCount = discussionThread.CommentsCount;
      if (commentCount == 0 && discussionThread.Comments != null)
        commentCount = ((IEnumerable<DiscussionComment>) discussionThread.Comments).Count<DiscussionComment>((Func<DiscussionComment, bool>) (c => !c.IsDeleted));
      return commentCount;
    }
  }
}
