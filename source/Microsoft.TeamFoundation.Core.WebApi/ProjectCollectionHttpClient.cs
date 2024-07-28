// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.ProjectCollectionHttpClient
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Collection;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [ResourceArea("79134C72-4A58-4B42-976C-04E7115F32BF")]
  public class ProjectCollectionHttpClient : VssHttpClientBase
  {
    private static readonly ApiResourceVersion s_resourceVersion = new ApiResourceVersion("1.0-preview.2");
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>()
    {
      {
        "CollectionDoesNotExistException",
        typeof (CollectionDoesNotExistException)
      }
    };

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) ProjectCollectionHttpClient.s_translatedExceptions;

    public ProjectCollectionHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ProjectCollectionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ProjectCollectionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ProjectCollectionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ProjectCollectionHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<TeamProjectCollection> GetProjectCollection(string id, object userState = null)
    {
      Guid collectionsLocationId = CoreConstants.ProjectCollectionsLocationId;
      ApiResourceVersion resourceVersion = ProjectCollectionHttpClient.s_resourceVersion;
      var routeValues = new{ collectionId = id };
      ApiResourceVersion version = resourceVersion;
      object userState1 = userState;
      CancellationToken cancellationToken = new CancellationToken();
      return this.GetAsync<TeamProjectCollection>(collectionsLocationId, (object) routeValues, version, userState: userState1, cancellationToken: cancellationToken);
    }

    public Task<IEnumerable<TeamProjectCollectionReference>> GetProjectCollections(
      int? top = null,
      int? skip = null,
      object userState = null)
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      QueryParamHelper.AddNonNullParam(keyValuePairList, "$top", (object) top);
      QueryParamHelper.AddNonNullParam(keyValuePairList, "$skip", (object) skip);
      return this.GetAsync<IEnumerable<TeamProjectCollectionReference>>(CoreConstants.ProjectCollectionsLocationId, version: ProjectCollectionHttpClient.s_resourceVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState);
    }
  }
}
