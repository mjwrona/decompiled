// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.CodeCoverageStatus
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [SoapType("CodeCoverageStatus")]
  public class CodeCoverageStatus
  {
    private bool m_buildCoverageProcessing;
    private string m_errorLogFile;

    public bool IsBuildCoverageProcessing
    {
      get => this.m_buildCoverageProcessing;
      set => this.m_buildCoverageProcessing = value;
    }

    public string ErrorLogFile
    {
      get => this.m_errorLogFile;
      set => this.m_errorLogFile = value;
    }
  }
}
