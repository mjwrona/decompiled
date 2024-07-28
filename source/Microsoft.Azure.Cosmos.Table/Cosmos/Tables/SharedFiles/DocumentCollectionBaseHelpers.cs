// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tables.SharedFiles.DocumentCollectionBaseHelpers
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Tables.SharedFiles
{
  internal sealed class DocumentCollectionBaseHelpers
  {
    private const int DefaultOfferThroughput = 800;
    private const string TestPartitionKey = "{D345FA99-84DC-48CC-BFCE-2CF8ED99D80D}";
    private const string TestRowKey = "{E18FAFBA-1953-4CB9-83B8-066D2121717D}";

    public static async Task<ResourceResponse<DocumentCollection>> HandleDocumentCollectionRetrieveAsync(
      string tableName,
      IDocumentClient client)
    {
      return await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri("TablesDB", tableName).ToString());
    }

    public static async Task<ResourceResponse<DocumentCollection>> HandleCollectionFeedInsertAsync(
      IDocumentClient client,
      string collectionName,
      IndexingPolicy indexingPolicy,
      RequestOptions requestOption,
      CancellationToken cancellationToken)
    {
      await DocumentCollectionBaseHelpers.ThrowIfCollectionExists(client, collectionName);
      RequestOptions requestOptions = requestOption;
      if ((requestOptions != null ? (!requestOptions.OfferThroughput.HasValue ? 1 : 0) : 1) != 0)
      {
        if (!await DocumentCollectionBaseHelpers.IsSharedThroughputEnabledAsync(client))
        {
          if (requestOption == null)
            requestOption = new RequestOptions();
          requestOption.OfferThroughput = new int?(800);
        }
      }
      ResourceResponse<DocumentCollection> collectionResponse = (ResourceResponse<DocumentCollection>) null;
      try
      {
        collectionResponse = await DocumentCollectionBaseHelpers.CreateDocumentCollectionAsync(collectionName, client, indexingPolicy, requestOption);
      }
      catch (DocumentClientException ex)
      {
        HttpStatusCode? statusCode = ex.StatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.NotFound;
        if (!(statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue))
          throw;
        ResourceResponse<Database> tablesDb = await DocumentCollectionBaseHelpers.CreateTablesDB(client);
        collectionResponse = await DocumentCollectionBaseHelpers.CreateDocumentCollectionAsync(collectionName, client, indexingPolicy, requestOption);
      }
      return collectionResponse;
    }

    public static async Task<ResourceResponse<Database>> CreateTablesDB(IDocumentClient client)
    {
      ResourceResponse<Database> response = (ResourceResponse<Database>) null;
      try
      {
        IDocumentClient documentClient = client;
        Database database = new Database();
        database.Id = "TablesDB";
        response = await documentClient.CreateDatabaseAsync(database);
      }
      catch (DocumentClientException ex)
      {
        HttpStatusCode? statusCode = ex.StatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.Conflict;
        if (!(statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue))
          throw;
      }
      return response;
    }

    private static async Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionAsync(
      string collectionName,
      IDocumentClient client,
      IndexingPolicy indexingPolicy,
      RequestOptions requestOption)
    {
      Uri databaseUri = UriFactory.CreateDatabaseUri("TablesDB");
      IDocumentClient documentClient = client;
      string databaseLink = databaseUri.ToString();
      DocumentCollection documentCollection1 = new DocumentCollection();
      documentCollection1.Id = collectionName;
      documentCollection1.PartitionKey = new PartitionKeyDefinition()
      {
        Paths = {
          "/'$pk'"
        }
      };
      DocumentCollection documentCollection2 = documentCollection1;
      IndexingPolicy indexingPolicy1 = indexingPolicy;
      if (indexingPolicy1 == null)
        indexingPolicy1 = new IndexingPolicy(new Index[2]
        {
          (Index) Index.Range(DataType.Number, (short) -1),
          (Index) Index.Range(DataType.String, (short) -1)
        });
      documentCollection2.IndexingPolicy = indexingPolicy1;
      DocumentCollection documentCollection3 = documentCollection1;
      RequestOptions options = requestOption;
      return await documentClient.CreateDocumentCollectionAsync(databaseLink, documentCollection3, options);
    }

    private static async Task<bool> IsSharedThroughputEnabledAsync(IDocumentClient client)
    {
      Uri databaseUri = UriFactory.CreateDatabaseUri("TablesDB");
      try
      {
        return (await client.CreateOfferQuery(string.Format("SELECT * FROM offers o WHERE o.resource = '{0}'", (object) (await client.ReadDatabaseAsync(databaseUri)).Resource.SelfLink)).AsDocumentQuery<object>().ExecuteNextAsync<Offer>()).AsEnumerable<Offer>().SingleOrDefault<Offer>() != null;
      }
      catch (NotFoundException ex)
      {
        return false;
      }
      catch (DocumentClientException ex)
      {
        if (ex.StatusCode.HasValue && ex.StatusCode.Value.Equals((object) HttpStatusCode.NotFound))
          return false;
        throw;
      }
    }

    private static async Task ThrowIfCollectionExists(IDocumentClient client, string tableName)
    {
      bool collectionExists = false;
      try
      {
        ResourceResponse<Document> resourceResponse = await DocumentEntityCollectionBaseHelpers.HandleEntityRetrieveAsync(tableName, "{D345FA99-84DC-48CC-BFCE-2CF8ED99D80D}", "{E18FAFBA-1953-4CB9-83B8-066D2121717D}", client, (RequestOptions) null, CancellationToken.None);
        collectionExists = true;
      }
      catch (DocumentClientException ex)
      {
        HttpStatusCode? statusCode = ex.StatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.NotFound;
        if (statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue)
        {
          if (!string.IsNullOrWhiteSpace(ex.ResourceAddress))
          {
            if (ex.ResourceAddress.ToLowerInvariant().Contains("/docs/"))
              collectionExists = true;
          }
        }
      }
      if (collectionExists)
        throw new DocumentClientException("The specified table already exists.", HttpStatusCode.Conflict, SubStatusCodes.Unknown);
    }
  }
}
