// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Public)]
  public class TestCaseResultIdentifier : IAreaUriProperty, IEquatable<TestCaseResultIdentifier>
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

    [XmlIgnore]
    public string AreaUri { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0}, {1})", (object) this.TestRunId, (object) this.TestResultId);

    public bool Equals(TestCaseResultIdentifier other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return this.TestRunId == other.TestRunId && this.TestResultId == other.TestResultId;
    }

    public override int GetHashCode() => (391 + this.TestRunId) * 23 + this.TestResultId;
  }
}
