// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.TestResultData
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [SoapType("TestResultData")]
  [ClassVisibility(ClientVisibility.Internal)]
  public class TestResultData
  {
    private string m_runId;
    private string m_name;
    private string m_runUser;
    private int m_testsTotal;
    private int m_testsPassed;
    private int m_testsFailed;
    private bool m_runPassed;

    public string RunId
    {
      get => this.m_runId;
      set => this.m_runId = value;
    }

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public string RunUser
    {
      get => this.m_runUser;
      set => this.m_runUser = value;
    }

    public int TestsTotal
    {
      get => this.m_testsTotal;
      set => this.m_testsTotal = value;
    }

    public int TestsPassed
    {
      get => this.m_testsPassed;
      set => this.m_testsPassed = value;
    }

    public int TestsFailed
    {
      get => this.m_testsFailed;
      set => this.m_testsFailed = value;
    }

    public bool RunPassed
    {
      get => this.m_runPassed;
      set => this.m_runPassed = value;
    }
  }
}
