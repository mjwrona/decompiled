// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SourceHostMigrationHttpClient
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.HostMigration;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Migration;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class SourceHostMigrationHttpClient : VssHttpClientBase
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>()
    {
      {
        "InvalidAccessException",
        typeof (InvalidAccessException)
      },
      {
        "DataMigrationEntryAlreadyExistsException",
        typeof (DataMigrationEntryAlreadyExistsException)
      }
    };

    public SourceHostMigrationHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public SourceHostMigrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public SourceHostMigrationHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<SourceHostMigration> CreateMigrationEntryAsync(
      SourceHostMigration migrationRequest,
      object userState = null)
    {
      return this.SendAsync<SourceHostMigration>(new HttpRequestMessage(HttpMethod.Put, VssHttpUriUtility.ConcatUri(this.BaseAddress, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/migration/sourcehostmigration", (object) HttpRouteCollectionExtensions.DefaultRoutePrefix)))
      {
        Content = (HttpContent) new ObjectContent<SourceHostMigration>(migrationRequest, this.Formatter)
      }, userState);
    }

    public Task<SourceHostMigration> UpdateMigrationEntryAsync(
      SourceHostMigration migrationRequest,
      object userState = null)
    {
      return this.SendAsync<SourceHostMigration>(new HttpRequestMessage(new HttpMethod("PATCH"), VssHttpUriUtility.ConcatUri(this.BaseAddress, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/migration/sourcehostmigration", (object) HttpRouteCollectionExtensions.DefaultRoutePrefix)))
      {
        Content = (HttpContent) new ObjectContent<SourceHostMigration>(migrationRequest, this.Formatter)
      }, userState);
    }

    public Task<SourceHostMigration> GetMigrationEntryAsync(
      Guid migrationId,
      string hostId = null,
      object userState = null)
    {
      HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, VssHttpUriUtility.ConcatUri(this.BaseAddress, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/migration/sourcehostmigration/{1}", (object) HttpRouteCollectionExtensions.DefaultRoutePrefix, (object) migrationId)));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(hostId))
        collection.Add(nameof (hostId), hostId);
      Guid migrationsLocationId = MigrationResourceIds.SourceMigrationsLocationId;
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      var routeValues = new{ migrationId = migrationId };
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = userState;
      CancellationToken cancellationToken = new CancellationToken();
      return this.GetAsync<SourceHostMigration>(migrationsLocationId, (object) routeValues, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken);
    }

    public SourceHostMigration GetMigrationEntry(Guid migrationId, string hostId = null, object userState = null)
    {
      SourceHostMigration migrationEntry = (SourceHostMigration) null;
      try
      {
        migrationEntry = this.GetMigrationEntryAsync(migrationId, hostId, userState).SyncResult<SourceHostMigration>();
      }
      catch (VssServiceResponseException ex)
      {
        int httpStatusCode = (int) ex.HttpStatusCode;
      }
      return migrationEntry;
    }

    public Task<SourceHostMigration> DeleteSourceMigrationAsync(Guid migrationId, object userState = null) => this.SendAsync<SourceHostMigration>(new HttpRequestMessage(HttpMethod.Delete, VssHttpUriUtility.ConcatUri(this.BaseAddress, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/migration/sourcehostmigration/{1}", (object) HttpRouteCollectionExtensions.DefaultRoutePrefix, (object) migrationId))), userState);

    public List<AdHocJobInfo> GetQueuedAdHocJobs(Guid jobSource) => this.SendAsync<List<AdHocJobInfo>>(new HttpRequestMessage(HttpMethod.Get, VssHttpUriUtility.ConcatUri(this.BaseAddress, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/migration/QueuedAdHocJobs/{1}", (object) HttpRouteCollectionExtensions.DefaultRoutePrefix, (object) jobSource)))).SyncResult<List<AdHocJobInfo>>();

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) SourceHostMigrationHttpClient.s_translatedExceptions;
  }
}
