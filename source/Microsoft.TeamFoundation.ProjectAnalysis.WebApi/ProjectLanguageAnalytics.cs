// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.WebApi.ProjectLanguageAnalytics
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F7D1B59D-FE5E-4B10-AAB1-4E05CDFBD17B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.ProjectAnalysis.WebApi
{
  [DataContract]
  public class ProjectLanguageAnalytics : LanguageMetricsSecuredObject, IAnalytics
  {
    [DataMember(EmitDefaultValue = false)]
    public string Url;
    [DataMember]
    public ResultPhase ResultPhase;
    [DataMember(EmitDefaultValue = false)]
    public IList<LanguageStatistics> LanguageBreakdown;
    [DataMember(EmitDefaultValue = false)]
    public IList<Microsoft.TeamFoundation.ProjectAnalysis.WebApi.RepositoryLanguageAnalytics> RepositoryLanguageAnalytics;

    public ProjectLanguageAnalytics(Guid projectId)
      : base(projectId)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public Guid Id => this.ProjectId;
  }
}
