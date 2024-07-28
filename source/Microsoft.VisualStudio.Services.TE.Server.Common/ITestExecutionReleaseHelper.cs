// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.ITestExecutionReleaseHelper
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public interface ITestExecutionReleaseHelper
  {
    ReleaseEnvironment GetReleaseEnvironmentByUri(
      TestExecutionRequestContext context,
      TeamProjectReference projectReference,
      string ReleaseUri,
      string ReleaseEnvironmentUri);

    int GetReleaseArtifactId(TestExecutionRequestContext context, string releaseUri);
  }
}
