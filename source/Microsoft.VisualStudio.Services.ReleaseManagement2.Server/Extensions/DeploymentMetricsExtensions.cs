// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.DeploymentMetricsExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class DeploymentMetricsExtensions
  {
    private static readonly Dictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus, string> DeploymentMetricNameMap = new Dictionary<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus, string>()
    {
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.PartiallySucceeded,
        "PartiallySuccessfulDeployments"
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.Succeeded,
        "SuccessfulDeployments"
      },
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus.Failed,
        "FailedDeployments"
      }
    };

    public static IList<Metric> ToContract(
      this IEnumerable<DeploymentMetric> deploymentMetricsList)
    {
      if (deploymentMetricsList == null)
        throw new ArgumentNullException(nameof (deploymentMetricsList));
      List<Metric> contract = new List<Metric>();
      foreach (DeploymentMetric deploymentMetrics in deploymentMetricsList)
      {
        if (DeploymentMetricsExtensions.DeploymentMetricNameMap.ContainsKey(deploymentMetrics.Status))
          contract.Add(new Metric()
          {
            Value = deploymentMetrics.Count,
            Name = DeploymentMetricsExtensions.DeploymentMetricNameMap[deploymentMetrics.Status]
          });
      }
      return (IList<Metric>) contract;
    }
  }
}
