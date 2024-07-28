// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssfServiceAuthorizationManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssfServiceAuthorizationManager : ServiceAuthorizationManager
  {
    protected override bool CheckAccessCore(OperationContext operationContext)
    {
      try
      {
        if (HttpContext.Current?.ApplicationInstance is VisualStudioServicesApplication applicationInstance)
        {
          IVssRequestContext vssRequestContext = applicationInstance.VssRequestContext;
          if (vssRequestContext != null)
            vssRequestContext.ValidateIdentity();
        }
      }
      catch (UnauthorizedRequestException ex)
      {
        WebOperationContext current = WebOperationContext.Current;
        if (current != null)
          current.OutgoingResponse.StatusCode = ex.HttpStatusCode;
        throw;
      }
      return base.CheckAccessCore(operationContext);
    }
  }
}
