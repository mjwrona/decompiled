// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkspaceVersionSpec
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [CallOnDeserialization("AfterDeserialize")]
  public class WorkspaceVersionSpec : VersionSpec
  {
    private string m_name;
    private string m_ownerName;
    private string m_ownerDisplayName;

    public WorkspaceVersionSpec()
    {
    }

    internal WorkspaceVersionSpec(string name, string owner, string ownerDisplayName)
    {
      this.m_name = name;
      this.m_ownerName = owner;
      this.m_ownerDisplayName = ownerDisplayName;
    }

    internal WorkspaceVersionSpec(Workspace workspace)
    {
      this.m_name = workspace.Name;
      this.m_ownerName = workspace.OwnerName;
      this.m_ownerDisplayName = workspace.OwnerDisplayName;
    }

    [XmlAttribute("name")]
    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    [XmlAttribute("owner")]
    public string OwnerName
    {
      get => this.m_ownerName;
      set => this.m_ownerName = value;
    }

    [XmlAttribute("ownerUniq")]
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

    [XmlAttribute("ownerDisp")]
    public string OwnerDisplayName
    {
      get => !string.IsNullOrEmpty(this.m_ownerDisplayName) ? this.m_ownerDisplayName : this.m_ownerName;
      set => this.m_ownerDisplayName = value;
    }

    public override int GetHashCode() => this.Name.GetHashCode() + this.OwnerName.GetHashCode();

    public override int CompareTo(VersionSpec other)
    {
      int num = base.CompareTo(other);
      if (num == 0)
      {
        WorkspaceVersionSpec workspaceVersionSpec = (WorkspaceVersionSpec) other;
        num = TFStringComparer.WorkspaceName.Compare(this.Name, workspaceVersionSpec.Name);
        if (num == 0)
          num = VssStringComparer.IdentityName.Compare(this.OwnerName, workspaceVersionSpec.OwnerName);
      }
      return num;
    }

    public override string ToString() => WorkspaceSpec.Combine(this.Name, this.OwnerDisplayName);

    internal Workspace findWorkspace(
      VersionControlRequestContext versionControlRequestContext)
    {
      return Workspace.FindWorkspace(versionControlRequestContext, this.OwnerName, this.Name, true);
    }

    internal override void QueryItems(
      VersionControlRequestContext versionControlRequestContext,
      ItemSpec itemSpec,
      Workspace localWorkspace,
      VersionedItemComponent db,
      DeletedState deletedState,
      ItemType itemType,
      IList items,
      out string queryPath,
      out string filePattern,
      int options)
    {
      this.QueryItems(versionControlRequestContext, itemSpec, localWorkspace, db, deletedState, itemType, out queryPath, out filePattern, options);
      Item obj;
      while (this.TryGetNextItem(out obj))
        items.Add((object) obj);
    }

    internal override void QueryItems(
      VersionControlRequestContext versionControlRequestContext,
      ItemSpec itemSpec,
      Workspace localWorkspace,
      VersionedItemComponent db,
      DeletedState deletedState,
      ItemType itemType,
      out string queryPath,
      out string filePattern,
      int options)
    {
      bool deleted = deletedState != 0;
      Workspace workspace = this.findWorkspace(versionControlRequestContext);
      if (this.m_useMappings)
        this.m_rc = db.QueryMappedItems(localWorkspace, itemSpec.ItemPathPair, itemSpec.RecursionType, options);
      else if (itemSpec.isServerItem || localWorkspace.Id != workspace.Id)
        this.m_rc = db.QueryWorkspaceItems(workspace, itemSpec.toServerItem(versionControlRequestContext.RequestContext, localWorkspace, false), itemSpec.RecursionType, itemType, deleted, options);
      else
        this.m_rc = db.QueryWorkspaceItemsLocal(workspace, itemSpec.Item, itemSpec.RecursionType, itemType, deleted, options, versionControlRequestContext.MaxSupportedServerPathLength);
      DetermineItemTypeColumns current = (DetermineItemTypeColumns) this.m_rc.GetCurrent<DeterminedItem>();
      current.MoveNext();
      queryPath = current.Current.QueryPath;
      filePattern = current.Current.FilePattern;
      this.m_rc.NextResult();
    }

    internal override bool TryGetNextItem(out Item item)
    {
      bool nextItem = false;
      item = (Item) null;
      if (this.m_rc != null)
      {
        if (this.m_useMappings)
        {
          nextItem = this.m_rc.GetCurrent<MappedItem>().MoveNext();
          if (nextItem)
            item = (Item) this.m_rc.GetCurrent<MappedItem>().Current;
        }
        else
        {
          nextItem = this.m_rc.GetCurrent<WorkspaceItem>().MoveNext();
          if (nextItem)
            item = (Item) this.m_rc.GetCurrent<WorkspaceItem>().Current;
        }
        if (!nextItem)
        {
          this.m_rc.Dispose();
          this.m_rc = (ResultCollection) null;
        }
      }
      return nextItem;
    }

    public override string ToDBString(IVssRequestContext requestContext)
    {
      Workspace workspace = this.findWorkspace(requestContext.GetService<TeamFoundationVersionControlService>().GetVersionControlRequestContext(requestContext));
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "W{0};{1}", (object) workspace.OwnerId, (object) workspace.Name);
    }

    internal override void Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      versionControlRequestContext.Validation.checkWorkspaceName(this.m_name, parameterName, false);
      if (this.m_ownerDisplayName == ".")
        this.m_ownerDisplayName = versionControlRequestContext.RequestContext.GetRequestingUserDisplayName();
      versionControlRequestContext.Validation.checkIdentity(ref this.m_ownerName, parameterName, false);
    }
  }
}
