// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.ClientArtifact
// Assembly: Microsoft.TeamFoundation.VersionControl.Common.Integration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2A9D748-4CEE-4498-9785-584B91A44F85
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.Integration.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public class ClientArtifact
  {
    [XmlElement(ElementName = "Url")]
    public string Url;
    [XmlAttribute("ArtifactType")]
    public string Type;
    [XmlAttribute("Item")]
    public string Item;
    [XmlAttribute("Folder")]
    public string Folder;
    [XmlAttribute("TeamProject")]
    public string TeamProject;
    [XmlAttribute("ItemRevision")]
    public string ItemVersion;
    [XmlAttribute("ChangeType")]
    public string ChangeType;
    private string m_serverItem;

    public ClientArtifact()
    {
    }

    public ClientArtifact(string url, string artifactType)
    {
      this.Url = url;
      this.Type = artifactType;
    }

    [XmlAttribute("ServerItem")]
    public string ServerItem
    {
      get => this.m_serverItem == null ? string.Empty : this.m_serverItem;
      set
      {
        this.m_serverItem = value;
        if (string.IsNullOrEmpty(this.m_serverItem))
          return;
        VersionControlPath.Parse(this.m_serverItem, out this.Folder, out this.Item);
        this.TeamProject = VersionControlPath.GetTeamProjectName(this.m_serverItem);
      }
    }
  }
}
