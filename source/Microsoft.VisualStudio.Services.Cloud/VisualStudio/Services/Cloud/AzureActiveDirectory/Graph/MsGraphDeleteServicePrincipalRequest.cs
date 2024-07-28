// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphDeleteServicePrincipalRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MsGraphDeleteServicePrincipalRequest : 
    MicrosoftGraphClientRequest<MsGraphDeleteServicePrincipalResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphDeleteServicePrincipalRequest";

    public Guid ServicePrincipalObjectId { get; set; }

    public override string ToString() => string.Format("DeleteServicePrincipalRequest{{ServicePrincipal ObjectId={0}}}", (object) this.ServicePrincipalObjectId);

    internal override void Validate() => ArgumentUtility.CheckForEmptyGuid(this.ServicePrincipalObjectId, "ServicePrincipalObjectId");

    internal override MsGraphDeleteServicePrincipalResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.Trace(44750210, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphDeleteServicePrincipalRequest), "Entering Microsoft Graph API for Delete Service Principal.");
        IServicePrincipalRequest applicationRequest = graphServiceClient.ServicePrincipals[this.ServicePrincipalObjectId.ToString()].Request();
        context.RunSynchronously((Func<Task>) (() => applicationRequest.DeleteAsync(new CancellationToken())));
        return new MsGraphDeleteServicePrincipalResponse()
        {
          Success = true
        };
      }
      catch (ServiceException ex) when (ex.IsResourceNotFoundError())
      {
        context.Trace(44750211, TraceLevel.Warning, "MicrosoftGraphClientRequest", nameof (MsGraphDeleteServicePrincipalRequest), string.Format("Failed to delete service principal for '{0}' bacause service principal does not exist, consider success. Exception: '{1}'.", (object) this.ServicePrincipalObjectId, (object) ex));
        return new MsGraphDeleteServicePrincipalResponse()
        {
          Success = true
        };
      }
      finally
      {
        context.Trace(44750212, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphDeleteServicePrincipalRequest), "Leaving Microsoft Graph API for Delete Service Principal.");
      }
    }
  }
}
