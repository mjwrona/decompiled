// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Environments.WebApi.EnvironmentsHttpClientBase
// Assembly: Microsoft.Azure.Pipelines.Environments.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C70F6957-8B2D-4EE8-9E67-48CC1F1F106C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Environments.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.Environments.WebApi
{
  [ResourceArea("0A833654-DF2A-437E-8253-FE6B63B82035")]
  public abstract class EnvironmentsHttpClientBase : VssHttpClientBase
  {
    public EnvironmentsHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public EnvironmentsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public EnvironmentsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public EnvironmentsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public EnvironmentsHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<string> GenerateEnvironmentAccessTokenAsync(
      string project,
      int environmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<string>(new HttpMethod("POST"), new Guid("c90d3b68-268d-429d-8b14-1aa864f7f0f6"), (object) new
      {
        project = project,
        environmentId = environmentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<string> GenerateEnvironmentAccessTokenAsync(
      Guid project,
      int environmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<string>(new HttpMethod("POST"), new Guid("c90d3b68-268d-429d-8b14-1aa864f7f0f6"), (object) new
      {
        project = project,
        environmentId = environmentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<EnvironmentDeploymentExecutionRecord>> GetEnvironmentDeploymentExecutionRecordsAsync(
      string project,
      int environmentId,
      string continuationToken = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0d9bba8c-e474-49a0-979a-e3b990b4bf76");
      object routeValues = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<EnvironmentDeploymentExecutionRecord>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<EnvironmentDeploymentExecutionRecord>> GetEnvironmentDeploymentExecutionRecordsAsync(
      Guid project,
      int environmentId,
      string continuationToken = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0d9bba8c-e474-49a0-979a-e3b990b4bf76");
      object routeValues = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add(nameof (top), top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<EnvironmentDeploymentExecutionRecord>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<EnvironmentInstance> AddEnvironmentAsync(
      string project,
      EnvironmentCreateParameter environmentCreateParameter,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d86b72de-d240-4d6f-8d06-08c2d66b015d");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<EnvironmentCreateParameter>(environmentCreateParameter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<EnvironmentInstance>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<EnvironmentInstance> AddEnvironmentAsync(
      Guid project,
      EnvironmentCreateParameter environmentCreateParameter,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d86b72de-d240-4d6f-8d06-08c2d66b015d");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<EnvironmentCreateParameter>(environmentCreateParameter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<EnvironmentInstance>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteEnvironmentAsync(
      string project,
      int environmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("d86b72de-d240-4d6f-8d06-08c2d66b015d"), (object) new
      {
        project = project,
        environmentId = environmentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteEnvironmentAsync(
      Guid project,
      int environmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("d86b72de-d240-4d6f-8d06-08c2d66b015d"), (object) new
      {
        project = project,
        environmentId = environmentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<EnvironmentInstance> GetEnvironmentByIdAsync(
      string project,
      int environmentId,
      EnvironmentExpands? expands = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d86b72de-d240-4d6f-8d06-08c2d66b015d");
      object routeValues = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expands.HasValue)
        keyValuePairList.Add(nameof (expands), expands.Value.ToString());
      return this.SendAsync<EnvironmentInstance>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<EnvironmentInstance> GetEnvironmentByIdAsync(
      Guid project,
      int environmentId,
      EnvironmentExpands? expands = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d86b72de-d240-4d6f-8d06-08c2d66b015d");
      object routeValues = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expands.HasValue)
        keyValuePairList.Add(nameof (expands), expands.Value.ToString());
      return this.SendAsync<EnvironmentInstance>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<EnvironmentInstance>> GetEnvironmentsAsync(
      string project,
      string name = null,
      string continuationToken = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d86b72de-d240-4d6f-8d06-08c2d66b015d");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<EnvironmentInstance>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<EnvironmentInstance>> GetEnvironmentsAsync(
      Guid project,
      string name = null,
      string continuationToken = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d86b72de-d240-4d6f-8d06-08c2d66b015d");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<EnvironmentInstance>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<EnvironmentInstance> UpdateEnvironmentAsync(
      string project,
      int environmentId,
      EnvironmentUpdateParameter environmentUpdateParameter,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d86b72de-d240-4d6f-8d06-08c2d66b015d");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<EnvironmentUpdateParameter>(environmentUpdateParameter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<EnvironmentInstance>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<EnvironmentInstance> UpdateEnvironmentAsync(
      Guid project,
      int environmentId,
      EnvironmentUpdateParameter environmentUpdateParameter,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d86b72de-d240-4d6f-8d06-08c2d66b015d");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<EnvironmentUpdateParameter>(environmentUpdateParameter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<EnvironmentInstance>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<KubernetesResource> AddKubernetesResourceAsync(
      string project,
      int environmentId,
      KubernetesResourceCreateParametersExistingEndpoint createParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("73fba52f-33ab-42b3-a538-ce67a9223b15");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<KubernetesResourceCreateParametersExistingEndpoint>(createParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<KubernetesResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<KubernetesResource> AddKubernetesResourceAsync(
      Guid project,
      int environmentId,
      KubernetesResourceCreateParametersExistingEndpoint createParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("73fba52f-33ab-42b3-a538-ce67a9223b15");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<KubernetesResourceCreateParametersExistingEndpoint>(createParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<KubernetesResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteKubernetesResourceAsync(
      string project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("73fba52f-33ab-42b3-a538-ce67a9223b15"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteKubernetesResourceAsync(
      Guid project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("73fba52f-33ab-42b3-a538-ce67a9223b15"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<KubernetesResource> GetKubernetesResourceAsync(
      string project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<KubernetesResource>(new HttpMethod("GET"), new Guid("73fba52f-33ab-42b3-a538-ce67a9223b15"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<KubernetesResource> GetKubernetesResourceAsync(
      Guid project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<KubernetesResource>(new HttpMethod("GET"), new Guid("73fba52f-33ab-42b3-a538-ce67a9223b15"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<KubernetesResource> UpdateKubernetesResourceAsync(
      string project,
      int environmentId,
      KubernetesResourcePatchParameters patchRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("73fba52f-33ab-42b3-a538-ce67a9223b15");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<KubernetesResourcePatchParameters>(patchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<KubernetesResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<KubernetesResource> UpdateKubernetesResourceAsync(
      Guid project,
      int environmentId,
      KubernetesResourcePatchParameters patchRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("73fba52f-33ab-42b3-a538-ce67a9223b15");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<KubernetesResourcePatchParameters>(patchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<KubernetesResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<TaskAgentPoolReference> GetLinkedPoolAsync(
      string project,
      int environmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TaskAgentPoolReference>(new HttpMethod("GET"), new Guid("d28f3dfe-5bb8-4b06-8420-0452882a4957"), (object) new
      {
        project = project,
        environmentId = environmentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<TaskAgentPoolReference> GetLinkedPoolAsync(
      Guid project,
      int environmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TaskAgentPoolReference>(new HttpMethod("GET"), new Guid("d28f3dfe-5bb8-4b06-8420-0452882a4957"), (object) new
      {
        project = project,
        environmentId = environmentId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<VirtualMachineGroup> AddVirtualMachineGroupAsync(
      string project,
      int environmentId,
      VirtualMachineGroupCreateParameters createParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("173a6347-3ddc-4637-8020-cce67d48909f");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<VirtualMachineGroupCreateParameters>(createParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VirtualMachineGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<VirtualMachineGroup> AddVirtualMachineGroupAsync(
      Guid project,
      int environmentId,
      VirtualMachineGroupCreateParameters createParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("173a6347-3ddc-4637-8020-cce67d48909f");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<VirtualMachineGroupCreateParameters>(createParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VirtualMachineGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteVirtualMachineGroupAsync(
      string project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("173a6347-3ddc-4637-8020-cce67d48909f"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteVirtualMachineGroupAsync(
      Guid project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("173a6347-3ddc-4637-8020-cce67d48909f"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<VirtualMachineGroup> GetVirtualMachineGroupAsync(
      string project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<VirtualMachineGroup>(new HttpMethod("GET"), new Guid("173a6347-3ddc-4637-8020-cce67d48909f"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<VirtualMachineGroup> GetVirtualMachineGroupAsync(
      Guid project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<VirtualMachineGroup>(new HttpMethod("GET"), new Guid("173a6347-3ddc-4637-8020-cce67d48909f"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<VirtualMachineGroup> UpdateVirtualMachineGroupAsync(
      string project,
      int environmentId,
      VirtualMachineGroup resource,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("173a6347-3ddc-4637-8020-cce67d48909f");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<VirtualMachineGroup>(resource, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VirtualMachineGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<VirtualMachineGroup> UpdateVirtualMachineGroupAsync(
      Guid project,
      int environmentId,
      VirtualMachineGroup resource,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("173a6347-3ddc-4637-8020-cce67d48909f");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<VirtualMachineGroup>(resource, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VirtualMachineGroup>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<VirtualMachine>> GetVirtualMachinesAsync(
      string project,
      int environmentId,
      int resourceId,
      string continuationToken = null,
      string name = null,
      bool? partialNameMatch = null,
      IEnumerable<string> tags = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3a3b5cf7-b7f7-4593-b9f5-58f170145e8d");
      object routeValues = (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      if (partialNameMatch.HasValue)
        keyValuePairList.Add(nameof (partialNameMatch), partialNameMatch.Value.ToString());
      if (tags != null && tags.Any<string>())
        keyValuePairList.Add(nameof (tags), string.Join(",", tags));
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<VirtualMachine>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<VirtualMachine>> GetVirtualMachinesAsync(
      Guid project,
      int environmentId,
      int resourceId,
      string continuationToken = null,
      string name = null,
      bool? partialNameMatch = null,
      IEnumerable<string> tags = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3a3b5cf7-b7f7-4593-b9f5-58f170145e8d");
      object routeValues = (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      if (partialNameMatch.HasValue)
        keyValuePairList.Add(nameof (partialNameMatch), partialNameMatch.Value.ToString());
      if (tags != null && tags.Any<string>())
        keyValuePairList.Add(nameof (tags), string.Join(",", tags));
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<VirtualMachine>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<VirtualMachine>> UpdateVirtualMachinesAsync(
      string project,
      int environmentId,
      int resourceId,
      IEnumerable<VirtualMachine> machines,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("3a3b5cf7-b7f7-4593-b9f5-58f170145e8d");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<VirtualMachine>>(machines, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<VirtualMachine>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<VirtualMachine>> UpdateVirtualMachinesAsync(
      Guid project,
      int environmentId,
      int resourceId,
      IEnumerable<VirtualMachine> machines,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("3a3b5cf7-b7f7-4593-b9f5-58f170145e8d");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<VirtualMachine>>(machines, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<VirtualMachine>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<VirtualMachineResource> AddVirtualMachineResourceAsync(
      string project,
      int environmentId,
      VirtualMachineResourceCreateParameters createParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("20cb45c7-bd6c-401e-b7e0-a634beda2683");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<VirtualMachineResourceCreateParameters>(createParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VirtualMachineResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<VirtualMachineResource> AddVirtualMachineResourceAsync(
      Guid project,
      int environmentId,
      VirtualMachineResourceCreateParameters createParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("20cb45c7-bd6c-401e-b7e0-a634beda2683");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<VirtualMachineResourceCreateParameters>(createParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VirtualMachineResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteVirtualMachineResourceAsync(
      string project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("20cb45c7-bd6c-401e-b7e0-a634beda2683"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteVirtualMachineResourceAsync(
      Guid project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("20cb45c7-bd6c-401e-b7e0-a634beda2683"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<VirtualMachineResource> GetVirtualMachineResourceAsync(
      string project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<VirtualMachineResource>(new HttpMethod("GET"), new Guid("20cb45c7-bd6c-401e-b7e0-a634beda2683"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<VirtualMachineResource> GetVirtualMachineResourceAsync(
      Guid project,
      int environmentId,
      int resourceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<VirtualMachineResource>(new HttpMethod("GET"), new Guid("20cb45c7-bd6c-401e-b7e0-a634beda2683"), (object) new
      {
        project = project,
        environmentId = environmentId,
        resourceId = resourceId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<VirtualMachineResource>> GetVirtualMachineResourcesAsync(
      string project,
      int environmentId,
      string name = null,
      IEnumerable<string> tags = null,
      string continuationToken = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("20cb45c7-bd6c-401e-b7e0-a634beda2683");
      object routeValues = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      if (tags != null && tags.Any<string>())
        keyValuePairList.Add(nameof (tags), string.Join(",", tags));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<VirtualMachineResource>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<VirtualMachineResource>> GetVirtualMachineResourcesAsync(
      Guid project,
      int environmentId,
      string name = null,
      IEnumerable<string> tags = null,
      string continuationToken = null,
      int? top = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("20cb45c7-bd6c-401e-b7e0-a634beda2683");
      object routeValues = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      if (tags != null && tags.Any<string>())
        keyValuePairList.Add(nameof (tags), string.Join(",", tags));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      if (top.HasValue)
        keyValuePairList.Add("$top", top.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<VirtualMachineResource>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<VirtualMachineResource> ReplaceVirtualMachineResourceAsync(
      string project,
      int environmentId,
      VirtualMachineResource resource,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("20cb45c7-bd6c-401e-b7e0-a634beda2683");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<VirtualMachineResource>(resource, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VirtualMachineResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<VirtualMachineResource> ReplaceVirtualMachineResourceAsync(
      Guid project,
      int environmentId,
      VirtualMachineResource resource,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("20cb45c7-bd6c-401e-b7e0-a634beda2683");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<VirtualMachineResource>(resource, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VirtualMachineResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<VirtualMachineResource> UpdateVirtualMachineResourceAsync(
      string project,
      int environmentId,
      VirtualMachineResource resource,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("20cb45c7-bd6c-401e-b7e0-a634beda2683");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<VirtualMachineResource>(resource, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VirtualMachineResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<VirtualMachineResource> UpdateVirtualMachineResourceAsync(
      Guid project,
      int environmentId,
      VirtualMachineResource resource,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("20cb45c7-bd6c-401e-b7e0-a634beda2683");
      object obj1 = (object) new
      {
        project = project,
        environmentId = environmentId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<VirtualMachineResource>(resource, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VirtualMachineResource>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
