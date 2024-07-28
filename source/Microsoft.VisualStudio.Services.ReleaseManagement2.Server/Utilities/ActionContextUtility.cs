// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ActionContextUtility
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.ObjectModel;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class ActionContextUtility
  {
    public static bool TryGetControllerResourceName(
      this HttpActionContext actionExecutingContext,
      out string controllerResourceName)
    {
      if (actionExecutingContext == null)
        throw new ArgumentNullException(nameof (actionExecutingContext));
      Collection<VersionedApiControllerCustomNameAttribute> customAttributes = actionExecutingContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<VersionedApiControllerCustomNameAttribute>();
      if (customAttributes != null || customAttributes.Count == 1)
        controllerResourceName = customAttributes[0].ResourceName;
      else
        actionExecutingContext.RequestContext.RouteData.Values.TryGetValue<string>("resource", out controllerResourceName);
      controllerResourceName = string.IsNullOrWhiteSpace(controllerResourceName) ? string.Empty : controllerResourceName;
      return !string.IsNullOrWhiteSpace(controllerResourceName);
    }
  }
}
