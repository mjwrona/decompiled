// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Converters.ReleaseConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Converters
{
  public static class ReleaseConverter
  {
    public static Release ToV1Release(this Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      foreach (DeploymentAttempt deploymentAttempt in release.Environments.SelectMany<ReleaseEnvironment, DeploymentAttempt>((Func<ReleaseEnvironment, IEnumerable<DeploymentAttempt>>) (environment => (IEnumerable<DeploymentAttempt>) environment.DeploySteps)))
      {
        if (deploymentAttempt.Job != null)
        {
          deploymentAttempt.Job.StartTime = new DateTime?();
          deploymentAttempt.Job.FinishTime = new DateTime?();
          if (deploymentAttempt.Job.Status == TaskStatus.PartiallySucceeded)
            deploymentAttempt.Job.Status = TaskStatus.Failed;
        }
        if (!deploymentAttempt.Tasks.IsNullOrEmpty<ReleaseTask>())
        {
          foreach (ReleaseTask task in deploymentAttempt.Tasks)
          {
            task.StartTime = new DateTime?();
            task.FinishTime = new DateTime?();
            if (task.Status == TaskStatus.PartiallySucceeded)
              task.Status = TaskStatus.Failed;
          }
        }
      }
      return release;
    }

    public static void HandleTaskStatus(this DeploymentAttempt deployStep)
    {
      if (deployStep == null)
        throw new ArgumentNullException(nameof (deployStep));
      if (deployStep.Job != null)
      {
        deployStep.Job.DateStarted = new DateTime?();
        deployStep.Job.DateEnded = new DateTime?();
        if (deployStep.Job.Status == TaskStatus.Success)
          deployStep.Job.Status = TaskStatus.Succeeded;
        if (deployStep.Job.Status == TaskStatus.Failure)
          deployStep.Job.Status = TaskStatus.Failed;
        if (deployStep.Job.Status == TaskStatus.PartiallySucceeded)
          deployStep.Job.Status = TaskStatus.Failed;
      }
      if (deployStep.Tasks.IsNullOrEmpty<ReleaseTask>())
        return;
      foreach (ReleaseTask task in deployStep.Tasks)
      {
        task.DateStarted = new DateTime?();
        task.DateEnded = new DateTime?();
        if (task.Status == TaskStatus.Success)
          task.Status = TaskStatus.Succeeded;
        if (task.Status == TaskStatus.Failure)
          task.Status = TaskStatus.Failed;
        if (task.Status == TaskStatus.PartiallySucceeded)
          task.Status = TaskStatus.Failed;
      }
    }
  }
}
