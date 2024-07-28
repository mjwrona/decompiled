// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.ShelvesetEvent
// Assembly: Microsoft.TeamFoundation.VersionControl.Common.Integration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2A9D748-4CEE-4498-9785-584B91A44F85
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.Integration.dll

using Microsoft.VisualStudio.Services.Notifications;
using System;
using System.Collections;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [XmlInclude(typeof (CheckinWorkItemInfo))]
  [SoapInclude(typeof (CheckinWorkItemInfo))]
  [XmlInclude(typeof (NameValuePair))]
  [SoapInclude(typeof (NameValuePair))]
  [XmlInclude(typeof (ClientArtifact))]
  [SoapInclude(typeof (ClientArtifact))]
  [NotificationEventBindings(EventSerializerType.Xml, "ms.vss-code.shelveset-event")]
  public class ShelvesetEvent : NotificationEvent, ICloneable
  {
    private const int c_maxTitleCharacters = 80;
    private const int c_minCommentCharacters = 20;
    private string m_shelvesetName;
    private ShelvesetEventType m_eventType;
    private string m_title;
    private string m_contentTitle;

    public ShelvesetEvent(
      string shelvesetName,
      DateTime creationDate,
      string owner,
      string comment,
      ShelvesetEventType eventType)
      : this(shelvesetName, creationDate, owner, owner, comment, eventType)
    {
    }

    public ShelvesetEvent(
      string shelvesetName,
      DateTime creationDate,
      string owner,
      string ownerDisplay,
      string comment,
      ShelvesetEventType eventType)
      : base(creationDate, owner, ownerDisplay, comment)
    {
      this.m_shelvesetName = shelvesetName;
      this.m_eventType = eventType;
    }

    public ShelvesetEvent()
    {
    }

    public object Clone()
    {
      ShelvesetEvent shelvesetEvent = (ShelvesetEvent) this.MemberwiseClone();
      shelvesetEvent.Artifacts = (ArrayList) this.Artifacts.Clone();
      shelvesetEvent.CheckinNotes = (ArrayList) this.CheckinNotes.Clone();
      shelvesetEvent.CheckinInformation = (ArrayList) this.CheckinInformation.Clone();
      shelvesetEvent.m_title = (string) null;
      shelvesetEvent.m_contentTitle = (string) null;
      shelvesetEvent.TeamProject = (string) null;
      return (object) shelvesetEvent;
    }

    public string Name
    {
      get => this.m_shelvesetName;
      set => this.m_shelvesetName = value;
    }

    public ShelvesetEventType EventType
    {
      get => this.m_eventType;
      set => this.m_eventType = value;
    }

    private string GetEventTypeString()
    {
      string empty = string.Empty;
      switch (this.EventType)
      {
        case ShelvesetEventType.Created:
          empty = VersionControlIntegrationResources.Get("ShelvesetCreated");
          break;
        case ShelvesetEventType.Updated:
          empty = VersionControlIntegrationResources.Get("ShelvesetUpdated");
          break;
      }
      return empty;
    }

    public string Title
    {
      get
      {
        if (this.m_title == null)
        {
          string str;
          if (this.EventType != ShelvesetEventType.None)
            str = VersionControlIntegrationResources.Format("ShelvesetEmailTitleTemplate", (object) this.TeamProject, (object) this.Name, (object) this.GetEventTypeString());
          else
            str = VersionControlIntegrationResources.Format("ShelvesetEmailTitleTemplateNoType", (object) this.TeamProject, (object) this.Name);
          int maxCommentChars = Math.Min(80, Math.Max(80 - str.Length, 20));
          if (maxCommentChars > 0 && !string.IsNullOrEmpty(this.Comment))
            this.m_title = VersionControlIntegrationResources.Format("ShelvesetEmailTitle", (object) str, (object) this.CommentText(this.Comment, maxCommentChars));
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
            this.m_contentTitle = VersionControlIntegrationResources.Format("ShelvesetContentTitle", (object) this.Name, (object) this.GetEventTypeString(), (object) this.CommentText(this.Comment, 80));
          else
            this.m_contentTitle = VersionControlIntegrationResources.Format("ShelvesetContentTitleNoComment", (object) this.Name, (object) this.GetEventTypeString());
        }
        return this.m_contentTitle;
      }
      set => this.m_contentTitle = value;
    }

    public static class Roles
    {
      public static string ShelvesetOwner = "shelvesetOwner";
    }
  }
}
