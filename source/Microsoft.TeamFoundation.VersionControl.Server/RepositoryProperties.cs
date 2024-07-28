// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.RepositoryProperties
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  public class RepositoryProperties
  {
    private Guid m_id;
    private int m_latestChangesetId;
    private byte[] m_downloadKey;

    [XmlAttribute("id")]
    public Guid Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    [XmlAttribute("name")]
    public string Name
    {
      get => string.Empty;
      set
      {
      }
    }

    [XmlAttribute("lcset")]
    public int LatestChangesetId
    {
      get => this.m_latestChangesetId;
      set => this.m_latestChangesetId = value;
    }

    [XmlAttribute("ver")]
    public string Version
    {
      get => string.Empty;
      set
      {
      }
    }

    [XmlAttribute("dkey")]
    public byte[] DownloadKey
    {
      get => this.m_downloadKey;
      set => this.m_downloadKey = value;
    }

    [XmlAttribute("features")]
    public int SupportedFeatures
    {
      get => 3967;
      set
      {
      }
    }
  }
}
