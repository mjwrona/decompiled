// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.NotificationEvent
// Assembly: Microsoft.TeamFoundation.VersionControl.Common.Integration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2A9D748-4CEE-4498-9785-584B91A44F85
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.Integration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [SoapInclude(typeof (CheckinWorkItemInfo))]
  [XmlInclude(typeof (NameValuePair))]
  [SoapInclude(typeof (NameValuePair))]
  [XmlInclude(typeof (ClientArtifact))]
  [SoapInclude(typeof (ClientArtifact))]
  public abstract class NotificationEvent
  {
    public bool AllChangesIncluded;
    public string Notice;
    [Obsolete]
    public string Subscriber;
    [XmlArray("CheckinNotes")]
    [XmlArrayItem("CheckinNote")]
    public ArrayList CheckinNotes;
    [XmlArray("CheckinInformation")]
    [XmlArrayItem("CheckinInformation")]
    public ArrayList CheckinInformation;
    [XmlArray("Artifacts")]
    [XmlArrayItem("Artifact")]
    public ArrayList Artifacts;
    private string m_policyOverrideComment;
    private string m_comment;
    private string m_owner;
    private string m_ownerDisplay;
    private string m_teamProject;
    private string m_timeZone;
    private string m_timeZoneOffset;
    private DateTime m_creationDate;

    protected NotificationEvent(DateTime creationDate, string owner, string comment)
      : this(creationDate, owner, owner, comment)
    {
    }

    protected NotificationEvent(
      DateTime creationDate,
      string owner,
      string ownerDisplay,
      string comment)
    {
      this.m_owner = owner;
      this.m_ownerDisplay = ownerDisplay;
      this.m_comment = comment;
      this.m_creationDate = creationDate;
    }

    protected NotificationEvent()
    {
    }

    public string Owner
    {
      get => this.m_owner;
      set => this.m_owner = value;
    }

    public string OwnerDisplay
    {
      get => this.m_ownerDisplay;
      set => this.m_ownerDisplay = value;
    }

    public string CreationDate
    {
      get => this.m_creationDate.ToLocalTime().ToString((IFormatProvider) CultureInfo.InstalledUICulture);
      set
      {
        DateTime result;
        if (!DateTime.TryParse(value, (IFormatProvider) CultureInfo.InstalledUICulture, DateTimeStyles.None, out result))
          return;
        this.m_creationDate = DateTime.SpecifyKind(result, DateTimeKind.Local);
      }
    }

    internal DateTime CreationDateInternal
    {
      get => this.m_creationDate;
      set => this.m_creationDate = value;
    }

    public string Comment
    {
      get => !string.IsNullOrEmpty(this.m_comment) ? this.m_comment : string.Empty;
      set => this.m_comment = value;
    }

    public string TimeZone
    {
      get
      {
        if (this.m_timeZone == null)
        {
          if (System.TimeZone.CurrentTimeZone.IsDaylightSavingTime(this.m_creationDate.ToLocalTime()))
            return System.TimeZone.CurrentTimeZone.DaylightName;
          this.m_timeZone = System.TimeZone.CurrentTimeZone.StandardName;
        }
        return this.m_timeZone;
      }
      set => this.m_timeZone = value;
    }

    public string TimeZoneOffset
    {
      get
      {
        if (this.m_timeZoneOffset == null)
          this.m_timeZoneOffset = TFCommonUtil.GetLocalTimeZoneOffset(this.m_creationDate.ToLocalTime());
        return this.m_timeZoneOffset;
      }
      set => this.m_timeZoneOffset = value;
    }

    public static string TeamProjectListSeparator => VersionControlIntegrationResources.Get(nameof (TeamProjectListSeparator));

    public string TeamProject
    {
      get
      {
        if (this.m_teamProject == null)
        {
          Set<string> set = new Set<string>((IEqualityComparer<string>) TFStringComparer.VersionControlPath);
          foreach (ClientArtifact artifact in this.Artifacts)
          {
            if ((VssStringComparer.ArtifactType.Equals(artifact.Type, "VersionedItem") || VssStringComparer.ArtifactType.Equals(artifact.Type, "ShelvedItem")) && !set.Contains(artifact.TeamProject))
            {
              set.Add(artifact.TeamProject);
              this.m_teamProject = this.m_teamProject != null ? this.m_teamProject + VersionControlIntegrationResources.Get("TeamProjectListSeparator") + artifact.TeamProject : artifact.TeamProject;
            }
          }
        }
        return this.m_teamProject;
      }
      set => this.m_teamProject = value;
    }

    public string PolicyOverrideComment
    {
      get => !string.IsNullOrEmpty(this.m_policyOverrideComment) ? this.m_policyOverrideComment : string.Empty;
      set => this.m_policyOverrideComment = value;
    }

    protected string CommentText(string comment, int maxCommentChars)
    {
      if (maxCommentChars <= 0 || string.IsNullOrEmpty(comment))
        return string.Empty;
      string str = comment.Trim();
      int count = Math.Min(str.Length, maxCommentChars);
      int length = str.IndexOf('\n', 0, count);
      if (length != -1)
      {
        if (length > 0 && str[length - 1] == '\r')
          --length;
        return str.Substring(0, length);
      }
      return str.Length <= maxCommentChars ? str : str.Substring(0, maxCommentChars) + "...";
    }
  }
}
