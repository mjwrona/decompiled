// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Configuration.RemoveMruEntryonDeletedProjectTeamAttribute
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Configuration
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class RemoveMruEntryonDeletedProjectTeamAttribute : FilterAttribute, IExceptionFilter
  {
    public void OnException(ExceptionContext filterContext)
    {
      if (!(filterContext.Controller is TfsController controller))
        return;
      Exception exception = filterContext.Exception;
      if (filterContext.Exception is HttpException)
        exception = filterContext.Exception.InnerException;
      switch (exception)
      {
        case ProjectDoesNotExistWithNameException _:
        case TeamDoesNotExistWithNameException _:
        case UnauthorizedAccessException _:
          string projectName = controller.Request.RequestContext.RouteData.Values.GetValue<string>("project");
          string teamName = controller.Request.RequestContext.RouteData.Values.GetValue<string>("team");
          if (string.IsNullOrEmpty(projectName))
            break;
          MRUNavigationContextEntryManager.RemoveMRUNavigationContextsForProject(controller.Request.RequestContext.TfsRequestContext(true), projectName, teamName);
          break;
      }
    }
  }
}
