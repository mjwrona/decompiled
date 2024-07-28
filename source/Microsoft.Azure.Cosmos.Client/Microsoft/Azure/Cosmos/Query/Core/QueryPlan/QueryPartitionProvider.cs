// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.QueryPlan.QueryPartitionProvider
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Azure.Cosmos.Query.Core.QueryPlan
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
    private readonly object serviceProviderStateLock;
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
      this.serviceProviderStateLock = new object();
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

    public TryCatch<PartitionedQueryExecutionInfo> TryGetPartitionedQueryExecutionInfo(
      string querySpecJsonString,
      PartitionKeyDefinition partitionKeyDefinition,
      bool requireFormattableOrderByQuery,
      bool isContinuationExpected,
      bool allowNonValueAggregateQuery,
      bool hasLogicalPartitionKey,
      bool allowDCount,
      bool useSystemPrefix)
    {
      TryCatch<PartitionedQueryExecutionInfoInternal> executionInfoInternal = this.TryGetPartitionedQueryExecutionInfoInternal(querySpecJsonString, partitionKeyDefinition, requireFormattableOrderByQuery, isContinuationExpected, allowNonValueAggregateQuery, hasLogicalPartitionKey, allowDCount, useSystemPrefix);
      return !executionInfoInternal.Succeeded ? TryCatch<PartitionedQueryExecutionInfo>.FromException(executionInfoInternal.Exception) : TryCatch<PartitionedQueryExecutionInfo>.FromResult(this.ConvertPartitionedQueryExecutionInfo(executionInfoInternal.Result, partitionKeyDefinition));
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

    internal unsafe TryCatch<PartitionedQueryExecutionInfoInternal> TryGetPartitionedQueryExecutionInfoInternal(
      string querySpecJsonString,
      PartitionKeyDefinition partitionKeyDefinition,
      bool requireFormattableOrderByQuery,
      bool isContinuationExpected,
      bool allowNonValueAggregateQuery,
      bool hasLogicalPartitionKey,
      bool allowDCount,
      bool useSystemPrefix)
    {
      if (querySpecJsonString == null || partitionKeyDefinition == null)
        return TryCatch<PartitionedQueryExecutionInfoInternal>.FromResult(QueryPartitionProvider.DefaultInfoInternal);
      List<string> stringList = new List<string>((IEnumerable<string>) partitionKeyDefinition.Paths);
      List<IReadOnlyList<string>> stringListList = new List<IReadOnlyList<string>>(stringList.Count);
      uint[] partitionKeyDefinitionPathTokenLengths = new uint[stringList.Count];
      int length1 = 0;
      for (int index = 0; index < stringList.Count; ++index)
      {
        IReadOnlyList<string> pathParts = PathParser.GetPathParts(stringList[index]);
        partitionKeyDefinitionPathTokenLengths[index] = (uint) pathParts.Count;
        stringListList.Add(pathParts);
        length1 += pathParts.Count;
      }
      string[] partitionKeyDefinitionPathTokens = new string[length1];
      int num = 0;
      foreach (IEnumerable<string> strings in stringListList)
      {
        foreach (string str in strings)
          partitionKeyDefinitionPathTokens[num++] = str;
      }
      PartitionKind kind = partitionKeyDefinition.Kind;
      Microsoft.Azure.Cosmos.GeospatialType geospatialType = Microsoft.Azure.Cosmos.GeospatialType.Geography;
      this.Initialize();
      int initialBufferSize = QueryPartitionProvider.InitialBufferSize;
      // ISSUE: untyped stack allocation
      Span<byte> span1 = new Span<byte>((void*) __untypedstackalloc((IntPtr) (uint) initialBufferSize), initialBufferSize);
      ServiceInteropWrapper.PartitionKeyRangesApiOptions partitionKeyRangesApiOptions = new ServiceInteropWrapper.PartitionKeyRangesApiOptions()
      {
        bAllowDCount = Convert.ToInt32(allowDCount),
        bAllowNonValueAggregateQuery = Convert.ToInt32(allowNonValueAggregateQuery),
        bHasLogicalPartitionKey = Convert.ToInt32(hasLogicalPartitionKey),
        bIsContinuationExpected = Convert.ToInt32(isContinuationExpected),
        bRequireFormattableOrderByQuery = Convert.ToInt32(requireFormattableOrderByQuery),
        bUseSystemPrefix = Convert.ToInt32(useSystemPrefix),
        eGeospatialType = Convert.ToInt32((object) geospatialType),
        ePartitionKind = Convert.ToInt32((object) kind)
      };
      uint serializedQueryExecutionInfoResultLength;
      uint rangesFromQuery3;
      fixed (byte* numPtr1 = &span1.GetPinnableReference())
      {
        rangesFromQuery3 = ServiceInteropWrapper.GetPartitionKeyRangesFromQuery3(this.serviceProvider, querySpecJsonString, partitionKeyRangesApiOptions, partitionKeyDefinitionPathTokens, partitionKeyDefinitionPathTokenLengths, (uint) partitionKeyDefinition.Paths.Count, new IntPtr((void*) numPtr1), (uint) span1.Length, out serializedQueryExecutionInfoResultLength);
        if ((int) rangesFromQuery3 == (int) QueryPartitionProvider.DISP_E_BUFFERTOOSMALL)
        {
          Span<byte> span2;
          if (serializedQueryExecutionInfoResultLength < 4096U)
          {
            int length2 = (int) serializedQueryExecutionInfoResultLength;
            // ISSUE: untyped stack allocation
            span2 = new Span<byte>((void*) __untypedstackalloc((IntPtr) (uint) length2), length2);
          }
          else
            span2 = (Span<byte>) new byte[(int) serializedQueryExecutionInfoResultLength];
          span1 = span2;
          fixed (byte* numPtr2 = &span1.GetPinnableReference())
            rangesFromQuery3 = ServiceInteropWrapper.GetPartitionKeyRangesFromQuery3(this.serviceProvider, querySpecJsonString, partitionKeyRangesApiOptions, partitionKeyDefinitionPathTokens, partitionKeyDefinitionPathTokenLengths, (uint) partitionKeyDefinition.Paths.Count, new IntPtr((void*) numPtr2), (uint) span1.Length, out serializedQueryExecutionInfoResultLength);
        }
      }
      string message = Encoding.UTF8.GetString((ReadOnlySpan<byte>) span1.Slice(0, (int) serializedQueryExecutionInfoResultLength));
      Exception exceptionForHr = Marshal.GetExceptionForHR((int) rangesFromQuery3);
      if (exceptionForHr != null)
        return TryCatch<PartitionedQueryExecutionInfoInternal>.FromException(!string.IsNullOrEmpty(message) ? (Exception) new ExpectedQueryPartitionProviderException(message, exceptionForHr) : (Exception) new UnexpectedQueryPartitionProviderException("Query service interop parsing hit an unexpected exception", exceptionForHr));
      return TryCatch<PartitionedQueryExecutionInfoInternal>.FromResult(JsonConvert.DeserializeObject<PartitionedQueryExecutionInfoInternal>(message, new JsonSerializerSettings()
      {
        DateParseHandling = DateParseHandling.None,
        MaxDepth = new int?(64)
      }));
    }

    internal static TryCatch<IntPtr> TryCreateServiceProvider(string queryEngineConfiguration)
    {
      try
      {
        IntPtr serviceProvider = IntPtr.Zero;
        Exception exceptionForHr = Marshal.GetExceptionForHR((int) ServiceInteropWrapper.CreateServiceProvider(queryEngineConfiguration, out serviceProvider));
        if (exceptionForHr == null)
          return TryCatch<IntPtr>.FromResult(serviceProvider);
        DefaultTrace.TraceWarning("QueryPartitionProvider.TryCreateServiceProvider failed with exception {0}", (object) exceptionForHr);
        return TryCatch<IntPtr>.FromException(exceptionForHr);
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceWarning("QueryPartitionProvider.TryCreateServiceProvider failed with exception {0}", (object) ex);
        return TryCatch<IntPtr>.FromException(ex);
      }
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
        TryCatch<IntPtr> serviceProvider = QueryPartitionProvider.TryCreateServiceProvider(this.queryengineConfiguration);
        this.serviceProvider = !serviceProvider.Failed ? serviceProvider.Result : throw ExceptionWithStackTraceException.UnWrapMonadExcepion(serviceProvider.Exception, (ITrace) NoOpTrace.Singleton);
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
