// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Domain.HostDomainTableEntity
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Domain
{
  internal class HostDomainTableEntity : ITableEntity
  {
    private const string DomainColumnName = "DomainId";
    private const string IsDefaultColumnName = "IsDefault";
    private const string RegionColumnName = "Region";
    private const string RedundancyTypeColumnName = "RedundancyType";
    private const string ShardsColumnName = "Shards";
    public static RangeFilter<RowKeyColumn> AllRowsFilter = new RangeFilter<RowKeyColumn>(new RangeMinimumBoundary<RowKeyColumn>((IColumnValue<RowKeyColumn>) new HostDomainTableEntity.HostDomainRowKeyColumn("ref^"), RangeBoundaryType.Exclusive), new RangeMaximumBoundary<RowKeyColumn>((IColumnValue<RowKeyColumn>) new HostDomainTableEntity.HostDomainRowKeyColumn("ref`"), RangeBoundaryType.Exclusive));

    public string PartitionKey { get; set; }

    public string RowKey { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public string ETag { get; set; }

    public string DomainId { get; set; }

    public bool IsDefaultDomain { get; set; }

    public string Region { get; set; }

    public string RedundancyType { get; set; }

    public HashSet<string> Shards { get; set; }

    public HostDomainTableEntity()
    {
    }

    public HostDomainTableEntity(Guid hostId) => this.PartitionKey = hostId.ToString("D");

    public HostDomainTableEntity(Guid hostId, string domainId)
    {
      this.PartitionKey = hostId.ToString("D");
      this.RowKey = HostDomainTableEntity.ComputeRowKey(domainId);
      this.DomainId = domainId;
    }

    public HostDomainTableEntity(
      Guid hostId,
      string domainId,
      bool isDefault,
      HashSet<string> shards)
    {
      this.PartitionKey = hostId.ToString("D");
      this.RowKey = HostDomainTableEntity.ComputeRowKey(domainId);
      this.DomainId = domainId;
      this.IsDefaultDomain = isDefault;
      this.Shards = shards;
    }

    public HostDomainTableEntity(
      Guid hostId,
      string domainId,
      bool isDefault,
      string region,
      string redundancyType,
      HashSet<string> shards)
    {
      this.PartitionKey = hostId.ToString("D");
      this.RowKey = HostDomainTableEntity.ComputeRowKey(domainId);
      this.DomainId = domainId;
      this.IsDefaultDomain = isDefault;
      this.Region = region;
      this.RedundancyType = redundancyType;
      this.Shards = shards;
    }

    public void ReadEntity(
      IDictionary<string, EntityProperty> properties,
      OperationContext operationContext)
    {
      this.DomainId = properties["DomainId"].StringValue;
      this.IsDefaultDomain = properties["IsDefault"].BooleanValue.GetValueOrDefault();
      this.Region = properties["Region"].StringValue;
      this.RedundancyType = properties["RedundancyType"].StringValue;
      this.Shards = JsonSerializer.Deserialize<HashSet<string>>(properties["Shards"].StringValue);
    }

    public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext) => (IDictionary<string, EntityProperty>) new Dictionary<string, EntityProperty>()
    {
      {
        this.DomainId,
        new EntityProperty(this.DomainId)
      },
      {
        "IsDefault",
        new EntityProperty(new bool?(this.IsDefaultDomain))
      },
      {
        "Region",
        new EntityProperty(this.Region)
      },
      {
        this.RedundancyType,
        new EntityProperty(this.RedundancyType)
      },
      {
        "Shards",
        new EntityProperty(JsonSerializer.Serialize<HashSet<string>>(this.Shards))
      }
    };

    private static byte[] ComputeDomainHash(string domainId)
    {
      using (IPoolHandle<SHA256CryptoServiceProvider> poolHandle = VsoHash.BorrowSHA256())
        return poolHandle.Value.ComputeHash(StrictEncodingWithoutBOM.UTF8.GetBytes(domainId));
    }

    private static string ComputeRowKey(string domainId) => "ref_" + HostDomainTableEntity.ComputeDomainHash(domainId).ToHexString();

    internal class HostDomainPartitionKeyColumn : PartitionKeyColumnValue
    {
      public HostDomainPartitionKeyColumn(Guid hostId)
        : base(hostId.ToString("D"))
      {
      }
    }

    internal class HostDomainRowKeyColumn : RowKeyColumnValue
    {
      public HostDomainRowKeyColumn(string domainId)
        : base(HostDomainTableEntity.ComputeRowKey(domainId))
      {
      }
    }

    public class DomainColumnValue : StringColumnValue<UserColumn>
    {
      private static readonly UserColumn columnInstance = new UserColumn("DomainId");

      public DomainColumnValue(string domain)
        : base(domain)
      {
      }

      public override UserColumn Column => HostDomainTableEntity.DomainColumnValue.columnInstance;
    }

    public class ShardsColumnValue : StringColumnValue<UserColumn>
    {
      private static readonly UserColumn columnInstance = new UserColumn("Shards");

      public ShardsColumnValue(string region)
        : base(region)
      {
      }

      public override UserColumn Column => HostDomainTableEntity.ShardsColumnValue.columnInstance;
    }

    public class RegionColumnValue : StringColumnValue<UserColumn>
    {
      private static readonly UserColumn columnInstance = new UserColumn("Region");

      public RegionColumnValue(string region)
        : base(region)
      {
      }

      public override UserColumn Column => HostDomainTableEntity.RegionColumnValue.columnInstance;
    }

    public class RedundancyTypeColumnValue : StringColumnValue<UserColumn>
    {
      private static readonly UserColumn columnInstance = new UserColumn("RedundancyType");

      public RedundancyTypeColumnValue(string redundancyType)
        : base(redundancyType)
      {
      }

      public override UserColumn Column => HostDomainTableEntity.RedundancyTypeColumnValue.columnInstance;
    }

    public class IsDefaultColumnValue : BooleanColumnValue<UserColumn>
    {
      private static readonly UserColumn columnInstance = new UserColumn("IsDefault");

      public IsDefaultColumnValue(bool isDefault)
        : base(isDefault)
      {
      }

      public override UserColumn Column => HostDomainTableEntity.IsDefaultColumnValue.columnInstance;
    }
  }
}
