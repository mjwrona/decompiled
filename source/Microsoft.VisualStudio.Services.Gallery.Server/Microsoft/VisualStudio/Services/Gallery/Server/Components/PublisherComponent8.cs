// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublisherComponent8
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
  internal class PublisherComponent8 : PublisherComponent7
  {
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
      bool isVerifiedManually = false)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdatePublisher");
      this.UpdatePublisherBindings(publisherName, displayName, flags, shortDescription, longDescription, publisherKey, updateTime, logoFileId, metadata, state, domain, isVerified, removeDomain, isVerifiedManually);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdatePublisher", this.RequestContext))
      {
        resultCollection.AddBinder<PublisherDbModel>((ObjectBinder<PublisherDbModel>) this.GetPublisherDbModelBinder());
        return resultCollection.GetCurrent<PublisherDbModel>().Items[0];
      }
    }

    public virtual Guid FetchDomainToken(string publisherName)
    {
      this.PrepareStoredProcedure("Gallery.prc_FetchDomainToken");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_FetchDomainToken", this.RequestContext))
      {
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new PublisherComponent8.DomainTokenBinder());
        Guid guid = resultCollection.GetCurrent<Guid>().Items[0];
        return !(guid == Guid.Empty) ? guid : throw new PublisherDomainDoesNotExistException(GalleryWebApiResources.PublisherDomainDoesNotExist());
      }
    }

    protected void UpdatePublisherBindings(
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
      this.BindBoolean("verifiedManually", isVerifiedManually);
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
    }

    internal class DomainTokenBinder : ObjectBinder<Guid>
    {
      private SqlColumnBinder tokenColumn = new SqlColumnBinder("Token");

      protected override Guid Bind() => this.tokenColumn.GetGuid((IDataReader) this.Reader, true);
    }
  }
}
