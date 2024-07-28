// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.ReferenceRowTableEntity
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  internal class ReferenceRowTableEntity : DedupTableEntity
  {
    private const string TypeColumnName = "Type";
    private const string TypeValue = "ROOT";
    private const string StateColumnName = "State";
    public static RangeFilter<RowKeyColumn> AllRowsFilter = new RangeFilter<RowKeyColumn>(new RangeMinimumBoundary<RowKeyColumn>((IColumnValue<RowKeyColumn>) new ReferenceRowTableEntity.ReferenceRowKeyColumnValue("ref^"), RangeBoundaryType.Exclusive), new RangeMaximumBoundary<RowKeyColumn>((IColumnValue<RowKeyColumn>) new ReferenceRowTableEntity.ReferenceRowKeyColumnValue("ref`"), RangeBoundaryType.Exclusive));
    public ReferenceState State;

    public ReferenceRowTableEntity()
    {
    }

    public ReferenceRowTableEntity(
      DedupIdentifier dedupId,
      IdBlobReference reference,
      string etagToMatch,
      IDomainId domainId,
      long? size)
      : this(dedupId, reference.Scope, reference.Name, etagToMatch, domainId, size)
    {
    }

    public ReferenceRowTableEntity(
      DedupIdentifier dedupId,
      string scope,
      string referenceId,
      string etagToMatch,
      IDomainId domainId,
      long? size)
      : base(dedupId, ReferenceRowTableEntity.ComputeRowKey(scope, referenceId), etagToMatch)
    {
      this.Scope = scope;
      this.ReferenceId = referenceId;
      this.State = ReferenceState.Active;
      this.DomainId = domainId;
      this.Size = size;
    }

    public IDomainId DomainId { get; set; }

    public string ReferenceId { get; set; }

    public string Scope { get; set; }

    public DateTimeOffset? StateChangeTime { get; set; }

    public long? Size { get; set; }

    public byte[] GetReferenceHash() => ReferenceRowTableEntity.ComputeReferenceHash(this.ReferenceId);

    public override void ReadEntity(
      IDictionary<string, EntityProperty> properties,
      OperationContext operationContext)
    {
      base.ReadEntity(properties, operationContext);
      EntityProperty entityProperty1;
      this.State = !properties.TryGetValue("State", out entityProperty1) ? ReferenceState.Active : (ReferenceState) Enum.Parse(typeof (ReferenceState), entityProperty1.StringValue);
      EntityProperty entityProperty2;
      this.DomainId = properties.TryGetValue(DomainColumn.Instance.Name, out entityProperty2) ? DomainIdFactory.Create(entityProperty2.StringValue) : WellKnownDomainIds.DefaultDomainId;
    }

    public override IDictionary<string, EntityProperty> WriteEntity(
      OperationContext operationContext)
    {
      IDictionary<string, EntityProperty> dictionary = base.WriteEntity(operationContext);
      dictionary.Add("Type", new EntityProperty("ROOT"));
      dictionary.Add("State", new EntityProperty(this.State.ToString()));
      dictionary.Add(DomainColumn.Instance.Name, new EntityProperty(this.DomainId.Serialize()));
      return dictionary;
    }

    private static byte[] ComputeReferenceHash(string referenceId)
    {
      using (IPoolHandle<System.Security.Cryptography.SHA256CryptoServiceProvider> poolHandle = Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.BorrowSHA256())
        return poolHandle.Value.ComputeHash(StrictEncodingWithoutBOM.UTF8.GetBytes(referenceId));
    }

    private static string ComputeRowKey(string scope, string referenceId)
    {
      byte[] referenceHash = ReferenceRowTableEntity.ComputeReferenceHash(referenceId);
      return "ref_ROOT_" + scope + "_" + referenceHash.ToHexString();
    }

    public bool IsSoftDeleted => this.State == ReferenceState.SoftDeleted;

    public class ReferenceRowKeyColumnValue : RowKeyColumnValue
    {
      public ReferenceRowKeyColumnValue(string rowKey)
        : base(rowKey)
      {
      }
    }

    public class ScopeColumnValue : StringColumnValue<UserColumn>
    {
      private static readonly UserColumn ColumnInstance = new UserColumn("Scope");

      public ScopeColumnValue(string scope)
        : base(scope)
      {
      }

      public override UserColumn Column => ReferenceRowTableEntity.ScopeColumnValue.ColumnInstance;
    }

    public class StateColumnValue : StringColumnValue<UserColumn>
    {
      private static readonly UserColumn ColumnInstance = new UserColumn("State");

      public StateColumnValue(ReferenceState state)
        : base(Enum.GetName(typeof (ReferenceState), (object) state))
      {
      }

      public override UserColumn Column => ReferenceRowTableEntity.StateColumnValue.ColumnInstance;
    }
  }
}
