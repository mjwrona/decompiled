// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Authentication.ConfidentialClientApplicationProviderTestJob
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud.Authentication
{
  public class ConfidentialClientApplicationProviderTestJob : ITeamFoundationJobExtension
  {
    private const int Tracepoint = 15104963;
    private const string c_area = "ConfidentialClientApplicationProviderTest";
    private const string c_layer = "ConfidentialClientApplicationProviderTestJob";
    private const string c_registryBasePath = "/Configuration/Settings/ConfidentialClientApplicationProviderTestJob/";

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      ConfidentialClientApplicationProvider applicationProvider = new ConfidentialClientApplicationProvider();
      ConfidentialClientApplicationAuthData authData = new ConfidentialClientApplicationAuthData()
      {
        TenantAuthority = new Uri(service.GetValue<string>(vssRequestContext, (RegistryQuery) "/Configuration/Settings/ConfidentialClientApplicationProviderTestJob/TenantAuthority", false, (string) null)),
        ClientId = service.GetValue<string>(vssRequestContext, (RegistryQuery) "/Configuration/Settings/ConfidentialClientApplicationProviderTestJob/ClientId", false, (string) null),
        CredentialType = service.GetValue<ConfidentialClientApplicationCredentialType>(vssRequestContext, (RegistryQuery) "/Configuration/Settings/ConfidentialClientApplicationProviderTestJob/CredentialType", false, ConfidentialClientApplicationCredentialType.StrongBoxCertificate),
        StrongBoxLookupKey = service.GetValue<string>(vssRequestContext, (RegistryQuery) "/Configuration/Settings/ConfidentialClientApplicationProviderTestJob/StrongBoxLookupKey", false, (string) null)
      };
      vssRequestContext.TraceAlways(15104963, TraceLevel.Info, "ConfidentialClientApplicationProviderTest", nameof (ConfidentialClientApplicationProviderTestJob), string.Format("Authenticating (tenantAuthority={0}, clientId={1}, credentialType={2}, strongBoxLookupKey={3})", (object) authData.TenantAuthority, (object) authData.ClientId, (object) authData.CredentialType, (object) authData.StrongBoxLookupKey));
      if ((object) authData.TenantAuthority == null || authData.ClientId == null)
      {
        resultMessage = "Required parameters not populated in registry.";
        return TeamFoundationJobExecutionResult.Failed;
      }
      AuthenticationResult result = applicationProvider.GetApplication(requestContext, authData).AcquireTokenForClient((IEnumerable<string>) new string[1]
      {
        authData.ClientId + "/.default"
      }).ExecuteAsync().ConfigureAwait(false).GetAwaiter().GetResult();
      string str = (result.AccessToken ?? "").PadRight(8).Substring(0, 8);
      vssRequestContext.TraceAlways(15104963, TraceLevel.Info, "ConfidentialClientApplicationProviderTest", nameof (ConfidentialClientApplicationProviderTestJob), string.Format("Authenticated to tenantId={0}, objectId={1}, tokenType={2}, correlationId={3}, expiresOn={4}, Scopes={5}, tokenSneakPeek={6}", (object) result.TenantId, (object) result.UniqueId, (object) result.TokenType, (object) result.CorrelationId, (object) result.ExpiresOn, (object) string.Join(",", result.Scopes), (object) str));
      resultMessage = "Auth successfull";
      return TeamFoundationJobExecutionResult.Succeeded;
    }
  }
}
