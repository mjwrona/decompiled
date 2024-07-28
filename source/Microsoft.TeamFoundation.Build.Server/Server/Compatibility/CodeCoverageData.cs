// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.CodeCoverageData
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [SoapType("CodeCoverageData")]
  [ClassVisibility(ClientVisibility.Internal)]
  public class CodeCoverageData
  {
    private string m_runId;
    private string m_name;
    private string m_runUser;
    private int m_linesCovered;
    private int m_linesNotCovered;
    private int m_linesPartiallyCovered;
    private int m_blocksCovered;
    private int m_blocksNotCovered;
    private bool m_buildCoverageProcessing;

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

    public int LinesCovered
    {
      get => this.m_linesCovered;
      set => this.m_linesCovered = value;
    }

    public int LinesNotCovered
    {
      get => this.m_linesNotCovered;
      set => this.m_linesNotCovered = value;
    }

    public int LinesPartiallyCovered
    {
      get => this.m_linesPartiallyCovered;
      set => this.m_linesPartiallyCovered = value;
    }

    public int BlocksCovered
    {
      get => this.m_blocksCovered;
      set => this.m_blocksCovered = value;
    }

    public int BlocksNotCovered
    {
      get => this.m_blocksNotCovered;
      set => this.m_blocksNotCovered = value;
    }

    public bool IsBuildCoverageProcessing
    {
      get => this.m_buildCoverageProcessing;
      set => this.m_buildCoverageProcessing = value;
    }
  }
}
