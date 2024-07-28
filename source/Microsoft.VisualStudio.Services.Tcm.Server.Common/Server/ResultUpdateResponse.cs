// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class ResultUpdateResponse
  {
    private int m_revision;
    private int[] m_attachmentIds;

    [XmlIgnore]
    internal int TestResultId { get; set; }

    [XmlIgnore]
    internal int TestPlanId { get; set; }

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

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string LastUpdatedByName { get; set; }

    [XmlArray]
    public int[] AttachmentIds
    {
      get => this.m_attachmentIds;
      set => this.m_attachmentIds = value;
    }

    [XmlAttribute]
    public int MaxReservedSubResultId { get; set; }
  }
}
