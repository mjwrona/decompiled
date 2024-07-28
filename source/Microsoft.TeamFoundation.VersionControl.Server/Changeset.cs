// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Changeset
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [RequiredClientService("VersionControlServer")]
  [CallOnDeserialization("AfterDeserialize")]
  public class Changeset : IValidatable, ICacheable
  {
    internal bool AllChangesIncluded;
    internal int checkinNoteId;
    internal Guid committerId;
    internal Guid ownerId;
    private StreamingCollection<PropertyValue> m_properties;
    private StreamingCollection<Change> m_changes = new StreamingCollection<Change>()
    {
      HandleExceptions = false
    };
    private string m_comment;
    private string m_committer;
    private string m_committerDisplayName;
    private DateTime m_creationDate;
    private int m_changesetId;
    private string m_owner;
    private string m_ownerDisplayName;
    private CheckinNote m_checkinNote = new CheckinNote();
    private PolicyOverrideInfo m_policyOverride = new PolicyOverrideInfo();

    public string Comment
    {
      get => this.m_comment;
      set => this.m_comment = value;
    }

    [XmlAttribute("cmtr")]
    public string Committer
    {
      get => this.m_committer;
      set => this.m_committer = value;
    }

    [XmlAttribute("cmtrdisp")]
    public string CommitterDisplayName
    {
      get => this.m_committerDisplayName;
      set => this.m_committerDisplayName = value;
    }

    [XmlAttribute("date")]
    public DateTime CreationDate
    {
      get => this.m_creationDate;
      set => this.m_creationDate = value;
    }

    [XmlAttribute("cset")]
    public int ChangesetId
    {
      get => this.m_changesetId;
      set => this.m_changesetId = value;
    }

    [XmlAttribute("owner")]
    public string Owner
    {
      get => this.m_owner;
      set => this.m_owner = value;
    }

    public Guid OwnerId => this.ownerId;

    [XmlAttribute("ownerdisp")]
    public string OwnerDisplayName
    {
      get => this.m_ownerDisplayName;
      set => this.m_ownerDisplayName = value;
    }

    public CheckinNote CheckinNote
    {
      get => this.m_checkinNote;
      set => this.m_checkinNote = value;
    }

    public PolicyOverrideInfo PolicyOverride
    {
      get => this.m_policyOverride;
      set => this.m_policyOverride = value;
    }

    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal, PropertyName = "InternalProperties")]
    public StreamingCollection<PropertyValue> Properties
    {
      get => this.m_properties;
      set => this.m_properties = value;
    }

    public StreamingCollection<Change> Changes
    {
      get => this.m_changes;
      set => this.m_changes = value;
    }

    internal void RecordInformation(MethodInformation methodInformation)
    {
      methodInformation.AddParameter("Changeset.CreationDate", (object) this.CreationDate.ToUniversalTime());
      if (!string.IsNullOrEmpty(this.Owner))
        methodInformation.AddParameter("Changeset.Author", (object) this.Owner);
      if (!string.IsNullOrEmpty(this.Comment))
        methodInformation.AddParameter("Changeset.Comment", (object) this.Comment.Substring(0, Math.Min(this.Comment.Length, 80)));
      if (this.CheckinNote == null || this.CheckinNote.Values == null || this.CheckinNote.Values.Length == 0)
        return;
      methodInformation.AddArrayParameter<CheckinNoteFieldValue>("checkinNotes", (IList<CheckinNoteFieldValue>) this.CheckinNote.Values);
    }

    internal static CheckinEvent QueryCheckin(
      VersionControlRequestContext versionControlRequestContext,
      int changeset,
      int changeCountLimit,
      VssNotificationEvent notificationEvent = null)
    {
      return Changeset.QueryCheckin(versionControlRequestContext.Elevate(), changeset, false, changeCountLimit, notificationEvent);
    }

    internal static CheckinEvent QueryCheckin(
      VersionControlRequestContext versionControlRequestContext,
      int changeset,
      bool includeWorkItems,
      VssNotificationEvent notificationEvent = null)
    {
      return Changeset.QueryCheckin(versionControlRequestContext, changeset, includeWorkItems, 0, notificationEvent);
    }

    internal static CheckinEvent QueryCheckin(
      VersionControlRequestContext versionControlRequestContext,
      int changeset,
      bool includeWorkItems,
      int changeCountLimit,
      VssNotificationEvent notificationEvent = null)
    {
      IVssRequestContext requestContext = versionControlRequestContext.RequestContext;
      using (CommandQueryChangeset commandQueryChangeset = new CommandQueryChangeset(versionControlRequestContext))
      {
        commandQueryChangeset.Execute(changeset, true, false, changeCountLimit);
        Changeset changeset1 = commandQueryChangeset.Changeset;
        CheckinEvent data = new CheckinEvent(changeset, changeset1.CreationDate, changeset1.Owner, changeset1.OwnerDisplayName, changeset1.Committer, changeset1.CommitterDisplayName, changeset1.Comment);
        data.AllChangesIncluded = changeset1.AllChangesIncluded;
        if (!string.Equals(CultureInfo.CurrentCulture.ToString(), CultureInfo.InstalledUICulture.ToString()))
        {
          DateTime dateTime = changeset1.CreationDate;
          dateTime = dateTime.ToLocalTime();
          string s = dateTime.ToString();
          dateTime = changeset1.CreationDate;
          dateTime = dateTime.ToLocalTime();
          string str = dateTime.ToString((IFormatProvider) CultureInfo.InstalledUICulture);
          DateTime result;
          DateTime.TryParse(s, (IFormatProvider) CultureInfo.InstalledUICulture, DateTimeStyles.None, out result);
          requestContext.TraceAlways(700353, TraceLevel.Verbose, TraceArea.General, TraceLayer.Component, "Detected Culture Change: {0} != {1}, {2}, {3}, {4}, {5}.", (object) CultureInfo.CurrentCulture, (object) CultureInfo.InstalledUICulture, (object) CultureInfo.CurrentUICulture, (object) s, (object) str, (object) result);
        }
        ArrayList arrayList = new ArrayList();
        ITswaServerHyperlinkService tswaHyperlinkService = versionControlRequestContext.RequestContext.GetService<ITswaServerHyperlinkService>();
        TeamFoundationLinkingService linkingService = (TeamFoundationLinkingService) null;
        string str1;
        try
        {
          str1 = tswaHyperlinkService.GetChangesetDetailsUrl(requestContext, changeset1.ChangesetId).AbsoluteUri;
        }
        catch (InvalidOperationException ex)
        {
          versionControlRequestContext.RequestContext.TraceException(700044, TraceLevel.Info, TraceArea.Linking, TraceLayer.BusinessLogic, (Exception) ex);
          tswaHyperlinkService = (ITswaServerHyperlinkService) null;
          if (linkingService == null)
            linkingService = versionControlRequestContext.RequestContext.GetService<TeamFoundationLinkingService>();
          VersionControlIntegrationUri controlIntegrationUri = (VersionControlIntegrationUri) new ChangesetUri(changeset1.ChangesetId, UriType.Extended);
          str1 = linkingService.GetArtifactUrlExternal(versionControlRequestContext.RequestContext, controlIntegrationUri.ArtifactId);
        }
        ClientArtifact clientArtifact1 = new ClientArtifact(str1, nameof (Changeset));
        arrayList.Add((object) clientArtifact1);
        int index;
        while (changeset1.Changes.MoveNext())
        {
          Change current = changeset1.Changes.Current;
          string versionedItemUrl;
          try
          {
            versionedItemUrl = Changeset.GetVersionedItemUrl(versionControlRequestContext, changeset1.ChangesetId, current, tswaHyperlinkService, linkingService);
          }
          catch (InvalidOperationException ex)
          {
            versionControlRequestContext.RequestContext.TraceException(700045, TraceLevel.Info, TraceArea.Bis, TraceLayer.BusinessLogic, (Exception) ex);
            tswaHyperlinkService = (ITswaServerHyperlinkService) null;
            if (linkingService == null)
              linkingService = versionControlRequestContext.RequestContext.GetService<TeamFoundationLinkingService>();
            versionedItemUrl = Changeset.GetVersionedItemUrl(versionControlRequestContext, changeset1.ChangesetId, current, tswaHyperlinkService, linkingService);
          }
          ClientArtifact clientArtifact2 = new ClientArtifact(versionedItemUrl, "VersionedItem");
          clientArtifact2.ServerItem = current.Item.ServerItem;
          ClientArtifact clientArtifact3 = clientArtifact2;
          index = current.Item.ChangesetId;
          string str2 = index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          clientArtifact3.ItemVersion = str2;
          clientArtifact2.ChangeType = Changeset.ChangeTypeToString(current.ChangeType);
          arrayList.Add((object) clientArtifact2);
        }
        data.AllChangesIncluded = changeset1.AllChangesIncluded;
        data.Artifacts = arrayList;
        if (includeWorkItems)
        {
          VersionControlIntegrationUri controlIntegrationUri = (VersionControlIntegrationUri) new ChangesetUri(changeset1.ChangesetId, UriType.Normal);
          CheckinWorkItemInfo[] c = new WorkItemHelper(versionControlRequestContext.RequestContext).QueryWorkItemInformation(controlIntegrationUri.ToString());
          if (c != null && c.Length != 0)
            data.CheckinInformation = new ArrayList((ICollection) c);
        }
        if (changeset1.CheckinNote != null && changeset1.CheckinNote.Values != null && changeset1.CheckinNote.Values.Length != 0)
        {
          data.CheckinNotes = new ArrayList(changeset1.CheckinNote.Values.Length);
          CheckinNoteFieldValue[] values = changeset1.CheckinNote.Values;
          for (index = 0; index < values.Length; ++index)
          {
            CheckinNoteFieldValue checkinNoteFieldValue = values[index];
            data.CheckinNotes.Add((object) new NameValuePair(checkinNoteFieldValue.Name, checkinNoteFieldValue.Value));
          }
        }
        if (changeset1.PolicyOverride != null)
        {
          if (!string.IsNullOrEmpty(changeset1.PolicyOverride.Comment))
            data.PolicyOverrideComment = changeset1.PolicyOverride.Comment;
          if (changeset1.PolicyOverride.PolicyFailures != null)
          {
            data.PolicyFailures = new ArrayList(changeset1.PolicyOverride.PolicyFailures.Length);
            PolicyFailureInfo[] policyFailures = changeset1.PolicyOverride.PolicyFailures;
            for (index = 0; index < policyFailures.Length; ++index)
            {
              PolicyFailureInfo policyFailureInfo = policyFailures[index];
              data.PolicyFailures.Add((object) new NameValuePair(policyFailureInfo.PolicyName, policyFailureInfo.Message));
            }
          }
        }
        if (notificationEvent != null)
        {
          notificationEvent.InitFromObject((object) data);
          notificationEvent.SourceEventCreatedTime = new DateTime?(data.ChangesetCreationDate);
          notificationEvent.ItemId = changeset.ToString();
          notificationEvent.AddActor(CheckinEvent.Roles.Committer, changeset1.committerId);
          notificationEvent.AddActor(CheckinEvent.Roles.Owner, changeset1.ownerId);
          notificationEvent.AddActor(VssNotificationEvent.Roles.Initiator, changeset1.committerId);
          notificationEvent.AddArtifactUri(str1);
        }
        return data;
      }
    }

    internal static string GetVersionedItemUrl(
      VersionControlRequestContext versionControlRequestContext,
      int changesetId,
      Change versionedItem,
      ITswaServerHyperlinkService tswaHyperlinkService,
      TeamFoundationLinkingService linkingService)
    {
      string versionedItemUrl;
      if (tswaHyperlinkService != null)
      {
        if (versionedItem.Item.ItemType == ItemType.File)
        {
          if ((versionedItem.ChangeType & ChangeType.Edit) == ChangeType.Edit && (versionedItem.ChangeType & (ChangeType.Add | ChangeType.Branch)) == ChangeType.None)
          {
            if ((versionedItem.ChangeType & ChangeType.Rename) == ChangeType.Rename)
            {
              string modifiedItemVersionSpec = versionedItem.Item.ChangesetId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
              versionedItemUrl = tswaHyperlinkService.GetDifferenceSourceControlItemsUrl(versionControlRequestContext.RequestContext, versionedItem.Item.ServerItem, "P" + modifiedItemVersionSpec, versionedItem.Item.ServerItem, modifiedItemVersionSpec).AbsoluteUri;
            }
            else
              versionedItemUrl = tswaHyperlinkService.GetDifferenceSourceControlItemsUrl(versionControlRequestContext.RequestContext, versionedItem.Item.ServerItem, versionedItem.Item.ChangesetId - 1, versionedItem.Item.ServerItem, versionedItem.Item.ChangesetId).AbsoluteUri;
          }
          else
            versionedItemUrl = tswaHyperlinkService.GetViewSourceControlItemUrl(versionControlRequestContext.RequestContext, versionedItem.Item.ServerItem, versionedItem.Item.ChangesetId).AbsoluteUri;
        }
        else
          versionedItemUrl = tswaHyperlinkService.GetSourceExplorerUrl(versionControlRequestContext.RequestContext, versionedItem.Item.ServerItem).AbsoluteUri;
      }
      else
      {
        VersionControlIntegrationUri controlIntegrationUri = (VersionControlIntegrationUri) new VersionedItemUri(versionedItem.Item.ServerItem, changesetId, versionedItem.Item.DeletionId, UriType.Extended);
        versionedItemUrl = linkingService.GetArtifactUrlExternal(versionControlRequestContext.RequestContext, controlIntegrationUri.ArtifactId);
      }
      return versionedItemUrl;
    }

    internal static string ChangeTypeToString(ChangeType type)
    {
      if (type == ChangeType.None)
        return Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Get("ChangeTypeNone");
      StringBuilder sb = new StringBuilder(256);
      if (type != ChangeType.Lock)
        type &= ~ChangeType.Lock;
      Changeset.AppendIf((type & ChangeType.Merge) != 0, sb, "ChangeTypeMerge");
      Changeset.AppendIf((type & ChangeType.Add) != 0, sb, "ChangeTypeAdd");
      Changeset.AppendIf((type & ChangeType.Branch) != 0, sb, "ChangeTypeBranch");
      Changeset.AppendIf((type & ChangeType.Delete) != 0, sb, "ChangeTypeDelete");
      Changeset.AppendIf((type & ChangeType.Encoding) != ChangeType.None && (type & (ChangeType.Add | ChangeType.Branch)) == ChangeType.None, sb, "ChangeTypeFileType");
      Changeset.AppendIf((type & ChangeType.Lock) != 0, sb, "ChangeTypeLock");
      Changeset.AppendIf((type & (ChangeType.Rename | ChangeType.TargetRename)) != 0, sb, "ChangeTypeRename");
      Changeset.AppendIf((type & ChangeType.Undelete) != 0, sb, "ChangeTypeUndelete");
      Changeset.AppendIf((type & ChangeType.Edit) != ChangeType.None && (type & ChangeType.Add) == ChangeType.None, sb, "ChangeTypeEdit");
      Changeset.AppendIf((type & ChangeType.Rollback) != 0, sb, "ChangeTypeRollback");
      Changeset.AppendIf((type & ChangeType.SourceRename) != 0, sb, "ChangeTypeSourceRename");
      Changeset.AppendIf((type & ChangeType.Property) != 0, sb, "ChangeTypeProperty");
      return sb.ToString();
    }

    private static void AppendIf(bool condition, StringBuilder sb, string resourceName)
    {
      if (!condition)
        return;
      if (sb.Length != 0)
        sb.Append(", ");
      sb.Append(Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Get(resourceName));
    }

    internal static void CheckUpdateChangesetPermission(
      VersionControlRequestContext versionControlRequestContext,
      Changeset changesetInfo,
      ObjectBinder<Change> changeBinder)
    {
      bool flag1 = false;
      bool flag2 = true;
      bool flag3 = false;
      if (IdentityDescriptorComparer.Instance.Equals(TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, changesetInfo.ownerId).Descriptor, versionControlRequestContext.RequestContext.UserContext))
        flag3 = true;
      while (changeBinder.MoveNext())
      {
        Change current = changeBinder.Current;
        if (current.Item.HasPermission(versionControlRequestContext, VersionedItemPermissions.Read))
          flag1 = true;
        if (!flag3 & flag2 && !current.Item.HasPermission(versionControlRequestContext, VersionedItemPermissions.ReviseOther))
          flag2 = false;
        if (((flag3 ? 0 : (!flag2 ? 1 : 0)) & (flag1 ? 1 : 0)) != 0 || flag1 & flag3)
          break;
      }
      if (!flag1)
        throw new ChangesetNotFoundException(changesetInfo.ChangesetId);
      if (!flag2)
        throw new ResourceAccessException(versionControlRequestContext.RequestContext.GetUserId().ToString(), "ReviseOther", Resources.Format("AllItemsInChangeset"));
    }

    internal static void UpdateChangeset(
      VersionControlRequestContext versionControlRequestContext,
      int changeset,
      string comment,
      CheckinNote checkinNote)
    {
      if ((changeset <= 0 || changeset > versionControlRequestContext.VersionControlService.GetLatestChangeset(versionControlRequestContext)) && versionControlRequestContext.RequestContext.UserContext != (IdentityDescriptor) null)
        throw new ChangesetNotFoundException(changeset);
      using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
      {
        ResultCollection rc = versionedItemComponent.QueryChangeset(changeset, true, true);
        Changeset changesetInfo = Changeset.LoadChangeSet(rc, changeset);
        rc.NextResult();
        using (versionControlRequestContext.RequestContext.AcquireExemptionLock())
          Changeset.CheckUpdateChangesetPermission(versionControlRequestContext, changesetInfo, rc.GetCurrent<Change>());
        versionedItemComponent.UpdateChangeset(changeset, comment, checkinNote);
      }
    }

    internal static Changeset LoadChangeSet(ResultCollection rc, int changeset)
    {
      ObjectBinder<Changeset> current = rc.GetCurrent<Changeset>();
      Changeset changeset1 = current.MoveNext() ? current.Current : throw new ChangesetNotFoundException(changeset);
      rc.NextResult();
      if (rc.GetCurrent<PolicyFailureInfo>().Items.Count > 0)
        changeset1.PolicyOverride.PolicyFailures = rc.GetCurrent<PolicyFailureInfo>().Items.ToArray();
      rc.NextResult();
      if (rc.GetCurrent<CheckinNoteFieldValue>().Items.Count > 0)
        changeset1.CheckinNote.Values = rc.GetCurrent<CheckinNoteFieldValue>().Items.ToArray();
      return changeset1;
    }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      versionControlRequestContext.Validation.checkChangesetComment(this.m_comment, "comment", true);
      versionControlRequestContext.Validation.nullToEmpty(ref this.m_comment);
      versionControlRequestContext.Validation.checkIdentity(ref this.m_owner, "owner", true);
      versionControlRequestContext.Validation.check((IValidatable) this.m_checkinNote, "CheckinNote", true);
      versionControlRequestContext.Validation.check((IValidatable) this.m_policyOverride, "PolicyOverride", true);
    }

    internal void LookupDisplayNames(
      VersionControlRequestContext versionControlRequestContext)
    {
      string identityName1;
      string displayName1;
      versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentityNames(versionControlRequestContext.RequestContext, this.ownerId, out identityName1, out displayName1);
      this.Owner = identityName1;
      this.OwnerDisplayName = displayName1;
      if (this.committerId == this.ownerId)
      {
        this.Committer = this.Owner;
        this.CommitterDisplayName = this.OwnerDisplayName;
      }
      else
      {
        string identityName2;
        string displayName2;
        versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentityNames(versionControlRequestContext.RequestContext, this.committerId, out identityName2, out displayName2);
        this.Committer = identityName2;
        this.CommitterDisplayName = displayName2;
      }
    }

    public int GetCachedSize()
    {
      int cachedSize = 750;
      if (this.CheckinNote != null && this.CheckinNote.Values != null)
        cachedSize += 100 * this.CheckinNote.Values.Length;
      if (this.PolicyOverride != null && this.PolicyOverride.PolicyFailures != null)
        cachedSize += 250 * this.PolicyOverride.PolicyFailures.Length;
      return cachedSize;
    }
  }
}
