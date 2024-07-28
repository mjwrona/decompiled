// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Zeus.DatabaseMigrationHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Zeus
{
  [ResourceArea("{D56223DF-8CCD-45C9-89B4-EDDF69240000}")]
  public abstract class DatabaseMigrationHttpClientBase : VssHttpClientBase
  {
    public DatabaseMigrationHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public DatabaseMigrationHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public DatabaseMigrationHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public DatabaseMigrationHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public DatabaseMigrationHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<HttpResponseMessage> DeleteDatabaseMigrationAsync(
      int migrationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync(new HttpMethod("DELETE"), new Guid("d56223df-8ccd-45c9-89b4-eddf69240000"), (object) new
      {
        migrationId = migrationId
      }, new ApiResourceVersion("2.0-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<DatabaseMigration> GetDatabaseMigrationAsync(
      int migrationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<DatabaseMigration>(new HttpMethod("GET"), new Guid("d56223df-8ccd-45c9-89b4-eddf69240000"), (object) new
      {
        migrationId = migrationId
      }, new ApiResourceVersion("2.0-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<DatabaseMigration>> GetDatabaseMigrationsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<DatabaseMigration>>(new HttpMethod("GET"), new Guid("d56223df-8ccd-45c9-89b4-eddf69240000"), version: new ApiResourceVersion("2.0-preview.1"), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<DatabaseMigration> QueueDatabaseMigrationAsync(
      DatabaseMigration migration,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d56223df-8ccd-45c9-89b4-eddf69240000");
      HttpContent httpContent = (HttpContent) new ObjectContent<DatabaseMigration>(migration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("2.0-preview.1");
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DatabaseMigration>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<DatabaseMigration> UpdateDatabaseMigrationAsync(
      DatabaseMigration migration,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("d56223df-8ccd-45c9-89b4-eddf69240000");
      HttpContent httpContent = (HttpContent) new ObjectContent<DatabaseMigration>(migration, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion("2.0-preview.1");
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<DatabaseMigration>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
