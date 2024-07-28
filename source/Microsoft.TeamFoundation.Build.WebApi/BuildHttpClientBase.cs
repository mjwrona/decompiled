// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildHttpClientBase
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [ResourceArea("965220D5-5BB9-42CF-8D67-9B146DF2A5A4")]
  public abstract class BuildHttpClientBase : VssHttpClientBase
  {
    public BuildHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public BuildHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public BuildHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public BuildHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public BuildHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task DeleteBuildAsync(
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return (Task) this.SendAsync(new HttpMethod("DELETE"), new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf"), (object) new
      {
        buildId = buildId
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task DeleteBuildAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return (Task) this.SendAsync(new HttpMethod("DELETE"), new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf"), (object) new
      {
        project = project,
        buildId = buildId
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task DeleteBuildAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return (Task) this.SendAsync(new HttpMethod("DELETE"), new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf"), (object) new
      {
        project = project,
        buildId = buildId
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> GetBuildAsync(
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(new HttpMethod("GET"), new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf"), (object) new
      {
        buildId = buildId
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> GetBuildAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(new HttpMethod("GET"), new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf"), (object) new
      {
        project = project,
        buildId = buildId
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> GetBuildAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(new HttpMethod("GET"), new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf"), (object) new
      {
        project = project,
        buildId = buildId
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsAsync(
      string project,
      string projectName = null,
      string requestedFor = null,
      string definition = null,
      int? maxBuildsPerDefinition = null,
      int? skip = null,
      int? top = null,
      string ids = null,
      DateTime? minFinishTime = null,
      string quality = null,
      BuildStatus? status = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectName))
        keyValuePairList.Add(nameof (projectName), projectName);
      if (!string.IsNullOrEmpty(requestedFor))
        keyValuePairList.Add(nameof (requestedFor), requestedFor);
      if (!string.IsNullOrEmpty(definition))
        keyValuePairList.Add(nameof (definition), definition);
      int num;
      if (maxBuildsPerDefinition.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxBuildsPerDefinition.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxBuildsPerDefinition), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (!string.IsNullOrEmpty(ids))
        keyValuePairList.Add(nameof (ids), ids);
      if (minFinishTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minFinishTime), minFinishTime.Value);
      if (!string.IsNullOrEmpty(quality))
        keyValuePairList.Add(nameof (quality), quality);
      if (status.HasValue)
        keyValuePairList.Add(nameof (status), status.Value.ToString());
      return this.SendAsync<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsAsync(
      Guid project,
      string projectName = null,
      string requestedFor = null,
      string definition = null,
      int? maxBuildsPerDefinition = null,
      int? skip = null,
      int? top = null,
      string ids = null,
      DateTime? minFinishTime = null,
      string quality = null,
      BuildStatus? status = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectName))
        keyValuePairList.Add(nameof (projectName), projectName);
      if (!string.IsNullOrEmpty(requestedFor))
        keyValuePairList.Add(nameof (requestedFor), requestedFor);
      if (!string.IsNullOrEmpty(definition))
        keyValuePairList.Add(nameof (definition), definition);
      int num;
      if (maxBuildsPerDefinition.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxBuildsPerDefinition.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxBuildsPerDefinition), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (!string.IsNullOrEmpty(ids))
        keyValuePairList.Add(nameof (ids), ids);
      if (minFinishTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minFinishTime), minFinishTime.Value);
      if (!string.IsNullOrEmpty(quality))
        keyValuePairList.Add(nameof (quality), quality);
      if (status.HasValue)
        keyValuePairList.Add(nameof (status), status.Value.ToString());
      return this.SendAsync<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> GetBuildsAsync(
      string projectName = null,
      string requestedFor = null,
      string definition = null,
      int? maxBuildsPerDefinition = null,
      int? skip = null,
      int? top = null,
      string ids = null,
      DateTime? minFinishTime = null,
      string quality = null,
      BuildStatus? status = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectName))
        keyValuePairList.Add(nameof (projectName), projectName);
      if (!string.IsNullOrEmpty(requestedFor))
        keyValuePairList.Add(nameof (requestedFor), requestedFor);
      if (!string.IsNullOrEmpty(definition))
        keyValuePairList.Add(nameof (definition), definition);
      int num;
      if (maxBuildsPerDefinition.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxBuildsPerDefinition.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxBuildsPerDefinition), str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (!string.IsNullOrEmpty(ids))
        keyValuePairList.Add(nameof (ids), ids);
      if (minFinishTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (minFinishTime), minFinishTime.Value);
      if (!string.IsNullOrEmpty(quality))
        keyValuePairList.Add(nameof (quality), quality);
      if (status.HasValue)
        keyValuePairList.Add(nameof (status), status.Value.ToString());
      return this.SendAsync<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(method, locationId, version: new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> UpdateBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new{ buildId = buildId };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("1.0");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> UpdateBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("1.0");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Microsoft.TeamFoundation.Build.WebApi.Build> UpdateBuildAsync(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0cd358e1-9217-4d94-8269-1c1ee6f93dcf");
      object obj1 = (object) new
      {
        project = project,
        buildId = buildId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.TeamFoundation.Build.WebApi.Build>(build, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("1.0");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.TeamFoundation.Build.WebApi.Build>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<BuildDefinition> GetDefinitionAsync(
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BuildDefinition>(new HttpMethod("GET"), new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6"), (object) new
      {
        definitionId = definitionId
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BuildDefinition> GetDefinitionAsync(
      string project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BuildDefinition>(new HttpMethod("GET"), new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6"), (object) new
      {
        project = project,
        definitionId = definitionId
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BuildDefinition> GetDefinitionAsync(
      Guid project,
      int definitionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BuildDefinition>(new HttpMethod("GET"), new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6"), (object) new
      {
        project = project,
        definitionId = definitionId
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BuildDefinition>> GetDefinitionsAsync(
      string project,
      string projectName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectName))
        keyValuePairList.Add(nameof (projectName), projectName);
      return this.SendAsync<List<BuildDefinition>>(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BuildDefinition>> GetDefinitionsAsync(
      Guid project,
      string projectName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectName))
        keyValuePairList.Add(nameof (projectName), projectName);
      return this.SendAsync<List<BuildDefinition>>(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BuildDefinition>> GetDefinitionsAsync(
      string projectName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("dbeaf647-6167-421a-bda9-c9327b25e2e6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectName))
        keyValuePairList.Add(nameof (projectName), projectName);
      return this.SendAsync<List<BuildDefinition>>(method, locationId, version: new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<DeploymentEnvironmentMetadata> CreateDeploymentEnvironmentsAsync(
      DeploymentEnvironmentApiData deploymentEnvironmentApiData,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("32696366-f57b-4529-aec4-61673d4c23c6");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentEnvironmentApiData>(deploymentEnvironmentApiData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("1.0");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentEnvironmentMetadata>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<DeploymentEnvironmentMetadata> CreateDeploymentEnvironmentsAsync(
      DeploymentEnvironmentApiData deploymentEnvironmentApiData,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("32696366-f57b-4529-aec4-61673d4c23c6");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<DeploymentEnvironmentApiData>(deploymentEnvironmentApiData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("1.0");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DeploymentEnvironmentMetadata>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<DeploymentEnvironmentMetadata>> GetDeploymentEnvironmentsAsync(
      string project,
      string serviceName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<DeploymentEnvironmentMetadata>>(new HttpMethod("GET"), new Guid("32696366-f57b-4529-aec4-61673d4c23c6"), (object) new
      {
        project = project,
        serviceName = serviceName
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<DeploymentEnvironmentMetadata>> GetDeploymentEnvironmentsAsync(
      Guid project,
      string serviceName = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<DeploymentEnvironmentMetadata>>(new HttpMethod("GET"), new Guid("32696366-f57b-4529-aec4-61673d4c23c6"), (object) new
      {
        project = project,
        serviceName = serviceName
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<InformationNode>> GetDetailsAsync(
      string project,
      int buildId,
      string[] types,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9f094d42-b41c-4920-95aa-597581a79821");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (types != null)
        keyValuePairList.Add(nameof (types), types.ToString());
      return this.SendAsync<List<InformationNode>>(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<InformationNode>> GetDetailsAsync(
      Guid project,
      int buildId,
      string[] types,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9f094d42-b41c-4920-95aa-597581a79821");
      object routeValues = (object) new
      {
        project = project,
        buildId = buildId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (types != null)
        keyValuePairList.Add(nameof (types), types.ToString());
      return this.SendAsync<List<InformationNode>>(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<InformationNode>> GetDetailsAsync(
      int buildId,
      string[] types,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("9f094d42-b41c-4920-95aa-597581a79821");
      object routeValues = (object) new{ buildId = buildId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (types != null)
        keyValuePairList.Add(nameof (types), types.ToString());
      return this.SendAsync<List<InformationNode>>(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task CreateQualityAsync(
      string project,
      string quality,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("82fba9f8-4198-4ab6-b719-6a363880c19e");
      object routeValues = (object) new
      {
        project = project,
        quality = quality
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectId))
        keyValuePairList.Add(nameof (projectId), projectId);
      return (Task) this.SendAsync(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task CreateQualityAsync(
      Guid project,
      string quality,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("82fba9f8-4198-4ab6-b719-6a363880c19e");
      object routeValues = (object) new
      {
        project = project,
        quality = quality
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectId))
        keyValuePairList.Add(nameof (projectId), projectId);
      return (Task) this.SendAsync(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task CreateQualityAsync(
      string quality,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("82fba9f8-4198-4ab6-b719-6a363880c19e");
      object routeValues = (object) new{ quality = quality };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectId))
        keyValuePairList.Add(nameof (projectId), projectId);
      return (Task) this.SendAsync(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task DeleteQualityAsync(
      string project,
      string quality,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("82fba9f8-4198-4ab6-b719-6a363880c19e");
      object routeValues = (object) new
      {
        project = project,
        quality = quality
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectId))
        keyValuePairList.Add(nameof (projectId), projectId);
      return (Task) this.SendAsync(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task DeleteQualityAsync(
      Guid project,
      string quality,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("82fba9f8-4198-4ab6-b719-6a363880c19e");
      object routeValues = (object) new
      {
        project = project,
        quality = quality
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectId))
        keyValuePairList.Add(nameof (projectId), projectId);
      return (Task) this.SendAsync(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task DeleteQualityAsync(
      string quality,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("82fba9f8-4198-4ab6-b719-6a363880c19e");
      object routeValues = (object) new{ quality = quality };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectId))
        keyValuePairList.Add(nameof (projectId), projectId);
      return (Task) this.SendAsync(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<InformationNode>> GetQualitiesAsync(
      string project,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("82fba9f8-4198-4ab6-b719-6a363880c19e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectId))
        keyValuePairList.Add(nameof (projectId), projectId);
      return this.SendAsync<List<InformationNode>>(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<InformationNode>> GetQualitiesAsync(
      Guid project,
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("82fba9f8-4198-4ab6-b719-6a363880c19e");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectId))
        keyValuePairList.Add(nameof (projectId), projectId);
      return this.SendAsync<List<InformationNode>>(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<InformationNode>> GetQualitiesAsync(
      string projectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("82fba9f8-4198-4ab6-b719-6a363880c19e");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectId))
        keyValuePairList.Add(nameof (projectId), projectId);
      return this.SendAsync<List<InformationNode>>(method, locationId, version: new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BuildController> GetQueueAsync(
      int controllerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BuildController>(new HttpMethod("GET"), new Guid("09f2a4b8-08c9-4991-85c3-d698937568be"), (object) new
      {
        controllerId = controllerId
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BuildController>> GetQueuesAsync(
      string controllerName = null,
      string serviceHost = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("09f2a4b8-08c9-4991-85c3-d698937568be");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(controllerName))
        keyValuePairList.Add(nameof (controllerName), controllerName);
      if (!string.IsNullOrEmpty(serviceHost))
        keyValuePairList.Add(nameof (serviceHost), serviceHost);
      return this.SendAsync<List<BuildController>>(method, locationId, version: new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BuildRequest> CreateRequestAsync(
      BuildRequest postContract,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("de3e9770-c7ef-4697-983e-f4b5bab3c016");
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildRequest>(postContract, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("1.0");
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BuildRequest>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<BuildRequest> CreateRequestAsync(
      BuildRequest postContract,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("de3e9770-c7ef-4697-983e-f4b5bab3c016");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildRequest>(postContract, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("1.0");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BuildRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<BuildRequest> CreateRequestAsync(
      BuildRequest postContract,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("de3e9770-c7ef-4697-983e-f4b5bab3c016");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildRequest>(postContract, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("1.0");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BuildRequest>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task DeleteRequestAsync(
      int requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return (Task) this.SendAsync(new HttpMethod("DELETE"), new Guid("de3e9770-c7ef-4697-983e-f4b5bab3c016"), (object) new
      {
        requestId = requestId
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task DeleteRequestAsync(
      string project,
      int requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return (Task) this.SendAsync(new HttpMethod("DELETE"), new Guid("de3e9770-c7ef-4697-983e-f4b5bab3c016"), (object) new
      {
        project = project,
        requestId = requestId
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task DeleteRequestAsync(
      Guid project,
      int requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return (Task) this.SendAsync(new HttpMethod("DELETE"), new Guid("de3e9770-c7ef-4697-983e-f4b5bab3c016"), (object) new
      {
        project = project,
        requestId = requestId
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BuildRequest> GetRequestAsync(
      int requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BuildRequest>(new HttpMethod("GET"), new Guid("de3e9770-c7ef-4697-983e-f4b5bab3c016"), (object) new
      {
        requestId = requestId
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BuildRequest> GetRequestAsync(
      string project,
      int requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BuildRequest>(new HttpMethod("GET"), new Guid("de3e9770-c7ef-4697-983e-f4b5bab3c016"), (object) new
      {
        project = project,
        requestId = requestId
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<BuildRequest> GetRequestAsync(
      Guid project,
      int requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BuildRequest>(new HttpMethod("GET"), new Guid("de3e9770-c7ef-4697-983e-f4b5bab3c016"), (object) new
      {
        project = project,
        requestId = requestId
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BuildRequest>> GetRequestsAsync(
      string project,
      string projectName = null,
      string requestedFor = null,
      int? queueId = null,
      string controllerName = null,
      int? definitionId = null,
      string definitionName = null,
      int? skip = null,
      int? top = null,
      string ids = null,
      int? maxCompletedAge = null,
      QueueStatus? status = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("de3e9770-c7ef-4697-983e-f4b5bab3c016");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectName))
        keyValuePairList.Add(nameof (projectName), projectName);
      if (!string.IsNullOrEmpty(requestedFor))
        keyValuePairList.Add(nameof (requestedFor), requestedFor);
      int num;
      if (queueId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = queueId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (queueId), str);
      }
      if (!string.IsNullOrEmpty(controllerName))
        keyValuePairList.Add(nameof (controllerName), controllerName);
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (!string.IsNullOrEmpty(definitionName))
        keyValuePairList.Add(nameof (definitionName), definitionName);
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (!string.IsNullOrEmpty(ids))
        keyValuePairList.Add(nameof (ids), ids);
      if (maxCompletedAge.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCompletedAge.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCompletedAge), str);
      }
      if (status.HasValue)
        keyValuePairList.Add(nameof (status), status.Value.ToString());
      return this.SendAsync<List<BuildRequest>>(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BuildRequest>> GetRequestsAsync(
      Guid project,
      string projectName = null,
      string requestedFor = null,
      int? queueId = null,
      string controllerName = null,
      int? definitionId = null,
      string definitionName = null,
      int? skip = null,
      int? top = null,
      string ids = null,
      int? maxCompletedAge = null,
      QueueStatus? status = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("de3e9770-c7ef-4697-983e-f4b5bab3c016");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectName))
        keyValuePairList.Add(nameof (projectName), projectName);
      if (!string.IsNullOrEmpty(requestedFor))
        keyValuePairList.Add(nameof (requestedFor), requestedFor);
      int num;
      if (queueId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = queueId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (queueId), str);
      }
      if (!string.IsNullOrEmpty(controllerName))
        keyValuePairList.Add(nameof (controllerName), controllerName);
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (!string.IsNullOrEmpty(definitionName))
        keyValuePairList.Add(nameof (definitionName), definitionName);
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (!string.IsNullOrEmpty(ids))
        keyValuePairList.Add(nameof (ids), ids);
      if (maxCompletedAge.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCompletedAge.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCompletedAge), str);
      }
      if (status.HasValue)
        keyValuePairList.Add(nameof (status), status.Value.ToString());
      return this.SendAsync<List<BuildRequest>>(method, locationId, routeValues, new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<BuildRequest>> GetRequestsAsync(
      string projectName = null,
      string requestedFor = null,
      int? queueId = null,
      string controllerName = null,
      int? definitionId = null,
      string definitionName = null,
      int? skip = null,
      int? top = null,
      string ids = null,
      int? maxCompletedAge = null,
      QueueStatus? status = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("de3e9770-c7ef-4697-983e-f4b5bab3c016");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(projectName))
        keyValuePairList.Add(nameof (projectName), projectName);
      if (!string.IsNullOrEmpty(requestedFor))
        keyValuePairList.Add(nameof (requestedFor), requestedFor);
      int num;
      if (queueId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = queueId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (queueId), str);
      }
      if (!string.IsNullOrEmpty(controllerName))
        keyValuePairList.Add(nameof (controllerName), controllerName);
      if (definitionId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = definitionId.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (definitionId), str);
      }
      if (!string.IsNullOrEmpty(definitionName))
        keyValuePairList.Add(nameof (definitionName), definitionName);
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (!string.IsNullOrEmpty(ids))
        keyValuePairList.Add(nameof (ids), ids);
      if (maxCompletedAge.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = maxCompletedAge.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add(nameof (maxCompletedAge), str);
      }
      if (status.HasValue)
        keyValuePairList.Add(nameof (status), status.Value.ToString());
      return this.SendAsync<List<BuildRequest>>(method, locationId, version: new ApiResourceVersion("1.0"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task UpdateRequestStatusAsync(
      BuildRequest request,
      int requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("de3e9770-c7ef-4697-983e-f4b5bab3c016");
      object obj1 = (object) new{ requestId = requestId };
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("1.0");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return (Task) this.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task UpdateRequestStatusAsync(
      BuildRequest request,
      string project,
      int requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("de3e9770-c7ef-4697-983e-f4b5bab3c016");
      object obj1 = (object) new
      {
        project = project,
        requestId = requestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("1.0");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return (Task) this.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task UpdateRequestStatusAsync(
      BuildRequest request,
      Guid project,
      int requestId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("de3e9770-c7ef-4697-983e-f4b5bab3c016");
      object obj1 = (object) new
      {
        project = project,
        requestId = requestId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<BuildRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("1.0");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return (Task) this.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
