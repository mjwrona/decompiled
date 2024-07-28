// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.MetricsService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class MetricsService : ReleaseManagement2ServiceBase
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Optional parameters")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public IEnumerable<DeploymentMetric> GetMetrics(
      IVssRequestContext requestContext,
      Guid projectId,
      DateTime minMetricsTime)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (minMetricsTime.Date < DateTime.UtcNow.AddDays(-31.0))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MinMetricsTimeNotSupported));
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "MetricsService.GetMetrics", 1900020))
      {
        Func<MetricSqlComponent, IEnumerable<DeploymentMetric>> action = (Func<MetricSqlComponent, IEnumerable<DeploymentMetric>>) (component => component.GetMetrics(projectId, minMetricsTime));
        return requestContext.ExecuteWithinUsingWithComponent<MetricSqlComponent, IEnumerable<DeploymentMetric>>(action);
      }
    }
  }
}
