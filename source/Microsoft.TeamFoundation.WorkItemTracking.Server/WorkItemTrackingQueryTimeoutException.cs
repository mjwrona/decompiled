// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrackingQueryTimeoutException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public class WorkItemTrackingQueryTimeoutException : WorkItemTrackingQueryException
  {
    public WorkItemTrackingQueryTimeoutException(IVssRequestContext requestContext)
      : base(WorkItemTrackingQueryTimeoutException.GetTimeOutExceptioMessage(requestContext))
    {
    }

    public WorkItemTrackingQueryTimeoutException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(requestContext)
    {
    }

    private static string GetTimeOutExceptioMessage(IVssRequestContext requestContext)
    {
      int num = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).WorkItemQueryTimeoutInSecond;
      if (num <= 0)
        num = int.MaxValue;
      return ServerResources.QueryTimeoutException((object) num);
    }
  }
}
