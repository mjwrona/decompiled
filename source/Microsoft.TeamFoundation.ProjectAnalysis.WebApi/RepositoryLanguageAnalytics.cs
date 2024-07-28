// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.WebApi.RepositoryLanguageAnalytics
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F7D1B59D-FE5E-4B10-AAB1-4E05CDFBD17B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.ProjectAnalysis.WebApi
{
  [DataContract]
  public class RepositoryLanguageAnalytics : LanguageMetricsSecuredObject
  {
    [DataMember(EmitDefaultValue = false)]
    public string Name;
    [DataMember]
    public ResultPhase ResultPhase;
    [DataMember(EmitDefaultValue = false)]
    public DateTime UpdatedTime;
    [DataMember(EmitDefaultValue = false)]
    public IList<LanguageStatistics> LanguageBreakdown;

    public RepositoryLanguageAnalytics(Guid projectId)
      : base(projectId)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public Guid Id { get; set; }
  }
}
