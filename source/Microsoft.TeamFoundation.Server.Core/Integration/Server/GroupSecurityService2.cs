// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.GroupSecurityService2
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Integration.Server
{
  [Obsolete("GroupSecurityService is obsolete.  Please use the IdentityManagementService or SecurityService instead.", false)]
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/GroupSecurity/03", Description = "DevOps extended Group Security web service")]
  internal class GroupSecurityService2 : GroupSecurityService
  {
    [WebMethod]
    public int GetIdentityChanges(int sequenceId, out StreamingCollection<Identity> identities)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetIdentityChanges), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (sequenceId), (object) sequenceId);
        this.EnterMethod(methodInformation);
        if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment || !this.AreDeprecatedMethodsEnabled(this.RequestContext))
          throw new DeprecatedIdentityMethodException();
        SecurityValidation.CheckSequenceId(sequenceId, nameof (sequenceId));
        this.RequestContext.GetService<IntegrationSecurityManager>().CheckGlobalPermission(this.RequestContext, "SYNCHRONIZE_READ");
        TeamFoundationGroupSecurityService service = this.RequestContext.GetService<TeamFoundationGroupSecurityService>();
        TeamFoundationDataReader resource = (TeamFoundationDataReader) null;
        try
        {
          resource = service.GetIdentityChanges(this.RequestContext, sequenceId);
          this.AddWebServiceResource((IDisposable) resource);
        }
        catch (Exception ex)
        {
          resource?.Dispose();
          throw;
        }
        int identityChanges = resource.Current<int>();
        resource.MoveNext();
        identities = resource.Current<StreamingCollection<Identity>>();
        return identityChanges;
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
