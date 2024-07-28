// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.WorkItemTrackingProcessHttpClientCompatBase
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 141BFF87-1CDC-4DC5-9A7C-F7D6EA031F34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.dll

using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi
{
  public abstract class WorkItemTrackingProcessHttpClientCompatBase : VssHttpClientBase
  {
    public WorkItemTrackingProcessHttpClientCompatBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public WorkItemTrackingProcessHttpClientCompatBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public WorkItemTrackingProcessHttpClientCompatBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public WorkItemTrackingProcessHttpClientCompatBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public WorkItemTrackingProcessHttpClientCompatBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<WorkItemTypeModel> GetWorkItemTypeAsync(
      Guid processId,
      string witRefName,
      GetWorkItemTypeExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e2e9d1a6-432d-4062-8870-bfcb8c324ad7");
      object routeValues = (object) new
      {
        processId = processId,
        witRefName = witRefName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<WorkItemTypeModel>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<WorkItemTypeModel>> GetWorkItemTypesAsync(
      Guid processId,
      GetWorkItemTypeExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("e2e9d1a6-432d-4062-8870-bfcb8c324ad7");
      object routeValues = (object) new
      {
        processId = processId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<WorkItemTypeModel>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<WorkItemBehavior> GetBehaviorAsync(
      Guid processId,
      string behaviorRefName,
      GetBehaviorsExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d1800200-f184-4e75-a5f2-ad0b04b4373e");
      object routeValues = (object) new
      {
        processId = processId,
        behaviorRefName = behaviorRefName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<WorkItemBehavior>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<WorkItemBehavior>> GetBehaviorsAsync(
      Guid processId,
      GetBehaviorsExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d1800200-f184-4e75-a5f2-ad0b04b4373e");
      object routeValues = (object) new
      {
        processId = processId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<WorkItemBehavior>>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<FieldRuleModel> AddWorkItemTypeRuleAsync(
      FieldRuleModel fieldRule,
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("76fe3432-d825-479d-a5f6-983bbb78b4f3");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FieldRuleModel>(fieldRule, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FieldRuleModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeleteWorkItemTypeRuleAsync(
      Guid processId,
      string witRefName,
      Guid ruleId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("76fe3432-d825-479d-a5f6-983bbb78b4f3"), (object) new
      {
        processId = processId,
        witRefName = witRefName,
        ruleId = ruleId
      }, new ApiResourceVersion("5.0-preview.1"), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<FieldRuleModel> GetWorkItemTypeRuleAsync(
      Guid processId,
      string witRefName,
      Guid ruleId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<FieldRuleModel>(new HttpMethod("GET"), new Guid("76fe3432-d825-479d-a5f6-983bbb78b4f3"), (object) new
      {
        processId = processId,
        witRefName = witRefName,
        ruleId = ruleId
      }, new ApiResourceVersion("5.0-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<FieldRuleModel>> GetWorkItemTypeRulesAsync(
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<FieldRuleModel>>(new HttpMethod("GET"), new Guid("76fe3432-d825-479d-a5f6-983bbb78b4f3"), (object) new
      {
        processId = processId,
        witRefName = witRefName
      }, new ApiResourceVersion("5.0-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<FieldRuleModel> UpdateWorkItemTypeRuleAsync(
      FieldRuleModel fieldRule,
      Guid processId,
      string witRefName,
      Guid ruleId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("76fe3432-d825-479d-a5f6-983bbb78b4f3");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName,
        ruleId = ruleId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FieldRuleModel>(fieldRule, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FieldRuleModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public Task<ProcessModel> CreateProcessAsync(
      CreateProcessModel createRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("02cc6a73-5cfb-427d-8c8e-b49fb086e8af");
      HttpContent httpContent = (HttpContent) new ObjectContent<CreateProcessModel>(createRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProcessModel>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public async Task DeleteProcessAsync(
      Guid processTypeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("02cc6a73-5cfb-427d-8c8e-b49fb086e8af"), (object) new
      {
        processTypeId = processTypeId
      }, new ApiResourceVersion("5.0-preview.1"), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public Task<ProcessModel> GetProcessByIdAsync(
      Guid processTypeId,
      GetProcessExpandLevel? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("02cc6a73-5cfb-427d-8c8e-b49fb086e8af");
      object routeValues = (object) new
      {
        processTypeId = processTypeId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<ProcessModel>(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public Task<List<ProcessModel>> GetProcessesAsync(
      GetProcessExpandLevel? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("02cc6a73-5cfb-427d-8c8e-b49fb086e8af");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<ProcessModel>>(method, locationId, version: new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public Task<ProcessModel> UpdateProcessAsync(
      UpdateProcessModel updateRequest,
      Guid processTypeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("02cc6a73-5cfb-427d-8c8e-b49fb086e8af");
      object obj1 = (object) new
      {
        processTypeId = processTypeId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateProcessModel>(updateRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProcessModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<FieldModel>> GetFieldsAsync(
      Guid processId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<FieldModel>>(new HttpMethod("GET"), new Guid("7a0e7a1a-0b34-4ae0-9744-0aaffb7d0ed1"), (object) new
      {
        processId = processId
      }, new ApiResourceVersion("5.0-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<List<FieldModel>> GetWorkItemTypeFieldsAsync(
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<FieldModel>>(new HttpMethod("GET"), new Guid("bc0ad8dc-e3f3-46b0-b06c-5bf861793196"), (object) new
      {
        processId = processId,
        witRefName = witRefName
      }, new ApiResourceVersion("5.0-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<ProcessWorkItemTypeField> GetWorkItemTypeFieldAsync(
      Guid processId,
      string witRefName,
      string fieldRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ProcessWorkItemTypeField>(new HttpMethod("GET"), new Guid("bc0ad8dc-e3f3-46b0-b06c-5bf861793196"), (object) new
      {
        processId = processId,
        witRefName = witRefName,
        fieldRefName = fieldRefName
      }, new ApiResourceVersion(5.2, 2), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
