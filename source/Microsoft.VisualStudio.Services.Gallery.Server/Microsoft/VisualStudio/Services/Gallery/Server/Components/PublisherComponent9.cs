// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublisherComponent9
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
  internal class PublisherComponent9 : PublisherComponent8
  {
    public override Publisher QueryPublisher(string publisherName, PublisherQueryFlags flags)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryPublisher");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt(nameof (flags), (int) flags);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_QueryPublisher", this.RequestContext))
      {
        resultCollection.AddBinder<Publisher>((ObjectBinder<Publisher>) new PublisherComponent.PublisherBinder2());
        Dictionary<Guid, Publisher> dictionary = this.ProcessPublisherResult(resultCollection, flags);
        return dictionary.Count != 0 ? dictionary.Values.First<Publisher>() : throw new PublisherDoesNotExistException(GalleryWebApiResources.PublisherDoesNotExist());
      }
    }

    public virtual void UpdateDomainVerificationState(
      Guid publisherId,
      UpdateVerificationStateOperation operation,
      DateTime updatedAt)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdateDomainVerificationState");
      this.BindString(nameof (publisherId), publisherId.ToString(), 36, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (operation), operation.ToString(), 14, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindDateTime("updateTime", updatedAt);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdateDomainVerificationState", this.RequestContext);
    }

    protected override PublisherComponent5.PublisherDbModelBinder GetPublisherDbModelBinder() => (PublisherComponent5.PublisherDbModelBinder) new PublisherComponent9.PublisherDbModelBinder4();

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
      bool removeDomain = false,
      bool isVerifiedManually = false,
      Guid token = default (Guid))
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdatePublisher");
      if (token == new Guid())
        token = Guid.NewGuid();
      this.BindGuid(nameof (token), token);
      this.UpdatePublisherBindings(publisherName, displayName, flags, shortDescription, longDescription, publisherKey, updateTime, logoFileId, metadata, state, domain, isVerified, removeDomain, isVerifiedManually);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdatePublisher", this.RequestContext))
      {
        resultCollection.AddBinder<PublisherDbModel>((ObjectBinder<PublisherDbModel>) this.GetPublisherDbModelBinder());
        return resultCollection.GetCurrent<PublisherDbModel>().Items[0];
      }
    }

    public List<PublisherDomainModel> FetchVerifiedPublishers()
    {
      this.PrepareStoredProcedure("Gallery.prc_FetchVerifiedPublishers");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_FetchVerifiedPublishers", this.RequestContext))
      {
        resultCollection.AddBinder<PublisherDomainModel>((ObjectBinder<PublisherDomainModel>) new PublisherComponent9.PublisherDomainModelBinder());
        return resultCollection.GetCurrent<PublisherDomainModel>().Items;
      }
    }

    internal class PublisherDbModelBinder4 : PublisherComponent7.PublisherDbModelBinder3
    {
      protected SqlColumnBinder isDnsTokenVerifiedColumn = new SqlColumnBinder("IsDnsTokenVerified");

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
        publisherDbModel.IsDnsTokenVerified = !this.isDnsTokenVerifiedColumn.IsNull((IDataReader) this.Reader) && this.isDnsTokenVerifiedColumn.GetBoolean((IDataReader) this.Reader);
        return publisherDbModel;
      }
    }

    internal class PublisherDomainModelBinder : ObjectBinder<PublisherDomainModel>
    {
      protected SqlColumnBinder publisherIdColumn = new SqlColumnBinder("PublisherId");
      protected SqlColumnBinder publisherNameColumn = new SqlColumnBinder("PublisherName");
      protected SqlColumnBinder domainColumn = new SqlColumnBinder("Domain");
      protected SqlColumnBinder tokenColumn = new SqlColumnBinder("Token");

      protected override PublisherDomainModel Bind() => new PublisherDomainModel()
      {
        PublisherId = this.publisherIdColumn.GetGuid((IDataReader) this.Reader),
        PublisherName = this.publisherNameColumn.GetString((IDataReader) this.Reader, false),
        Domain = this.domainColumn.GetString((IDataReader) this.Reader, true),
        Token = this.tokenColumn.GetGuid((IDataReader) this.Reader)
      };
    }
  }
}
