// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.ProjectContracter
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Parsing;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Utils;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  internal class ProjectContracter : EntityContracter
  {
    public ProjectContracter(IndexingExecutionContext indexingExecutionContext, IndexInfo indexInfo)
      : base(indexingExecutionContext.ServiceSettings.ProvisionerSettings, indexInfo.GetDocumentContractType(indexingExecutionContext), (IRoutingDataProvider) new ProjectEntityRoutingProvider(indexInfo?.Routing))
    {
    }

    public override AbstractSearchDocumentContract GetContractDocumentInternal(
      IndexingExecutionContext indexingExecutionContext,
      IMetaDataStoreItem documentItem)
    {
      IObjectStoreItem crawledItem1;
      if (!this.RetrieveCrawledItem(documentItem, "Metadata", out crawledItem1))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetProjectContractBatch.AddProjectContractToBatch : Failed to get metadata store document for objectId: '{0}', key: '{1}' and path: '{2}'", (object) documentItem.ContentId.HexHash, (object) "Metadata", (object) documentItem.Path));
      byte[] blob = crawledItem1.Blob;
      if (blob == null || blob.Length == 0)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetProjectContractBatch.AddProjectContractToBatch : No Content found for metadata key with Content Id {0} and Path {1}", (object) documentItem.ContentId.HexHash, (object) documentItem.Path));
      TextEncoding textEncoding = new TextEncoding();
      ProjectRepositoryEntityMetadata repositoryEntityMetadata1 = new ProjectRepositoryEntityMetadata();
      byte[] bytes = blob;
      ProjectRepositoryEntityMetadata repositoryEntityMetadata2 = ProjectContracter.DeserializeFromJson(textEncoding.GetString(bytes));
      string contentKey;
      switch (repositoryEntityMetadata2.EntityType)
      {
        case CrawlEntityType.Project:
          contentKey = "Description";
          break;
        case CrawlEntityType.Repository:
          contentKey = "ReadMe";
          break;
        default:
          contentKey = "Description";
          break;
      }
      IObjectStoreItem crawledItem2;
      if (!this.RetrieveCrawledItem(documentItem, contentKey, out crawledItem2))
        throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetProjectContractBatch.AddProjectContractToBatch : Failed to get crawlstore document for description key: '{0}', key: '{1}' and path: '{2}'", (object) documentItem.ContentId.HexHash, (object) "Description", (object) documentItem.Path));
      byte[] numArray = crawledItem2 != null && crawledItem2.Blob != null ? crawledItem2.Blob : throw new InvalidDataException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetProjectContractBatch.AddProjectContractToBatch : No Content found for description key with Content Id {0} and Path {1}", (object) documentItem.ContentId.HexHash, (object) documentItem.Path));
      ProjectRepoContractBase contract = (ProjectRepoContractBase) ProjectRepoContractFactory.CreateContract(repositoryEntityMetadata2.EntityType);
      IVssRequestContext requestContext = indexingExecutionContext.RequestContext;
      ProjectRepositoryEntityMetadata data = repositoryEntityMetadata2;
      IMetaDataStore metaDataStore = this.MetaDataStore;
      ParsedData parsedData = new ParsedData();
      parsedData.Content = numArray;
      ProvisionerSettings settings = this.Settings;
      contract.PopulateFileContractDetails(requestContext, (object) data, metaDataStore, parsedData, settings);
      return (AbstractSearchDocumentContract) contract;
    }

    internal bool RetrieveCrawledItem(
      IMetaDataStoreItem documentItem,
      string contentKey,
      out IObjectStoreItem crawledItem)
    {
      return this.CrawlStore.TryGet(new ContentId(new Hash(documentItem.ContentId.RawHash), contentKey), out crawledItem);
    }

    private static ProjectRepositoryEntityMetadata DeserializeFromJson(string input)
    {
      using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(input)))
        return (ProjectRepositoryEntityMetadata) new DataContractJsonSerializer(typeof (ProjectRepositoryEntityMetadata)).ReadObject((Stream) memoryStream);
    }
  }
}
