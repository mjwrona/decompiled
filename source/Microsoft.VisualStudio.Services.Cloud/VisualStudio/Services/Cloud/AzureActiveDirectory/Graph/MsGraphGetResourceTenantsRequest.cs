// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetResourceTenantsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MsGraphGetResourceTenantsRequest : 
    MicrosoftGraphClientRequest<MsGraphGetResourceTenantsResponse>
  {
    private const string PropertiesToFetch = "tenantId,displayName";
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetResourceTenantsRequest";

    public Microsoft.VisualStudio.Services.Aad.Graph.IdentifierType IdentifierType { get; set; }

    public string Identifier { get; set; }

    public string HomeTenantId { get; set; }

    internal override void Validate()
    {
      if (this.IdentifierType != Microsoft.VisualStudio.Services.Aad.Graph.IdentifierType.LiveId && this.IdentifierType != Microsoft.VisualStudio.Services.Aad.Graph.IdentifierType.OrgId)
        throw new ArgumentException("Unsupported identifier type: " + this.IdentifierType.ToString());
      ArgumentUtility.CheckStringForNullOrWhiteSpace(this.Identifier, "Identifier");
    }

    public override string ToString() => string.Format("GetResourceTenants IdentifierType={0},Identifier={1},HomeTenantId={2}", (object) this.IdentifierType, (object) this.Identifier, (object) this.HomeTenantId);

    internal override MsGraphGetResourceTenantsResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.TraceConditionally(44750240, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetResourceTenantsRequest), (Func<string>) (() => "Entering Microsoft Graph API for GetResourceTenants"));
        string base64String = Convert.ToBase64String(HexConverter.ToByteArray(this.Identifier));
        GetResourceTenantsRequest resourceTenantsRequest = new GetResourceTenantsRequest()
        {
          AlternativeSecurityId = new AlternativeSecurityId()
          {
            Type = this.IdentifierType == Microsoft.VisualStudio.Services.Aad.Graph.IdentifierType.OrgId ? 5 : 1,
            IdentityProvider = (string) null,
            Key = base64String
          },
          HomeTenantId = this.HomeTenantId
        };
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, ((BaseClient) graphServiceClient).BaseUrl + "/tenantRelationships/getResourceTenants?$select=tenantId,displayName");
        requestMessage.Content = (HttpContent) new StringContent(JsonConvert.SerializeObject((object) resourceTenantsRequest), Encoding.UTF8, "application/json");
        context.RunSynchronously((Func<Task>) (() => ((BaseClient) graphServiceClient).AuthenticationProvider.AuthenticateRequestAsync(requestMessage)));
        HttpResponseMessage response = context.RunSynchronously<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (() => ((BaseClient) graphServiceClient).HttpProvider.SendAsync(requestMessage)));
        response.EnsureSuccessStatusCode();
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return new MsGraphGetResourceTenantsResponse()
        {
          Tenants = JsonConvert.DeserializeObject<ResourceTenantDetailCollection>(context.RunSynchronously<string>((Func<Task<string>>) (() => response.Content.ReadAsStringAsync()))).Value.Select<ResourceTenantDetail, AadTenant>(MsGraphGetResourceTenantsRequest.\u003C\u003EO.\u003C0\u003E__ConvertResourceTenant ?? (MsGraphGetResourceTenantsRequest.\u003C\u003EO.\u003C0\u003E__ConvertResourceTenant = new Func<ResourceTenantDetail, AadTenant>(MicrosoftGraphConverters.ConvertResourceTenant))) ?? throw new MicrosoftGraphException("Failed to Get Resource Tenants: connection returned null response.")
        };
      }
      finally
      {
        context.TraceConditionally(44750241, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetResourceTenantsRequest), (Func<string>) (() => "Leaving Microsoft Graph API for GetResourceTenants"));
      }
    }
  }
}
