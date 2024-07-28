// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.DeploymentJobExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class DeploymentJobExtensions
  {
    public static IEnumerable<ReleaseTask> GetAllTasks(this IEnumerable<DeploymentJob> deploymentJob) => deploymentJob.SelectMany<DeploymentJob, ReleaseTask>((Func<DeploymentJob, IEnumerable<ReleaseTask>>) (d => (IEnumerable<ReleaseTask>) d.Tasks));

    public static IEnumerable<ReleaseTask> GetAllJobs(this IEnumerable<DeploymentJob> deploymentJob) => deploymentJob.Select<DeploymentJob, ReleaseTask>((Func<DeploymentJob, ReleaseTask>) (d => d.Job));
  }
}
