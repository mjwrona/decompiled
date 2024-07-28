// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.TagsOperationsExtensions
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest.Azure;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ResourceManager
{
  public static class TagsOperationsExtensions
  {
    public static void DeleteValue(
      this ITagsOperations operations,
      string tagName,
      string tagValue)
    {
      operations.DeleteValueAsync(tagName, tagValue).GetAwaiter().GetResult();
    }

    public static async Task DeleteValueAsync(
      this ITagsOperations operations,
      string tagName,
      string tagValue,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteValueWithHttpMessagesAsync(tagName, tagValue, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static TagValue CreateOrUpdateValue(
      this ITagsOperations operations,
      string tagName,
      string tagValue)
    {
      return operations.CreateOrUpdateValueAsync(tagName, tagValue).GetAwaiter().GetResult();
    }

    public static async Task<TagValue> CreateOrUpdateValueAsync(
      this ITagsOperations operations,
      string tagName,
      string tagValue,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TagValue body;
      using (AzureOperationResponse<TagValue> _result = await operations.CreateOrUpdateValueWithHttpMessagesAsync(tagName, tagValue, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static TagDetails CreateOrUpdate(this ITagsOperations operations, string tagName) => operations.CreateOrUpdateAsync(tagName).GetAwaiter().GetResult();

    public static async Task<TagDetails> CreateOrUpdateAsync(
      this ITagsOperations operations,
      string tagName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TagDetails body;
      using (AzureOperationResponse<TagDetails> _result = await operations.CreateOrUpdateWithHttpMessagesAsync(tagName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void Delete(this ITagsOperations operations, string tagName) => operations.DeleteAsync(tagName).GetAwaiter().GetResult();

    public static async Task DeleteAsync(
      this ITagsOperations operations,
      string tagName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteWithHttpMessagesAsync(tagName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static IPage<TagDetails> List(this ITagsOperations operations) => operations.ListAsync().GetAwaiter().GetResult();

    public static async Task<IPage<TagDetails>> ListAsync(
      this ITagsOperations operations,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<TagDetails> body;
      using (AzureOperationResponse<IPage<TagDetails>> _result = await operations.ListWithHttpMessagesAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static TagsResource CreateOrUpdateAtScope(
      this ITagsOperations operations,
      string scope,
      TagsResource parameters)
    {
      return operations.CreateOrUpdateAtScopeAsync(scope, parameters).GetAwaiter().GetResult();
    }

    public static async Task<TagsResource> CreateOrUpdateAtScopeAsync(
      this ITagsOperations operations,
      string scope,
      TagsResource parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TagsResource body;
      using (AzureOperationResponse<TagsResource> _result = await operations.CreateOrUpdateAtScopeWithHttpMessagesAsync(scope, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static TagsResource UpdateAtScope(
      this ITagsOperations operations,
      string scope,
      TagsPatchResource parameters)
    {
      return operations.UpdateAtScopeAsync(scope, parameters).GetAwaiter().GetResult();
    }

    public static async Task<TagsResource> UpdateAtScopeAsync(
      this ITagsOperations operations,
      string scope,
      TagsPatchResource parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TagsResource body;
      using (AzureOperationResponse<TagsResource> _result = await operations.UpdateAtScopeWithHttpMessagesAsync(scope, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static TagsResource GetAtScope(this ITagsOperations operations, string scope) => operations.GetAtScopeAsync(scope).GetAwaiter().GetResult();

    public static async Task<TagsResource> GetAtScopeAsync(
      this ITagsOperations operations,
      string scope,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TagsResource body;
      using (AzureOperationResponse<TagsResource> _result = await operations.GetAtScopeWithHttpMessagesAsync(scope, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void DeleteAtScope(this ITagsOperations operations, string scope) => operations.DeleteAtScopeAsync(scope).GetAwaiter().GetResult();

    public static async Task DeleteAtScopeAsync(
      this ITagsOperations operations,
      string scope,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteAtScopeWithHttpMessagesAsync(scope, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static IPage<TagDetails> ListNext(this ITagsOperations operations, string nextPageLink) => operations.ListNextAsync(nextPageLink).GetAwaiter().GetResult();

    public static async Task<IPage<TagDetails>> ListNextAsync(
      this ITagsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<TagDetails> body;
      using (AzureOperationResponse<IPage<TagDetails>> _result = await operations.ListNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }
  }
}
