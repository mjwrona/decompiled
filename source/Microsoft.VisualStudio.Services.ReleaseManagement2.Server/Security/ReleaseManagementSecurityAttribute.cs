// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security.ReleaseManagementSecurityAttribute
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true)]
  public sealed class ReleaseManagementSecurityAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      Collection<ReleaseManagementSecurityPermissionAttribute> collection = actionContext != null ? actionContext.ActionDescriptor.GetCustomAttributes<ReleaseManagementSecurityPermissionAttribute>() : throw new ArgumentNullException(nameof (actionContext));
      if (collection == null || collection.Count != 1)
      {
        ReleaseManagementException innerException = new ReleaseManagementException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Security attribute {0} does not present in {1}", (object) typeof (ReleaseManagementSecurityPermissionAttribute).Name, (object) actionContext.ActionDescriptor.ActionName));
        throw new UnauthorizedRequestException(innerException.Message, (Exception) innerException);
      }
      if (actionContext.ControllerContext.Controller is TfsProjectApiController controller)
      {
        if (!ReleaseManagementSecurityProcessor.CheckAttributePermission(controller.TfsRequestContext.RootContext, collection[0], actionContext.ActionArguments, controller.ProjectId, ReleaseManagementSecurityAttribute.IsByePassDataspaceFaultInHeaderSet(collection[0], actionContext)))
        {
          ResourceAccessException innerException = new ResourceAccessException(controller.TfsRequestContext.RootContext.GetUserId().ToString(), collection[0].Permission);
          throw new UnauthorizedRequestException(innerException.Message, (Exception) innerException);
        }
        base.OnActionExecuting(actionContext);
      }
      else
      {
        InvalidOperationException innerException = new InvalidOperationException("Invalid controller type");
        throw new UnauthorizedRequestException(innerException.Message, (Exception) innerException);
      }
    }

    private static bool IsByePassDataspaceFaultInHeaderSet(
      ReleaseManagementSecurityPermissionAttribute securityAttribute,
      HttpActionContext actionContext)
    {
      IEnumerable<string> values;
      if (securityAttribute == null || !securityAttribute.AllowByePassDataspaceFaultIn || !actionContext.Request.Headers.TryGetValues("x-ms-byepassrmfaultin", out values) || values == null)
        return false;
      string a = values.FirstOrDefault<string>();
      return a != null && string.Equals(a, "true", StringComparison.OrdinalIgnoreCase);
    }
  }
}
