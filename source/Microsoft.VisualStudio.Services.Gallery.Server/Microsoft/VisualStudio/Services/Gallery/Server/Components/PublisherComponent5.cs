// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublisherComponent5
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublisherComponent5 : PublisherComponent4
  {
    public virtual PublisherDbModel CreatePublisher(
      string publisherName,
      string displayName,
      Guid ownerId,
      PublisherFlags flags,
      string shortDescription,
      string longDescription,
      Guid publisherKey,
      DateTime creationTime,
      PublisherMetadata metadata)
    {
      this.PrepareStoredProcedure("Gallery.prc_CreatePublisher");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (displayName), displayName, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid(nameof (ownerId), ownerId);
      this.BindInt(nameof (flags), (int) flags);
      this.BindString(nameof (shortDescription), shortDescription, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (longDescription), longDescription, 1024, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableGuid(nameof (publisherKey), publisherKey);
      this.BindDateTime(nameof (creationTime), creationTime);
      string parameterValue;
      try
      {
        parameterValue = JsonConvert.SerializeObject((object) metadata, new JsonSerializerSettings()
        {
          ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
        });
      }
      catch (JsonSerializationException ex)
      {
        throw new PublisherMetadataSerializationException(GalleryResources.PublisherMetadataSerializationError(), (Exception) ex);
      }
      this.BindString(nameof (metadata), parameterValue, 4000, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_CreatePublisher", this.RequestContext))
      {
        resultCollection.AddBinder<PublisherDbModel>((ObjectBinder<PublisherDbModel>) new PublisherComponent5.PublisherDbModelBinder());
        return resultCollection.GetCurrent<PublisherDbModel>().Items[0];
      }
    }

    public virtual PublisherDbModel UpdatePublisher(
      string publisherName,
      string displayName,
      PublisherFlags flags,
      string shortDescription,
      string longDescription,
      Guid publisherKey,
      DateTime updateTime,
      int? logoFileId,
      PublisherMetadata metadata)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdatePublisher");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (displayName), displayName, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableInt(nameof (flags), (int) flags, 1073741824);
      this.BindString(nameof (shortDescription), shortDescription, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (longDescription), longDescription, 1024, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableGuid(nameof (publisherKey), publisherKey);
      this.BindDateTime(nameof (updateTime), updateTime);
      this.BindNullableInt(nameof (logoFileId), !logoFileId.HasValue ? new int?(-1) : logoFileId);
      string parameterValue;
      try
      {
        parameterValue = JsonConvert.SerializeObject((object) metadata, new JsonSerializerSettings()
        {
          ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
        });
        if (parameterValue.Length > 4000)
          throw new PublisherMetadataLengthExceededMaxLimitException(GalleryResources.PublisherMetadataLengthExceeded());
      }
      catch (JsonSerializationException ex)
      {
        throw new PublisherMetadataSerializationException(GalleryResources.PublisherMetadataSerializationError(), (Exception) ex);
      }
      this.BindString(nameof (metadata), parameterValue, 4000, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdatePublisher", this.RequestContext))
      {
        resultCollection.AddBinder<PublisherDbModel>((ObjectBinder<PublisherDbModel>) new PublisherComponent5.PublisherDbModelBinder());
        return resultCollection.GetCurrent<PublisherDbModel>().Items[0];
      }
    }

    public virtual PublisherDbModel QueryPublisherDbModel(
      string publisherName,
      PublisherQueryFlags flags)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryPublisher");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt(nameof (flags), (int) flags);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_QueryPublisher", this.RequestContext))
      {
        resultCollection.AddBinder<PublisherDbModel>((ObjectBinder<PublisherDbModel>) this.GetPublisherDbModelBinder());
        Dictionary<Guid, PublisherDbModel> dictionary = this.ProcessPublisherDbModelResult(resultCollection, flags);
        return dictionary.Count != 0 ? dictionary.Values.First<PublisherDbModel>() : throw new PublisherDoesNotExistException(GalleryWebApiResources.PublisherDoesNotExist());
      }
    }

    public virtual PublisherDbModelQueryResult QueryPublisherDbModelsByQuery(
      List<QueryFilter> filters,
      List<QueryFilterValue> filterValues,
      PublisherQueryFlags flags)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryPublishersByQuery");
      this.BindQueryFilterTable("queryFilter", (IEnumerable<QueryFilter>) filters);
      this.BindQueryFilterValueTable("queryFilterValue", (IEnumerable<QueryFilterValue>) filterValues);
      this.BindInt(nameof (flags), (int) flags);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.QueryExtensionsByQuery", this.RequestContext))
      {
        resultCollection.AddBinder<QueryMatch>((ObjectBinder<QueryMatch>) new QueryMatchBinder());
        List<QueryMatch> items = resultCollection.GetCurrent<QueryMatch>().Items;
        resultCollection.AddBinder<PublisherDbModel>((ObjectBinder<PublisherDbModel>) this.GetPublisherDbModelBinder());
        resultCollection.NextResult();
        Dictionary<Guid, PublisherDbModel> dictionary = this.ProcessPublisherDbModelResult(resultCollection, flags);
        PublisherDbModelQueryResult modelQueryResult = new PublisherDbModelQueryResult();
        modelQueryResult.Results = new List<PublisherDbModelFilterResult>();
        foreach (QueryFilter filter in filters)
          modelQueryResult.Results.Add(new PublisherDbModelFilterResult());
        foreach (QueryMatch queryMatch in items)
        {
          PublisherDbModelFilterResult result = modelQueryResult.Results[queryMatch.QueryIndex];
          if (result.Publishers == null)
            result.Publishers = new List<PublisherDbModel>();
          result.Publishers.Add(dictionary[queryMatch.ReferenceId]);
        }
        return modelQueryResult;
      }
    }

    protected Dictionary<Guid, PublisherDbModel> ProcessPublisherDbModelResult(
      ResultCollection resultCollection,
      PublisherQueryFlags flags)
    {
      List<PublisherDbModel> items = resultCollection.GetCurrent<PublisherDbModel>().Items;
      Dictionary<Guid, PublisherDbModel> dictionary = new Dictionary<Guid, PublisherDbModel>();
      foreach (PublisherDbModel publisherDbModel in items)
        dictionary[publisherDbModel.PublisherId] = publisherDbModel;
      if ((flags & PublisherQueryFlags.IncludeExtensions) != PublisherQueryFlags.None)
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) new PublishedExtensionBinder());
        resultCollection.NextResult();
        foreach (PublishedExtension publishedExtension in resultCollection.GetCurrent<PublishedExtension>().Items)
        {
          PublisherDbModel publisherDbModel = dictionary[publishedExtension.Publisher.PublisherId];
          if (publisherDbModel.Extensions == null)
            publisherDbModel.Extensions = new List<PublishedExtension>();
          publisherDbModel.Extensions.Add(publishedExtension);
        }
      }
      return dictionary;
    }

    protected virtual PublisherComponent5.PublisherDbModelBinder GetPublisherDbModelBinder() => new PublisherComponent5.PublisherDbModelBinder();

    internal class PublisherDbModelBinder : ObjectBinder<PublisherDbModel>
    {
      protected SqlColumnBinder publisherIdColumn = new SqlColumnBinder("PublisherId");
      protected SqlColumnBinder publisherNameColumn = new SqlColumnBinder("PublisherName");
      protected SqlColumnBinder displayNameColumn = new SqlColumnBinder("DisplayName");
      protected SqlColumnBinder flagsColumn = new SqlColumnBinder("Flags");
      protected SqlColumnBinder lastUpdatedColumn = new SqlColumnBinder("LastUpdated");
      protected SqlColumnBinder shortDescriptionColumn = new SqlColumnBinder("ShortDescription");
      protected SqlColumnBinder longDescriptionColumn = new SqlColumnBinder("LongDescription");
      protected SqlColumnBinder publisherKeyColumn = new SqlColumnBinder("PublisherKey");
      protected SqlColumnBinder logoFileIdColumn = new SqlColumnBinder("LogoFileId");
      protected SqlColumnBinder metadataColumn = new SqlColumnBinder("Metadata");

      protected override PublisherDbModel Bind()
      {
        PublisherMetadata publisherMetadata = (PublisherMetadata) null;
        try
        {
          string str = this.metadataColumn.GetString((IDataReader) this.Reader, true);
          if (!string.IsNullOrEmpty(str))
            publisherMetadata = JsonConvert.DeserializeObject<PublisherMetadata>(str, new JsonSerializerSettings()
            {
              ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
            });
        }
        catch (Exception ex)
        {
        }
        PublisherDbModel publisherDbModel = new PublisherDbModel();
        publisherDbModel.PublisherId = this.publisherIdColumn.GetGuid((IDataReader) this.Reader);
        publisherDbModel.PublisherName = this.publisherNameColumn.GetString((IDataReader) this.Reader, false);
        publisherDbModel.DisplayName = this.displayNameColumn.GetString((IDataReader) this.Reader, false);
        publisherDbModel.Flags = (PublisherFlags) this.flagsColumn.GetInt32((IDataReader) this.Reader);
        publisherDbModel.LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) this.Reader);
        publisherDbModel.ShortDescription = this.shortDescriptionColumn.GetString((IDataReader) this.Reader, true);
        publisherDbModel.LongDescription = this.longDescriptionColumn.GetString((IDataReader) this.Reader, true);
        publisherDbModel.LogoFileId = this.logoFileIdColumn.GetInt32((IDataReader) this.Reader);
        publisherDbModel.Metadata = publisherMetadata;
        return publisherDbModel;
      }
    }
  }
}
