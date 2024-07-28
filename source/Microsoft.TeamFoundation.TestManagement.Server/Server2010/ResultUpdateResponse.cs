// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.ResultUpdateResponse
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class ResultUpdateResponse
  {
    private int m_revision;
    private int[] m_attachmentIds;

    [XmlAttribute]
    public int Revision
    {
      get => this.m_revision;
      set => this.m_revision = value;
    }

    [XmlAttribute]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    public DateTime LastUpdated { get; set; }

    [XmlAttribute]
    public Guid LastUpdatedBy { get; set; }

    [XmlArray]
    public int[] AttachmentIds
    {
      get => this.m_attachmentIds;
      set => this.m_attachmentIds = value;
    }
  }
}
