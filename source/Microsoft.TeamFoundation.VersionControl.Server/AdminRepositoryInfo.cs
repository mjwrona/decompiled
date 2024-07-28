// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.AdminRepositoryInfo
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class AdminRepositoryInfo
  {
    private int m_userCount;
    private int m_groupCount;
    private int m_workspaceCount;
    private int m_shelvesetCount;
    private int m_fileCount;
    private int m_folderCount;
    private int m_maxChangesetId;
    private int m_pendingChangeCount;
    private int m_shelvesetDeletedCount;

    [XmlAttribute]
    public int UserCount
    {
      get => this.m_userCount;
      set => this.m_userCount = value;
    }

    [XmlAttribute]
    public int GroupCount
    {
      get => this.m_groupCount;
      set => this.m_groupCount = value;
    }

    [XmlAttribute]
    public int WorkspaceCount
    {
      get => this.m_workspaceCount;
      set => this.m_workspaceCount = value;
    }

    [XmlAttribute]
    public int ShelvesetCount
    {
      get => this.m_shelvesetCount;
      set => this.m_shelvesetCount = value;
    }

    [XmlAttribute]
    public int FileCount
    {
      get => this.m_fileCount;
      set => this.m_fileCount = value;
    }

    [XmlAttribute]
    public int FolderCount
    {
      get => this.m_folderCount;
      set => this.m_folderCount = value;
    }

    [XmlAttribute]
    public int MaxChangesetID
    {
      get => this.m_maxChangesetId;
      set => this.m_maxChangesetId = value;
    }

    [XmlAttribute]
    public int PendingChangeCount
    {
      get => this.m_pendingChangeCount;
      set => this.m_pendingChangeCount = value;
    }

    [XmlAttribute]
    public int ShelvesetDeletedCount
    {
      get => this.m_shelvesetDeletedCount;
      set => this.m_shelvesetDeletedCount = value;
    }
  }
}
