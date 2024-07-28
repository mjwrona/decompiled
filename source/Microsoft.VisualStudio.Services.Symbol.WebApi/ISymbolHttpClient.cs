// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.ISymbolHttpClient
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  public interface ISymbolHttpClient : IArtifactHttpClient
  {
    Task<DebugEntry> CreateDebugEntryAsync(
      string requestId,
      DebugEntry entry,
      CancellationToken cancellationToken);

    Task<List<DebugEntry>> CreateDebugEntriesAsync(
      string requestId,
      DebugEntryCreateBatch batch,
      CancellationToken cancellationToken);

    Task<Request> CreateRequestAsync(Request request, CancellationToken cancellationToken);

    Task<HttpResponseMessage> DeleteRequestAsync(
      string requestId,
      CancellationToken cancellationToken,
      bool synchronous = false);

    Task<Request> FinalizeRequestAsync(
      string requestId,
      DateTime? expirationDate,
      bool isUpdateOperation,
      CancellationToken cancellationToken);

    Task<List<Request>> GetAllRequestsAsync(
      CancellationToken cancellationToken,
      SizeOptions sizeOptions = null,
      ExpirationDateOptions expirationDateOptions = null,
      IDomainId domainIdOption = null,
      RetrievalOptions retrievalOptions = RetrievalOptions.ExcludeSoftDeleted,
      RequestStatus? requestStatus = null);

    Task<List<Request>> GetRequestPaginatedAsync(
      string continueFromRequestId,
      int pageSize,
      CancellationToken cancellationToken,
      SizeOptions sizeOptions = null,
      ExpirationDateOptions expirationDateOptions = null,
      IDomainId domainIdOption = null,
      RetrievalOptions retrievalOptions = RetrievalOptions.ExcludeSoftDeleted,
      RequestStatus? requestStatus = null);

    Task<List<DebugEntry>> GetDebugEntriesAsync(
      string path,
      int? startEntry,
      int? maxEntries,
      DebugEntrySortOrder sortOrder,
      CancellationToken cancellationToken);

    Task<HttpResponseMessage> GetOptionsAsync(Guid location, CancellationToken cancellationToken);

    Task<Request> GetRequestAsync(string requestId, CancellationToken cancellationToken);

    Task<Request> GetRequestByNameAsync(string requestName, CancellationToken cancellationToken);

    Task<List<DebugEntry>> GetRequestDebugEntriesAsync(
      string requestId,
      string debugEntryId,
      CancellationToken cancellationToken);

    Task<HttpResponseMessage> GetSymSrvItemAsync(string path, CancellationToken cancellationToken);

    Task<bool> IsAvailableAsync(CancellationToken cancellationToken);

    Task<Request> UpdateRequestAsync(
      string requestId,
      DateTime expirationDate,
      CancellationToken cancellationToken);
  }
}
