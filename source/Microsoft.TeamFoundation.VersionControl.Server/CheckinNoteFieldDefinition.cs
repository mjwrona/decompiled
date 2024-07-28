// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CheckinNoteFieldDefinition
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.VersionControl.Common;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class CheckinNoteFieldDefinition : IValidatable
  {
    private ItemPathPair m_itemPathPair;
    private string m_name;
    private bool m_required;
    private int m_displayOrder;

    [XmlAttribute("ai")]
    public string ServerItem
    {
      get => this.ItemPathPair.ProjectNamePath;
      set => this.ItemPathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair ItemPathPair
    {
      get => this.m_itemPathPair;
      set => this.m_itemPathPair = value;
    }

    [XmlAttribute("name")]
    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    [XmlAttribute("req")]
    [DefaultValue(false)]
    public bool Required
    {
      get => this.m_required;
      set => this.m_required = value;
    }

    [XmlAttribute("do")]
    [DefaultValue(0)]
    public int DisplayOrder
    {
      get => this.m_displayOrder;
      set => this.m_displayOrder = value;
    }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      PathLength serverPathLength = versionControlRequestContext.MaxSupportedServerPathLength;
      string serverItem = this.ServerItem;
      versionControlRequestContext.Validation.checkServerItem(ref serverItem, "ServerItem", true, false, true, false, serverPathLength);
      versionControlRequestContext.Validation.checkFieldName(this.m_name, "Name", false);
      if (serverItem == null)
        return;
      this.m_itemPathPair = new ItemPathPair(serverItem, this.ItemPathPair.ProjectGuidPath);
    }

    public override string ToString() => this.Name + "; " + (object) this.Required + "; " + (object) this.DisplayOrder + "; " + this.ServerItem;
  }
}
