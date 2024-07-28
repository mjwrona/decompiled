// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.TaskYieldOnExceptionAttribute
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  [AttributeUsage(AttributeTargets.Class)]
  public class TaskYieldOnExceptionAttribute : Attribute, IControllerConfiguration
  {
    public void Initialize(
      HttpControllerSettings controllerSettings,
      HttpControllerDescriptor controllerDescriptor)
    {
      controllerSettings?.Services.Add(typeof (IFilterProvider), (object) new TaskYieldOnExceptionAttribute.YieldFilterProvider());
    }

    private class YieldFilterProvider : IFilterProvider
    {
      public IEnumerable<FilterInfo> GetFilters(
        HttpConfiguration configuration,
        HttpActionDescriptor actionDescriptor)
      {
        yield return new FilterInfo((IFilter) new TaskYieldOnExceptionAttribute.YieldFilter(), FilterScope.Controller | FilterScope.Action);
      }
    }

    private class YieldFilter : FilterAttribute, IActionFilter, IFilter
    {
      public async Task<HttpResponseMessage> ExecuteActionFilterAsync(
        HttpActionContext actionContext,
        CancellationToken cancellationToken,
        Func<Task<HttpResponseMessage>> continuation)
      {
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
        HttpResponseMessage httpResponseMessage = response;
        response = (HttpResponseMessage) null;
        return httpResponseMessage;
      }
    }
  }
}
