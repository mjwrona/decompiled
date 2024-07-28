// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkspaceItem
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [RequiredClientService("VersionControlServer")]
  public class WorkspaceItem : Item
  {
    internal Microsoft.TeamFoundation.VersionControl.Server.ChangeType PendingChange;
    internal Microsoft.TeamFoundation.VersionControl.Server.ChangeType RecursivePendingChange;

    [XmlAttribute("li")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string LocalItem { get; set; }

    [XmlAttribute("ct")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public int ChangeType
    {
      get => (int) this.PendingChange;
      set => this.PendingChange = (Microsoft.TeamFoundation.VersionControl.Server.ChangeType) value;
    }

    [XmlAttribute("rct")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public int RecursiveChangeType
    {
      get => (int) this.RecursivePendingChange;
      set => this.RecursivePendingChange = (Microsoft.TeamFoundation.VersionControl.Server.ChangeType) value;
    }

    [XmlAttribute("csi")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string CommittedServerItem
    {
      get => this.CommittedItemPathPair.ProjectNamePath;
      set => this.CommittedItemPathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair CommittedItemPathPair { get; set; }

    internal override bool HasPermission(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions required)
    {
      if (!base.HasPermission(versionControlRequestContext, required))
        return false;
      return this.CommittedServerItem == null || VersionControlPath.Equals(this.ServerItem, this.CommittedServerItem) || versionControlRequestContext.VersionControlService.SecurityWrapper.HasItemPermission(versionControlRequestContext, required, this.CommittedItemPathPair);
    }
  }
}
