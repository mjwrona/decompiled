// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.WebApi.LanguageMetricsSecuredObject
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F7D1B59D-FE5E-4B10-AAB1-4E05CDFBD17B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.ProjectAnalysis.WebApi
{
  public class LanguageMetricsSecuredObject : ISecuredObject
  {
    public LanguageMetricsSecuredObject(Guid projectId) => this.ProjectId = projectId;

    public Guid NamespaceId => ProjectAnalysisSecurityConstants.SecurityNamespaceId;

    public int RequiredPermissions => 1;

    public string GetToken() => this.ProjectId.ToLanguageMetricsToken();

    public Guid ProjectId { get; set; }
  }
}
