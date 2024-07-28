// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildHttpClient
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [ResourceArea("965220D5-5BB9-42CF-8D67-9B146DF2A5A4")]
  public class BuildHttpClient : BuildHttpClientBase
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();

    static BuildHttpClient()
    {
      BuildHttpClient.s_translatedExceptions.Add("BuildNotFoundException", typeof (BuildNotFoundException));
      BuildHttpClient.s_translatedExceptions.Add("BuildRequestNotFoundException", typeof (BuildRequestNotFoundException));
      BuildHttpClient.s_translatedExceptions.Add("BuildDefinitionDoesNotExistException", typeof (BuildDefinitionDoesNotExistException));
      BuildHttpClient.s_translatedExceptions.Add("BuildDefinitionDisabledException", typeof (BuildDefinitionDisabledException));
      BuildHttpClient.s_translatedExceptions.Add("BuildRequestUpdateException", typeof (BuildRequestUpdateException));
      BuildHttpClient.s_translatedExceptions.Add("BuildRequestPropertyInvalidException", typeof (BuildRequestPropertyInvalidException));
      BuildHttpClient.s_translatedExceptions.Add("DefinitionNotFoundException", typeof (DefinitionNotFoundException));
      BuildHttpClient.s_translatedExceptions.Add("BuildControllerNotFoundException", typeof (BuildControllerNotFoundException));
      BuildHttpClient.s_translatedExceptions.Add("BuildControllerDeletionException", typeof (BuildControllerDeletionException));
      BuildHttpClient.s_translatedExceptions.Add("BuildControllerUpdateException", typeof (BuildControllerUpdateException));
      BuildHttpClient.s_translatedExceptions.Add("BuildAgentNotFoundException", typeof (BuildAgentNotFoundException));
      BuildHttpClient.s_translatedExceptions.Add("BuildAgentDeletionException", typeof (BuildAgentDeletionException));
      BuildHttpClient.s_translatedExceptions.Add("BuildAgentUpdateException", typeof (BuildAgentUpdateException));
      BuildHttpClient.s_translatedExceptions.Add("BuildServerNotFoundException", typeof (BuildServerNotFoundException));
      BuildHttpClient.s_translatedExceptions.Add("CannotDeleteDefinitionBuildExistsException", typeof (CannotDeleteDefinitionBuildExistsException));
      BuildHttpClient.s_translatedExceptions.Add("BuildProcessTemplateNotFoundException", typeof (BuildProcessTemplateNotFoundException));
      BuildHttpClient.s_translatedExceptions.Add("BuildProcessTemplateDeletionException", typeof (BuildProcessTemplateDeletionException));
      BuildHttpClient.s_translatedExceptions.Add("AccessDeniedException", typeof (AccessDeniedException));
    }

    public BuildHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public BuildHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public BuildHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public BuildHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public BuildHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsByIdAsync(List<int> ids)
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = "";
      bool flag = true;
      foreach (int id in ids)
      {
        if (id <= 0)
          throw new ArgumentException(id.ToString(), nameof (ids));
        if (!flag)
          str += ",";
        else
          flag = false;
        str += id.ToString();
      }
      keyValuePairList.Add(nameof (ids), str);
      return this.SendAsync<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(HttpMethod.Get, BuildResourceIds.Builds, version: new ApiResourceVersion("1.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList);
    }

    public Task<List<BuildRequest>> GetRequestsByIdAsync(List<int> ids)
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = "";
      bool flag = true;
      foreach (int id in ids)
      {
        if (id <= 0)
          throw new ArgumentException(id.ToString(), nameof (ids));
        if (!flag)
          str += ",";
        else
          flag = false;
        str += id.ToString();
      }
      keyValuePairList.Add(nameof (ids), str);
      return this.SendAsync<List<BuildRequest>>(HttpMethod.Get, BuildResourceIds.Requests, version: new ApiResourceVersion("1.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList);
    }

    public Task<BuildRequest> CreateRequestAsync(
      int definitionId,
      QueuePriority priority = QueuePriority.Normal,
      BuildReason reason = BuildReason.Manual)
    {
      BuildRequest postContract = new BuildRequest();
      BuildRequest buildRequest = postContract;
      DefinitionReference definitionReference = new DefinitionReference();
      definitionReference.Id = definitionId;
      buildRequest.Definition = (ShallowReference) definitionReference;
      postContract.Priority = priority;
      postContract.Reason = reason;
      ObjectContent<BuildRequest> objectContent = new ObjectContent<BuildRequest>(postContract, this.Formatter);
      return this.CreateRequestAsync(postContract);
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) BuildHttpClient.s_translatedExceptions;
  }
}
