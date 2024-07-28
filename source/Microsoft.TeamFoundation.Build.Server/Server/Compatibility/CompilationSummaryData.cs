// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.CompilationSummaryData
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [SoapType("CompilationSummaryData")]
  [ClassVisibility(ClientVisibility.Internal)]
  public class CompilationSummaryData
  {
    private string m_logFileName;
    private long m_totalCompilerErrors;
    private long m_totalCompilerWarnings;
    private long m_totalCodeAnalysisErrors;
    private long m_totalCodeAnalysisWarnings;

    public string LogFileName
    {
      get => this.m_logFileName;
      set => this.m_logFileName = value;
    }

    public long TotalCompilerErrors
    {
      get => this.m_totalCompilerErrors;
      set => this.m_totalCompilerErrors = value;
    }

    public long TotalCompilerWarnings
    {
      get => this.m_totalCompilerWarnings;
      set => this.m_totalCompilerWarnings = value;
    }

    public long TotalCodeAnalysisErrors
    {
      get => this.m_totalCodeAnalysisErrors;
      set => this.m_totalCodeAnalysisErrors = value;
    }

    public long TotalCodeAnalysisWarnings
    {
      get => this.m_totalCodeAnalysisWarnings;
      set => this.m_totalCodeAnalysisWarnings = value;
    }
  }
}
