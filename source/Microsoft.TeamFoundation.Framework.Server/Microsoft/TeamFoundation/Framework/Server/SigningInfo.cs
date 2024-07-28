// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SigningInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  public class SigningInfo
  {
    private byte[] m_downloadKey;
    private Guid m_id;

    internal SigningInfo()
    {
    }

    public SigningInfo(Guid id) => this.m_id = id;

    [XmlAttribute("dkey")]
    public byte[] DownloadKey
    {
      get => this.m_downloadKey;
      set => this.m_downloadKey = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public Guid Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }
  }
}
