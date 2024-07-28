// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.DeploymentHttpClient
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts;
using Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi
{
  [ResourceArea("{8580c551-69db-4092-9050-c9ccd4521d2e}")]
  public class DeploymentHttpClient : VssHttpClientBase
  {
    public DeploymentHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public DeploymentHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public DeploymentHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public DeploymentHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public DeploymentHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<ArtifactProvenance>> GetArtifactProvenancesAsync(
      IEnumerable<string> resourceUri,
      string typeFilters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d943a6f4-a813-4498-823a-4da53bf9d0cd");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      string str = (string) null;
      if (resourceUri != null)
        str = string.Join(",", resourceUri);
      keyValuePairList.Add(nameof (resourceUri), str);
      if (typeFilters != null)
        keyValuePairList.Add(nameof (typeFilters), typeFilters);
      return this.SendAsync<List<ArtifactProvenance>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task AddAttesationDetailsAsync(
      AttestationDetails attestationDetails,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentHttpClient deploymentHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("45eed45c-a02d-4f52-99ae-4f1282049f6b");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<AttestationDetails>(attestationDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      DeploymentHttpClient deploymentHttpClient2 = deploymentHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await deploymentHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task AddAttesationDetailsAsync(
      AttestationDetails attestationDetails,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentHttpClient deploymentHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("45eed45c-a02d-4f52-99ae-4f1282049f6b");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<AttestationDetails>(attestationDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      DeploymentHttpClient deploymentHttpClient2 = deploymentHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await deploymentHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task AddDeploymentDetailsAsync(
      DeploymentDetails deploymentDetails,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentHttpClient deploymentHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bb302ef9-066f-4ffb-aee2-d61b91783b2a");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentDetails>(deploymentDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      DeploymentHttpClient deploymentHttpClient2 = deploymentHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await deploymentHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task AddDeploymentDetailsAsync(
      DeploymentDetails deploymentDetails,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentHttpClient deploymentHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bb302ef9-066f-4ffb-aee2-d61b91783b2a");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentDetails>(deploymentDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      DeploymentHttpClient deploymentHttpClient2 = deploymentHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await deploymentHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task AddImageDetailsAsync(
      ImageDetails imageDetails,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentHttpClient deploymentHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("647bb185-908a-4660-b59b-dff3d1ace8de");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ImageDetails>(imageDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      DeploymentHttpClient deploymentHttpClient2 = deploymentHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await deploymentHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task AddImageDetailsAsync(
      ImageDetails imageDetails,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentHttpClient deploymentHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("647bb185-908a-4660-b59b-dff3d1ace8de");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<ImageDetails>(imageDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      DeploymentHttpClient deploymentHttpClient2 = deploymentHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await deploymentHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> GetImageResourceIdsAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("GET"), new Guid("647bb185-908a-4660-b59b-dff3d1ace8de"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<string>> GetImageResourceIdsAsync(
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<string>>(new HttpMethod("GET"), new Guid("647bb185-908a-4660-b59b-dff3d1ace8de"), (object) new
      {
        project = project,
        runId = runId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task AddVulnerabilityDetailsAsync(
      VulnerabilityDetails vulnerabilityDetails,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentHttpClient deploymentHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ab55f461-1075-4c26-b84d-35cd2d5833bd");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<VulnerabilityDetails>(vulnerabilityDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      DeploymentHttpClient deploymentHttpClient2 = deploymentHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await deploymentHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task AddVulnerabilityDetailsAsync(
      VulnerabilityDetails vulnerabilityDetails,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentHttpClient deploymentHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("ab55f461-1075-4c26-b84d-35cd2d5833bd");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<VulnerabilityDetails>(vulnerabilityDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      DeploymentHttpClient deploymentHttpClient2 = deploymentHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await deploymentHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
