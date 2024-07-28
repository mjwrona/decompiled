// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MicrosoftGraph.MsGraphGetProfileDataRequest
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
  public class MsGraphGetProfileDataRequest : 
    MicrosoftGraphClientRequest<MsGraphGetProfileDataResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetProfileDataRequest";
    private const string PrimaryMailType = "main";

    public Guid ObjectId { get; set; }

    internal override MsGraphGetProfileDataResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        context.Trace(44750260, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetProfileDataRequest), "Entering Microsoft Graph API for Get Profile Data.");
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, ((BaseClient) graphServiceClient).BaseUrl + "/users/" + this.ObjectId.ToString() + "/profile");
        context.RunSynchronously((Func<Task>) (() => ((BaseClient) graphServiceClient).AuthenticationProvider.AuthenticateRequestAsync(requestMessage)));
        HttpResponseMessage httpResponseMessage = context.RunSynchronously<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (() => ((BaseClient) graphServiceClient).HttpProvider.SendAsync(requestMessage)));
        httpResponseMessage.EnsureSuccessStatusCode();
        GetProfileDataResponseObject dataResponseObject = JsonConvert.DeserializeObject<GetProfileDataResponseObject>(context.RunSynchronously<string>(new Func<Task<string>>(httpResponseMessage.Content.ReadAsStringAsync)));
        IEnumerable<NamesDetails> names = dataResponseObject.Names;
        string displayName = names != null ? names.FirstOrDefault<NamesDetails>()?.DisplayName : (string) null;
        IEnumerable<EmailsDetails> emails = dataResponseObject.Emails;
        string address = emails != null ? emails.FirstOrDefault<EmailsDetails>((Func<EmailsDetails, bool>) (mail => mail.Type.Equals("main", StringComparison.OrdinalIgnoreCase)))?.Address : (string) null;
        return !string.IsNullOrWhiteSpace(displayName) && !string.IsNullOrWhiteSpace(address) ? new MsGraphGetProfileDataResponse()
        {
          DisplayName = displayName,
          Mail = address
        } : throw new MicrosoftGraphException("Failed to Get Profile Data: connection returned null or empty response for DisplayName or Email of the profile!");
      }
      catch (Exception ex)
      {
        context.TraceException(44750261, TraceLevel.Error, "MicrosoftGraphClientRequest", nameof (MsGraphGetProfileDataRequest), ex);
        throw;
      }
      finally
      {
        context.Trace(44750262, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetProfileDataRequest), "Leaving Microsoft Graph API for Get Profile Data.");
      }
    }

    public override string ToString() => "MsGraphGetProfileDataRequest{" + string.Format("ObjectId={0}", (object) this.ObjectId) + "}";
  }
}
