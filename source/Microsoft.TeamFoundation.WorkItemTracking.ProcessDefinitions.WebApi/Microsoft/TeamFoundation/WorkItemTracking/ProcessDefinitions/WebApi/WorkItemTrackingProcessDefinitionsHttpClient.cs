// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.WorkItemTrackingProcessDefinitionsHttpClient
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BABD213-FC9A-4DAB-8690-D2FF2DA1955C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.dll

using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi
{
  [ResourceArea("5264459E-E5E0-4BD8-B118-0985E68A4EC5")]
  public class WorkItemTrackingProcessDefinitionsHttpClient : VssHttpClientBase
  {
    public WorkItemTrackingProcessDefinitionsHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public WorkItemTrackingProcessDefinitionsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public WorkItemTrackingProcessDefinitionsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public WorkItemTrackingProcessDefinitionsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public WorkItemTrackingProcessDefinitionsHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<BehaviorModel> CreateBehaviorAsync(
      BehaviorCreateModel behavior,
      Guid processId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("47a651f4-fb70-43bf-b96b-7c0ba947142b");
      object obj1 = (object) new{ processId = processId };
      HttpContent httpContent = (HttpContent) new ObjectContent<BehaviorCreateModel>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BehaviorModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteBehaviorAsync(
      Guid processId,
      string behaviorId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("47a651f4-fb70-43bf-b96b-7c0ba947142b"), (object) new
      {
        processId = processId,
        behaviorId = behaviorId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<BehaviorModel> GetBehaviorAsync(
      Guid processId,
      string behaviorId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<BehaviorModel>(new HttpMethod("GET"), new Guid("47a651f4-fb70-43bf-b96b-7c0ba947142b"), (object) new
      {
        processId = processId,
        behaviorId = behaviorId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<BehaviorModel>> GetBehaviorsAsync(
      Guid processId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<BehaviorModel>>(new HttpMethod("GET"), new Guid("47a651f4-fb70-43bf-b96b-7c0ba947142b"), (object) new
      {
        processId = processId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<BehaviorModel> ReplaceBehaviorAsync(
      BehaviorReplaceModel behaviorData,
      Guid processId,
      string behaviorId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("47a651f4-fb70-43bf-b96b-7c0ba947142b");
      object obj1 = (object) new
      {
        processId = processId,
        behaviorId = behaviorId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<BehaviorReplaceModel>(behaviorData, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<BehaviorModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Control> AddControlToGroupAsync(
      Control control,
      Guid processId,
      string witRefName,
      string groupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e2e3166a-627a-4e9b-85b2-d6a097bbd731");
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

    public Task<Control> EditControlAsync(
      Control control,
      Guid processId,
      string witRefName,
      string groupId,
      string controlId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e2e3166a-627a-4e9b-85b2-d6a097bbd731");
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

    public async Task RemoveControlFromGroupAsync(
      Guid processId,
      string witRefName,
      string groupId,
      string controlId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("e2e3166a-627a-4e9b-85b2-d6a097bbd731"), (object) new
      {
        processId = processId,
        witRefName = witRefName,
        groupId = groupId,
        controlId = controlId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<Control> SetControlInGroupAsync(
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
      Guid guid = new Guid("e2e3166a-627a-4e9b-85b2-d6a097bbd731");
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

    public Task<FieldModel> CreateFieldAsync(
      FieldModel field,
      Guid processId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("f36c66c7-911d-4163-8938-d3c5d0d7f5aa");
      object obj1 = (object) new{ processId = processId };
      HttpContent httpContent = (HttpContent) new ObjectContent<FieldModel>(field, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FieldModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<FieldModel> UpdateFieldAsync(
      FieldUpdate field,
      Guid processId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("f36c66c7-911d-4163-8938-d3c5d0d7f5aa");
      object obj1 = (object) new{ processId = processId };
      HttpContent httpContent = (HttpContent) new ObjectContent<FieldUpdate>(field, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FieldModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
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
      Guid guid = new Guid("2617828b-e850-4375-a92a-04855704d4c3");
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

    public Task<Group> EditGroupAsync(
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
      Guid guid = new Guid("2617828b-e850-4375-a92a-04855704d4c3");
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

    public async Task RemoveGroupAsync(
      Guid processId,
      string witRefName,
      string pageId,
      string sectionId,
      string groupId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("2617828b-e850-4375-a92a-04855704d4c3"), (object) new
      {
        processId = processId,
        witRefName = witRefName,
        pageId = pageId,
        sectionId = sectionId,
        groupId = groupId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<Group> SetGroupInPageAsync(
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
      Guid guid = new Guid("2617828b-e850-4375-a92a-04855704d4c3");
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

    public Task<Group> SetGroupInSectionAsync(
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
      Guid guid = new Guid("2617828b-e850-4375-a92a-04855704d4c3");
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

    public Task<FormLayout> GetFormLayoutAsync(
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<FormLayout>(new HttpMethod("GET"), new Guid("3eacc80a-ddca-4404-857a-6331aac99063"), (object) new
      {
        processId = processId,
        witRefName = witRefName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<PickListMetadataModel>> GetListsMetadataAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<PickListMetadataModel>>(new HttpMethod("GET"), new Guid("b45cc931-98e3-44a1-b1cd-2e8e9c6dc1c6"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<PickListModel> CreateListAsync(
      PickListModel picklist,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0b6179e2-23ce-46b2-b094-2ffa5ee70286");
      HttpContent httpContent = (HttpContent) new ObjectContent<PickListModel>(picklist, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PickListModel>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteListAsync(
      Guid listId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("0b6179e2-23ce-46b2-b094-2ffa5ee70286"), (object) new
      {
        listId = listId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<PickListModel> GetListAsync(
      Guid listId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PickListModel>(new HttpMethod("GET"), new Guid("0b6179e2-23ce-46b2-b094-2ffa5ee70286"), (object) new
      {
        listId = listId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<PickListModel> UpdateListAsync(
      PickListModel picklist,
      Guid listId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("0b6179e2-23ce-46b2-b094-2ffa5ee70286");
      object obj1 = (object) new{ listId = listId };
      HttpContent httpContent = (HttpContent) new ObjectContent<PickListModel>(picklist, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PickListModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Page> AddPageAsync(
      Page page,
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1b4ac126-59b2-4f37-b4df-0a48ba807edb");
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

    public Task<Page> EditPageAsync(
      Page page,
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1b4ac126-59b2-4f37-b4df-0a48ba807edb");
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
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("1b4ac126-59b2-4f37-b4df-0a48ba807edb"), (object) new
      {
        processId = processId,
        witRefName = witRefName,
        pageId = pageId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<WorkItemStateResultModel> CreateStateDefinitionAsync(
      WorkItemStateInputModel stateModel,
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4303625d-08f4-4461-b14b-32c65bba5599");
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
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("4303625d-08f4-4461-b14b-32c65bba5599"), (object) new
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
      return this.SendAsync<WorkItemStateResultModel>(new HttpMethod("GET"), new Guid("4303625d-08f4-4461-b14b-32c65bba5599"), (object) new
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
      return this.SendAsync<List<WorkItemStateResultModel>>(new HttpMethod("GET"), new Guid("4303625d-08f4-4461-b14b-32c65bba5599"), (object) new
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
      Guid guid = new Guid("4303625d-08f4-4461-b14b-32c65bba5599");
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
      Guid guid = new Guid("4303625d-08f4-4461-b14b-32c65bba5599");
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

    public Task<WorkItemTypeBehavior> AddBehaviorToWorkItemTypeAsync(
      WorkItemTypeBehavior behavior,
      Guid processId,
      string witRefNameForBehaviors,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("921dfb88-ef57-4c69-94e5-dd7da2d7031d");
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
      return this.SendAsync<WorkItemTypeBehavior>(new HttpMethod("GET"), new Guid("921dfb88-ef57-4c69-94e5-dd7da2d7031d"), (object) new
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
      return this.SendAsync<List<WorkItemTypeBehavior>>(new HttpMethod("GET"), new Guid("921dfb88-ef57-4c69-94e5-dd7da2d7031d"), (object) new
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
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("921dfb88-ef57-4c69-94e5-dd7da2d7031d"), (object) new
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
      Guid guid = new Guid("921dfb88-ef57-4c69-94e5-dd7da2d7031d");
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

    public Task<WorkItemTypeModel> CreateWorkItemTypeAsync(
      WorkItemTypeModel workItemType,
      Guid processId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("1ce0acad-4638-49c3-969c-04aa65ba6bea");
      object obj1 = (object) new{ processId = processId };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemTypeModel>(workItemType, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemTypeModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteWorkItemTypeAsync(
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("1ce0acad-4638-49c3-969c-04aa65ba6bea"), (object) new
      {
        processId = processId,
        witRefName = witRefName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<WorkItemTypeModel> GetWorkItemTypeAsync(
      Guid processId,
      string witRefName,
      GetWorkItemTypeExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1ce0acad-4638-49c3-969c-04aa65ba6bea");
      object routeValues = (object) new
      {
        processId = processId,
        witRefName = witRefName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<WorkItemTypeModel>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<WorkItemTypeModel>> GetWorkItemTypesAsync(
      Guid processId,
      GetWorkItemTypeExpand? expand = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1ce0acad-4638-49c3-969c-04aa65ba6bea");
      object routeValues = (object) new
      {
        processId = processId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (expand.HasValue)
        keyValuePairList.Add("$expand", expand.Value.ToString());
      return this.SendAsync<List<WorkItemTypeModel>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<WorkItemTypeModel> UpdateWorkItemTypeAsync(
      WorkItemTypeUpdateModel workItemTypeUpdate,
      Guid processId,
      string witRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("1ce0acad-4638-49c3-969c-04aa65ba6bea");
      object obj1 = (object) new
      {
        processId = processId,
        witRefName = witRefName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemTypeUpdateModel>(workItemTypeUpdate, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemTypeModel>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<WorkItemTypeFieldModel2> AddFieldToWorkItemTypeAsync(
      WorkItemTypeFieldModel2 field,
      Guid processId,
      string witRefNameForFields,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("976713b4-a62e-499e-94dc-eeb869ea9126");
      object obj1 = (object) new
      {
        processId = processId,
        witRefNameForFields = witRefNameForFields
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemTypeFieldModel2>(field, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemTypeFieldModel2>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<WorkItemTypeFieldModel2> GetWorkItemTypeFieldAsync(
      Guid processId,
      string witRefNameForFields,
      string fieldRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<WorkItemTypeFieldModel2>(new HttpMethod("GET"), new Guid("976713b4-a62e-499e-94dc-eeb869ea9126"), (object) new
      {
        processId = processId,
        witRefNameForFields = witRefNameForFields,
        fieldRefName = fieldRefName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<WorkItemTypeFieldModel2>> GetWorkItemTypeFieldsAsync(
      Guid processId,
      string witRefNameForFields,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<WorkItemTypeFieldModel2>>(new HttpMethod("GET"), new Guid("976713b4-a62e-499e-94dc-eeb869ea9126"), (object) new
      {
        processId = processId,
        witRefNameForFields = witRefNameForFields
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RemoveFieldFromWorkItemTypeAsync(
      Guid processId,
      string witRefNameForFields,
      string fieldRefName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("976713b4-a62e-499e-94dc-eeb869ea9126"), (object) new
      {
        processId = processId,
        witRefNameForFields = witRefNameForFields,
        fieldRefName = fieldRefName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<WorkItemTypeFieldModel2> UpdateWorkItemTypeFieldAsync(
      WorkItemTypeFieldModel2 field,
      Guid processId,
      string witRefNameForFields,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("976713b4-a62e-499e-94dc-eeb869ea9126");
      object obj1 = (object) new
      {
        processId = processId,
        witRefNameForFields = witRefNameForFields
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<WorkItemTypeFieldModel2>(field, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<WorkItemTypeFieldModel2>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
