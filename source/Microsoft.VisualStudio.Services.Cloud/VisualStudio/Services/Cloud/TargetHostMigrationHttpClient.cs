// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.TargetHostMigrationHttpClient
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class TargetHostMigrationHttpClient : VssHttpClientBase
  {
    private static readonly Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>()
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

    public TargetHostMigrationHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TargetHostMigrationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TargetHostMigrationHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<TargetHostMigration> CreateMigrationEntryAsync(
      TargetHostMigration migrationRequest,
      object userState = null)
    {
      return this.SendAsync<TargetHostMigration>(new HttpRequestMessage(HttpMethod.Put, VssHttpUriUtility.ConcatUri(this.BaseAddress, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/migration/targethostmigration", (object) HttpRouteCollectionExtensions.DefaultRoutePrefix)))
      {
        Content = (HttpContent) new ObjectContent<TargetHostMigration>(migrationRequest, this.Formatter)
      }, userState);
    }

    public Task<TargetHostMigration> UpdateMigrationEntryAsync(
      TargetHostMigration migrationRequest,
      object userState = null)
    {
      return this.SendAsync<TargetHostMigration>(new HttpRequestMessage(new HttpMethod("PATCH"), VssHttpUriUtility.ConcatUri(this.BaseAddress, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/migration/TargetHostMigration", (object) HttpRouteCollectionExtensions.DefaultRoutePrefix)))
      {
        Content = (HttpContent) new ObjectContent<TargetHostMigration>(migrationRequest, this.Formatter)
      }, userState);
    }

    public Task<TargetHostMigration> GetMigrationEntryAsync(Guid migrationId, object userState = null) => this.SendAsync<TargetHostMigration>(new HttpRequestMessage(HttpMethod.Get, VssHttpUriUtility.ConcatUri(this.BaseAddress, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/migration/TargetHostMigration/{1}", (object) HttpRouteCollectionExtensions.DefaultRoutePrefix, (object) migrationId))), userState);

    public TargetHostMigration GetMigrationEntry(Guid migrationId, object userState = null)
    {
      TargetHostMigration migrationEntry = (TargetHostMigration) null;
      try
      {
        migrationEntry = this.GetMigrationEntryAsync(migrationId, userState).SyncResult<TargetHostMigration>();
      }
      catch (VssServiceResponseException ex)
      {
        int httpStatusCode = (int) ex.HttpStatusCode;
      }
      return migrationEntry;
    }

    public Task<string> FinalizationCheckAsync(FinalizationCheckRequest request, object userState = null) => this.SendAsync<string>(new HttpRequestMessage(new HttpMethod("PATCH"), VssHttpUriUtility.ConcatUri(this.BaseAddress, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/migration/{1}", (object) HttpRouteCollectionExtensions.DefaultRoutePrefix, (object) "FinalizationCheck")))
    {
      Content = (HttpContent) new ObjectContent<FinalizationCheckRequest>(request, this.Formatter)
    }, userState);

    public string FinalizationCheck(
      SourceHostMigration sourceMigration,
      IEnumerable<Guid> databaseBackedHostIds,
      IEnumerable<Guid> virtualHostIds,
      object userState = null)
    {
      ArgumentUtility.CheckForNull<SourceHostMigration>(sourceMigration, nameof (sourceMigration));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(databaseBackedHostIds, nameof (databaseBackedHostIds));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(virtualHostIds, nameof (virtualHostIds));
      return this.FinalizationCheckAsync(new FinalizationCheckRequest()
      {
        SourceMigration = sourceMigration,
        DatabaseBackedHostIds = databaseBackedHostIds.ToArray<Guid>(),
        VirtualHostIds = virtualHostIds.ToArray<Guid>()
      }).SyncResult<string>();
    }

    public Task<TargetHostMigration> DeleteTargetMigrationAsync(Guid migrationId, object userState = null) => this.SendAsync<TargetHostMigration>(new HttpRequestMessage(HttpMethod.Delete, VssHttpUriUtility.ConcatUri(this.BaseAddress, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/migration/TargetHostMigration/{1}", (object) HttpRouteCollectionExtensions.DefaultRoutePrefix, (object) migrationId))), userState);

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) TargetHostMigrationHttpClient.s_translatedExceptions;
  }
}
