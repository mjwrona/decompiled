// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.EnvironmentStatusExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class EnvironmentStatusExtensions
  {
    private static readonly Dictionary<EnvironmentStatus, ServiceEndpointExecutionResult> ServiceEndpointExecutionResultMap = new Dictionary<EnvironmentStatus, ServiceEndpointExecutionResult>()
    {
      {
        EnvironmentStatus.Canceled,
        ServiceEndpointExecutionResult.Canceled
      },
      {
        EnvironmentStatus.Rejected,
        ServiceEndpointExecutionResult.Failed
      },
      {
        EnvironmentStatus.Succeeded,
        ServiceEndpointExecutionResult.Succeeded
      },
      {
        EnvironmentStatus.PartiallySucceeded,
        ServiceEndpointExecutionResult.SucceededWithIssues
      }
    };

    public static ServiceEndpointExecutionResult ToServiceEndpointExecutionResult(
      EnvironmentStatus status)
    {
      return EnvironmentStatusExtensions.ServiceEndpointExecutionResultMap.ContainsKey(status) ? EnvironmentStatusExtensions.ServiceEndpointExecutionResultMap[status] : throw new ReleaseManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.EnvironmentStatusCannotBeConvertedError, (object) status));
    }
  }
}
