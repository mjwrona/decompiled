// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemIdentifier
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class ItemIdentifier : IValidatable
  {
    private ItemPathPair m_itemPathPair;
    private bool m_isServerItem;
    private int m_deletionId;
    private VersionSpec m_version;
    private ItemValidationOptions m_validateOptions;
    private VersionSpecValidationOptions m_versionOptions = VersionSpecValidationOptions.All;
    private ChangeType m_changeType;

    [XmlAttribute("it")]
    public string Item
    {
      get => this.ItemPathPair.ProjectNamePath;
      set => this.ItemPathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair ItemPathPair
    {
      get => this.m_itemPathPair;
      set
      {
        this.m_itemPathPair = value;
        if (this.m_itemPathPair.ProjectNamePath != null)
          this.m_isServerItem = VersionControlPath.IsServerItem(this.m_itemPathPair.ProjectNamePath);
        else
          this.m_isServerItem = false;
      }
    }

    public VersionSpec Version
    {
      get => this.m_version;
      set => this.m_version = value;
    }

    [XmlAttribute("di")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public int DeletionId
    {
      get => this.m_deletionId;
      set => this.m_deletionId = value;
    }

    [XmlAttribute("ctype")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public int ChangeTypeEx
    {
      get => (int) this.m_changeType;
      set
      {
      }
    }

    internal ChangeType ChangeType
    {
      set => this.m_changeType = value;
    }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      string projectNamePath = this.Item != null && this.Item.Length != 0 ? this.Item : throw new ItemRequiredException();
      if ((this.m_validateOptions & ItemValidationOptions.DisallowLocalItem) == ItemValidationOptions.DisallowLocalItem)
      {
        PathLength serverPathLength = versionControlRequestContext.MaxSupportedServerPathLength;
        versionControlRequestContext.Validation.checkServerItem(ref projectNamePath, "Item", false, (this.m_validateOptions & ItemValidationOptions.AllowWildCards) == ItemValidationOptions.AllowWildCards, (this.m_validateOptions & ItemValidationOptions.Allow8Dot3Paths) == ItemValidationOptions.Allow8Dot3Paths, (this.m_validateOptions & ItemValidationOptions.CheckReservedCharacters) == ItemValidationOptions.CheckReservedCharacters, serverPathLength);
      }
      else if ((this.m_validateOptions & ItemValidationOptions.DisallowServerItem) == ItemValidationOptions.DisallowServerItem)
      {
        versionControlRequestContext.Validation.checkLocalItem(projectNamePath, "Item", false, (this.m_validateOptions & ItemValidationOptions.AllowWildCards) == ItemValidationOptions.AllowWildCards, (this.m_validateOptions & ItemValidationOptions.Allow8Dot3Paths) == ItemValidationOptions.Allow8Dot3Paths, (this.m_validateOptions & ItemValidationOptions.CheckReservedCharacters) == ItemValidationOptions.CheckReservedCharacters);
      }
      else
      {
        PathLength serverPathLength = versionControlRequestContext.MaxSupportedServerPathLength;
        versionControlRequestContext.Validation.checkItem(ref projectNamePath, "Item", false, (this.m_validateOptions & ItemValidationOptions.AllowWildCards) == ItemValidationOptions.AllowWildCards, (this.m_validateOptions & ItemValidationOptions.Allow8Dot3Paths) == ItemValidationOptions.Allow8Dot3Paths, (this.m_validateOptions & ItemValidationOptions.CheckReservedCharacters) == ItemValidationOptions.CheckReservedCharacters, serverPathLength);
      }
      if ((this.m_validateOptions & ItemValidationOptions.DisallowRoot) == ItemValidationOptions.DisallowRoot && VersionControlPath.IsRootFolder(this.Item))
        throw new CannotChangeRootFolderException();
      if (this.m_version == null)
        this.m_version = (VersionSpec) new LatestVersionSpec();
      else if ((!(this.m_version is DateVersionSpec) || (this.m_versionOptions & VersionSpecValidationOptions.Date) == VersionSpecValidationOptions.None) && (!(this.m_version is LabelVersionSpec) || (this.m_versionOptions & VersionSpecValidationOptions.Label) == VersionSpecValidationOptions.None) && (!(this.m_version is WorkspaceVersionSpec) || (this.m_versionOptions & VersionSpecValidationOptions.Workspace) == VersionSpecValidationOptions.None) && (!(this.m_version is ChangesetVersionSpec) || (this.m_versionOptions & VersionSpecValidationOptions.Changeset) == VersionSpecValidationOptions.None) && (!(this.m_version is LatestVersionSpec) || (this.m_versionOptions & VersionSpecValidationOptions.Latest) == VersionSpecValidationOptions.None))
        throw new InvalidVersionSpecForOperationException(this.m_version.GetType().Name, this.m_versionOptions.ToString());
      this.ItemPathPair = new ItemPathPair(projectNamePath, this.ItemPathPair.ProjectGuidPath);
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.Item);
      if (this.m_version != null)
      {
        stringBuilder.Append(";");
        stringBuilder.Append(this.m_version.ToString());
      }
      if (this.m_deletionId != 0)
      {
        stringBuilder.Append(";X");
        stringBuilder.Append(this.m_deletionId);
      }
      return stringBuilder.ToString();
    }

    internal void SetValidationOptions(
      ItemValidationOptions options,
      VersionSpecValidationOptions versionOptions)
    {
      this.m_validateOptions = options;
      this.m_versionOptions = versionOptions;
    }
  }
}
