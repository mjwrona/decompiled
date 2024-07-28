// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WebApis.TaskYieldOnExceptionAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server.WebApis
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true)]
  public sealed class TaskYieldOnExceptionAttribute : Attribute, IControllerConfiguration
  {
    private static readonly TaskYieldOnExceptionAttribute.YieldFilterProvider s_yieldFilterProviderInstance = new TaskYieldOnExceptionAttribute.YieldFilterProvider();

    public void Initialize(
      HttpControllerSettings controllerSettings,
      HttpControllerDescriptor controllerDescriptor)
    {
      controllerSettings?.Services.Add(typeof (IFilterProvider), (object) TaskYieldOnExceptionAttribute.s_yieldFilterProviderInstance);
    }

    private class YieldFilterProvider : IFilterProvider
    {
      private static readonly FilterInfo[] s_filterInfos = new FilterInfo[1]
      {
        new FilterInfo((IFilter) new TaskYieldOnExceptionAttribute.YieldFilter(), FilterScope.Controller | FilterScope.Action)
      };

      public IEnumerable<FilterInfo> GetFilters(
        HttpConfiguration configuration,
        HttpActionDescriptor actionDescriptor)
      {
        return (IEnumerable<FilterInfo>) TaskYieldOnExceptionAttribute.YieldFilterProvider.s_filterInfos;
      }
    }

    private class YieldFilter : FilterAttribute, IActionFilter, IFilter
    {
      public async Task<HttpResponseMessage> ExecuteActionFilterAsync(
        HttpActionContext actionContext,
        CancellationToken cancellationToken,
        Func<Task<HttpResponseMessage>> continuation)
      {
        bool flag = true;
        IVssRequestContext tfsRequestContext = actionContext?.ControllerContext?.Controller is TfsApiController controller ? controller.TfsRequestContext : (IVssRequestContext) null;
        if (tfsRequestContext != null)
          flag = !tfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Framework.DisableYieldOnException");
        if (!flag)
          return await continuation();
        HttpResponseMessage response;
        try
        {
          response = await continuation();
        }
        catch (object ex)
        {
          await Task.Yield();
          throw;
        }
        return response;
      }
    }
  }
}
