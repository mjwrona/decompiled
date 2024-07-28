// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.SessionTableEntity
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  internal class SessionTableEntity : TableEntity
  {
    public Guid Id;
    public Guid Owner;

    public SessionTableEntity()
    {
    }

    public SessionTableEntity(Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session session)
      : this(session.Id, session.Owner, session.Status, session.Expiration, session.ContentId, session.Parts)
    {
    }

    public SessionTableEntity(
      Guid id,
      Guid owner,
      SessionState status,
      DateTime expiration,
      DedupIdentifier contentId,
      List<Part> parts)
    {
      this.Id = id;
      this.Owner = owner;
      this.Status = status;
      this.Expiration = expiration;
      this.ContentId = contentId;
      this.Parts = JsonSerializer.Serialize<List<Part>>(parts);
      this.PartitionKey = this.Id.ToString();
      this.RowKey = this.Owner.ToString();
    }

    public SessionState Status { get; set; }

    public DateTime Expiration { get; set; }

    public DedupIdentifier ContentId { get; set; }

    public string Parts { get; set; }

    public override void ReadEntity(
      IDictionary<string, EntityProperty> properties,
      OperationContext operationContext)
    {
      base.ReadEntity(properties, operationContext);
      this.Id = new Guid(this.PartitionKey);
      this.Owner = new Guid(this.RowKey);
      this.Status = (SessionState) Enum.Parse(typeof (SessionState), properties["Status"].StringValue);
      this.Expiration = properties["Expiration"].DateTime.Value;
      if (string.IsNullOrWhiteSpace(properties["ContentId"].StringValue))
        return;
      this.ContentId = DedupIdentifier.Create(properties["ContentId"].StringValue);
    }

    public override IDictionary<string, EntityProperty> WriteEntity(
      OperationContext operationContext)
    {
      IDictionary<string, EntityProperty> dictionary = base.WriteEntity(operationContext);
      dictionary.Add("Status", new EntityProperty(this.Status.ToString()));
      dictionary.Add("ContentId", new EntityProperty(this.ContentId?.ValueString ?? string.Empty));
      return dictionary;
    }

    public class StatusColumnValue : StringColumnValue<UserColumn>
    {
      private static readonly UserColumn ColumnInstance = new UserColumn("Status");

      public StatusColumnValue(SessionState Status)
        : base(Status.ToString())
      {
      }

      public override UserColumn Column => SessionTableEntity.StatusColumnValue.ColumnInstance;
    }

    public class ExpirationColumnValue : StringColumnValue<UserColumn>
    {
      private static readonly UserColumn ColumnInstance = new UserColumn("Expiration");

      public ExpirationColumnValue(DateTime expiration)
        : base(expiration.ToString())
      {
      }

      public override UserColumn Column => SessionTableEntity.ExpirationColumnValue.ColumnInstance;
    }

    public class ContentIdColumnValue : StringColumnValue<UserColumn>
    {
      private static readonly UserColumn ColumnInstance = new UserColumn("ContentId");

      public ContentIdColumnValue(DedupIdentifier ContentId)
        : base(ContentId.ValueString)
      {
      }

      public override UserColumn Column => SessionTableEntity.ContentIdColumnValue.ColumnInstance;
    }

    public class PartsColumnValue : StringColumnValue<UserColumn>
    {
      private static readonly UserColumn ColumnInstance = new UserColumn("Parts");

      public PartsColumnValue(string Parts)
        : base(Parts)
      {
      }

      public override UserColumn Column => SessionTableEntity.PartsColumnValue.ColumnInstance;
    }
  }
}
