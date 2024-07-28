// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.WebApi.SecurityTokenExtensions
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F7D1B59D-FE5E-4B10-AAB1-4E05CDFBD17B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.ProjectAnalysis.WebApi
{
  public static class SecurityTokenExtensions
  {
    public static string ToLanguageMetricsToken(this Guid projectId) => ProjectAnalysisSecurityConstants.LanguageMetricsTokenRoot + projectId.ToString();
  }
}
