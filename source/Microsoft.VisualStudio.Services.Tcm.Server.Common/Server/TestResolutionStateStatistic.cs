// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResolutionStateStatistic
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestResolutionStateStatistic
  {
    private TestResolutionState m_state;
    private int m_count;

    [XmlElement]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public TestResolutionState State
    {
      get => this.m_state;
      set => this.m_state = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int Count
    {
      get => this.m_count;
      set => this.m_count = value;
    }
  }
}
