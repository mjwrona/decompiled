// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts.CodeCoverageDataProviderContract
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Coverage.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.Contracts
{
  [DataContract]
  public class CodeCoverageDataProviderContract
  {
    [DataMember(EmitDefaultValue = false)]
    public PipelineCoverageSummary PipelineCoverageSummary { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CoverageChangeSummaryResponse ChangedFiles { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int BuildDurationCheckTimeInMinutes { get; set; }
  }
}
