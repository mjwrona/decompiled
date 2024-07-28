// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupMetadataPageRetrievalOption
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public sealed class DedupMetadataPageRetrievalOption
  {
    private string m_key;

    public string Prefix { get; private set; }

    public string StartingKey
    {
      get => this.m_key != null ? this.m_key : this.Prefix;
      internal set
      {
        if (value == null)
          return;
        if (this.Prefix == null)
          throw new ArgumentException("Cannot set starting partition key if the prefix is null.");
        this.m_key = value.StartsWith(this.Prefix) ? value : throw new ArgumentException("Cannot set a starting partition key (" + value + ") which isn't started with the given Prefix (" + this.Prefix + ").");
      }
    }

    public DateTimeOffset? Start { get; }

    public DateTimeOffset? End { get; }

    public ResultArrangement Arrangement { get; }

    public int PageSize { get; internal set; }

    public StateFilter StateFilter { get; set; }

    public ArtifactScopeType? ScopeFilter { get; set; }

    public IDomainId Domain { get; private set; }

    public int BoundedCapacity { get; set; }

    public DedupMetadataPageRetrievalOption(
      ResultArrangement arrangement,
      int pageSize,
      StateFilter stateFilter)
      : this(string.Empty, new DateTimeOffset?(), new DateTimeOffset?(), arrangement, pageSize, stateFilter, (IDomainId) null)
    {
    }

    public DedupMetadataPageRetrievalOption(
      string prefix,
      DateTimeOffset? start,
      DateTimeOffset? end,
      ResultArrangement arrangement,
      int pageSize,
      StateFilter stateFilter)
      : this(prefix, start, end, arrangement, pageSize, stateFilter, (IDomainId) null)
    {
    }

    public DedupMetadataPageRetrievalOption(
      string prefix,
      DateTimeOffset? start,
      DateTimeOffset? end,
      ResultArrangement arrangement,
      int pageSize,
      StateFilter stateFilter,
      IDomainId domainId)
    {
      this.Prefix = prefix;
      this.Start = start;
      this.End = end;
      this.Arrangement = arrangement;
      this.PageSize = pageSize;
      this.StateFilter = stateFilter;
      this.Domain = domainId;
    }
  }
}
