// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationServiceHostActivity
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [CallOnDeserialization("AfterDeserialize")]
  public class TeamFoundationServiceHostActivity
  {
    private List<TeamFoundationRequestInformation> m_activeRequests;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public Guid Id { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Name { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime StartTime { get; set; }

    [XmlAttribute("Status")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public int StatusValue { get; set; }

    [XmlIgnore]
    public TeamFoundationServiceHostStatus Status
    {
      get => (TeamFoundationServiceHostStatus) this.StatusValue;
      set => this.StatusValue = (int) value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string StatusReason { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public List<TeamFoundationRequestInformation> ActiveRequests
    {
      get
      {
        if (this.m_activeRequests == null)
          this.m_activeRequests = new List<TeamFoundationRequestInformation>();
        return this.m_activeRequests;
      }
    }
  }
}
