// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphRemoveApplicationFederatedCredentialsRequest
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
  public class MsGraphRemoveApplicationFederatedCredentialsRequest : 
    MicrosoftGraphClientRequest<MsGraphRemoveApplicationFederatedCredentialsResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphRemoveApplicationFederatedCredentialsRequest";

    public Guid AppObjectId { get; set; }

    public string FederationName { get; set; }

    public override string ToString() => string.Format("RemoveApplicationFederatedCredentialsRequest{{Application ObjectId={0}}}", (object) this.AppObjectId);

    internal override void Validate()
    {
      ArgumentUtility.CheckForEmptyGuid(this.AppObjectId, "AppObjectId");
      ArgumentUtility.CheckStringForNullOrEmpty(this.FederationName, "FederationName");
    }

    internal override MsGraphRemoveApplicationFederatedCredentialsResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.Trace(44750233, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphRemoveApplicationFederatedCredentialsRequest), "Entering Microsoft Graph API for Remove Application Federated Credentials.");
        IFederatedIdentityCredentialRequest federatedIdentityRequest = graphServiceClient.Applications[this.AppObjectId.ToString()].FederatedIdentityCredentials[this.FederationName].Request();
        context.RunSynchronously((Func<Task>) (() => federatedIdentityRequest.DeleteAsync(new CancellationToken())));
        return new MsGraphRemoveApplicationFederatedCredentialsResponse()
        {
          Success = true
        };
      }
      catch (ServiceException ex) when (ex.IsBadRequest() || ex.IsResourceNotFoundError())
      {
        context.TraceAlways(44750191, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphRemoveApplicationFederatedCredentialsRequest), string.Format("Failed to remove application federated credential '{0}' for Application '{1}' bacause application or key does not exist, consider success. Exception: '{2}'.", (object) this.FederationName, (object) this.AppObjectId, (object) ex));
        return new MsGraphRemoveApplicationFederatedCredentialsResponse()
        {
          Success = true
        };
      }
      finally
      {
        context.Trace(44750235, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphRemoveApplicationFederatedCredentialsRequest), "Leaving Microsoft Graph API for Remove Application Federated Identity Credential.");
      }
    }
  }
}
