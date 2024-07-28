// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.SDStalenessHandler.CustomRepositoryBranchStalenessProvider
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.WebServer.SDStalenessHandler
{
  public static class CustomRepositoryBranchStalenessProvider
  {
    public static CustomRepositoryBranchStatusResponse GetBranchStalenessData(
      IVssRequestContext requestContext,
      string project,
      string branch)
    {
      CustomRepositoryBranchStatusResponse branchStalenessData = new CustomRepositoryBranchStatusResponse();
      string controlAzureLocation = SourceControlFactory.GetSourceControlAzureLocation(project, branch);
      string credentialsForHosted = KeyVaultProvider.GetCredentialsForHosted(requestContext);
      if (credentialsForHosted == null)
        throw new AggregateException("unable to connect to Azure Storage Table");
      if (controlAzureLocation == null)
        throw new AggregateException("Unable to find workitem table for project: " + project + ", branch: " + branch);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1083162, "REST-API", "REST-API", FormattableString.Invariant(FormattableStringFactory.Create("SdStaleness request for project: {0}, branch: {1}, workitemTable: {2}", (object) project, (object) branch, (object) controlAzureLocation)));
      WorkItem[] workItem1 = new AzureWorkItemStore(credentialsForHosted, controlAzureLocation, TimeSpan.FromSeconds(30.0), 3).GetWorkItem(project, branch);
      WorkItem workItem2 = workItem1 != null && workItem1.Length != 0 ? workItem1[workItem1.Length - 1] : throw new AggregateException("Could not find workitem row for project: " + project + ", branch: " + branch + " combination");
      branchStalenessData.LastIndexedChangeId = workItem2.LastIndexedChangeId;
      branchStalenessData.LatestChangeId = workItem2.LatestChangeId;
      branchStalenessData.LastIndexedChangeIdChangeTime = workItem2.LastIndexedChangeIdChangeTime != null ? DateTime.Parse(workItem2.LastIndexedChangeIdChangeTime, (IFormatProvider) CultureInfo.InvariantCulture) : DateTime.MinValue;
      branchStalenessData.LatestChangeIdChangeTime = workItem2.LatestChangeIdChangeTime != null ? DateTime.Parse(workItem2.LatestChangeIdChangeTime, (IFormatProvider) CultureInfo.InvariantCulture) : DateTime.MinValue;
      return branchStalenessData;
    }
  }
}
