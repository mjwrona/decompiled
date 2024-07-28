// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.ArchivalService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class ArchivalService : IArchivalService, IVssFrameworkService
  {
    private const int DefaultJobDelayInSeconds = 300;
    private const string JobDelayAfterTransformationPath = "/Service/Analytics/Archival/Jobs/JobDelayInSecondsAfterTransformation";
    private static readonly RegistryQuery JobDelayRegQuery = new RegistryQuery("/Service/Analytics/Archival/Jobs/JobDelayInSecondsAfterTransformation");

    public void NotifyDataChange(IVssRequestContext requestContext, string tableName)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment || !(tableName == "Model.TestRun") && !(tableName == "Model.TestResult") && !(tableName == "Model.Test"))
        return;
      this.QueueArchivalJob(requestContext, new Guid[1]
      {
        Constants.TestEntitiesArchivalJobId
      });
    }

    private void QueueArchivalJob(IVssRequestContext requestContext, Guid[] jobIds)
    {
      if (!requestContext.GetService<IOrganizationPolicyService>().GetPolicy<bool>(requestContext, "Policy.IsInternal", false).EffectiveValue || !requestContext.IsFeatureEnabled("Analytics.Archival.EnableArchival") && !requestContext.IsFeatureEnabled("Archival.Sdk.EnableArchival"))
        return;
      int maxDelaySeconds = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in ArchivalService.JobDelayRegQuery, true, 300);
      requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) jobIds, maxDelaySeconds);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
