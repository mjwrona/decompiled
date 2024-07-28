// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.CheckinEvent
// Assembly: Microsoft.TeamFoundation.VersionControl.Common.Integration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2A9D748-4CEE-4498-9785-584B91A44F85
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.Integration.dll

using Microsoft.VisualStudio.Services.Notifications;
using System;
using System.Collections;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [XmlInclude(typeof (CheckinWorkItemInfo))]
  [SoapInclude(typeof (CheckinWorkItemInfo))]
  [XmlInclude(typeof (NameValuePair))]
  [SoapInclude(typeof (NameValuePair))]
  [XmlInclude(typeof (ClientArtifact))]
  [SoapInclude(typeof (ClientArtifact))]
  [NotificationEventBindings(EventSerializerType.Xml, "ms.vss-code.checkin-event")]
  public class CheckinEvent : NotificationEvent, ICloneable
  {
    [XmlArray("PolicyFailures")]
    [XmlArrayItem("PolicyFailure")]
    public ArrayList PolicyFailures;
    private const int c_maxTitleCharacters = 80;
    private const int c_minCommentCharacters = 20;
    private int m_changeset;
    private string m_committer;
    private string m_committerDisplay;
    private string m_title;
    private string m_contentTitle;

    public CheckinEvent(
      int changeset,
      DateTime creationDate,
      string owner,
      string committer,
      string comment)
      : this(changeset, creationDate, owner, owner, committer, committer, comment)
    {
    }

    public CheckinEvent(
      int changeset,
      DateTime creationDate,
      string owner,
      string ownerDisplay,
      string committer,
      string committerDisplay,
      string comment)
      : base(creationDate, owner, ownerDisplay, comment)
    {
      this.m_changeset = changeset;
      this.m_committer = committer;
      this.m_committerDisplay = committerDisplay;
    }

    public CheckinEvent(int changeset, DateTime creationDate)
      : base(creationDate, (string) null, (string) null)
    {
      this.m_changeset = changeset;
    }

    public CheckinEvent()
    {
    }

    public object Clone()
    {
      CheckinEvent checkinEvent = (CheckinEvent) this.MemberwiseClone();
      checkinEvent.Artifacts = (ArrayList) this.Artifacts.Clone();
      checkinEvent.CheckinNotes = (ArrayList) this.CheckinNotes.Clone();
      checkinEvent.PolicyFailures = (ArrayList) this.PolicyFailures.Clone();
      checkinEvent.CheckinInformation = (ArrayList) this.CheckinInformation.Clone();
      checkinEvent.m_title = (string) null;
      checkinEvent.m_contentTitle = (string) null;
      checkinEvent.TeamProject = (string) null;
      return (object) checkinEvent;
    }

    public string Title
    {
      get
      {
        if (this.m_title == null)
        {
          string str = VersionControlIntegrationResources.Format("CheckinEmailTitleTemplate", (object) this.TeamProject, (object) this.m_changeset.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          int maxCommentChars = Math.Min(80, Math.Max(80 - str.Length, 20));
          if (maxCommentChars > 0 && !string.IsNullOrEmpty(this.Comment))
            this.m_title = VersionControlIntegrationResources.Format("CheckinEmailTitle", (object) str, (object) this.CommentText(this.Comment, maxCommentChars));
          else
            this.m_title = str;
        }
        return this.m_title;
      }
      set => this.m_title = value;
    }

    public string ContentTitle
    {
      get
      {
        if (this.m_contentTitle == null)
        {
          if (!string.IsNullOrEmpty(this.Comment))
            this.m_contentTitle = VersionControlIntegrationResources.Format("CheckinContentTitle", (object) this.m_changeset.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) this.CommentText(this.Comment, 80));
          else
            this.m_contentTitle = VersionControlIntegrationResources.Format("CheckinContentTitleNoComment", (object) this.m_changeset.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        }
        return this.m_contentTitle;
      }
      set => this.m_contentTitle = value;
    }

    public string Committer
    {
      get => this.m_committer;
      set => this.m_committer = value;
    }

    public string CommitterDisplay
    {
      get => this.m_committerDisplay;
      set => this.m_committerDisplay = value;
    }

    public int Number
    {
      get => this.m_changeset;
      set => this.m_changeset = value;
    }

    [XmlIgnore]
    public DateTime ChangesetCreationDate
    {
      get => this.CreationDateInternal;
      set => this.CreationDateInternal = value;
    }

    public static class Roles
    {
      public static string Committer = "committer";
      public static string Owner = "owner";
    }
  }
}
