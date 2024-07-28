// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ActivityLoggingAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ActivityLoggingAttribute : AuthorizationFilterAttribute
  {
    private const string c_area = "ActivityLogging";
    private const string c_layer = "ActionFilter";

    public override void OnAuthorization(HttpActionContext actionContext)
    {
      base.OnAuthorization(actionContext);
      if (!(actionContext.ControllerContext.Controller is TfsApiController controller))
        return;
      if (string.IsNullOrEmpty(controller.TfsRequestContext.ServiceName))
        controller.TfsRequestContext.ServiceName = "Web-Api";
      bool isLongRunning = false;
      bool keepsHostAwake = true;
      TimeSpan timeout = new TimeSpan();
      MethodType methodType = MethodType.Normal;
      EstimatedMethodCost estimatedCost = EstimatedMethodCost.Low;
      string str = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName + "." + actionContext.ActionDescriptor.ActionName;
      if (controller is IOverrideLoggingMethodNames loggingMethodNames)
      {
        try
        {
          IEnumerable<ModelBinderParameterBinding> source = actionContext.ActionDescriptor.ActionBinding.ParameterBindings.OfType<ModelBinderParameterBinding>();
          new HttpActionBinding(actionContext.ActionDescriptor, (HttpParameterBinding[]) source.ToArray<ModelBinderParameterBinding>()).ExecuteBindingAsync(actionContext, new CancellationToken()).SyncResult();
        }
        catch (Exception ex)
        {
          controller.TfsRequestContext.TraceException(7100, "ActivityLogging", "ActionFilter", ex);
        }
        str = loggingMethodNames.GetLoggingMethodName(str, actionContext);
      }
      MethodInformationAttribute informationAttribute = actionContext.ActionDescriptor.GetCustomAttributes<MethodInformationAttribute>().FirstOrDefault<MethodInformationAttribute>();
      if (informationAttribute != null)
      {
        timeout = informationAttribute.Timeout;
        methodType = informationAttribute.MethodType;
        estimatedCost = informationAttribute.EstimatedCost;
        isLongRunning = informationAttribute.IsLongRunning;
        keepsHostAwake = informationAttribute.KeepsHostAwake;
      }
      MethodInformation methodInformation = new MethodInformation(str, methodType, estimatedCost, keepsHostAwake, isLongRunning, timeout);
      foreach (string key in actionContext.ActionArguments.Keys)
      {
        object actionArgument = actionContext.ActionArguments[key];
        switch (actionArgument)
        {
          case string[] _:
            methodInformation.AddArrayParameter<string>(key, (IList<string>) (string[]) actionArgument);
            continue;
          case IList<int> _:
            methodInformation.AddArrayParameter<int>(key, (IList<int>) actionArgument);
            continue;
          default:
            methodInformation.AddParameter(key, actionContext.ActionArguments[key]);
            continue;
        }
      }
      ApiTelemetryService service = controller.TfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<ApiTelemetryService>();
      if (service.ShouldLog(controller.TfsRequestContext, actionContext.ActionDescriptor.ControllerDescriptor.ControllerType))
        actionContext.RequestContext.RouteData.Values["apiCIData"] = (object) service.GetRestTelemetry(controller.TfsRequestContext, controller);
      controller.TfsRequestContext.RootContext.EnterMethod(methodInformation);
    }
  }
}
