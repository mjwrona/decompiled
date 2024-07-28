// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AzureInstanceMetadataProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class AzureInstanceMetadataProvider : IAzureInstanceMetadataProvider
  {
    public static readonly string InstanceMetadataEndpoint = "http://169.254.169.254/metadata";
    private const string c_multipleUserAssignedIdentitiesExist = "Multiple user assigned identities exist, please specify the clientId / resourceId of the identity in the token request";
    private const string c_identityNotFound = "\"Identity not found\"";

    public AzureInstanceMetadataProvider(HttpClient httpClient, string version = "2018-02-01")
    {
      this.Client = httpClient;
      this.Version = version;
    }

    private HttpClient Client { get; }

    public string Version { get; }

    public bool HasMetadata()
    {
      try
      {
        return this.GetMetadata("instance", new Dictionary<string, string>()
        {
          ["format"] = "text"
        }) != null;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public string GetMetadata(string category, Dictionary<string, string> parameters = null)
    {
      if (parameters == null)
        parameters = new Dictionary<string, string>();
      HttpResponseMessage result1 = this.Client.SendAsync(this.BuildRequest(AzureInstanceMetadataProvider.InstanceMetadataEndpoint + "/" + category, parameters)).Result;
      if (!result1.IsSuccessStatusCode)
      {
        string result2 = result1.Content.ReadAsStringAsync().Result;
        if (result2 != null)
        {
          if (result2.Contains("Multiple user assigned identities exist, please specify the clientId / resourceId of the identity in the token request"))
            throw new MultipleUserAssignedManagedIdentitiesExistException();
          string str;
          if (result2.Contains("\"Identity not found\"") && parameters.TryGetValue("client_id", out str))
            throw new ManagedIdentityNotFoundException(FrameworkResources.ManagedIdentityNotFound((object) str));
        }
        throw new Exception(string.Format("Error retrieving metadata category {0}. Received status {1}: {2}", (object) category, (object) result1.StatusCode, (object) result2));
      }
      return result1.Content.ReadAsStringAsync().Result;
    }

    private HttpRequestMessage BuildRequest(string url, Dictionary<string, string> parameters)
    {
      UriBuilder uriBuilder = new UriBuilder(url);
      NameValueCollection queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
      if (!parameters.ContainsKey("api-version"))
        parameters.Add("api-version", this.Version);
      foreach (KeyValuePair<string, string> parameter in parameters)
        queryString[parameter.Key] = parameter.Value;
      uriBuilder.Query = queryString.ToString();
      HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
      httpRequestMessage.Headers.Add("Metadata", "true");
      return httpRequestMessage;
    }
  }
}
