// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DistributedTaskProjectApiController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  public abstract class DistributedTaskProjectApiController : TfsProjectApiController
  {
    public override string TraceArea => "DistributedTask";

    public override string ActivityLogArea => "DistributedTask";

    public IDistributedTaskResourceService ResourceService => this.TfsRequestContext.GetService<IDistributedTaskResourceService>();

    public IDistributedTaskService TaskService => this.TfsRequestContext.GetService<IDistributedTaskService>();

    [NonAction]
    public override Task<HttpResponseMessage> ExecuteAsync(
      HttpControllerContext controllerContext,
      CancellationToken cancellationToken)
    {
      object obj;
      if (controllerContext != null && controllerContext.RouteData != null && controllerContext.RouteData.Values != null && controllerContext.RouteData.Values.TryGetValue("scopeIdentifier", out obj))
      {
        Guid result;
        if (!Guid.TryParse(obj.ToString(), out result))
          return this.BadRequest(TaskResources.InvalidScopeId((object) result)).ExecuteAsync(cancellationToken);
        this.ProjectInfo = new ProjectInfo() { Id = result };
      }
      return base.ExecuteAsync(controllerContext, cancellationToken);
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      DistributedTaskExceptionMapper.Map(exceptionMap);
    }

    protected IList<int> ParseArray(string array, char delimiter = ',') => DistributedTaskApiControllerHelper.ParseArray(array, delimiter);

    protected IList<int> ToInt32UniqueList(string array, char delimiter = ',') => (IList<int>) DistributedTaskApiControllerHelper.ParseArray(array, delimiter).Distinct<int>().ToList<int>();

    protected static void SetContinuationToken(
      HttpResponseMessage responseMessage,
      string tokenValue)
    {
      if (responseMessage == null)
        throw new ArgumentNullException(nameof (responseMessage));
      if (string.IsNullOrWhiteSpace(tokenValue))
        return;
      responseMessage.Headers.Add("X-MS-ContinuationToken", tokenValue);
    }
  }
}
