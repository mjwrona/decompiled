// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.BackendMetricsTokenizer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Text;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal static class BackendMetricsTokenizer
  {
    public static (BackendMetricsTokenizer.TokenType? tokenType, ReadOnlyMemory<byte> buffer) Read(
      ReadOnlySpan<byte> corpus)
    {
      if (corpus[0] == (byte) 61)
        return (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.EqualsDelimiter), new ReadOnlyMemory<byte>());
      if (corpus[0] == (byte) 59)
        return (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.SemiColonDelimiter), new ReadOnlyMemory<byte>());
      if (char.IsDigit((char) corpus[0]))
        return (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.NumberToken), new ReadOnlyMemory<byte>());
      int length = corpus.IndexOf<byte>((byte) 61);
      return length == -1 ? (new BackendMetricsTokenizer.TokenType?(), new ReadOnlyMemory<byte>()) : BackendMetricsTokenizer.GetTokenType(corpus.Slice(0, length));
    }

    private static (BackendMetricsTokenizer.TokenType? tokenType, ReadOnlyMemory<byte> tokenBuffer) GetTokenType(
      ReadOnlySpan<byte> buffer)
    {
      switch (buffer.Length)
      {
        case 18:
          return BackendMetricsTokenizer.GetTokenTypeLength18(buffer);
        case 19:
          return BackendMetricsTokenizer.GetTokenTypeLength19(buffer);
        case 20:
          return BackendMetricsTokenizer.GetTokenTypeLength20(buffer);
        case 21:
          return BackendMetricsTokenizer.GetTokenTypeLength21(buffer);
        case 22:
          return BackendMetricsTokenizer.GetTokenTypeLength22(buffer);
        case 25:
          return BackendMetricsTokenizer.GetTokenTypeLength25(buffer);
        case 27:
          return BackendMetricsTokenizer.GetTokenTypeLength27(buffer);
        case 29:
          return BackendMetricsTokenizer.GetTokenTypeLength29(buffer);
        case 30:
          return BackendMetricsTokenizer.GetTokenTypeLength30(buffer);
        default:
          return (new BackendMetricsTokenizer.TokenType?(), new ReadOnlyMemory<byte>());
      }
    }

    private static (BackendMetricsTokenizer.TokenType? tokenType, ReadOnlyMemory<byte> tokenBuffer) GetTokenTypeLength20(
      ReadOnlySpan<byte> buffer)
    {
      if (buffer.SequenceEqual<byte>(BackendMetricsTokenizer.TokenBuffers.DocumentLoadTimeInMs.Span))
        return (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.DocumentLoadTimeInMs), BackendMetricsTokenizer.TokenBuffers.DocumentLoadTimeInMs);
      return buffer.SequenceEqual<byte>(BackendMetricsTokenizer.TokenBuffers.QueryCompileTimeInMs.Span) ? (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.QueryCompileTimeInMs), BackendMetricsTokenizer.TokenBuffers.QueryCompileTimeInMs) : (new BackendMetricsTokenizer.TokenType?(), new ReadOnlyMemory<byte>());
    }

    private static (BackendMetricsTokenizer.TokenType? tokenType, ReadOnlyMemory<byte> tokenBuffer) GetTokenTypeLength19(
      ReadOnlySpan<byte> buffer)
    {
      if (buffer.SequenceEqual<byte>(BackendMetricsTokenizer.TokenBuffers.WriteOutputTimeInMs.Span))
        return (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.WriteOutputTimeInMs), BackendMetricsTokenizer.TokenBuffers.WriteOutputTimeInMs);
      if (buffer.SequenceEqual<byte>(BackendMetricsTokenizer.TokenBuffers.IndexLookupTimeInMs.Span))
        return (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.IndexLookupTimeInMs), BackendMetricsTokenizer.TokenBuffers.IndexLookupTimeInMs);
      if (buffer.SequenceEqual<byte>(BackendMetricsTokenizer.TokenBuffers.OutputDocumentCount.Span))
        return (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.OutputDocumentCount), BackendMetricsTokenizer.TokenBuffers.OutputDocumentCount);
      return buffer.SequenceEqual<byte>(BackendMetricsTokenizer.TokenBuffers.VMExecutionTimeInMs.Span) ? (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.VMExecutionTimeInMs), BackendMetricsTokenizer.TokenBuffers.VMExecutionTimeInMs) : (new BackendMetricsTokenizer.TokenType?(), new ReadOnlyMemory<byte>());
    }

    private static (BackendMetricsTokenizer.TokenType? tokenType, ReadOnlyMemory<byte> tokenBuffer) GetTokenTypeLength21(
      ReadOnlySpan<byte> buffer)
    {
      if (buffer.SequenceEqual<byte>(BackendMetricsTokenizer.TokenBuffers.IndexUtilizationRatio.Span))
        return (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.IndexUtilizationRatio), BackendMetricsTokenizer.TokenBuffers.IndexUtilizationRatio);
      return buffer.SequenceEqual<byte>(BackendMetricsTokenizer.TokenBuffers.RetrievedDocumentSize.Span) ? (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.RetrievedDocumentSize), BackendMetricsTokenizer.TokenBuffers.RetrievedDocumentSize) : (new BackendMetricsTokenizer.TokenType?(), new ReadOnlyMemory<byte>());
    }

    private static (BackendMetricsTokenizer.TokenType? tokenType, ReadOnlyMemory<byte> tokenBuffer) GetTokenTypeLength29(
      ReadOnlySpan<byte> buffer)
    {
      if (buffer.SequenceEqual<byte>(BackendMetricsTokenizer.TokenBuffers.QueryLogicalPlanBuildTimeInMs.Span))
        return (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.QueryLogicalPlanBuildTimeInMs), BackendMetricsTokenizer.TokenBuffers.QueryLogicalPlanBuildTimeInMs);
      return buffer.SequenceEqual<byte>(BackendMetricsTokenizer.TokenBuffers.SystemFunctionExecuteTimeInMs.Span) ? (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.SystemFunctionExecuteTimeInMs), BackendMetricsTokenizer.TokenBuffers.SystemFunctionExecuteTimeInMs) : (new BackendMetricsTokenizer.TokenType?(), new ReadOnlyMemory<byte>());
    }

    private static (BackendMetricsTokenizer.TokenType? tokenType, ReadOnlyMemory<byte> tokenBuffer) GetTokenTypeLength18(
      ReadOnlySpan<byte> buffer)
    {
      return buffer.SequenceEqual<byte>(BackendMetricsTokenizer.TokenBuffers.OutputDocumentSize.Span) ? (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.OutputDocumentSize), BackendMetricsTokenizer.TokenBuffers.OutputDocumentSize) : (new BackendMetricsTokenizer.TokenType?(), new ReadOnlyMemory<byte>());
    }

    private static (BackendMetricsTokenizer.TokenType? tokenType, ReadOnlyMemory<byte> tokenBuffer) GetTokenTypeLength30(
      ReadOnlySpan<byte> buffer)
    {
      return buffer.SequenceEqual<byte>(BackendMetricsTokenizer.TokenBuffers.QueryPhysicalPlanBuildTimeInMs.Span) ? (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.QueryPhysicalPlanBuildTimeInMs), BackendMetricsTokenizer.TokenBuffers.QueryPhysicalPlanBuildTimeInMs) : (new BackendMetricsTokenizer.TokenType?(), new ReadOnlyMemory<byte>());
    }

    private static (BackendMetricsTokenizer.TokenType? tokenType, ReadOnlyMemory<byte> tokenBuffer) GetTokenTypeLength25(
      ReadOnlySpan<byte> buffer)
    {
      return buffer.SequenceEqual<byte>(BackendMetricsTokenizer.TokenBuffers.QueryOptimizationTimeInMs.Span) ? (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.QueryOptimizationTimeInMs), BackendMetricsTokenizer.TokenBuffers.QueryOptimizationTimeInMs) : (new BackendMetricsTokenizer.TokenType?(), new ReadOnlyMemory<byte>());
    }

    private static (BackendMetricsTokenizer.TokenType? tokenType, ReadOnlyMemory<byte> tokenBuffer) GetTokenTypeLength22(
      ReadOnlySpan<byte> buffer)
    {
      if (buffer.SequenceEqual<byte>(BackendMetricsTokenizer.TokenBuffers.RetrievedDocumentCount.Span))
        return (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.RetrievedDocumentCount), BackendMetricsTokenizer.TokenBuffers.RetrievedDocumentCount);
      return buffer.SequenceEqual<byte>(BackendMetricsTokenizer.TokenBuffers.TotalExecutionTimeInMs.Span) ? (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.TotalExecutionTimeInMs), BackendMetricsTokenizer.TokenBuffers.TotalExecutionTimeInMs) : (new BackendMetricsTokenizer.TokenType?(), new ReadOnlyMemory<byte>());
    }

    private static (BackendMetricsTokenizer.TokenType? tokenType, ReadOnlyMemory<byte> tokenBuffer) GetTokenTypeLength27(
      ReadOnlySpan<byte> buffer)
    {
      return buffer.SequenceEqual<byte>(BackendMetricsTokenizer.TokenBuffers.UserFunctionExecuteTimeInMs.Span) ? (new BackendMetricsTokenizer.TokenType?(BackendMetricsTokenizer.TokenType.UserFunctionExecuteTimeInMs), BackendMetricsTokenizer.TokenBuffers.UserFunctionExecuteTimeInMs) : (new BackendMetricsTokenizer.TokenType?(), new ReadOnlyMemory<byte>());
    }

    public enum TokenType
    {
      DocumentLoadTimeInMs,
      WriteOutputTimeInMs,
      IndexUtilizationRatio,
      IndexLookupTimeInMs,
      QueryLogicalPlanBuildTimeInMs,
      OutputDocumentCount,
      OutputDocumentSize,
      QueryPhysicalPlanBuildTimeInMs,
      QueryCompileTimeInMs,
      QueryOptimizationTimeInMs,
      RetrievedDocumentCount,
      RetrievedDocumentSize,
      SystemFunctionExecuteTimeInMs,
      TotalExecutionTimeInMs,
      UserFunctionExecuteTimeInMs,
      VMExecutionTimeInMs,
      EqualsDelimiter,
      SemiColonDelimiter,
      NumberToken,
    }

    public static class TokenBuffers
    {
      public static readonly ReadOnlyMemory<byte> DocumentLoadTimeInMs = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("documentLoadTimeInMs");
      public static readonly ReadOnlyMemory<byte> WriteOutputTimeInMs = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("writeOutputTimeInMs");
      public static readonly ReadOnlyMemory<byte> IndexUtilizationRatio = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("indexUtilizationRatio");
      public static readonly ReadOnlyMemory<byte> IndexLookupTimeInMs = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("indexLookupTimeInMs");
      public static readonly ReadOnlyMemory<byte> QueryLogicalPlanBuildTimeInMs = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("queryLogicalPlanBuildTimeInMs");
      public static readonly ReadOnlyMemory<byte> OutputDocumentCount = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("outputDocumentCount");
      public static readonly ReadOnlyMemory<byte> OutputDocumentSize = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("outputDocumentSize");
      public static readonly ReadOnlyMemory<byte> QueryPhysicalPlanBuildTimeInMs = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("queryPhysicalPlanBuildTimeInMs");
      public static readonly ReadOnlyMemory<byte> QueryCompileTimeInMs = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("queryCompileTimeInMs");
      public static readonly ReadOnlyMemory<byte> QueryOptimizationTimeInMs = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("queryOptimizationTimeInMs");
      public static readonly ReadOnlyMemory<byte> RetrievedDocumentCount = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("retrievedDocumentCount");
      public static readonly ReadOnlyMemory<byte> RetrievedDocumentSize = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("retrievedDocumentSize");
      public static readonly ReadOnlyMemory<byte> SystemFunctionExecuteTimeInMs = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("systemFunctionExecuteTimeInMs");
      public static readonly ReadOnlyMemory<byte> TotalExecutionTimeInMs = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("totalExecutionTimeInMs");
      public static readonly ReadOnlyMemory<byte> UserFunctionExecuteTimeInMs = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes("userFunctionExecuteTimeInMs");
      public static readonly ReadOnlyMemory<byte> VMExecutionTimeInMs = (ReadOnlyMemory<byte>) Encoding.UTF8.GetBytes(nameof (VMExecutionTimeInMs));
    }
  }
}
