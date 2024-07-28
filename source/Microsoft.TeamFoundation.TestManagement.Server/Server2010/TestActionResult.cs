// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.TestActionResult
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
  [XmlInclude(typeof (TestIterationResult))]
  [XmlInclude(typeof (TestStepResult))]
  [XmlInclude(typeof (SharedStepResult))]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class TestActionResult : TestResult
  {
    private string m_actionPath;
    private int m_sharedStepId;
    private int m_iterationId;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string ActionPath
    {
      get => this.m_actionPath;
      set => this.m_actionPath = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public int SetId
    {
      get => this.m_sharedStepId;
      set => this.m_sharedStepId = value;
    }

    [XmlAttribute]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public int IterationId
    {
      get => this.m_iterationId;
      set => this.m_iterationId = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TestActionResult ({0}, {1})", (object) this.IterationId, (object) this.ActionPath);
  }
}
