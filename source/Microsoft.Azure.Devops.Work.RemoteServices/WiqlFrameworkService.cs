// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.RemoteServices.WiqlFrameworkService
// Assembly: Microsoft.Azure.Devops.Work.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C97796CA-4166-42B2-B96F-9A166B07FF72
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Devops.Work.RemoteServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Threading;

namespace Microsoft.Azure.Devops.Work.RemoteServices
{
  internal class WiqlFrameworkService : IWiqlRemotableService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public WorkItemQueryResult QueryByWiql(
      IVssRequestContext requestContext,
      string wiql,
      Guid projectId,
      bool? timePrecision = null,
      int? top = null,
      bool skipWiqlTextValidation = false)
    {
      throw new NotImplementedException();
    }

    public WorkItemQueryResult QueryByWiql(
      IVssRequestContext requestContext,
      string wiql,
      string projectName,
      bool? timePrecision = null,
      int? top = null,
      bool skipWiqlTextValidation = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<WorkItemQueryResult>(919800, 919801, 919802, "FrameworkServices", "WorkItemFrameworkService", nameof (QueryByWiql), (Func<WorkItemQueryResult>) (() =>
      {
        WorkItemTrackingHttpClient client = requestContext.GetClient<WorkItemTrackingHttpClient>();
        Wiql wiql1 = new Wiql();
        wiql1.Query = wiql;
        string project = projectName;
        bool? timePrecision1 = new bool?();
        int? top1 = new int?();
        CancellationToken cancellationToken = new CancellationToken();
        return client.QueryByWiqlAsync(wiql1, project, timePrecision1, top1, cancellationToken: cancellationToken).Result;
      }));
    }
  }
}
