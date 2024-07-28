// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestExecutionReleaseHelper
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TestExecutionReleaseHelper : ITestExecutionReleaseHelper
  {
    public ReleaseEnvironment GetReleaseEnvironmentByUri(
      TestExecutionRequestContext context,
      TeamProjectReference projectReference,
      string ReleaseUri,
      string ReleaseEnvironmentUri)
    {
      if (string.IsNullOrEmpty(ReleaseUri) || string.IsNullOrEmpty(ReleaseEnvironmentUri))
        return (ReleaseEnvironment) null;
      ReleaseHttpClient client = context.RequestContext.GetClient<ReleaseHttpClient>();
      int releaseArtifactId = this.GetReleaseArtifactId(context, ReleaseUri);
      int releaseEnvId = this.GetReleaseArtifactId(context, ReleaseEnvironmentUri);
      Guid id = projectReference.Id;
      int releaseId = releaseArtifactId;
      SingleReleaseExpands? nullable = new SingleReleaseExpands?(SingleReleaseExpands.None);
      ApprovalFilters? approvalFilters = new ApprovalFilters?();
      SingleReleaseExpands? expand = nullable;
      int? topGateRecords = new int?();
      CancellationToken cancellationToken = new CancellationToken();
      return client.GetReleaseAsync(id, releaseId, approvalFilters, expand: expand, topGateRecords: topGateRecords, cancellationToken: cancellationToken).Result.Environments.Where<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (env => env.Id == releaseEnvId)).FirstOrDefault<ReleaseEnvironment>();
    }

    public int GetReleaseArtifactId(TestExecutionRequestContext context, string releaseUri)
    {
      int result;
      if (!int.TryParse(LinkingUtilities.DecodeUri(releaseUri).ToolSpecificId, out result))
        new DtaLogger(context, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ServiceLayer).Error(6200205, "Invalid release uri " + releaseUri);
      return result;
    }
  }
}
