// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.QueryPartitionProvider
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Azure.Documents.Query
{
  internal sealed class QueryPartitionProvider : IDisposable
  {
    private static readonly int InitialBufferSize = 1024;
    private static readonly uint DISP_E_BUFFERTOOSMALL = 2147614739;
    private static readonly PartitionedQueryExecutionInfoInternal DefaultInfoInternal = new PartitionedQueryExecutionInfoInternal()
    {
      QueryInfo = new QueryInfo(),
      QueryRanges = new List<Range<PartitionKeyInternal>>()
      {
        new Range<PartitionKeyInternal>(PartitionKeyInternal.InclusiveMinimum, PartitionKeyInternal.ExclusiveMaximum, true, false)
      }
    };
    private readonly object serviceProviderStateLock = new object();
    private IntPtr serviceProvider;
    private bool disposed;
    private string queryengineConfiguration;

    public QueryPartitionProvider(
      IDictionary<string, object> queryengineConfiguration)
    {
      if (queryengineConfiguration == null)
        throw new ArgumentNullException(nameof (queryengineConfiguration));
      if (queryengineConfiguration.Count == 0)
        throw new ArgumentException("queryengineConfiguration cannot be empty!");
      this.disposed = false;
      this.queryengineConfiguration = JsonConvert.SerializeObject((object) queryengineConfiguration);
      this.serviceProvider = IntPtr.Zero;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public void Update(
      IDictionary<string, object> queryengineConfiguration)
    {
      if (queryengineConfiguration == null)
        throw new ArgumentNullException(nameof (queryengineConfiguration));
      if (queryengineConfiguration.Count == 0)
        throw new ArgumentException("queryengineConfiguration cannot be empty!");
      if (this.disposed)
        throw new ObjectDisposedException(typeof (QueryPartitionProvider).Name);
      lock (this.serviceProviderStateLock)
      {
        this.queryengineConfiguration = JsonConvert.SerializeObject((object) queryengineConfiguration);
        if (this.disposed || !(this.serviceProvider != IntPtr.Zero))
          return;
        Exception exceptionForHr = Marshal.GetExceptionForHR((int) ServiceInteropWrapper.UpdateServiceProvider(this.serviceProvider, this.queryengineConfiguration));
        if (exceptionForHr != null)
          throw exceptionForHr;
      }
    }

    public PartitionedQueryExecutionInfo GetPartitionedQueryExecutionInfo(
      SqlQuerySpec querySpec,
      PartitionKeyDefinition partitionKeyDefinition,
      bool requireFormattableOrderByQuery,
      bool isContinuationExpected,
      bool allowNonValueAggregateQuery,
      bool hasLogicalPartitionKey)
    {
      return this.ConvertPartitionedQueryExecutionInfo(this.GetPartitionedQueryExecutionInfoInternal(querySpec, partitionKeyDefinition, requireFormattableOrderByQuery, isContinuationExpected, allowNonValueAggregateQuery, hasLogicalPartitionKey), partitionKeyDefinition);
    }

    internal PartitionedQueryExecutionInfo ConvertPartitionedQueryExecutionInfo(
      PartitionedQueryExecutionInfoInternal queryInfoInternal,
      PartitionKeyDefinition partitionKeyDefinition)
    {
      List<Range<string>> rangeList = new List<Range<string>>(queryInfoInternal.QueryRanges.Count);
      foreach (Range<PartitionKeyInternal> queryRange in queryInfoInternal.QueryRanges)
        rangeList.Add(new Range<string>(queryRange.Min.GetEffectivePartitionKeyString(partitionKeyDefinition, false), queryRange.Max.GetEffectivePartitionKeyString(partitionKeyDefinition, false), queryRange.IsMinInclusive, queryRange.IsMaxInclusive));
      rangeList.Sort((IComparer<Range<string>>) Range<string>.MinComparer.Instance);
      return new PartitionedQueryExecutionInfo()
      {
        QueryInfo = queryInfoInternal.QueryInfo,
        QueryRanges = rangeList
      };
    }

    internal unsafe PartitionedQueryExecutionInfoInternal GetPartitionedQueryExecutionInfoInternal(
      SqlQuerySpec querySpec,
      PartitionKeyDefinition partitionKeyDefinition,
      bool requireFormattableOrderByQuery,
      bool isContinuationExpected,
      bool allowNonValueAggregateQuery,
      bool hasLogicalPartitionKey)
    {
      if (querySpec == null || partitionKeyDefinition == null)
        return QueryPartitionProvider.DefaultInfoInternal;
      string query = JsonConvert.SerializeObject((object) querySpec);
      List<string> stringList = new List<string>((IEnumerable<string>) partitionKeyDefinition.Paths);
      List<string[]> pathParts = new List<string[]>();
      Action<string> action = (Action<string>) (path => pathParts.Add(((IEnumerable<string>) PathParser.GetPathParts(path)).ToArray<string>()));
      stringList.ForEach(action);
      string[] array1 = pathParts.SelectMany<string[], string>((Func<string[], IEnumerable<string>>) (parts => (IEnumerable<string>) parts)).ToArray<string>();
      uint[] array2 = pathParts.Select<string[], uint>((Func<string[], uint>) (parts => (uint) parts.Length)).ToArray<uint>();
      PartitionKind kind = partitionKeyDefinition.Kind;
      this.Initialize();
      byte[] bytes = new byte[QueryPartitionProvider.InitialBufferSize];
      uint serializedQueryExecutionInfoResultLength;
      uint keyRangesFromQuery;
      fixed (byte* numPtr1 = bytes)
      {
        keyRangesFromQuery = ServiceInteropWrapper.GetPartitionKeyRangesFromQuery(this.serviceProvider, query, requireFormattableOrderByQuery, isContinuationExpected, allowNonValueAggregateQuery, hasLogicalPartitionKey, array1, array2, (uint) partitionKeyDefinition.Paths.Count, kind, new IntPtr((void*) numPtr1), (uint) bytes.Length, out serializedQueryExecutionInfoResultLength);
        if ((int) keyRangesFromQuery == (int) QueryPartitionProvider.DISP_E_BUFFERTOOSMALL)
        {
          bytes = new byte[(int) serializedQueryExecutionInfoResultLength];
          fixed (byte* numPtr2 = bytes)
            keyRangesFromQuery = ServiceInteropWrapper.GetPartitionKeyRangesFromQuery(this.serviceProvider, query, requireFormattableOrderByQuery, isContinuationExpected, allowNonValueAggregateQuery, hasLogicalPartitionKey, array1, array2, (uint) partitionKeyDefinition.Paths.Count, kind, new IntPtr((void*) numPtr2), (uint) bytes.Length, out serializedQueryExecutionInfoResultLength);
        }
      }
      string str = Encoding.UTF8.GetString(bytes, 0, (int) serializedQueryExecutionInfoResultLength);
      Exception exceptionForHr = Marshal.GetExceptionForHR((int) keyRangesFromQuery);
      if (exceptionForHr != null)
      {
        DefaultTrace.TraceInformation("QueryEngineConfiguration: " + this.queryengineConfiguration);
        throw new BadRequestException("Message: " + str, exceptionForHr);
      }
      return JsonConvert.DeserializeObject<PartitionedQueryExecutionInfoInternal>(str, new JsonSerializerSettings()
      {
        DateParseHandling = DateParseHandling.None
      });
    }

    ~QueryPartitionProvider() => this.Dispose(false);

    private void Initialize()
    {
      if (this.disposed)
        throw new ObjectDisposedException(typeof (QueryPartitionProvider).Name);
      if (!(this.serviceProvider == IntPtr.Zero))
        return;
      lock (this.serviceProviderStateLock)
      {
        if (this.disposed || !(this.serviceProvider == IntPtr.Zero))
          return;
        Exception exceptionForHr = Marshal.GetExceptionForHR((int) ServiceInteropWrapper.CreateServiceProvider(this.queryengineConfiguration, out this.serviceProvider));
        if (exceptionForHr != null)
          throw exceptionForHr;
      }
    }

    private void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      int num = disposing ? 1 : 0;
      lock (this.serviceProviderStateLock)
      {
        if (this.serviceProvider != IntPtr.Zero)
        {
          Marshal.Release(this.serviceProvider);
          this.serviceProvider = IntPtr.Zero;
        }
        this.disposed = true;
      }
    }
  }
}
