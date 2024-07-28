// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublisherComponent4
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublisherComponent4 : PublisherComponent3
  {
    public override Publisher CreatePublisher(
      string publisherName,
      string displayName,
      Guid ownerId,
      PublisherFlags flags,
      string shortDescription,
      string longDescription,
      Guid publisherKey,
      DateTime creationTime)
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
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_CreatePublisher", this.RequestContext))
      {
        resultCollection.AddBinder<Publisher>((ObjectBinder<Publisher>) new PublisherComponent.PublisherBinder());
        return resultCollection.GetCurrent<Publisher>().Items[0];
      }
    }

    public override Publisher UpdatePublisher(
      string publisherName,
      string displayName,
      PublisherFlags flags,
      string shortDescription,
      string longDescription,
      Guid publisherKey,
      DateTime updateTime)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdatePublisher");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (displayName), displayName, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableInt(nameof (flags), (int) flags, 1073741824);
      this.BindString(nameof (shortDescription), shortDescription, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (longDescription), longDescription, 1024, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindNullableGuid(nameof (publisherKey), publisherKey);
      this.BindDateTime(nameof (updateTime), updateTime);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdatePublisher", this.RequestContext))
      {
        resultCollection.AddBinder<Publisher>((ObjectBinder<Publisher>) new PublisherComponent.PublisherBinder());
        return resultCollection.GetCurrent<Publisher>().Items[0];
      }
    }
  }
}
