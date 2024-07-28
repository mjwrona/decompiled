// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.HandlePropertySelectorsAttribute
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true)]
  public sealed class HandlePropertySelectorsAttribute : ActionFilterAttribute
  {
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to error out in case anything goes wrong here, since we have already executed the REST API")]
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      if (actionContext == null)
        throw new ArgumentNullException(nameof (actionContext));
      base.OnActionExecuting(actionContext);
      if (string.Equals(actionContext.Request.Method.ToString(), HttpMethod.Get.ToString(), StringComparison.OrdinalIgnoreCase))
        return;
      TfsProjectApiController projectApiController = (TfsProjectApiController) null;
      try
      {
        projectApiController = actionContext.ControllerContext.Controller as TfsProjectApiController;
        Dictionary<string, object> actionArguments = actionContext.ActionArguments;
        if (actionArguments == null || actionArguments.Count <= 0)
          return;
        foreach (KeyValuePair<string, object> keyValuePair in actionArguments)
        {
          if (keyValuePair.Value != null && !keyValuePair.Value.GetType().IsPrimitive)
          {
            if (keyValuePair.Value is IEnumerable && keyValuePair.Value.GetType() != typeof (string))
            {
              if (keyValuePair.Value is IEnumerable<object> source)
              {
                foreach (object obj in source.Where<object>((Func<object, bool>) (e => !e.GetType().IsPrimitive)))
                  PropertySelectorsHelper.RemoveNullFromLists(obj);
              }
            }
            else
              PropertySelectorsHelper.RemoveNullFromLists(keyValuePair.Value);
          }
        }
      }
      catch (Exception ex)
      {
        if (projectApiController == null)
          return;
        projectApiController.TfsRequestContext.RootContext.Trace(1976376, TraceLevel.Info, "ReleaseManagementService", "Service", "removing nulls from collection failed : {0}", (object) ex.StackTrace);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to error out in case anything goes wrong here, since we have already executed the REST API")]
    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      if (actionExecutedContext == null)
        throw new ArgumentNullException(nameof (actionExecutedContext));
      base.OnActionExecuted(actionExecutedContext);
      TfsProjectApiController controller1 = actionExecutedContext.ActionContext.ControllerContext.Controller as TfsProjectApiController;
      if (actionExecutedContext == null || actionExecutedContext.Response == null || !actionExecutedContext.Response.IsSuccessStatusCode || controller1 == null || controller1.TfsRequestContext == null)
        return;
      string str = actionExecutedContext.Request.GetQueryNameValuePairs().FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (pair => string.Compare(pair.Key, "propertySelectors", StringComparison.OrdinalIgnoreCase) == 0)).Value;
      if (string.IsNullOrEmpty(str))
        return;
      IHttpController controller2 = actionExecutedContext.ActionContext.ControllerContext.Controller;
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(controller1.TfsRequestContext.RootContext, "Service", "HandlePropertySelectorsAttribute.OnActionExecuted", 1973210, 10, true))
      {
        IDictionary<string, object> values = actionExecutedContext.ActionContext.ControllerContext.RouteData.Values;
        releaseManagementTimer.SetAdditionalData<string>("ControllerName", values["controller"].ToString());
        object obj = values["resource"];
        if (obj != null)
          releaseManagementTimer.SetAdditionalData<string>("ResourceName", obj.ToString());
        else
          releaseManagementTimer.SetAdditionalData<string>("ResourceName", (string) null);
        try
        {
          ObjectContent content = (ObjectContent) actionExecutedContext.Response.Content;
          int byteCount = Encoding.Unicode.GetByteCount(JsonConvert.SerializeObject(content.Value));
          releaseManagementTimer.RecordLap("Service", "HandlePropertySelectorsAttribute.StringifyOriginalResponse", 1973210);
          List<PropertySelector> propertySelectors = JsonConvert.DeserializeObject<List<PropertySelector>>(str);
          PropertySelectorsHelper.HandlePropertySelection(content.Value, (IList<PropertySelector>) propertySelectors);
          releaseManagementTimer.RecordLap("Service", "HandlePropertySelectorsAttribute.ApplyPropertySelector", 1973210);
          int num = byteCount - Encoding.Unicode.GetByteCount(JsonConvert.SerializeObject(content.Value));
          releaseManagementTimer.RecordLap("Service", "HandlePropertySelectorsAttribute.ApplyPropertySelector", 1973210);
          releaseManagementTimer.SetAdditionalData<int>("MemoryGain", num);
        }
        catch (Exception ex)
        {
          if (controller1 == null)
            return;
          controller1.TfsRequestContext.RootContext.TraceException(1961074, "ReleaseManagementService", "Service", ex);
        }
      }
    }
  }
}
