// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ConflictInformation
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class ConflictInformation
  {
    private ConflictType m_conflictType;
    private int m_versionFrom;
    private int m_pendingChangeId;
    private string m_sourceLocalItem;
    private string m_targetLocalItem;
    private int m_reason;

    [XmlAttribute("ct")]
    public ConflictType ConflictType
    {
      get => this.m_conflictType;
      set => this.m_conflictType = value;
    }

    [XmlAttribute("vf")]
    public int VersionFrom
    {
      get => this.m_versionFrom;
      set => this.m_versionFrom = value;
    }

    [XmlAttribute("pcid")]
    public int PendingChangeId
    {
      get => this.m_pendingChangeId;
      set => this.m_pendingChangeId = value;
    }

    [XmlAttribute("slocal")]
    public string SourceLocalItem
    {
      get => this.m_sourceLocalItem;
      set => this.m_sourceLocalItem = value;
    }

    [XmlAttribute("tlocal")]
    public string TargetLocalItem
    {
      get => this.m_targetLocalItem;
      set => this.m_targetLocalItem = value;
    }

    [XmlAttribute("re")]
    public int Reason
    {
      get => this.m_reason;
      set => this.m_reason = value;
    }
  }
}
