// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.WorkItemTrackingProcessHttpClient
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 141BFF87-1CDC-4DC5-9A7C-F7D6EA031F34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.dll

using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi
{
  [ResourceArea("5264459E-E5E0-4BD8-B118-0985E68A4EC5")]
  public class WorkItemTrackingProcessHttpClient : WorkItemTrackingProcessHttpClientCompatBase
  {
    public WorkItemTrackingProcessHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public WorkItemTrackingProcessHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public WorkItemTrackingProcessHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public WorkItemTrackingProcessHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public WorkItemTrackingProcessHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<ProcessBehavior> CreateProcessBehaviorAsync(
      ProcessBehaviorCreateRequest behavior,
      Guid processId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d1800200-f184-4e75-a5f2-ad0b04b4373e");
      object obj1 = (object) new{ processId = processId };
      HttpContent httpContent = (HttpContent) new ObjectContent<ProcessBehaviorCreateRequest>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProcessBehavior>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteProcessBehaviorAsync(
      Guid processId,
      string behaviorRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("d1800200-f184-4e75-a5f2-ad0b04b4373e"), (object) new
      {
        processId = processId,
        behaviorRefName = behaviorRefName
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<ProcessBehavior> GetProcessBehaviorAsync(
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
      return this.SendAsync<ProcessBehavior>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<ProcessBehavior>> GetProcessBehaviorsAsync(
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
      return this.SendAsync<List<ProcessBehavior>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ProcessBehavior> UpdateProcessBehaviorAsync(
      ProcessBehaviorUpdateRequest behaviorData,
      Guid processId,
      string behaviorRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("d1800200-f184-4e75-a5f2-ad0b04b4373e");
      object obj1 = (object) new
      {
        processId = processId,
        behaviorRefName = behaviorRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<ProcessBehaviorUpdateRequest>(behaviorData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProcessBehavior>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Control> CreateControlInGroupAsync(
      Control control,
      Guid processId,
      string witRefName,
      string groupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1f59b363-a2d0-4b7e-9bc6-eb9f5f3f0e58");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName,
        groupId = groupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Control>(control, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Control>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Control> MoveControlToGroupAsync(
      Control control,
      Guid processId,
      string witRefName,
      string groupId,
      string controlId,
      string removeFromGroupId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("1f59b363-a2d0-4b7e-9bc6-eb9f5f3f0e58");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName,
        groupId = groupId,
        controlId = controlId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Control>(control, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (removeFromGroupId != null)
        collection.Add(nameof (removeFromGroupId), removeFromGroupId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Control>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public async Task RemoveControlFromGroupAsync(
      Guid processId,
      string witRefName,
      string groupId,
      string controlId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("1f59b363-a2d0-4b7e-9bc6-eb9f5f3f0e58"), (object) new
      {
        processId = processId,
        witRefName = witRefName,
        groupId = groupId,
        controlId = controlId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<Control> UpdateControlAsync(
      Control control,
      Guid processId,
      string witRefName,
      string groupId,
      string controlId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1f59b363-a2d0-4b7e-9bc6-eb9f5f3f0e58");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName,
        groupId = groupId,
        controlId = controlId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Control>(control, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Control>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<ProcessWorkItemTypeField> AddFieldToWorkItemTypeAsync(
      AddProcessWorkItemTypeFieldRequest field,
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bc0ad8dc-e3f3-46b0-b06c-5bf861793196");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<AddProcessWorkItemTypeFieldRequest>(field, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProcessWorkItemTypeField>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<List<ProcessWorkItemTypeField>> GetAllWorkItemTypeFieldsAsync(
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ProcessWorkItemTypeField>>(new HttpMethod("GET"), new Guid("bc0ad8dc-e3f3-46b0-b06c-5bf861793196"), (object) new
      {
        processId = processId,
        witRefName = witRefName
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ProcessWorkItemTypeField> GetWorkItemTypeFieldAsync(
      Guid processId,
      string witRefName,
      string fieldRefName,
      ProcessWorkItemTypeFieldsExpandLevel? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("bc0ad8dc-e3f3-46b0-b06c-5bf861793196");
      object routeValues = (object) new
      {
        processId = processId,
        witRefName = witRefName,
        fieldRefName = fieldRefName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<ProcessWorkItemTypeField>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RemoveWorkItemTypeFieldAsync(
      Guid processId,
      string witRefName,
      string fieldRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("bc0ad8dc-e3f3-46b0-b06c-5bf861793196"), (object) new
      {
        processId = processId,
        witRefName = witRefName,
        fieldRefName = fieldRefName
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<ProcessWorkItemTypeField> UpdateWorkItemTypeFieldAsync(
      UpdateProcessWorkItemTypeFieldRequest field,
      Guid processId,
      string witRefName,
      string fieldRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("bc0ad8dc-e3f3-46b0-b06c-5bf861793196");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName,
        fieldRefName = fieldRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateProcessWorkItemTypeFieldRequest>(field, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProcessWorkItemTypeField>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Group> AddGroupAsync(
      Group group,
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("766e44e1-36a8-41d7-9050-c343ff02f7a5");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName,
        pageId = pageId,
        sectionId = sectionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Group>(group, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Group>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Group> MoveGroupToPageAsync(
      Group group,
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      string groupId,
      string removeFromPageId,
      string removeFromSectionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("766e44e1-36a8-41d7-9050-c343ff02f7a5");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName,
        pageId = pageId,
        sectionId = sectionId,
        groupId = groupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Group>(group, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (removeFromPageId), removeFromPageId);
      collection.Add(nameof (removeFromSectionId), removeFromSectionId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Group>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public Task<Group> MoveGroupToSectionAsync(
      Group group,
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      string groupId,
      string removeFromSectionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("766e44e1-36a8-41d7-9050-c343ff02f7a5");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName,
        pageId = pageId,
        sectionId = sectionId,
        groupId = groupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Group>(group, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (removeFromSectionId), removeFromSectionId);
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Group>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    public async Task RemoveGroupAsync(
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      string groupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("766e44e1-36a8-41d7-9050-c343ff02f7a5"), (object) new
      {
        processId = processId,
        witRefName = witRefName,
        pageId = pageId,
        sectionId = sectionId,
        groupId = groupId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<Group> UpdateGroupAsync(
      Group group,
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      string groupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("766e44e1-36a8-41d7-9050-c343ff02f7a5");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName,
        pageId = pageId,
        sectionId = sectionId,
        groupId = groupId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Group>(group, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Group>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<FormLayout> GetFormLayoutAsync(
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<FormLayout>(new HttpMethod("GET"), new Guid("fa8646eb-43cd-4b71-9564-40106fd63e40"), (object) new
      {
        processId = processId,
        witRefName = witRefName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<PickList> CreateListAsync(
      PickList picklist,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("01e15468-e27c-4e20-a974-bd957dcccebc");
      HttpContent httpContent = (HttpContent) new ObjectContent<PickList>(picklist, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PickList>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteListAsync(
      Guid listId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("01e15468-e27c-4e20-a974-bd957dcccebc"), (object) new
      {
        listId = listId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<PickList> GetListAsync(
      Guid listId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PickList>(new HttpMethod("GET"), new Guid("01e15468-e27c-4e20-a974-bd957dcccebc"), (object) new
      {
        listId = listId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<PickListMetadata>> GetListsMetadataAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<PickListMetadata>>(new HttpMethod("GET"), new Guid("01e15468-e27c-4e20-a974-bd957dcccebc"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<PickList> UpdateListAsync(
      PickList picklist,
      Guid listId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("01e15468-e27c-4e20-a974-bd957dcccebc");
      object obj1 = (object) new{ listId = listId };
      HttpContent httpContent = (HttpContent) new ObjectContent<PickList>(picklist, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PickList>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Page> AddPageAsync(
      Page page,
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1cc7b29f-6697-4d9d-b0a1-2650d3e1d584");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Page>(page, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Page>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task RemovePageAsync(
      Guid processId,
      string witRefName,
      string pageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("1cc7b29f-6697-4d9d-b0a1-2650d3e1d584"), (object) new
      {
        processId = processId,
        witRefName = witRefName,
        pageId = pageId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<Page> UpdatePageAsync(
      Page page,
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1cc7b29f-6697-4d9d-b0a1-2650d3e1d584");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Page>(page, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Page>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<ProcessInfo> CreateNewProcessAsync(
      CreateProcessModel createRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("02cc6a73-5cfb-427d-8c8e-b49fb086e8af");
      HttpContent httpContent = (HttpContent) new ObjectContent<CreateProcessModel>(createRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProcessInfo>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteProcessByIdAsync(
      Guid processTypeId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("02cc6a73-5cfb-427d-8c8e-b49fb086e8af"), (object) new
      {
        processTypeId = processTypeId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<ProcessInfo> EditProcessAsync(
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
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProcessInfo>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<List<ProcessInfo>> GetListOfProcessesAsync(
      GetProcessExpandLevel? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("02cc6a73-5cfb-427d-8c8e-b49fb086e8af");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<ProcessInfo>>(method, locationId, version: new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ProcessInfo> GetProcessByItsIdAsync(
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
      return this.SendAsync<ProcessInfo>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ProcessRule> AddProcessWorkItemTypeRuleAsync(
      CreateProcessRuleRequest processRuleCreate,
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
      HttpContent httpContent = (HttpContent) new ObjectContent<CreateProcessRuleRequest>(processRuleCreate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProcessRule>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteProcessWorkItemTypeRuleAsync(
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
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<ProcessRule> GetProcessWorkItemTypeRuleAsync(
      Guid processId,
      string witRefName,
      Guid ruleId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<ProcessRule>(new HttpMethod("GET"), new Guid("76fe3432-d825-479d-a5f6-983bbb78b4f3"), (object) new
      {
        processId = processId,
        witRefName = witRefName,
        ruleId = ruleId
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<ProcessRule>> GetProcessWorkItemTypeRulesAsync(
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<ProcessRule>>(new HttpMethod("GET"), new Guid("76fe3432-d825-479d-a5f6-983bbb78b4f3"), (object) new
      {
        processId = processId,
        witRefName = witRefName
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ProcessRule> UpdateProcessWorkItemTypeRuleAsync(
      UpdateProcessRuleRequest processRule,
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
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateProcessRuleRequest>(processRule, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProcessRule>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<WorkItemStateResultModel> CreateStateDefinitionAsync(
      WorkItemStateInputModel stateModel,
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("31015d57-2dff-4a46-adb3-2fb4ee3dcec9");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemStateInputModel>(stateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemStateResultModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteStateDefinitionAsync(
      Guid processId,
      string witRefName,
      Guid stateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("31015d57-2dff-4a46-adb3-2fb4ee3dcec9"), (object) new
      {
        processId = processId,
        witRefName = witRefName,
        stateId = stateId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<WorkItemStateResultModel> GetStateDefinitionAsync(
      Guid processId,
      string witRefName,
      Guid stateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemStateResultModel>(new HttpMethod("GET"), new Guid("31015d57-2dff-4a46-adb3-2fb4ee3dcec9"), (object) new
      {
        processId = processId,
        witRefName = witRefName,
        stateId = stateId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<WorkItemStateResultModel>> GetStateDefinitionsAsync(
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemStateResultModel>>(new HttpMethod("GET"), new Guid("31015d57-2dff-4a46-adb3-2fb4ee3dcec9"), (object) new
      {
        processId = processId,
        witRefName = witRefName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WorkItemStateResultModel> HideStateDefinitionAsync(
      HideStateModel hideStateModel,
      Guid processId,
      string witRefName,
      Guid stateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("31015d57-2dff-4a46-adb3-2fb4ee3dcec9");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName,
        stateId = stateId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<HideStateModel>(hideStateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemStateResultModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<WorkItemStateResultModel> UpdateStateDefinitionAsync(
      WorkItemStateInputModel stateModel,
      Guid processId,
      string witRefName,
      Guid stateId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("31015d57-2dff-4a46-adb3-2fb4ee3dcec9");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName,
        stateId = stateId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemStateInputModel>(stateModel, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemStateResultModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Control[]> DeleteSystemControlAsync(
      Guid processId,
      string witRefName,
      string controlId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Control[]>(new HttpMethod("DELETE"), new Guid("ff9a3d2c-32b7-4c6c-991c-d5a251fb9098"), (object) new
      {
        processId = processId,
        witRefName = witRefName,
        controlId = controlId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Control[]> GetSystemControlsAsync(
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Control[]>(new HttpMethod("GET"), new Guid("ff9a3d2c-32b7-4c6c-991c-d5a251fb9098"), (object) new
      {
        processId = processId,
        witRefName = witRefName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Control> UpdateSystemControlAsync(
      Control control,
      Guid processId,
      string witRefName,
      string controlId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ff9a3d2c-32b7-4c6c-991c-d5a251fb9098");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName,
        controlId = controlId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Control>(control, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Control>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<ProcessWorkItemType> CreateProcessWorkItemTypeAsync(
      CreateProcessWorkItemTypeRequest workItemType,
      Guid processId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e2e9d1a6-432d-4062-8870-bfcb8c324ad7");
      object obj1 = (object) new{ processId = processId };
      HttpContent httpContent = (HttpContent) new ObjectContent<CreateProcessWorkItemTypeRequest>(workItemType, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProcessWorkItemType>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteProcessWorkItemTypeAsync(
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("e2e9d1a6-432d-4062-8870-bfcb8c324ad7"), (object) new
      {
        processId = processId,
        witRefName = witRefName
      }, new ApiResourceVersion(7.2, 2), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<ProcessWorkItemType> GetProcessWorkItemTypeAsync(
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
      return this.SendAsync<ProcessWorkItemType>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<ProcessWorkItemType>> GetProcessWorkItemTypesAsync(
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
      return this.SendAsync<List<ProcessWorkItemType>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<ProcessWorkItemType> UpdateProcessWorkItemTypeAsync(
      UpdateProcessWorkItemTypeRequest workItemTypeUpdate,
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e2e9d1a6-432d-4062-8870-bfcb8c324ad7");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateProcessWorkItemTypeRequest>(workItemTypeUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ProcessWorkItemType>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<WorkItemTypeBehavior> AddBehaviorToWorkItemTypeAsync(
      WorkItemTypeBehavior behavior,
      Guid processId,
      string witRefNameForBehaviors,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6d765a2e-4e1b-4b11-be93-f953be676024");
      object obj1 = (object) new
      {
        processId = processId,
        witRefNameForBehaviors = witRefNameForBehaviors
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemTypeBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemTypeBehavior>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<WorkItemTypeBehavior> GetBehaviorForWorkItemTypeAsync(
      Guid processId,
      string witRefNameForBehaviors,
      string behaviorRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemTypeBehavior>(new HttpMethod("GET"), new Guid("6d765a2e-4e1b-4b11-be93-f953be676024"), (object) new
      {
        processId = processId,
        witRefNameForBehaviors = witRefNameForBehaviors,
        behaviorRefName = behaviorRefName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<WorkItemTypeBehavior>> GetBehaviorsForWorkItemTypeAsync(
      Guid processId,
      string witRefNameForBehaviors,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemTypeBehavior>>(new HttpMethod("GET"), new Guid("6d765a2e-4e1b-4b11-be93-f953be676024"), (object) new
      {
        processId = processId,
        witRefNameForBehaviors = witRefNameForBehaviors
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RemoveBehaviorFromWorkItemTypeAsync(
      Guid processId,
      string witRefNameForBehaviors,
      string behaviorRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("6d765a2e-4e1b-4b11-be93-f953be676024"), (object) new
      {
        processId = processId,
        witRefNameForBehaviors = witRefNameForBehaviors,
        behaviorRefName = behaviorRefName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<WorkItemTypeBehavior> UpdateBehaviorToWorkItemTypeAsync(
      WorkItemTypeBehavior behavior,
      Guid processId,
      string witRefNameForBehaviors,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("6d765a2e-4e1b-4b11-be93-f953be676024");
      object obj1 = (object) new
      {
        processId = processId,
        witRefNameForBehaviors = witRefNameForBehaviors
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemTypeBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemTypeBehavior>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
