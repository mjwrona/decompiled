// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Shelveset
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [RequiredClientService("VersionControlServer")]
  [CallOnDeserialization("AfterDeserialize")]
  [CallOnSerialization("BeforeSerialize")]
  public class Shelveset : IValidatable, IPropertyMergerItem
  {
    internal Guid ownerId;
    internal int shelvesetId;
    internal int checkinNoteId;
    private string m_comment;
    private DateTime m_creationDate;
    private string m_name;
    private string m_owner;
    private string m_ownerDisplayName;
    private string m_policyOverrideComment;
    private CheckinNote m_checkinNote = new CheckinNote();
    private VersionControlLink[] m_links;
    private bool m_changesExcluded;
    private StreamingCollection<PropertyValue> m_properties;
    private string m_versionString;
    private int m_version = -2;
    private const int c_maxCommentLength = 1073741823;
    private const int c_maxPolicyOverrideCommentLength = 2048;
    internal const int MinimumVersion = -1;

    public string Comment
    {
      get => this.m_comment;
      set => this.m_comment = value;
    }

    [XmlAttribute("date")]
    public DateTime CreationDate
    {
      get => this.m_creationDate;
      set => this.m_creationDate = value;
    }

    [XmlAttribute("name")]
    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    [XmlAttribute("owner")]
    [ClientProperty(ClientVisibility.Private)]
    public string Owner
    {
      get => this.m_owner;
      set => this.m_owner = value;
    }

    [XmlAttribute("ownerdisp")]
    [ClientProperty(ClientVisibility.Private)]
    public string OwnerDisplayName
    {
      get => this.m_ownerDisplayName;
      set => this.m_ownerDisplayName = value;
    }

    [XmlAttribute("owneruniq")]
    [ClientProperty(ClientVisibility.Private)]
    public string OwnerUniqueName
    {
      get => this.m_owner;
      set
      {
        if (!string.IsNullOrEmpty(this.m_owner))
          return;
        this.m_owner = value;
      }
    }

    [ClientProperty(ClientVisibility.Private)]
    public Guid OwnerId => this.ownerId;

    internal int Version
    {
      get
      {
        if (this.m_version == -2 && !string.IsNullOrEmpty(this.m_versionString))
          int.TryParse(this.m_versionString, out this.m_version);
        return this.m_version;
      }
      set
      {
        ArgumentUtility.CheckForOutOfRange(value, "ShelveSet.Version", -1);
        this.m_version = value;
        this.m_versionString = value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    internal string VersionString
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_versionString) && this.m_version != -2)
          this.m_versionString = this.m_version.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        return this.m_versionString;
      }
      set
      {
        this.m_versionString = value;
        if (string.IsNullOrEmpty(this.m_versionString) || !int.TryParse(this.m_versionString, out this.m_version))
          return;
        ArgumentUtility.CheckForOutOfRange(this.m_version, "ShelveSet.VersionString", -1);
      }
    }

    public string PolicyOverrideComment
    {
      get => this.m_policyOverrideComment;
      set => this.m_policyOverrideComment = value;
    }

    public CheckinNote CheckinNote
    {
      get => this.m_checkinNote;
      set => this.m_checkinNote = value;
    }

    [ClientProperty(ClientVisibility.Private)]
    public VersionControlLink[] Links
    {
      get => this.m_links;
      set => this.m_links = value;
    }

    [XmlAttribute("ce")]
    [DefaultValue(false)]
    public bool ChangesExcluded
    {
      get => this.m_changesExcluded;
      set => this.m_changesExcluded = value;
    }

    [ClientProperty(ClientVisibility.Private, PropertyName = "InternalProperties")]
    public StreamingCollection<PropertyValue> Properties
    {
      get => this.m_properties;
      set => this.m_properties = value;
    }

    StreamingCollection<PropertyValue> IPropertyMergerItem.GetProperties(Guid artifactKind) => this.Properties;

    void IPropertyMergerItem.SetProperties(
      Guid artifactKind,
      StreamingCollection<PropertyValue> properties)
    {
      this.Properties = properties;
    }

    public ArtifactSpec GetArtifactSpec(Guid artifactKind) => new ArtifactSpec(artifactKind, this.shelvesetId, 0);

    int IPropertyMergerItem.SequenceId { get; set; }

    internal static void Delete(
      VersionControlRequestContext versionControlRequestContext,
      string shelvesetName,
      string ownerName,
      int shelvesetVersion)
    {
      Guid result;
      Microsoft.VisualStudio.Services.Identity.Identity identity;
      if (Guid.TryParse(ownerName, out result))
      {
        identity = TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, result, false);
      }
      else
      {
        try
        {
          identity = TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, ownerName);
        }
        catch (IdentityNotFoundException ex)
        {
          versionControlRequestContext.RequestContext.TraceException(700160, TraceLevel.Info, TraceArea.Identities, TraceLayer.BusinessLogic, (Exception) ex);
          throw new ShelvesetNotFoundException(shelvesetName, ownerName);
        }
      }
      if (!versionControlRequestContext.RequestContext.IsSystemContext)
      {
        IdentityDescriptor userContext = versionControlRequestContext.RequestContext.UserContext;
        if (!IdentityDescriptorComparer.Instance.Equals(identity.Descriptor, userContext))
          versionControlRequestContext.VersionControlService.SecurityWrapper.CheckGlobalPermission(versionControlRequestContext, GlobalPermissions.AdminShelvesets);
      }
      using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
        versionedItemComponent.DeleteShelveset(identity.Id, shelvesetName, shelvesetVersion);
    }

    internal void Shelve(
      VersionControlRequestContext versionControlRequestContext,
      Workspace workspace,
      IEnumerable<string> serverItems,
      bool replace,
      List<Failure> failures)
    {
      if (serverItems == null)
        serverItems = (IEnumerable<string>) Array.Empty<string>();
      SecurityManager securityWrapper = versionControlRequestContext.VersionControlService.SecurityWrapper;
      securityWrapper.CheckWorkspacePermission(versionControlRequestContext, 2, workspace);
      IdentityDescriptor userContext = versionControlRequestContext.RequestContext.UserContext;
      Microsoft.VisualStudio.Services.Identity.Identity identity = TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, this.Owner);
      ClientTrace.Publish(versionControlRequestContext.RequestContext, nameof (Shelve));
      int result;
      if (string.IsNullOrEmpty(this.VersionString))
        result = -1;
      else if (!int.TryParse(this.VersionString, out result))
        throw new ArgumentException("Bad version");
      ArgumentUtility.CheckForOutOfRange(result, "shelvesetVersion", -1, 0);
      if (!IdentityDescriptorComparer.Instance.Equals(identity.Descriptor, userContext))
        throw new IllegalShelvesetOwnerException();
      foreach (string serverItem in serverItems)
      {
        try
        {
          securityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.PendChange, serverItem);
        }
        catch (RequestCanceledException ex)
        {
          throw;
        }
        catch (ApplicationException ex)
        {
          versionControlRequestContext.RequestContext.TraceException(700161, TraceLevel.Info, TraceArea.Shelve, TraceLayer.BusinessLogic, (Exception) ex);
          failures.Add(new Failure((Exception) ex));
        }
      }
      if (failures.Count > 0)
        return;
      ShelvesetNotification notificationEvent = new ShelvesetNotification(this.Name, identity, serverItems, this.Comment, this.CheckinNote, this.PolicyOverrideComment, this.Links, workspace.Name, workspace.OwnerName, workspace.Computer, replace ? ShelvesetNotificationType.Update : ShelvesetNotificationType.Create);
      ITeamFoundationEventService service = versionControlRequestContext.RequestContext.GetService<ITeamFoundationEventService>();
      service.PublishDecisionPoint(versionControlRequestContext.RequestContext, (object) notificationEvent);
      bool flag;
      using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
        flag = versionedItemComponent.Shelve(workspace.OwnerId, workspace.Name, identity.Id, this.Name, result, this.Comment, this.PolicyOverrideComment, serverItems, this.CheckinNote, this.Links, replace, this.Properties);
      notificationEvent.ShelvesetNotificationType = flag ? ShelvesetNotificationType.Update : ShelvesetNotificationType.Create;
      service.PublishNotification(versionControlRequestContext.RequestContext, (object) notificationEvent);
    }

    internal static List<Shelveset> Query(
      VersionControlRequestContext versionControlRequestContext,
      string ownerName,
      string shelvesetName,
      int shelvesetVersion)
    {
      return versionControlRequestContext.VersionControlService.QueryShelvesets(versionControlRequestContext.RequestContext, shelvesetName, ownerName, shelvesetVersion);
    }

    internal static ShelvesetEvent QueryShelveset(
      VersionControlRequestContext versionControlRequestContext,
      string shelvesetName,
      string shelvesetOwner,
      int shelvesetVersion,
      bool includeWorkItems,
      int changeCountLimit,
      VssNotificationEvent notificationEvent = null)
    {
      IVssRequestContext requestContext = versionControlRequestContext.RequestContext;
      List<Shelveset> shelvesetList = Shelveset.Query(versionControlRequestContext, shelvesetOwner, shelvesetName, shelvesetVersion);
      if (shelvesetList.Count != 1)
        throw new ShelvesetNotFoundException(shelvesetName, shelvesetOwner);
      ShelvesetEvent data = new ShelvesetEvent(shelvesetList[0].Name, shelvesetList[0].CreationDate, shelvesetList[0].Owner, shelvesetList[0].OwnerDisplayName, shelvesetList[0].Comment, ShelvesetEventType.None);
      ArrayList arrayList = new ArrayList();
      ITswaServerHyperlinkService tswaHyperlinkService = versionControlRequestContext.RequestContext.GetService<ITswaServerHyperlinkService>();
      TeamFoundationLinkingService linkingService = (TeamFoundationLinkingService) null;
      string str;
      try
      {
        str = tswaHyperlinkService.GetShelvesetDetailsUrl(versionControlRequestContext.RequestContext, shelvesetName, shelvesetOwner).AbsoluteUri;
      }
      catch (InvalidOperationException ex)
      {
        versionControlRequestContext.RequestContext.TraceException(700162, TraceLevel.Info, TraceArea.Linking, TraceLayer.BusinessLogic, (Exception) ex);
        tswaHyperlinkService = (ITswaServerHyperlinkService) null;
        if (linkingService == null)
          linkingService = versionControlRequestContext.RequestContext.GetService<TeamFoundationLinkingService>();
        VersionControlIntegrationUri controlIntegrationUri = (VersionControlIntegrationUri) new ShelvesetUri(shelvesetName, shelvesetOwner, UriType.Extended);
        str = linkingService.GetArtifactUrlExternal(versionControlRequestContext.RequestContext, controlIntegrationUri.ArtifactId);
      }
      ClientArtifact clientArtifact = new ClientArtifact(str, nameof (Shelveset));
      arrayList.Add((object) clientArtifact);
      using (CommandQueryPendingSets queryPendingSets = new CommandQueryPendingSets(versionControlRequestContext))
      {
        queryPendingSets.Execute((Workspace) null, shelvesetOwner, new ItemSpec[1]
        {
          new ItemSpec()
          {
            ItemPathPair = ItemPathPair.FromServerItem("$/"),
            RecursionType = RecursionType.Full
          }
        }, shelvesetName, shelvesetVersion, PendingSetType.Shelveset, false, changeCountLimit + 1, (string) null, false, false, (string[]) null, false);
        if (queryPendingSets.PendingSets.MoveNext())
        {
          int num = 0;
          while (queryPendingSets.PendingSets.Current.PendingChanges.MoveNext())
          {
            if (num < changeCountLimit)
            {
              PendingChange current = queryPendingSets.PendingSets.Current.PendingChanges.Current;
              string shelvedItemUrl;
              try
              {
                shelvedItemUrl = Shelveset.GetShelvedItemUrl(versionControlRequestContext, shelvesetName, shelvesetOwner, current, str, tswaHyperlinkService, linkingService);
              }
              catch (InvalidOperationException ex)
              {
                versionControlRequestContext.RequestContext.TraceException(700163, TraceLevel.Info, TraceArea.Linking, TraceLayer.BusinessLogic, (Exception) ex);
                tswaHyperlinkService = (ITswaServerHyperlinkService) null;
                if (linkingService == null)
                  linkingService = versionControlRequestContext.RequestContext.GetService<TeamFoundationLinkingService>();
                shelvedItemUrl = Shelveset.GetShelvedItemUrl(versionControlRequestContext, shelvesetName, shelvesetOwner, current, str, tswaHyperlinkService, linkingService);
              }
              arrayList.Add((object) new ClientArtifact(shelvedItemUrl, "ShelvedItem")
              {
                ServerItem = current.ServerItem,
                ChangeType = Changeset.ChangeTypeToString(current.ChangeType)
              });
              ++num;
            }
          }
          data.AllChangesIncluded = num < changeCountLimit && queryPendingSets.CanAccessAllChanges;
        }
      }
      data.Artifacts = arrayList;
      if (includeWorkItems && shelvesetList[0].Links != null && shelvesetList[0].Links.Length != 0)
      {
        CheckinWorkItemInfo[] c = new WorkItemHelper(versionControlRequestContext.RequestContext).QueryWorkItemInformation(shelvesetList[0].Links);
        if (c != null && c.Length != 0)
          data.CheckinInformation = new ArrayList((ICollection) c);
      }
      if (shelvesetList[0].CheckinNote != null && shelvesetList[0].CheckinNote.Values != null && shelvesetList[0].CheckinNote.Values.Length != 0)
      {
        data.CheckinNotes = new ArrayList(shelvesetList[0].CheckinNote.Values.Length);
        foreach (CheckinNoteFieldValue checkinNoteFieldValue in shelvesetList[0].CheckinNote.Values)
          data.CheckinNotes.Add((object) new NameValuePair(checkinNoteFieldValue.Name, checkinNoteFieldValue.Value));
      }
      data.PolicyOverrideComment = shelvesetList[0].PolicyOverrideComment;
      if (notificationEvent != null)
      {
        notificationEvent.InitFromObject((object) data);
        notificationEvent.ItemId = shelvesetName + "-" + shelvesetOwner;
        notificationEvent.SourceEventCreatedTime = new DateTime?(shelvesetList[0].CreationDate);
        notificationEvent.AddActor(VssNotificationEvent.Roles.Initiator, shelvesetList[0].ownerId);
        notificationEvent.AddActor(ShelvesetEvent.Roles.ShelvesetOwner, shelvesetList[0].ownerId);
        notificationEvent.AddArtifactUri(str);
      }
      return data;
    }

    internal static string GetShelvedItemUrl(
      VersionControlRequestContext versionControlRequestContext,
      string shelvesetName,
      string shelvesetOwner,
      PendingChange shelvedItem,
      string shelvesetUrl,
      ITswaServerHyperlinkService tswaHyperlinkService,
      TeamFoundationLinkingService linkingService)
    {
      string shelvedItemUrl;
      if (tswaHyperlinkService != null)
      {
        if (shelvedItem.ItemType == ItemType.File)
        {
          if ((shelvedItem.ChangeType & ChangeType.Edit) == ChangeType.Edit && (shelvedItem.ChangeType & (ChangeType.Add | ChangeType.Branch)) == ChangeType.None)
          {
            string originalItemServerPath;
            int originalItemChangeset;
            if (!string.IsNullOrEmpty(shelvedItem.SourceServerItem))
            {
              originalItemServerPath = shelvedItem.SourceServerItem;
              originalItemChangeset = shelvedItem.SourceVersionFrom;
            }
            else
            {
              originalItemServerPath = shelvedItem.ServerItem;
              originalItemChangeset = shelvedItem.Version;
            }
            shelvedItemUrl = tswaHyperlinkService.GetDifferenceSourceControlShelvedItemUrl(versionControlRequestContext.RequestContext, originalItemServerPath, originalItemChangeset, shelvedItem.ServerItem, shelvesetName, shelvesetOwner).AbsoluteUri;
          }
          else
            shelvedItemUrl = tswaHyperlinkService.GetViewSourceControlShelvedItemUrl(versionControlRequestContext.RequestContext, shelvedItem.ServerItem, shelvesetName, shelvesetOwner).AbsoluteUri;
        }
        else
          shelvedItemUrl = shelvesetUrl;
      }
      else
      {
        VersionControlIntegrationUri controlIntegrationUri = (VersionControlIntegrationUri) new ShelvedItemUri(shelvedItem.ServerItem, shelvesetName, shelvesetOwner, UriType.Extended);
        shelvedItemUrl = linkingService.GetArtifactUrlExternal(versionControlRequestContext.RequestContext, controlIntegrationUri.ArtifactId);
      }
      return shelvedItemUrl;
    }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      versionControlRequestContext.Validation.checkIdentity(ref this.m_owner, "Shelveset.Owner", false);
      versionControlRequestContext.Validation.checkShelvesetName(this.m_name, "Shelveset.Name", false);
      versionControlRequestContext.Validation.checkComment(this.m_comment, "Shelveset.Comment", true, 1073741823);
      versionControlRequestContext.Validation.checkComment(this.m_policyOverrideComment, "Shelveset.PolicyOverrideComment", true, 2048);
      versionControlRequestContext.Validation.nullToEmpty(ref this.m_comment);
      versionControlRequestContext.Validation.check((IValidatable[]) this.m_links, "Shelveset.Links", true);
      if (this.m_links != null)
        return;
      this.m_links = Array.Empty<VersionControlLink>();
    }
  }
}
