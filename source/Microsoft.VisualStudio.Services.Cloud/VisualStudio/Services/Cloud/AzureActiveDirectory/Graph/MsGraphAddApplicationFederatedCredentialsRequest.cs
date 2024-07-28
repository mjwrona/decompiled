// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphAddApplicationFederatedCredentialsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MsGraphAddApplicationFederatedCredentialsRequest : 
    MicrosoftGraphClientRequest<MsGraphAddApplicationFederatedCredentialsResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphAddApplicationFederatedCredentialsRequest";

    public Guid AppObjectId { get; set; }

    public string FederationName { get; set; }

    public string FederationDescription { get; set; }

    public string FederationSubject { get; set; }

    public string FederationAudience { get; set; }

    public string FederationIssuer { get; set; }

    public override string ToString() => string.Format("AddApplicationFederatedCredentialsRequest{{Application ObjectId={0}}}", (object) this.AppObjectId);

    internal override void Validate()
    {
      ArgumentUtility.CheckForEmptyGuid(this.AppObjectId, "AppObjectId");
      ArgumentUtility.CheckStringForNullOrEmpty(this.FederationIssuer, "FederationIssuer");
      ArgumentUtility.CheckStringForNullOrEmpty(this.FederationName, "FederationName");
      ArgumentUtility.CheckStringForNullOrEmpty(this.FederationDescription, "FederationDescription");
      ArgumentUtility.CheckStringForNullOrEmpty(this.FederationSubject, "FederationSubject");
      ArgumentUtility.CheckStringForNullOrEmpty(this.FederationAudience, "FederationAudience");
    }

    internal override MsGraphAddApplicationFederatedCredentialsResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.Trace(44750230, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphAddApplicationFederatedCredentialsRequest), "Entering Microsoft Graph API for Add Application Federated Credentials.");
        FederatedIdentityCredential credential = new FederatedIdentityCredential()
        {
          Name = this.FederationName,
          Description = this.FederationDescription,
          Subject = this.FederationSubject,
          Issuer = this.FederationIssuer,
          Audiences = (IEnumerable<string>) new string[1]
          {
            this.FederationAudience
          }
        };
        IApplicationFederatedIdentityCredentialsCollectionRequest federatedIdentityRequest = graphServiceClient.Applications[this.AppObjectId.ToString()].FederatedIdentityCredentials.Request();
        credential = context.RunSynchronously<FederatedIdentityCredential>((Func<Task<FederatedIdentityCredential>>) (() => federatedIdentityRequest.AddAsync(credential, new CancellationToken())));
        return credential != null ? new MsGraphAddApplicationFederatedCredentialsResponse()
        {
          FederatedIdentityCredentialCreated = MicrosoftGraphConverters.ConvertFederatedIdentityCredential(credential)
        } : throw new MicrosoftGraphException("Microsoft Graph API Create Application call returned null.");
      }
      finally
      {
        context.Trace(44750232, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphAddApplicationFederatedCredentialsRequest), "Leaving Microsoft Graph API for Add Application Federated Identity Credential.");
      }
    }
  }
}
