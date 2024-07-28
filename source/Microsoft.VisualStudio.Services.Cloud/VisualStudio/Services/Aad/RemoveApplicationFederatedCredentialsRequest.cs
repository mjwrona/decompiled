// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.RemoveApplicationFederatedCredentialsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using System;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class RemoveApplicationFederatedCredentialsRequest : AadServiceRequest
  {
    public Guid AppObjectId { get; set; }

    public string FederationName { get; set; }

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      MsGraphRemoveApplicationFederatedCredentialsRequest credentialsRequest = new MsGraphRemoveApplicationFederatedCredentialsRequest();
      credentialsRequest.AccessToken = context.GetAccessToken(true);
      credentialsRequest.AppObjectId = this.AppObjectId;
      credentialsRequest.FederationName = this.FederationName;
      MsGraphRemoveApplicationFederatedCredentialsRequest request = credentialsRequest;
      MsGraphRemoveApplicationFederatedCredentialsResponse credentialsResponse = context.GetMsGraphClient().RemoveApplicationFederatedCredentials(context.VssRequestContext, request);
      return (AadServiceResponse) new RemoveApplicationFederatedCredentialsResponse()
      {
        Success = credentialsResponse.Success
      };
    }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.MicrosoftGraphOnly;
  }
}
