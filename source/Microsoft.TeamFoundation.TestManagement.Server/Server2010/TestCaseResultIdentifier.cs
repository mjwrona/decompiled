// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.TestCaseResultIdentifier
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Public)]
  public class TestCaseResultIdentifier
  {
    private int m_testRunId;
    private int m_testResultId;

    public TestCaseResultIdentifier()
    {
    }

    public TestCaseResultIdentifier(int testRunId, int testResultId)
    {
      this.m_testRunId = testRunId;
      this.m_testResultId = testResultId;
    }

    [XmlAttribute]
    public int TestRunId
    {
      get => this.m_testRunId;
      set => this.m_testRunId = value;
    }

    [XmlAttribute]
    public int TestResultId
    {
      get => this.m_testResultId;
      set => this.m_testResultId = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0}, {1})", (object) this.TestRunId, (object) this.TestResultId);
  }
}
