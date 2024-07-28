// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MicrosoftGraph.MsGraphGetOrganizationDataBoundaryRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MicrosoftGraph
{
  public class MsGraphGetOrganizationDataBoundaryRequest : 
    MicrosoftGraphClientRequest<MsGraphGetOrganizationDataBoundaryResponse>
  {
    private const string PropertiesToFetch = "dataBoundary";
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetOrganizationDataBoundaryRequest";

    internal override MsGraphGetOrganizationDataBoundaryResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.Trace(44750250, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetOrganizationDataBoundaryRequest), "Entering Microsoft Graph API for Get Organization.");
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, ((BaseClient) graphServiceClient).BaseUrl + "/organization?$select=dataBoundary");
        context.RunSynchronously((Func<Task>) (() => ((BaseClient) graphServiceClient).AuthenticationProvider.AuthenticateRequestAsync(requestMessage)));
        HttpResponseMessage httpResponseMessage = context.RunSynchronously<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (() => ((BaseClient) graphServiceClient).HttpProvider.SendAsync(requestMessage)));
        httpResponseMessage.EnsureSuccessStatusCode();
        IEnumerable<OrganizationDetails> source = JsonConvert.DeserializeObject<GetOrganizationDetailsResponseObject>(context.RunSynchronously<string>(new Func<Task<string>>(httpResponseMessage.Content.ReadAsStringAsync))).Value;
        string dataBoundary = source != null ? source.FirstOrDefault<OrganizationDetails>()?.DataBoundary : (string) null;
        bool flag = !string.IsNullOrWhiteSpace(dataBoundary) && !dataBoundary.Equals("none", StringComparison.OrdinalIgnoreCase);
        return new MsGraphGetOrganizationDataBoundaryResponse()
        {
          DataBoundary = flag ? dataBoundary : string.Empty
        };
      }
      catch
      {
        context.Trace(44750251, TraceLevel.Error, "MicrosoftGraphClientRequest", nameof (MsGraphGetOrganizationDataBoundaryRequest), "Error during execution of Microsoft Graph API for Get Organization!");
        throw;
      }
      finally
      {
        context.Trace(44750252, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetOrganizationDataBoundaryRequest), "Leaving Microsoft Graph API for Get Organization.");
      }
    }

    public override string ToString() => "MsGraphGetOrganizationRequest{}";
  }
}
