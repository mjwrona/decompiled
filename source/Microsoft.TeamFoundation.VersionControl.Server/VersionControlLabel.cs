// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionControlLabel
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [RequiredClientService("VersionControlServer")]
  [CallOnDeserialization("AfterDeserialize")]
  public class VersionControlLabel : IValidatable, ICacheable
  {
    internal Guid ownerId;
    internal Item filterItem;
    private string m_comment;
    private StreamingCollection<Item> m_items;
    private DateTime m_lastModifiedDate;
    private string m_name;
    private string m_ownerName;
    private string m_ownerDisplayName;
    private ItemPathPair m_scopePair;
    private int m_labelId;

    [XmlElement("Comment")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Comment
    {
      get => this.m_comment;
      set => this.m_comment = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public StreamingCollection<Item> Items
    {
      get => this.m_items;
      set => this.m_items = value;
    }

    [XmlAttribute("date")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime LastModifiedDate
    {
      get => this.m_lastModifiedDate;
      set => this.m_lastModifiedDate = value;
    }

    [XmlAttribute("name")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    [XmlAttribute("owner")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string OwnerName
    {
      get => this.m_ownerName;
      set => this.m_ownerName = value;
    }

    [XmlAttribute("ownerdisp")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string OwnerDisplayName
    {
      get => this.m_ownerDisplayName;
      set => this.m_ownerDisplayName = value;
    }

    [XmlAttribute("owneruniq")]
    [ClientProperty(ClientVisibility.Private)]
    public string OwnerUniqueName
    {
      get => this.m_ownerName;
      set
      {
        if (!string.IsNullOrEmpty(this.m_ownerName))
          return;
        this.m_ownerName = value;
      }
    }

    [XmlAttribute("scope")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Scope
    {
      get => this.ScopePair.ProjectNamePath;
      set => this.ScopePair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair ScopePair
    {
      get => this.m_scopePair;
      set => this.m_scopePair = value;
    }

    [XmlAttribute("lid")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int LabelId
    {
      get => this.m_labelId;
      set => this.m_labelId = value;
    }

    public int GetCachedSize() => 1200;

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      versionControlRequestContext.Validation.checkLabelName(this.m_name, "Name", false, false);
      versionControlRequestContext.Validation.checkIdentity(ref this.m_ownerName, "OwnerName", true);
      PathLength serverPathLength = versionControlRequestContext.MaxSupportedServerPathLength;
      string scope = this.Scope;
      versionControlRequestContext.Validation.checkServerItem(ref scope, "Scope", true, false, true, false, serverPathLength);
      versionControlRequestContext.Validation.emptyToNull(ref scope);
      versionControlRequestContext.Validation.checkComment(this.m_comment, "Comment", true, 2048);
      if (scope == null)
        return;
      this.m_scopePair = new ItemPathPair(scope, this.ScopePair.ProjectGuidPath);
    }

    internal static List<LabelResult> DeleteLabel(
      VersionControlRequestContext versionControlRequestContext,
      string labelName,
      string labelScope)
    {
      IdentityDescriptor userContext = versionControlRequestContext.RequestContext.UserContext;
      List<VersionControlLabel> items;
      using (LabelComponent labelComponent = versionControlRequestContext.VersionControlService.GetLabelComponent(versionControlRequestContext))
        items = labelComponent.QueryLabels(labelName, ItemPathPair.FromServerItem(labelScope), (Microsoft.VisualStudio.Services.Identity.Identity) null, new ItemPathPair(), 0).GetCurrent<VersionControlLabel>().Items;
      if (items.Count == 0)
        throw new LabelNotFoundException(LabelSpec.Combine(labelName, labelScope));
      if (items.Count > 1)
        throw new LabelNotUniqueException(labelName);
      List<LabelResult> labelResults = new List<LabelResult>(1);
      VersionControlLabel versionControlLabel = items[0];
      if (!IdentityDescriptorComparer.Instance.Equals(TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, versionControlLabel.ownerId).Descriptor, userContext))
        versionControlRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.LabelOther, versionControlLabel.ScopePair);
      using (LabelComponent labelComponent = versionControlRequestContext.VersionControlService.GetLabelComponent(versionControlRequestContext))
        labelComponent.DeleteLabel(versionControlLabel.Name, versionControlLabel.ScopePair);
      LabelResult labelResult = new LabelResult(versionControlLabel.Name, versionControlLabel.Scope, versionControlLabel.LabelId, LabelResultStatus.Deleted);
      labelResults.Add(labelResult);
      LabelNotification notificationEvent = new LabelNotification(versionControlRequestContext.RequestContext.GetUserIdentity(), labelResults);
      versionControlRequestContext.RequestContext.GetService<ITeamFoundationEventService>().PublishNotification(versionControlRequestContext.RequestContext, (object) notificationEvent);
      return labelResults;
    }

    internal static List<LabelResult> LabelItem(
      VersionControlRequestContext vcRequestContext,
      Workspace localWorkspace,
      VersionControlLabel label,
      LabelItemSpec[] labelSpecs,
      LabelChildOption childOption,
      out List<Failure> failures)
    {
      IVssRequestContext requestContext = vcRequestContext.RequestContext;
      SecurityManager securityWrapper = vcRequestContext.VersionControlService.SecurityWrapper;
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = vcRequestContext.RequestContext.GetUserIdentity();
      failures = new List<Failure>();
      List<LabelResult> toReturn = new List<LabelResult>();
      ItemPathPair itemPathPair1 = ItemPathPair.FromServerItem(label.Scope);
      List<RecursiveLabelItem> itemsToLabel = new List<RecursiveLabelItem>();
      ItemPathPair existingLabelScopePair = new ItemPathPair();
      Dictionary<string, VersionControlLabel.QueryItemResult> queriedItems = new Dictionary<string, VersionControlLabel.QueryItemResult>();
      List<VersionControlLabel> childLabels = (List<VersionControlLabel>) null;
      VersionControlLabel existingLabel = (VersionControlLabel) null;
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      WorkspaceVersionSpec workspaceVersionSpec = (WorkspaceVersionSpec) null;
      string str = (string) null;
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add(nameof (label), (object) label.Name);
      ctData.Add(nameof (childOption), (object) childOption);
      if (labelSpecs != null)
      {
        ctData.Add("length", (object) labelSpecs.Length);
        ctData.Add("items", (object) ((IEnumerable<LabelItemSpec>) labelSpecs).Take<LabelItemSpec>(10).Select<LabelItemSpec, string>((Func<LabelItemSpec, string>) (x => string.Format("{0};{1};{2};{3}", (object) x.ItemSpec?.ItemPathPair.ProjectNamePath, (object) x.ItemSpec?.RecursionType, (object) x.Exclude, (object) x.Version))).ToList<string>());
      }
      ClientTrace.Publish(requestContext, nameof (LabelItem), ctData);
      requestContext.Trace(700331, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "LabelItem {0} Scope {1}", (object) label.Name, (object) label.Scope);
      if (labelSpecs == null || labelSpecs.Length == 0)
      {
        requestContext.Trace(700331, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "Can't create recursive label because labelSpecs input is null or empty");
      }
      else
      {
        foreach (LabelItemSpec labelSpec in labelSpecs)
        {
          if (labelSpec.Version is LabelVersionSpec)
          {
            requestContext.Trace(700331, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "Can't create recursive label based on another label {0}", (object) labelSpec.Version.ToString());
            goto label_71;
          }
          else if (labelSpec.ItemSpec.RecursionType != RecursionType.Full)
          {
            requestContext.Trace(700331, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "Can't create recursive label because RecursionType = {0} {1}", (object) labelSpec.ItemSpec.RecursionType, (object) labelSpec.ItemSpec.Item);
            goto label_71;
          }
          else if (Wildcard.IsWildcard(labelSpec.ItemSpec.Item))
          {
            requestContext.Trace(700331, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "Can't create recursive label because wildcard in {0}", (object) labelSpec.ItemSpec.Item);
            goto label_71;
          }
          else
          {
            if (labelSpec.Version is WorkspaceVersionSpec)
            {
              if (labelSpecs.Length > 1)
              {
                requestContext.Trace(700331, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "Can't create a recursive label based on a WorkspaceVersionSpec if there are multiple labelspecs. label {0}", (object) labelSpec.Version.ToString());
                goto label_71;
              }
              else
                workspaceVersionSpec = (WorkspaceVersionSpec) labelSpec.Version;
            }
            VersionControlLabel.QueryItemResult queryItemResult = VersionControlLabel.QueryLabelSpecItem(vcRequestContext, queriedItems, labelSpec, localWorkspace);
            if (queryItemResult.Item == null)
            {
              if (workspaceVersionSpec != null)
              {
                requestContext.Trace(700331, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "Can't create recursive label because item not found in workspace. item:{0} version:{1}", (object) labelSpec.ItemSpec.Item, (object) labelSpec.Version);
                goto label_71;
              }
              else
              {
                requestContext.Trace(700331, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "Can't create label on nonexistent item. item:{0} version:{1}", (object) labelSpec.ItemSpec.Item, (object) labelSpec.Version);
                failures.Add(new Failure((Exception) new ItemNotFoundException(vcRequestContext.RequestContext, labelSpec.ItemSpec, labelSpec.Version)));
              }
            }
            else
            {
              str = queryItemResult.Item.ServerItem;
              if (queryItemResult.Item is WorkspaceItem)
              {
                WorkspaceItem workspaceItem = (WorkspaceItem) queryItemResult.Item;
                if (workspaceItem.ServerItem != workspaceItem.CommittedServerItem)
                  str = workspaceItem.CommittedServerItem;
              }
              ItemPathPair itemPathPair2 = new ItemPathPair(str, (string) null);
              if (!securityWrapper.HasItemPermissionForAllChildren(vcRequestContext, VersionedItemPermissions.Read | VersionedItemPermissions.Label, itemPathPair2))
              {
                requestContext.Trace(700331, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "Can't create recursive label because user does not have permissions for item {0}", (object) labelSpec.ItemSpec.Item);
                goto label_71;
              }
              else
              {
                int changeset = labelSpec.Version.ToChangeset(requestContext);
                itemsToLabel.Add(new RecursiveLabelItem((!labelSpec.Exclude ? 1 : 0) != 0, str.TrimEnd('/') + "/", changeset));
                existingLabelScopePair = VersionControlLabel.computeLabelScope(existingLabelScopePair, itemPathPair2);
              }
            }
          }
        }
        if (itemsToLabel.Count == 0)
        {
          requestContext.Trace(700331, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "Can't create recursive label {0} because itemsToLabel is empty", (object) label.Name);
          return toReturn;
        }
        if (itemPathPair1.ProjectNamePath == null && existingLabelScopePair.ProjectNamePath != null)
          itemPathPair1 = new ItemPathPair(VersionControlPath.GetTeamProject(existingLabelScopePair.ProjectNamePath), existingLabelScopePair.ProjectGuidPath != null ? VersionControlPath.GetTeamProject(existingLabelScopePair.ProjectGuidPath, true) : (string) null);
        if (label.OwnerName != null)
          identity = TfvcIdentityHelper.FindIdentity(vcRequestContext.RequestContext, label.OwnerName);
        if (identity == null)
          identity = userIdentity;
        if (identity.Id.Equals(userIdentity.Id))
          vcRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(vcRequestContext, VersionedItemPermissions.Label, itemPathPair1);
        else
          vcRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(vcRequestContext, VersionedItemPermissions.LabelOther, itemPathPair1);
        childLabels = VersionControlLabel.FindChildLabels(vcRequestContext, label, itemPathPair1, out existingLabel);
        if (existingLabel != null)
        {
          requestContext.Trace(700331, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "Can't create recursive label because a label already exists with a different scope {0} {1}", (object) label.Name, (object) itemPathPair1);
        }
        else
        {
          if (childLabels.Count > 0)
          {
            if (childOption == LabelChildOption.Fail)
              throw new LabelHasChildrenException(label.Name);
            if (childOption == LabelChildOption.Merge)
            {
              requestContext.Trace(700331, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "Can't create recursive label because child labels exist and childOption = {0}", (object) childOption);
              goto label_71;
            }
            else
            {
              foreach (VersionControlLabel versionControlLabel in childLabels)
              {
                if (!userIdentity.Id.Equals(versionControlLabel.ownerId))
                  vcRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(vcRequestContext, VersionedItemPermissions.LabelOther, versionControlLabel.ScopePair);
                toReturn.Add(new LabelResult(versionControlLabel.Name, versionControlLabel.Scope, LabelResultStatus.Deleted));
              }
            }
          }
          if (VersionControlLabel.SimplifyLabelItems(requestContext, ref itemsToLabel, failures))
          {
            if (workspaceVersionSpec != null)
            {
              Workspace workspace = workspaceVersionSpec.findWorkspace(vcRequestContext);
              if (VersionControlPath.IsServerItem(str))
              {
                using (LabelComponent labelComponent = vcRequestContext.VersionControlService.GetLabelComponent(vcRequestContext))
                {
                  requestContext.Trace(700331, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "Calling CreateWorkspaceLabel {0} on {1}", (object) label.Name, (object) str);
                  labelComponent.CreateWorkspaceLabel(label.Name, itemPathPair1, identity.Id, label.Comment, workspace.OwnerId, workspace.Name, str);
                }
              }
              else
              {
                requestContext.Trace(700331, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "Calling CreateWorkspaceLabelLocal {0} on {1}", (object) label.Name, (object) str);
                using (LabelComponent labelComponent = vcRequestContext.VersionControlService.GetLabelComponent(vcRequestContext))
                  labelComponent.CreateWorkspaceLabelLocal(label.Name, itemPathPair1, identity.Id, label.Comment, workspace.OwnerId, workspace.Name, str, vcRequestContext.MaxSupportedServerPathLength);
              }
            }
            else
            {
              requestContext.Trace(700331, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "Creating recursive label {0} with scope {1}", (object) label.Name, (object) itemPathPair1.ProjectNamePath);
              using (LabelComponent labelComponent = vcRequestContext.VersionControlService.GetLabelComponent(vcRequestContext))
                labelComponent.CreateRecursiveLabel(label.Name, itemPathPair1, identity.Id, label.Comment, itemsToLabel);
            }
            toReturn.Add(new LabelResult(label.Name, itemPathPair1.ProjectNamePath, LabelResultStatus.Created));
            VersionControlLabel.SendLabelNotification(vcRequestContext.RequestContext, userIdentity, localWorkspace, toReturn);
            return toReturn;
          }
        }
      }
label_71:
      return VersionControlLabel.CreateLabelOld(vcRequestContext, localWorkspace, label, labelSpecs, childOption, childLabels, existingLabel, queriedItems, out failures);
    }

    private static List<LabelResult> CreateLabelOld(
      VersionControlRequestContext vcRequestContext,
      Workspace localWorkspace,
      VersionControlLabel label,
      LabelItemSpec[] labelSpecs,
      LabelChildOption childOption,
      List<VersionControlLabel> childLabels,
      VersionControlLabel existingLabel,
      Dictionary<string, VersionControlLabel.QueryItemResult> queriedItems,
      out List<Failure> failures)
    {
      vcRequestContext.RequestContext.Trace(700334, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "CreateLabelOld {0}", (object) label.Name);
      List<LabelResult> toReturn = new List<LabelResult>();
      failures = new List<Failure>();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = vcRequestContext.RequestContext.GetUserIdentity();
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (label.OwnerName != null)
        identity = TfvcIdentityHelper.FindIdentity(vcRequestContext.RequestContext, label.OwnerName);
      List<Microsoft.TeamFoundation.VersionControl.Server.LabelItem> items = (List<Microsoft.TeamFoundation.VersionControl.Server.LabelItem>) null;
      ItemPathPair existingLabelScopePair = new ItemPathPair();
      ItemPathPair itemPathPair1 = ItemPathPair.FromServerItem(label.Scope);
      bool flag1 = true;
      string str = (string) null;
      Guid guid;
      int changeset;
      while (true)
      {
        if (labelSpecs != null)
        {
          List<LabelItemSpec> labelItemSpecList = new List<LabelItemSpec>();
          List<LabelItemSpec> exclusionList = new List<LabelItemSpec>();
          foreach (LabelItemSpec labelSpec in labelSpecs)
          {
            if (labelSpec.Exclude)
            {
              if (!labelSpec.ItemSpec.isServerItem)
                throw new ServerItemRequiredException(labelSpec.ItemSpec.Item);
              exclusionList.Add(labelSpec);
            }
            else
              labelItemSpecList.Add(labelSpec);
          }
          if (!flag1 || 1 == labelSpecs.Length && !labelSpecs[0].Exclude && RecursionType.Full == labelSpecs[0].ItemSpec.RecursionType && !(labelSpecs[0].Version is LabelVersionSpec))
          {
            bool flag2 = false;
            foreach (LabelItemSpec labelSpec in labelItemSpecList)
            {
              Item obj1 = (Item) null;
              bool flag3 = false;
              string queryPath;
              string filePattern;
              if (flag1)
              {
                if (!Wildcard.IsWildcard(labelSpec.ItemSpec.Item))
                {
                  VersionControlLabel.QueryItemResult queryItemResult = VersionControlLabel.QueryLabelSpecItem(vcRequestContext, queriedItems, labelSpec, localWorkspace);
                  queryPath = queryItemResult.QueryPath;
                  filePattern = queryItemResult.FilePattern;
                  Item obj2 = queryItemResult.Item;
                  ItemPathPair itemPathPair2;
                  if (!VersionControlPath.IsServerItem(queryPath))
                  {
                    if (labelSpec.Version is WorkspaceVersionSpec)
                      itemPathPair2 = localWorkspace.LocalToServerItemPathPair(vcRequestContext.RequestContext, queryPath, false);
                    else
                      goto label_138;
                  }
                  else
                    itemPathPair2 = ItemPathPair.FromServerItem(queryPath);
                  if (filePattern == null)
                  {
                    if (obj2 != null)
                    {
                      if (obj2.ItemType == ItemType.Folder)
                      {
                        if (obj2.ChangesetId != 0)
                        {
                          if (vcRequestContext.VersionControlService.SecurityWrapper.HasItemPermissionForAllChildren(vcRequestContext, VersionedItemPermissions.Read | VersionedItemPermissions.Label, itemPathPair2))
                          {
                            str = queryPath;
                            existingLabelScopePair = VersionControlLabel.computeLabelScope(existingLabelScopePair, itemPathPair2);
                            flag3 = true;
                            flag2 = true;
                          }
                          else
                            goto label_138;
                        }
                        else
                          goto label_138;
                      }
                      else
                        goto label_138;
                    }
                    else
                      goto label_138;
                  }
                  else
                    goto label_138;
                }
                else
                  goto label_138;
              }
              else
              {
                using (VersionedItemComponent versionedItemComponent = vcRequestContext.VersionControlService.GetVersionedItemComponent(vcRequestContext))
                {
                  using (vcRequestContext.RequestContext.AcquireExemptionLock())
                  {
                    labelSpec.Version.QueryItems(vcRequestContext, labelSpec.ItemSpec, localWorkspace, versionedItemComponent, DeletedState.Any, ItemType.Any, out queryPath, out filePattern, 8);
                    while (labelSpec.Version.TryGetNextItem(out obj1))
                    {
                      if (labelSpec.ItemSpec.postMatch(obj1.ServerItem) && obj1.HasPermission(vcRequestContext, VersionedItemPermissions.Read))
                      {
                        flag3 = true;
                        if (obj1.ChangesetId == 0)
                          failures.Add(new Failure((Exception) new LabelPendingAddException(obj1.ServerItem, LabelSpec.Combine(label.Name, label.Scope))));
                        else if (!VersionControlLabel.IsInExclusionList(obj1, exclusionList))
                        {
                          if (!obj1.HasPermission(vcRequestContext, VersionedItemPermissions.Label))
                          {
                            List<Failure> failureList = failures;
                            guid = vcRequestContext.RequestContext.GetUserId();
                            Failure failure = new Failure((Exception) new ResourceAccessException(guid.ToString(), "Label", obj1.ServerItem));
                            failureList.Add(failure);
                          }
                          else
                          {
                            items.Add(new Microsoft.TeamFoundation.VersionControl.Server.LabelItem(obj1.ItemId, obj1.ServerItem, obj1.ChangesetId, false));
                            existingLabelScopePair = VersionControlLabel.computeLabelScope(existingLabelScopePair, obj1.ItemPathPair);
                            flag2 = true;
                          }
                        }
                      }
                    }
                  }
                }
              }
              if (!flag3)
                failures.Add(new Failure((Exception) new ItemNotFoundException(vcRequestContext.RequestContext, labelSpec.ItemSpec, labelSpec.Version)));
            }
            if (!flag2 && labelSpecs.Length != 0)
              break;
          }
          else
            goto label_138;
        }
        if (itemPathPair1.ProjectNamePath == null && existingLabelScopePair.ProjectNamePath != null)
          itemPathPair1 = new ItemPathPair(VersionControlPath.GetTeamProject(existingLabelScopePair.ProjectNamePath), existingLabelScopePair.ProjectGuidPath != null ? VersionControlPath.GetTeamProject(existingLabelScopePair.ProjectGuidPath, true) : (string) null);
        if (childLabels == null)
          childLabels = VersionControlLabel.FindChildLabels(vcRequestContext, label, itemPathPair1, out existingLabel);
        if (!flag1 || existingLabel == null)
        {
          if (childLabels.Count > 0)
          {
            if (childOption != LabelChildOption.Fail)
            {
              if (LabelChildOption.Merge == childOption)
              {
                if (!flag1)
                {
                  using (VersionedItemComponent versionedItemComponent = vcRequestContext.VersionControlService.GetVersionedItemComponent(vcRequestContext))
                  {
                    foreach (VersionControlLabel childLabel in childLabels)
                    {
                      using (ResultCollection resultCollection = versionedItemComponent.QueryLabelItems(childLabel.Name, childLabel.ScopePair, ItemPathPair.FromServerItem("$/"), RecursionType.Full, ItemType.Any, DeletedState.Any, 8))
                      {
                        resultCollection.NextResult();
                        foreach (Item obj in resultCollection.GetCurrent<Item>())
                          items.Add(new Microsoft.TeamFoundation.VersionControl.Server.LabelItem(obj.ItemId, obj.ServerItem, obj.ChangesetId, true));
                      }
                    }
                  }
                }
                else
                  goto label_138;
              }
              foreach (VersionControlLabel childLabel in childLabels)
              {
                guid = userIdentity.Id;
                if (!guid.Equals(childLabel.ownerId))
                  vcRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(vcRequestContext, VersionedItemPermissions.LabelOther, childLabel.ScopePair);
              }
            }
            else
              goto label_57;
          }
          if (existingLabel == null)
          {
            if (labelSpecs != null && labelSpecs.Length != 0)
            {
              if (flag1 || items.Count != 0)
              {
                itemPathPair1 = string.IsNullOrEmpty(itemPathPair1.ProjectNamePath) ? ItemPathPair.FromServerItem("$/") : itemPathPair1;
                if (identity == null)
                  identity = userIdentity;
                guid = identity.Id;
                if (guid.Equals(userIdentity.Id))
                  vcRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(vcRequestContext, VersionedItemPermissions.Label, itemPathPair1);
                else
                  vcRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(vcRequestContext, VersionedItemPermissions.LabelOther, itemPathPair1);
                if (flag1)
                {
                  if (!(labelSpecs[0].Version is WorkspaceVersionSpec))
                  {
                    if (VersionControlPath.IsServerItem(str))
                    {
                      changeset = labelSpecs[0].Version.ToChangeset(vcRequestContext.RequestContext);
                      if (VersionSpec.UnknownChangeset != changeset)
                        goto label_111;
                    }
                  }
                  else
                    goto label_98;
                }
                else
                  goto label_116;
              }
              else
                goto label_90;
            }
            else
              goto label_88;
          }
          else
            goto label_126;
        }
label_138:
        flag1 = false;
        str = (string) null;
        items = new List<Microsoft.TeamFoundation.VersionControl.Server.LabelItem>();
        existingLabelScopePair = new ItemPathPair();
        itemPathPair1 = label.ScopePair;
        failures.Clear();
        toReturn.Clear();
      }
      return toReturn;
label_57:
      throw new LabelHasChildrenException(label.Name);
label_88:
      failures.Add(new Failure((Exception) new LabelNotFoundException(LabelSpec.Combine(label.Name, itemPathPair1.ProjectNamePath))));
      return toReturn;
label_90:
      return toReturn;
label_98:
      Workspace workspace = ((WorkspaceVersionSpec) labelSpecs[0].Version).findWorkspace(vcRequestContext);
      if (VersionControlPath.IsServerItem(str))
      {
        using (LabelComponent labelComponent = vcRequestContext.VersionControlService.GetLabelComponent(vcRequestContext))
        {
          labelComponent.CreateWorkspaceLabel(label.Name, itemPathPair1, identity.Id, label.Comment, workspace.OwnerId, workspace.Name, str);
          goto label_121;
        }
      }
      else
      {
        using (LabelComponent labelComponent = vcRequestContext.VersionControlService.GetLabelComponent(vcRequestContext))
        {
          labelComponent.CreateWorkspaceLabelLocal(label.Name, itemPathPair1, identity.Id, label.Comment, workspace.OwnerId, workspace.Name, str, vcRequestContext.MaxSupportedServerPathLength);
          goto label_121;
        }
      }
label_111:
      using (LabelComponent labelComponent = vcRequestContext.VersionControlService.GetLabelComponent(vcRequestContext))
      {
        labelComponent.CreateRecursiveLabel(label.Name, itemPathPair1, identity.Id, label.Comment, new List<RecursiveLabelItem>()
        {
          new RecursiveLabelItem(true, str, changeset)
        });
        goto label_121;
      }
label_116:
      using (LabelComponent labelComponent = vcRequestContext.VersionControlService.GetLabelComponent(vcRequestContext))
        VersionControlLabel.ProcessFailures(labelComponent.CreateLabel(label.Name, itemPathPair1, identity.Id, label.Comment, items), failures);
label_121:
      toReturn.Add(new LabelResult(label.Name, itemPathPair1.ProjectNamePath, LabelResultStatus.Created));
      using (List<VersionControlLabel>.Enumerator enumerator = childLabels.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          VersionControlLabel current = enumerator.Current;
          toReturn.Add(new LabelResult(current.Name, current.Scope, LabelResultStatus.Deleted));
        }
        goto label_137;
      }
label_126:
      if (identity != null)
      {
        guid = identity.Id;
        if (!guid.Equals(existingLabel.ownerId))
          throw new LabelOwnerChangeException();
      }
      if (!existingLabel.ownerId.Equals(userIdentity.Id))
        vcRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(vcRequestContext, VersionedItemPermissions.LabelOther, existingLabel.ScopePair);
      using (LabelComponent labelComponent = vcRequestContext.VersionControlService.GetLabelComponent(vcRequestContext))
        VersionControlLabel.ProcessFailures(labelComponent.UpdateLabel(label.Name, existingLabel.ScopePair, identity == null ? Guid.Empty : identity.Id, label.Comment, items), failures);
      toReturn.Add(new LabelResult(label.Name, existingLabel.Scope, LabelResultStatus.Updated));
label_137:
      VersionControlLabel.SendLabelNotification(vcRequestContext.RequestContext, userIdentity, localWorkspace, toReturn);
      return toReturn;
    }

    private static void SendLabelNotification(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity caller,
      Workspace localWorkspace,
      List<LabelResult> toReturn)
    {
      LabelNotification notificationEvent = new LabelNotification(caller, localWorkspace?.Name, localWorkspace?.Owner, localWorkspace?.Computer, toReturn);
      requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) notificationEvent);
    }

    private static bool SimplifyLabelItems(
      IVssRequestContext requestContext,
      ref List<RecursiveLabelItem> itemsToLabel,
      List<Failure> failures)
    {
      List<RecursiveLabelItem> recursiveLabelItemList = new List<RecursiveLabelItem>();
      itemsToLabel.Sort((Comparison<RecursiveLabelItem>) ((x, y) => x.ServerItem.Length - y.ServerItem.Length));
      foreach (RecursiveLabelItem recursiveLabelItem1 in itemsToLabel)
      {
        RecursiveLabelItem item = recursiveLabelItem1;
        int num = 0;
        int version = 0;
        foreach (RecursiveLabelItem recursiveLabelItem2 in recursiveLabelItemList.FindAll((Predicate<RecursiveLabelItem>) (x => item.ServerItem.StartsWith(x.ServerItem))))
        {
          if (string.Equals(recursiveLabelItem2.ServerItem, item.ServerItem, StringComparison.OrdinalIgnoreCase))
            throw new DuplicateServerItemException(item.ServerItem);
          version = recursiveLabelItem2.VersionFrom;
          num += recursiveLabelItem2.Active ? 1 : -1;
        }
        if (version != 0 && item.VersionFrom != version)
        {
          requestContext.Trace(700331, TraceLevel.Verbose, TraceArea.General, TraceLayer.BusinessLogic, "Can't create a recursive label because versions differ. {0} != {1}", (object) version, (object) item.VersionFrom);
          return false;
        }
        if (num == 1 != item.Active)
          recursiveLabelItemList.Add(item);
        else
          failures.Add(new Failure((Exception) new DuplicateItemFoundException(item.ServerItem.TrimEnd('/'), version)));
      }
      itemsToLabel = recursiveLabelItemList;
      return true;
    }

    private static List<VersionControlLabel> FindChildLabels(
      VersionControlRequestContext vcRequestContext,
      VersionControlLabel label,
      ItemPathPair labelScope,
      out VersionControlLabel existingLabel)
    {
      existingLabel = (VersionControlLabel) null;
      List<VersionControlLabel> childLabels = new List<VersionControlLabel>();
      using (LabelComponent labelComponent = vcRequestContext.VersionControlService.GetLabelComponent(vcRequestContext))
      {
        foreach (VersionControlLabel versionControlLabel in labelComponent.QueryLabels(label.Name, labelScope, (Microsoft.VisualStudio.Services.Identity.Identity) null, new ItemPathPair(), 0).GetCurrent<VersionControlLabel>())
        {
          if (labelScope.ProjectNamePath == null || VersionControlPath.IsSubItem(labelScope.ProjectNamePath, versionControlLabel.Scope))
          {
            if (existingLabel != null)
              throw new LabelNotUniqueException(label.Name);
            existingLabel = versionControlLabel;
          }
          else if (labelScope.ProjectNamePath != null && VersionControlPath.IsSubItem(versionControlLabel.Scope, labelScope.ProjectNamePath))
            childLabels.Add(versionControlLabel);
        }
      }
      return childLabels;
    }

    private static VersionControlLabel.QueryItemResult QueryLabelSpecItem(
      VersionControlRequestContext vcRequestContext,
      Dictionary<string, VersionControlLabel.QueryItemResult> queriedItems,
      LabelItemSpec labelSpec,
      Workspace localWorkspace)
    {
      string key = labelSpec.ItemSpec.Item;
      VersionControlLabel.QueryItemResult queryItemResult;
      if (!queriedItems.TryGetValue(key, out queryItemResult))
      {
        using (VersionedItemComponent versionedItemComponent = vcRequestContext.VersionControlService.GetVersionedItemComponent(vcRequestContext))
        {
          using (vcRequestContext.RequestContext.AcquireExemptionLock())
          {
            labelSpec.Version.QueryItems(vcRequestContext, new ItemSpec(key, RecursionType.None), localWorkspace, versionedItemComponent, DeletedState.Any, ItemType.Any, out queryItemResult.QueryPath, out queryItemResult.FilePattern, 8);
            Item obj;
            while (labelSpec.Version.TryGetNextItem(out obj))
              queryItemResult.Item = obj;
          }
        }
        queriedItems.Add(key, queryItemResult);
      }
      return queryItemResult;
    }

    private static void ProcessFailures(ResultCollection rc, List<Failure> failures)
    {
      ObjectBinder<Failure> current = rc.GetCurrent<Failure>();
      while (current.MoveNext())
        failures.Add(current.Current);
    }

    private static bool IsInExclusionList(Item item, List<LabelItemSpec> exclusionList)
    {
      foreach (LabelItemSpec exclusion in exclusionList)
      {
        switch (exclusion.ItemSpec.RecursionType)
        {
          case RecursionType.None:
            if (VersionControlPath.Equals(exclusion.ItemSpec.Item, item.ServerItem))
              return true;
            continue;
          case RecursionType.OneLevel:
            if (VersionControlPath.Equals(exclusion.ItemSpec.Item, item.ServerItem) || VersionControlPath.IsImmediateChild(item.ServerItem, exclusion.ItemSpec.Item))
              return true;
            continue;
          case RecursionType.Full:
            if (VersionControlPath.IsSubItem(item.ServerItem, exclusion.ItemSpec.Item))
              return true;
            continue;
          default:
            continue;
        }
      }
      return false;
    }

    internal static VersionControlLabel FindLabelByLabelId(
      VersionControlRequestContext versionControlRequestContext,
      int labelId)
    {
      VersionControlLabel labelByLabelId = (VersionControlLabel) null;
      using (LabelComponent labelComponent = versionControlRequestContext.VersionControlService.GetLabelComponent(versionControlRequestContext))
      {
        ObjectBinder<VersionControlLabel> current = labelComponent.FindLabelByLabelId(labelId).GetCurrent<VersionControlLabel>();
        if (current.MoveNext())
          labelByLabelId = current.Current;
      }
      if (labelByLabelId == null)
        throw new LabelNotFoundException(labelId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.Read, labelByLabelId.ScopePair);
      string identityName;
      string displayName;
      versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentityNames(versionControlRequestContext.RequestContext, labelByLabelId.ownerId, out identityName, out displayName);
      labelByLabelId.OwnerName = identityName;
      labelByLabelId.OwnerDisplayName = displayName;
      return labelByLabelId;
    }

    internal static List<LabelResult> UnlabelItem(
      VersionControlRequestContext versionControlRequestContext,
      Workspace localWorkspace,
      string labelName,
      string labelScope,
      ItemSpec[] itemSpecs,
      VersionSpec version,
      List<Failure> failures)
    {
      ItemPathPair labelScope1 = ItemPathPair.FromServerItem(labelScope);
      ItemPathPair existingLabelScopePair = new ItemPathPair();
      List<LabelResult> labelResults = new List<LabelResult>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentity(versionControlRequestContext.RequestContext);
      List<Microsoft.TeamFoundation.VersionControl.Server.LabelItem> items = new List<Microsoft.TeamFoundation.VersionControl.Server.LabelItem>();
      using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
      {
        foreach (ItemSpec itemSpec in itemSpecs)
        {
          int num = 0;
          version.QueryItems(versionControlRequestContext, itemSpec, localWorkspace, versionedItemComponent, DeletedState.Any, ItemType.Any, out string _, out string _, 8);
          Item obj;
          while (version.TryGetNextItem(out obj))
          {
            using (versionControlRequestContext.RequestContext.AcquireExemptionLock())
            {
              if (itemSpec.postMatch(obj.ServerItem))
              {
                if (obj.HasPermission(versionControlRequestContext, VersionedItemPermissions.Read))
                {
                  ++num;
                  if (!obj.HasPermission(versionControlRequestContext, VersionedItemPermissions.Label))
                  {
                    failures.Add(new Failure((Exception) new ResourceAccessException(versionControlRequestContext.RequestContext.GetUserId().ToString(), "Label", obj.ServerItem)));
                  }
                  else
                  {
                    obj.ChangesetId = 0;
                    items.Add(new Microsoft.TeamFoundation.VersionControl.Server.LabelItem(obj.ItemId, obj.ServerItem, obj.ChangesetId, false));
                    if (labelScope == null)
                      existingLabelScopePair = VersionControlLabel.computeLabelScope(existingLabelScopePair, obj.ItemPathPair);
                  }
                }
              }
            }
          }
          if (num == 0)
            failures.Add(new Failure((Exception) new ItemNotFoundException(versionControlRequestContext.RequestContext, itemSpec, version)));
        }
      }
      if (items.Count == 0)
        return labelResults;
      if (labelScope == null)
      {
        labelScope = existingLabelScopePair.ProjectNamePath;
        labelScope1 = existingLabelScopePair;
      }
      using (LabelComponent labelComponent = versionControlRequestContext.VersionControlService.GetLabelComponent(versionControlRequestContext))
      {
        VersionControlLabel versionControlLabel1 = (VersionControlLabel) null;
        foreach (VersionControlLabel versionControlLabel2 in labelComponent.QueryLabels(labelName, labelScope1, (Microsoft.VisualStudio.Services.Identity.Identity) null, new ItemPathPair(), 0).GetCurrent<VersionControlLabel>().Items)
        {
          if (VersionControlPath.IsSubItem(labelScope, versionControlLabel2.Scope))
            versionControlLabel1 = versionControlLabel2;
        }
        if (versionControlLabel1 == null)
          throw new LabelNotFoundException(LabelSpec.Combine(labelName, labelScope));
        if (!versionControlLabel1.ownerId.Equals(identity.Id))
          versionControlRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.LabelOther, versionControlLabel1.ScopePair);
        labelComponent.UpdateLabel(labelName, versionControlLabel1.ScopePair, Guid.Empty, (string) null, items);
        labelResults.Add(new LabelResult(labelName, versionControlLabel1.Scope, LabelResultStatus.Updated));
      }
      string workspaceName = (string) null;
      Microsoft.VisualStudio.Services.Identity.Identity owner = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      string computerName = (string) null;
      if (localWorkspace != null)
      {
        workspaceName = localWorkspace.Name;
        owner = localWorkspace.Owner;
        computerName = localWorkspace.Computer;
      }
      LabelNotification notificationEvent = new LabelNotification(versionControlRequestContext.RequestContext.GetUserIdentity(), workspaceName, owner, computerName, labelResults);
      versionControlRequestContext.RequestContext.GetService<ITeamFoundationEventService>().PublishNotification(versionControlRequestContext.RequestContext, (object) notificationEvent);
      return labelResults;
    }

    private static ItemPathPair computeLabelScope(
      ItemPathPair existingLabelScopePair,
      ItemPathPair itemPathPair)
    {
      if (existingLabelScopePair.ProjectNamePath == null)
        return itemPathPair;
      string str1 = existingLabelScopePair.ProjectNamePath;
      string str2 = existingLabelScopePair.ProjectGuidPath;
      while (!VersionControlPath.IsSubItem(itemPathPair.ProjectNamePath, str1))
        str1 = VersionControlPath.GetFolderName(str1);
      if (itemPathPair.ProjectGuidPath != null && str2 != null)
      {
        while (!VersionControlPath.IsSubItem(itemPathPair.ProjectGuidPath, str2))
          str2 = VersionControlPath.GetFolderName(str2);
      }
      return new ItemPathPair(str1, str2);
    }

    private struct QueryItemResult
    {
      public string QueryPath;
      public string FilePattern;
      public Item Item;
    }
  }
}
