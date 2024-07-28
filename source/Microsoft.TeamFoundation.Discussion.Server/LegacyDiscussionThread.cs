// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Discussion.Server.LegacyDiscussionThread
// Assembly: Microsoft.TeamFoundation.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4DCA91C2-88ED-4792-BE4A-3104961AE8D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Discussion.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  [ClientType("DiscussionThread")]
  [XmlType("Discussion")]
  public class LegacyDiscussionThread
  {
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, PropertyName = "Id")]
    public int DiscussionId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public byte Status { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public byte Severity { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int WorkItemId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    [ClientType(typeof (Uri))]
    public string VersionUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string ItemPath { get; set; }

    [XmlElement]
    [ClientProperty(ClientVisibility.Private)]
    public LegacyDiscussionPosition Position { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public DateTime PublishedDate { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public DateTime LastUpdatedDate { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int Revision { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public bool IsDirty { get; set; }

    [XmlIgnore]
    internal string VersionId { get; set; }

    [XmlIgnore]
    internal int ChangesetId { get; set; }

    [XmlIgnore]
    internal string ShelvesetName { get; set; }

    [XmlIgnore]
    internal string ShelvesetOwner { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[Discussion Id={0} WorkItemId={1} VersionUri={2} ItemPath={3} Position={4}]", (object) this.DiscussionId, (object) this.WorkItemId, !string.IsNullOrEmpty(this.VersionUri) ? (object) this.VersionUri : (object) this.VersionId, (object) this.ItemPath, (object) this.Position);

    internal DiscussionThread ToDiscussionThread()
    {
      PropertiesCollection propertiesCollection = new PropertiesCollection();
      if (!string.IsNullOrEmpty(this.ItemPath))
        propertiesCollection["Microsoft.TeamFoundation.Discussion.ItemPath"] = (object) this.ItemPath;
      if (this.Position != null)
      {
        propertiesCollection["Microsoft.TeamFoundation.Discussion.Position.StartLine"] = (object) this.Position.StartLine;
        propertiesCollection["Microsoft.TeamFoundation.Discussion.Position.EndLine"] = (object) this.Position.EndLine;
        propertiesCollection["Microsoft.TeamFoundation.Discussion.Position.StartColumn"] = (object) this.Position.StartColumn;
        propertiesCollection["Microsoft.TeamFoundation.Discussion.Position.EndColumn"] = (object) this.Position.EndColumn;
        propertiesCollection["Microsoft.TeamFoundation.Discussion.Position.StartCharPosition"] = (object) this.Position.StartCharPosition;
        propertiesCollection["Microsoft.TeamFoundation.Discussion.Position.EndCharPosition"] = (object) this.Position.EndCharPosition;
        if (!string.IsNullOrEmpty(this.Position.PositionContext))
          propertiesCollection["Microsoft.TeamFoundation.Discussion.Position.PositionContext"] = (object) this.Position.PositionContext;
      }
      ArtifactDiscussionThread discussionThread = new ArtifactDiscussionThread();
      discussionThread.DiscussionId = this.DiscussionId;
      discussionThread.Status = (DiscussionStatus) this.Status;
      discussionThread.Severity = (DiscussionSeverity) this.Severity;
      discussionThread.ArtifactUri = this.VersionUri;
      discussionThread.PublishedDate = this.PublishedDate;
      discussionThread.LastUpdatedDate = this.LastUpdatedDate;
      discussionThread.Revision = this.Revision;
      discussionThread.IsDirty = this.IsDirty;
      discussionThread.Properties = propertiesCollection;
      discussionThread.WorkItemId = this.WorkItemId;
      return (DiscussionThread) discussionThread;
    }
  }
}
