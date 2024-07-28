// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Account.Client.AccountHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Account.Client
{
  [ResourceArea("0D55247A-1C47-4462-9B1F-5E2125590EE6")]
  public class AccountHttpClient : AccountVersion1HttpClient
  {
    private const double c_apiVersion = 5.0;

    public AccountHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
      this.CurrentApiVersion = new ApiResourceVersion(5.0);
    }

    public AccountHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
      this.CurrentApiVersion = new ApiResourceVersion(5.0);
    }

    public AccountHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
      this.CurrentApiVersion = new ApiResourceVersion(5.0);
    }

    public AccountHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
      this.CurrentApiVersion = new ApiResourceVersion(5.0);
    }

    public AccountHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
      this.CurrentApiVersion = new ApiResourceVersion(5.0);
    }

    public async Task<List<Microsoft.VisualStudio.Services.Account.Account>> GetAccountsByOwnerAsync(
      Guid ownerId,
      IEnumerable<string> propertyNameFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountHttpClient accountHttpClient1 = this;
      List<Microsoft.VisualStudio.Services.Account.Account> accountsByOwnerAsync;
      using (new VssHttpClientBase.OperationScope("Account", "GetAccountsByOwner"))
      {
        ArgumentUtility.CheckForEmptyGuid(ownerId, nameof (ownerId));
        AccountHttpClient accountHttpClient2 = accountHttpClient1;
        Guid? nullable = new Guid?(ownerId);
        IEnumerable<string> strings = propertyNameFilter;
        Guid? creatorId = new Guid?();
        Guid? ownerId1 = nullable;
        Guid? memberId = new Guid?();
        Guid? accountId = new Guid?();
        bool? includeOwner = new bool?(false);
        bool? includeDisabledAccounts = new bool?(false);
        bool? includeDeletedUsers = new bool?(false);
        IEnumerable<string> propertyNames = strings;
        bool? usePrecreated = new bool?(false);
        List<KeyValuePair<string, string>> queryParameters = accountHttpClient2.AppendQueryString(creatorId, ownerId1, memberId, accountId, includeOwner, includeDisabledAccounts, includeDeletedUsers, propertyNames, usePrecreated);
        accountsByOwnerAsync = await accountHttpClient1.GetAsync<List<Microsoft.VisualStudio.Services.Account.Account>>(AccountResourceIds.Account, version: accountHttpClient1.CurrentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return accountsByOwnerAsync;
    }

    public async Task<List<Microsoft.VisualStudio.Services.Account.Account>> GetAccountsByMemberAsync(
      Guid memberId,
      IEnumerable<string> propertyNameFilter = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AccountHttpClient accountHttpClient1 = this;
      List<Microsoft.VisualStudio.Services.Account.Account> accountsByMemberAsync;
      using (new VssHttpClientBase.OperationScope("Account", "GetAccountsByMember"))
      {
        ArgumentUtility.CheckForEmptyGuid(memberId, nameof (memberId));
        AccountHttpClient accountHttpClient2 = accountHttpClient1;
        Guid? nullable = new Guid?(memberId);
        IEnumerable<string> strings = propertyNameFilter;
        Guid? creatorId = new Guid?();
        Guid? ownerId = new Guid?();
        Guid? memberId1 = nullable;
        Guid? accountId = new Guid?();
        bool? includeOwner = new bool?(false);
        bool? includeDisabledAccounts = new bool?(false);
        bool? includeDeletedUsers = new bool?(false);
        IEnumerable<string> propertyNames = strings;
        bool? usePrecreated = new bool?(false);
        List<KeyValuePair<string, string>> queryParameters = accountHttpClient2.AppendQueryString(creatorId, ownerId, memberId1, accountId, includeOwner, includeDisabledAccounts, includeDeletedUsers, propertyNames, usePrecreated);
        accountsByMemberAsync = await accountHttpClient1.GetAsync<List<Microsoft.VisualStudio.Services.Account.Account>>(AccountResourceIds.Account, version: new ApiResourceVersion(new Version(5, 0), 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
      return accountsByMemberAsync;
    }
  }
}
