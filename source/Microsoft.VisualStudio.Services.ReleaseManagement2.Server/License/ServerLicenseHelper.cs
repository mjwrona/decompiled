// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.License.ServerLicenseHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.License
{
  public static class ServerLicenseHelper
  {
    public static void CheckRequestAllowedForAnyUserLicense(
      this HttpActionContext actionExecutingContext)
    {
      if (actionExecutingContext == null)
        throw new ArgumentNullException(nameof (actionExecutingContext));
      string method = actionExecutingContext.Request.Method.Method;
      if (method.Equals(HttpMethod.Get.Method, StringComparison.OrdinalIgnoreCase))
      {
        string controllerResourceName;
        if (actionExecutingContext.TryGetControllerResourceName(out controllerResourceName) && controllerResourceName.Equals("changes", StringComparison.OrdinalIgnoreCase))
          throw new MissingLicenseException(Resources.ReleaseManagementViewChangesFeature);
      }
      else
      {
        string str = string.Empty;
        object obj;
        if (actionExecutingContext.Request.Properties != null && actionExecutingContext.Request.Properties.TryGetValue("MS_HttpActionDescriptor", out obj) && obj != null)
          str = !(obj is ReflectedHttpActionDescriptor actionDescriptor) ? string.Empty : actionDescriptor.ActionName;
        string controllerResourceName;
        if (actionExecutingContext.TryGetControllerResourceName(out controllerResourceName))
        {
          if (controllerResourceName.Equals("approvals", StringComparison.OrdinalIgnoreCase) || str.Equals("UpdatePipelineEnvironment", StringComparison.OrdinalIgnoreCase))
            return;
          if (method.Equals("patch", StringComparison.OrdinalIgnoreCase) && controllerResourceName.Equals("environments", StringComparison.OrdinalIgnoreCase))
          {
            ReleaseEnvironmentUpdateMetadata environmentUpdateData = (ReleaseEnvironmentUpdateMetadata) null;
            if (actionExecutingContext != null && RmReleaseEnvironmentsController.TryGetEnvironmentUpdateData(actionExecutingContext, out environmentUpdateData) && environmentUpdateData.IsApprovalScheduledDeploymentUpdate())
              return;
          }
        }
        throw new MissingLicenseException(Resources.ReleaseManagementFeatureName);
      }
    }
  }
}
