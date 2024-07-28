// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.RemovedAggregation
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public sealed class RemovedAggregation : 
    IAggregation<RemovedAggregation, RemovedAggregation.RemovedAggregationAccessor>,
    IAggregation,
    IAggregationAccessorBootstrapper
  {
    public RemovedAggregation(AggregationDefinition definition, string versionName)
    {
      if (string.IsNullOrWhiteSpace(versionName))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof (versionName));
      this.Definition = definition ?? throw new ArgumentNullException(nameof (definition));
      this.VersionName = versionName;
    }

    public IAggregationAccessor Bootstrap(IVssRequestContext requestContext) => throw new RemovedAggregation.RemovedAggregationException(this.Definition, this.VersionName);

    public AggregationDefinition Definition { get; }

    public string VersionName { get; }

    public sealed class RemovedAggregationAccessor : 
      IAggregationAccessor<RemovedAggregation>,
      IAggregationAccessor
    {
      private RemovedAggregationAccessor() => throw new RemovedAggregation.RemovedAggregationException();

      public IAggregation Aggregation => throw new RemovedAggregation.RemovedAggregationException();

      public Task ApplyCommitAsync(
        IFeedRequest feedRequest,
        IReadOnlyList<ICommitLogEntry> commitLogEntries)
      {
        throw new RemovedAggregation.RemovedAggregationException();
      }
    }

    private sealed class RemovedAggregationException : Exception
    {
      public RemovedAggregationException()
        : base("The implementation of this aggregation has been removed.")
      {
      }

      public RemovedAggregationException(AggregationDefinition definition, string versionName)
        : base(string.Format("The implementation of the aggregation with protocol {0}, name {1}, version {2}, has been removed.", (object) definition.Protocol, (object) definition.Name, (object) versionName))
      {
      }
    }
  }
}
