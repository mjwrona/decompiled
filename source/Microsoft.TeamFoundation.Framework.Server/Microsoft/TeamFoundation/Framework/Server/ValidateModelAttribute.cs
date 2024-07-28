// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ValidateModelAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ValidateModelAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      if (actionContext.ModelState.IsValid)
        return;
      ModelStateDictionary modelState1 = actionContext.ModelState;
      StringBuilder stringBuilder = new StringBuilder();
      if (modelState1 != null)
      {
        foreach (ModelState modelState2 in (IEnumerable<ModelState>) modelState1.Values)
        {
          foreach (ModelError error in (Collection<ModelError>) modelState2.Errors)
          {
            if (error.Exception != null)
              stringBuilder.AppendLine(error.Exception.Message);
            else
              stringBuilder.AppendLine(error.ErrorMessage);
          }
        }
      }
      actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, stringBuilder.ToString());
    }
  }
}
