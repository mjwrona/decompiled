// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.CustomerIntelligenceAttribute
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class CustomerIntelligenceAttribute : ActionFilterAttribute
  {
    private readonly string m_area;
    private readonly string m_feature;
    internal const string RequestItemKey = "Build2.CustomerIntelligenceAttribute.Data";

    public CustomerIntelligenceAttribute(string area, string feature)
    {
      this.m_area = area;
      this.m_feature = feature;
    }

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      if ((actionContext.ControllerContext?.Controller is TfsApiController controller1 ? controller1.TfsRequestContext : (IVssRequestContext) null) != null)
      {
        IVssRequestContext tfsRequestContext = controller1.TfsRequestContext;
        try
        {
          CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
          if (actionContext.ControllerContext?.Controller is TfsProjectApiController controller)
            intelligenceData.Add("ProjectId", (object) controller.ProjectId);
          tfsRequestContext.Items["Build2.CustomerIntelligenceAttribute.Data"] = (object) intelligenceData;
        }
        catch (Exception ex)
        {
          tfsRequestContext.TraceException(nameof (CustomerIntelligenceAttribute), ex);
        }
      }
      base.OnActionExecuting(actionContext);
    }

    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      if ((actionExecutedContext.ActionContext?.ControllerContext?.Controller is TfsApiController controller ? controller.TfsRequestContext : (IVssRequestContext) null) != null)
      {
        IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
        try
        {
          CustomerIntelligenceData properties;
          if (tfsRequestContext.Items.TryGetValue<CustomerIntelligenceData>("Build2.CustomerIntelligenceAttribute.Data", out properties))
            tfsRequestContext.GetService<CustomerIntelligenceService>().Publish(tfsRequestContext, tfsRequestContext.ServiceHost.InstanceId, this.m_area, this.m_feature, properties);
        }
        catch (Exception ex)
        {
          tfsRequestContext.TraceException(nameof (CustomerIntelligenceAttribute), ex);
        }
      }
      base.OnActionExecuted(actionExecutedContext);
    }
  }
}
