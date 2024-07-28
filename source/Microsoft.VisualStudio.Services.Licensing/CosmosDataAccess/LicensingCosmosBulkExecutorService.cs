// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess.LicensingCosmosBulkExecutorService
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.Azure.CosmosDB.BulkExecutor.BulkImport;
using Microsoft.Azure.Documents;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.DocDB;
using Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess
{
  public class LicensingCosmosBulkExecutorService : DocDBBulkExecutorService
  {
    internal void Upsert<TDocument>(
      IVssRequestContext requestContext,
      IEnumerable<TDocument> documents)
      where TDocument : DocDBSerializableDocument, new()
    {
      requestContext.RunSynchronously((Func<Task>) (async () =>
      {
        BulkImportResponse bulkImportResponse = await this.UpsertAsync<TDocument>(requestContext, documents, new CancellationToken());
        string format = string.Format("Imported {0} documents in {1}, consuming {2} RUs", (object) bulkImportResponse.NumberOfDocumentsImported, (object) bulkImportResponse.TotalTimeTaken, (object) bulkImportResponse.TotalRequestUnitsConsumed);
        requestContext.TraceAlways(1035400, TraceLevel.Info, nameof (LicensingCosmosBulkExecutorService), nameof (Upsert), format);
      }));
    }

    internal void Import<TDocument>(
      IVssRequestContext requestContext,
      IEnumerable<TDocument> documents)
      where TDocument : DocDBSerializableDocument, new()
    {
      if (documents.Any<TDocument>((Func<TDocument, bool>) (doc => ((Resource) (object) doc).Id == null)) || documents.Any<TDocument>((Func<TDocument, bool>) (doc => doc.PartitionKey == null)))
        throw new ArgumentException("Cannot import licensing documents which have not been previously created by the regular ADO flow.");
      requestContext.RunSynchronously((Func<Task>) (async () =>
      {
        BulkImportResponse bulkImportResponse = await this.ImportAsync<TDocument>(documents, new CancellationToken());
        string format = string.Format("Migrated {0} documents in {1}, consuming {2} RUs", (object) bulkImportResponse.NumberOfDocumentsImported, (object) bulkImportResponse.TotalTimeTaken, (object) bulkImportResponse.TotalRequestUnitsConsumed);
        requestContext.TraceAlways(1035400, TraceLevel.Info, nameof (LicensingCosmosBulkExecutorService), nameof (Import), format);
      }));
    }

    internal void DeletePartition(IVssRequestContext requestContext, PartitionKey partitionKey = null)
    {
      SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem> documentMethodOptions = new SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>();
      ((DocDBMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) documentMethodOptions).ExplicitPartitionKey = partitionKey;
      SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem> methodOptions = documentMethodOptions;
      requestContext.RunSynchronously((Func<Task>) (async () =>
      {
        DocDBBulkDeleteResponse bulkDeleteResponse = await this.DeleteAsync<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>(requestContext, methodOptions, new CancellationToken());
        DocDBBulkOperationResponse queryResponse = bulkDeleteResponse.QueryResponse;
        string format1 = string.Format("Retrieved {0} ID tuples in {1}, consuming {2} RUs", (object) queryResponse.NumberOfDocumentsAffected, (object) queryResponse.TotalTimeTaken, (object) queryResponse.TotalRequestUnitsConsumed);
        requestContext.TraceAlways(1035401, TraceLevel.Info, nameof (LicensingCosmosBulkExecutorService), nameof (DeletePartition), format1);
        DocDBBulkOperationResponse deleteResponse = bulkDeleteResponse.DeleteResponse;
        string format2 = string.Format("Deleted {0} documents in {1}, consuming {2} RUs", (object) deleteResponse.NumberOfDocumentsAffected, (object) deleteResponse.TotalTimeTaken, (object) deleteResponse.TotalRequestUnitsConsumed);
        requestContext.TraceAlways(1035401, TraceLevel.Info, nameof (LicensingCosmosBulkExecutorService), nameof (DeletePartition), format2);
      }));
    }

    protected virtual string CollectionId => "Licensing";
  }
}
