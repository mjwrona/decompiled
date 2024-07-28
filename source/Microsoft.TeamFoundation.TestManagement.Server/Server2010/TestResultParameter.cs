// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.TestResultParameter
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class TestResultParameter
  {
    private int m_testRunId;
    private int m_testResultId;
    private int m_iterationId;
    private string m_actionPath;
    private string m_parameterName;
    private byte m_dataType;
    private byte[] m_expected;
    private byte[] m_actual;

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

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public int IterationId
    {
      get => this.m_iterationId;
      set => this.m_iterationId = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public string ActionPath
    {
      get => this.m_actionPath;
      set => this.m_actionPath = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string ParameterName
    {
      get => this.m_parameterName;
      set => this.m_parameterName = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Internal)]
    public byte DataType
    {
      get => this.m_dataType;
      set => this.m_dataType = value;
    }

    [XmlElement]
    [ClientProperty(ClientVisibility.Internal)]
    public byte[] Expected
    {
      get => this.m_expected;
      set => this.m_expected = value;
    }

    [XmlElement]
    [ClientProperty(ClientVisibility.Internal)]
    public byte[] Actual
    {
      get => this.m_actual;
      set => this.m_actual = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestResultParameter Name={0} ({1}, {2}, {3}, {4})", (object) this.ParameterName, (object) this.TestRunId, (object) this.TestResultId, (object) this.IterationId, (object) this.ActionPath);
  }
}
