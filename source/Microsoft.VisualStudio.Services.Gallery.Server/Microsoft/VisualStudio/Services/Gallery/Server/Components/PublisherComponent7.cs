// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublisherComponent7
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublisherComponent7 : PublisherComponent6
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
      PublisherMetadata metadata,
      PublisherState state,
      string domain)
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
      this.BindInt("publisherState", (int) state);
      this.BindString(nameof (domain), domain, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
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
        resultCollection.AddBinder<PublisherDbModel>((ObjectBinder<PublisherDbModel>) new PublisherComponent7.PublisherDbModelBinder3());
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
      PublisherMetadata metadata,
      PublisherState state,
      string domain,
      bool isVerified = false,
      bool removeDomain = false)
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
      this.BindInt("publisherState", (int) state);
      this.BindString(nameof (domain), domain, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindBoolean(nameof (isVerified), isVerified);
      this.BindBoolean(nameof (removeDomain), removeDomain);
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
        resultCollection.AddBinder<PublisherDbModel>((ObjectBinder<PublisherDbModel>) new PublisherComponent7.PublisherDbModelBinder3());
        return resultCollection.GetCurrent<PublisherDbModel>().Items[0];
      }
    }

    protected override PublisherComponent5.PublisherDbModelBinder GetPublisherDbModelBinder() => (PublisherComponent5.PublisherDbModelBinder) new PublisherComponent7.PublisherDbModelBinder3();

    internal class PublisherDbModelBinder3 : PublisherComponent6.PublisherDbModelBinder2
    {
      protected SqlColumnBinder domainColumn = new SqlColumnBinder("Domain");
      protected SqlColumnBinder isDomainVerifiedColumn = new SqlColumnBinder("IsVerified");

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
        catch (JsonSerializationException ex)
        {
          throw new PublisherMetadataDeserializationException(GalleryResources.PublisherMetadataDeserializationError(), (Exception) ex);
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
        publisherDbModel.State = (PublisherState) this.publisherStateColumn.GetInt32((IDataReader) this.Reader);
        publisherDbModel.Domain = this.domainColumn.GetString((IDataReader) this.Reader, true);
        publisherDbModel.IsDomainVerified = !this.isDomainVerifiedColumn.IsNull((IDataReader) this.Reader) && this.isDomainVerifiedColumn.GetBoolean((IDataReader) this.Reader);
        return publisherDbModel;
      }
    }
  }
}
