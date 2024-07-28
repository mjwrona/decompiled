// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.WebApi.Generated.CargoApiHttpClient
// Assembly: Microsoft.VisualStudio.Services.Cargo.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 79D1C655-766F-4F71-AAEA-7C02E794C2F8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.WebApi.dll

using Microsoft.VisualStudio.Services.Cargo.WebApi.Types;
using Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cargo.WebApi.Generated
{
  [ResourceArea("71F96160-8701-4914-AED9-C44B89F20CCD")]
  public class CargoApiHttpClient : VssHttpClientBase
  {
    public CargoApiHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public CargoApiHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public CargoApiHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public CargoApiHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public CargoApiHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task UpdatePackageVersionsAsync(
      CargoPackagesBatchRequest batchRequest,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CargoApiHttpClient cargoApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2b169a5b-f125-4c5b-85e5-9e9c0b52387b");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<CargoPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CargoApiHttpClient cargoApiHttpClient2 = cargoApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await cargoApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdatePackageVersionsAsync(
      CargoPackagesBatchRequest batchRequest,
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CargoApiHttpClient cargoApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2b169a5b-f125-4c5b-85e5-9e9c0b52387b");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CargoPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CargoApiHttpClient cargoApiHttpClient2 = cargoApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await cargoApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdatePackageVersionsAsync(
      CargoPackagesBatchRequest batchRequest,
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CargoApiHttpClient cargoApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("2b169a5b-f125-4c5b-85e5-9e9c0b52387b");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CargoPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CargoApiHttpClient cargoApiHttpClient2 = cargoApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await cargoApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdateRecycleBinPackageVersionsAsync(
      CargoPackagesBatchRequest batchRequest,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CargoApiHttpClient cargoApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0e5ddda8-e228-4eb6-9cc5-2eb6651a5560");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<CargoPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CargoApiHttpClient cargoApiHttpClient2 = cargoApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await cargoApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdateRecycleBinPackageVersionsAsync(
      CargoPackagesBatchRequest batchRequest,
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CargoApiHttpClient cargoApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0e5ddda8-e228-4eb6-9cc5-2eb6651a5560");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CargoPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CargoApiHttpClient cargoApiHttpClient2 = cargoApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await cargoApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdateRecycleBinPackageVersionsAsync(
      CargoPackagesBatchRequest batchRequest,
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CargoApiHttpClient cargoApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("0e5ddda8-e228-4eb6-9cc5-2eb6651a5560");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CargoPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CargoApiHttpClient cargoApiHttpClient2 = cargoApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await cargoApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task DeletePackageVersionFromRecycleBinAsync(
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("e6af1663-81e3-4d52-81e4-ed51d362efbc"), (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task DeletePackageVersionFromRecycleBinAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("e6af1663-81e3-4d52-81e4-ed51d362efbc"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task DeletePackageVersionFromRecycleBinAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("e6af1663-81e3-4d52-81e4-ed51d362efbc"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<CargoPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBinAsync(
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CargoPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("e6af1663-81e3-4d52-81e4-ed51d362efbc"), (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<CargoPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBinAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CargoPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("e6af1663-81e3-4d52-81e4-ed51d362efbc"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<CargoPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBinAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<CargoPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("e6af1663-81e3-4d52-81e4-ed51d362efbc"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RestorePackageVersionFromRecycleBinAsync(
      CargoRecycleBinPackageVersionDetails packageVersionDetails,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CargoApiHttpClient cargoApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e6af1663-81e3-4d52-81e4-ed51d362efbc");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CargoRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CargoApiHttpClient cargoApiHttpClient2 = cargoApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await cargoApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task RestorePackageVersionFromRecycleBinAsync(
      CargoRecycleBinPackageVersionDetails packageVersionDetails,
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CargoApiHttpClient cargoApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e6af1663-81e3-4d52-81e4-ed51d362efbc");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CargoRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CargoApiHttpClient cargoApiHttpClient2 = cargoApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await cargoApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task RestorePackageVersionFromRecycleBinAsync(
      CargoRecycleBinPackageVersionDetails packageVersionDetails,
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CargoApiHttpClient cargoApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e6af1663-81e3-4d52-81e4-ed51d362efbc");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<CargoRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CargoApiHttpClient cargoApiHttpClient2 = cargoApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await cargoApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public Task<UpstreamingBehavior> GetUpstreamingBehaviorAsync(
      string feed,
      string packageName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("190eeaa1-54fd-4568-84ff-5a5c46edbfc7"), (object) new
      {
        feed = feed,
        packageName = packageName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<UpstreamingBehavior> GetUpstreamingBehaviorAsync(
      string project,
      string feed,
      string packageName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("190eeaa1-54fd-4568-84ff-5a5c46edbfc7"), (object) new
      {
        project = project,
        feed = feed,
        packageName = packageName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<UpstreamingBehavior> GetUpstreamingBehaviorAsync(
      Guid project,
      string feed,
      string packageName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("190eeaa1-54fd-4568-84ff-5a5c46edbfc7"), (object) new
      {
        project = project,
        feed = feed,
        packageName = packageName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task SetUpstreamingBehaviorAsync(
      string feed,
      string packageName,
      UpstreamingBehavior behavior,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CargoApiHttpClient cargoApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("190eeaa1-54fd-4568-84ff-5a5c46edbfc7");
      object obj1 = (object) new
      {
        feed = feed,
        packageName = packageName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CargoApiHttpClient cargoApiHttpClient2 = cargoApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await cargoApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task SetUpstreamingBehaviorAsync(
      string project,
      string feed,
      string packageName,
      UpstreamingBehavior behavior,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CargoApiHttpClient cargoApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("190eeaa1-54fd-4568-84ff-5a5c46edbfc7");
      object obj1 = (object) new
      {
        project = project,
        feed = feed,
        packageName = packageName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CargoApiHttpClient cargoApiHttpClient2 = cargoApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await cargoApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task SetUpstreamingBehaviorAsync(
      Guid project,
      string feed,
      string packageName,
      UpstreamingBehavior behavior,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CargoApiHttpClient cargoApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("190eeaa1-54fd-4568-84ff-5a5c46edbfc7");
      object obj1 = (object) new
      {
        project = project,
        feed = feed,
        packageName = packageName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CargoApiHttpClient cargoApiHttpClient2 = cargoApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await cargoApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public Task<Package> DeletePackageVersionAsync(
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("DELETE"), new Guid("d0921ac1-d9a3-4f3b-bf39-6e6647ab1367"), (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> DeletePackageVersionAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("DELETE"), new Guid("d0921ac1-d9a3-4f3b-bf39-6e6647ab1367"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> DeletePackageVersionAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("DELETE"), new Guid("d0921ac1-d9a3-4f3b-bf39-6e6647ab1367"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> GetPackageVersionAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      bool? showDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d0921ac1-d9a3-4f3b-bf39-6e6647ab1367");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (showDeleted.HasValue)
        keyValuePairList.Add(nameof (showDeleted), showDeleted.Value.ToString());
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> GetPackageVersionAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      bool? showDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d0921ac1-d9a3-4f3b-bf39-6e6647ab1367");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (showDeleted.HasValue)
        keyValuePairList.Add(nameof (showDeleted), showDeleted.Value.ToString());
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> GetPackageVersionAsync(
      string feedId,
      string packageName,
      string packageVersion,
      bool? showDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d0921ac1-d9a3-4f3b-bf39-6e6647ab1367");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (showDeleted.HasValue)
        keyValuePairList.Add(nameof (showDeleted), showDeleted.Value.ToString());
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task UpdatePackageVersionAsync(
      PackageVersionDetails packageVersionDetails,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CargoApiHttpClient cargoApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d0921ac1-d9a3-4f3b-bf39-6e6647ab1367");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CargoApiHttpClient cargoApiHttpClient2 = cargoApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await cargoApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdatePackageVersionAsync(
      PackageVersionDetails packageVersionDetails,
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CargoApiHttpClient cargoApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d0921ac1-d9a3-4f3b-bf39-6e6647ab1367");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CargoApiHttpClient cargoApiHttpClient2 = cargoApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await cargoApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdatePackageVersionAsync(
      PackageVersionDetails packageVersionDetails,
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CargoApiHttpClient cargoApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d0921ac1-d9a3-4f3b-bf39-6e6647ab1367");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      CargoApiHttpClient cargoApiHttpClient2 = cargoApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await cargoApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
