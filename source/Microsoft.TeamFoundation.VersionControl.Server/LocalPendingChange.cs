// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LocalPendingChange
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class LocalPendingChange : ICacheable, IValidatable
  {
    private string m_targetServerItem;
    private string m_committedServerItem;
    private string m_branchFromItem;

    [XmlAttribute("tsi")]
    public string TargetServerItem
    {
      get => this.m_targetServerItem;
      set => this.m_targetServerItem = value;
    }

    [XmlAttribute("csi")]
    public string CommittedServerItem
    {
      get => this.m_committedServerItem;
      set => this.m_committedServerItem = value;
    }

    [XmlAttribute("bfi")]
    public string BranchFromItem
    {
      get => this.m_branchFromItem;
      set => this.m_branchFromItem = value;
    }

    [XmlAttribute("v")]
    public int Version { get; set; }

    [XmlAttribute("bfv")]
    public int BranchFromVersion { get; set; }

    [XmlAttribute("c")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public int PendingCommand { get; set; }

    [XmlAttribute("it")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public byte ItemType { get; set; }

    [XmlAttribute("e")]
    public int Encoding { get; set; }

    [XmlAttribute("l")]
    public byte LockStatus { get; set; }

    [XmlAttribute("iid")]
    [ClientProperty(Direction = ClientPropertySerialization.ServerToClientOnly)]
    public int ItemId { get; set; }

    [XmlAttribute("cd")]
    public DateTime CreationDate { get; set; }

    [XmlAttribute("hv")]
    [ClientProperty(Direction = ClientPropertySerialization.ServerToClientOnly)]
    public byte[] HashValue { get; set; }

    [XmlAttribute("di")]
    [ClientProperty(Direction = ClientPropertySerialization.ServerToClientOnly)]
    public int DeletionId { get; set; }

    [XmlAttribute("fl")]
    [ClientProperty(Direction = ClientPropertySerialization.ClientToServerOnly)]
    public byte Flags { get; set; }

    [XmlIgnore]
    internal bool HasMergeConflict
    {
      get => ((uint) this.Flags & 1U) > 0U;
      set
      {
        if (value)
          this.Flags |= (byte) 1;
        else
          this.Flags &= (byte) 254;
      }
    }

    [XmlIgnore]
    internal Workspace Workspace { get; set; }

    public int GetCachedSize() => 1000;

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      this.PendingCommand &= 8191;
      ChangeType pendingCommand = (ChangeType) this.PendingCommand;
      bool checkReservedCharacters = false;
      if ((pendingCommand & ChangeType.Add) != ChangeType.None || (pendingCommand & (ChangeType.Merge | ChangeType.TargetRename)) == ChangeType.TargetRename)
        checkReservedCharacters = true;
      PathLength serverPathLength = versionControlRequestContext.MaxSupportedServerPathLength;
      versionControlRequestContext.Validation.checkServerItem(ref this.m_targetServerItem, parameterName + ".TargetServerItem", false, false, !checkReservedCharacters, checkReservedCharacters, serverPathLength);
      if ((pendingCommand & ChangeType.Add) != ChangeType.None && (pendingCommand & ~(ChangeType.Add | ChangeType.Edit | ChangeType.Encoding | ChangeType.Lock | ChangeType.Property)) != ChangeType.None)
        throw new NotSupportedException(Resources.Format("ChangeTypeNotSupported", (object) this.TargetServerItem, (object) pendingCommand));
      if ((pendingCommand & ChangeType.Edit) != ChangeType.None && this.ItemType == (byte) 1)
        throw new FolderEditException(this.TargetServerItem);
      if ((pendingCommand & (ChangeType.Delete | ChangeType.Undelete)) == (ChangeType.Delete | ChangeType.Undelete))
        throw new NotSupportedException(Resources.Format("ChangeTypeNotSupported", (object) this.TargetServerItem, (object) pendingCommand));
      if ((pendingCommand & (ChangeType.Rename | ChangeType.SourceRename)) != ChangeType.None)
        throw new NotSupportedException(Resources.Format("ChangeTypeNotSupported", (object) this.TargetServerItem, (object) pendingCommand));
      if ((pendingCommand & ChangeType.Add) != ChangeType.None)
      {
        this.CommittedServerItem = (string) null;
        this.Version = 0;
      }
      else
        versionControlRequestContext.Validation.checkServerItem(ref this.m_committedServerItem, parameterName + ".CommittedServerItem", false, false, true, false, serverPathLength);
      if ((pendingCommand & ChangeType.TargetRename) != ChangeType.None && (this.Version == 0 || string.IsNullOrEmpty(this.CommittedServerItem)))
        throw new NotSupportedException(Resources.Format("RenameNotAllowedOnUncommittedItem", (object) pendingCommand, (object) this.TargetServerItem));
      if ((pendingCommand & (ChangeType.Branch | ChangeType.TargetRename)) == ChangeType.Branch)
      {
        versionControlRequestContext.Validation.checkServerItem(ref this.m_branchFromItem, parameterName + ".BranchFromItem", false, false, true, false, serverPathLength);
      }
      else
      {
        this.BranchFromItem = (string) null;
        this.BranchFromVersion = 0;
      }
      Microsoft.TeamFoundation.VersionControl.Server.ItemType itemType = (Microsoft.TeamFoundation.VersionControl.Server.ItemType) this.ItemType;
      this.ItemType = itemType != Microsoft.TeamFoundation.VersionControl.Server.ItemType.Any ? (byte) itemType : throw new ArgumentException(Resources.Format("InvalidAddEncoding", (object) ("ItemType." + this.ItemType.ToString((IFormatProvider) CultureInfo.InvariantCulture))), parameterName + ".ItemType");
      if (this.Encoding == -2)
        throw new ArgumentException(Resources.Format("InvalidAddEncoding", (object) this.Encoding));
      Microsoft.TeamFoundation.VersionControl.Server.LockStatus lockStatus = (Microsoft.TeamFoundation.VersionControl.Server.LockStatus) this.LockStatus;
      if (lockStatus == Microsoft.TeamFoundation.VersionControl.Server.LockStatus.CheckOut)
        throw new CannotTakeCheckoutLockInLocalWorkspaceException(this.TargetServerItem, this.Workspace.Name, this.Workspace.OwnerDisplayName);
      if ((this.PendingCommand & 256) == 0)
        lockStatus = Microsoft.TeamFoundation.VersionControl.Server.LockStatus.None;
      this.LockStatus = (byte) lockStatus;
      this.CreationDate = this.CreationDate.ToUniversalTime();
    }
  }
}
