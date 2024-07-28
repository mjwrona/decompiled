// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.TfvcFileStats
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [RequiredClientService("VersionControlServer")]
  [CallOnDeserialization("AfterDeserialize")]
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  public class TfvcFileStats
  {
    private int m_changesetId;
    private long m_fileCountTotal;

    [XmlAttribute]
    public int ChangesetId
    {
      get => this.m_changesetId;
      set => this.m_changesetId = value;
    }

    [XmlAttribute]
    public long FileCountTotal
    {
      get => this.m_fileCountTotal;
      set => this.m_fileCountTotal = value;
    }
  }
}
