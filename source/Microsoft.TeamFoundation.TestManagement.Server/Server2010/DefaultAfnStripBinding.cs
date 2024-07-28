// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.DefaultAfnStripBinding
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class DefaultAfnStripBinding
  {
    private int m_testCaseId;
    private int m_testRunId;
    private int m_testResultId;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int TestCaseId
    {
      get => this.m_testCaseId;
      set => this.m_testCaseId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int TestRunId
    {
      get => this.m_testRunId;
      set => this.m_testRunId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public int TestResultId
    {
      get => this.m_testResultId;
      set => this.m_testResultId = value;
    }
  }
}
