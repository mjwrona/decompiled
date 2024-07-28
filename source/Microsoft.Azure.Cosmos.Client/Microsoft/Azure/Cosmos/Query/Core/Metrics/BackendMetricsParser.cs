// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Metrics.BackendMetricsParser
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Buffers.Text;
using System.Text;

namespace Microsoft.Azure.Cosmos.Query.Core.Metrics
{
  internal static class BackendMetricsParser
  {
    public static unsafe bool TryParse(string deliminatedString, out BackendMetrics backendMetrics)
    {
      switch (deliminatedString)
      {
        case null:
          throw new ArgumentNullException(nameof (deliminatedString));
        case "":
          backendMetrics = BackendMetrics.Empty;
          return true;
        default:
          long retrievedDocumentCount = 0;
          long retrievedDocumentSize = 0;
          long outputDocumentCount = 0;
          long outputDocumentSize = 0;
          double indexHitRatio = 0.0;
          TimeSpan timeSpan1 = new TimeSpan();
          TimeSpan timeSpan2 = new TimeSpan();
          TimeSpan timeSpan3 = new TimeSpan();
          TimeSpan timeSpan4 = new TimeSpan();
          TimeSpan timeSpan5 = new TimeSpan();
          TimeSpan timeSpan6 = new TimeSpan();
          TimeSpan timeSpan7 = new TimeSpan();
          TimeSpan timeSpan8 = new TimeSpan();
          TimeSpan timeSpan9 = new TimeSpan();
          TimeSpan timeSpan10 = new TimeSpan();
          TimeSpan timeSpan11 = new TimeSpan();
          int length1 = deliminatedString.Length * 4;
          Span<byte> span;
          if (length1 <= 4096)
          {
            int length2 = length1;
            // ISSUE: untyped stack allocation
            span = new Span<byte>((void*) __untypedstackalloc((IntPtr) (uint) length2), length2);
          }
          else
            span = (Span<byte>) new byte[length1];
          ReadOnlySpan<byte> readOnlySpan = (ReadOnlySpan<byte>) span;
          fixed (char* chars = deliminatedString)
            fixed (byte* bytes1 = &readOnlySpan.GetPinnableReference())
            {
              int bytes2 = Encoding.UTF8.GetBytes(chars, deliminatedString.Length, bytes1, readOnlySpan.Length);
              readOnlySpan = readOnlySpan.Slice(0, bytes2);
            }
          while (!readOnlySpan.IsEmpty)
          {
            BackendMetricsTokenizer.TokenType? tokenType1 = BackendMetricsTokenizer.Read(readOnlySpan).tokenType;
            int bytesConsumed;
            if (!tokenType1.HasValue)
            {
              int num = readOnlySpan.IndexOf<byte>((byte) 59);
              bytesConsumed = num != -1 ? num : readOnlySpan.Length;
            }
            else
            {
              switch (tokenType1.Value)
              {
                case BackendMetricsTokenizer.TokenType.DocumentLoadTimeInMs:
                  readOnlySpan = readOnlySpan.Slice(BackendMetricsTokenizer.TokenBuffers.DocumentLoadTimeInMs.Length);
                  if (!BackendMetricsParser.TryParseTimeSpanField(readOnlySpan, out timeSpan7, out bytesConsumed))
                  {
                    backendMetrics = (BackendMetrics) null;
                    return false;
                  }
                  break;
                case BackendMetricsTokenizer.TokenType.WriteOutputTimeInMs:
                  readOnlySpan = readOnlySpan.Slice(BackendMetricsTokenizer.TokenBuffers.WriteOutputTimeInMs.Length);
                  if (!BackendMetricsParser.TryParseTimeSpanField(readOnlySpan, out timeSpan9, out bytesConsumed))
                  {
                    backendMetrics = (BackendMetrics) null;
                    return false;
                  }
                  break;
                case BackendMetricsTokenizer.TokenType.IndexUtilizationRatio:
                  readOnlySpan = readOnlySpan.Slice(BackendMetricsTokenizer.TokenBuffers.IndexUtilizationRatio.Length);
                  if (!BackendMetricsParser.TryParseDoubleField(readOnlySpan, out indexHitRatio, out bytesConsumed))
                  {
                    backendMetrics = (BackendMetrics) null;
                    return false;
                  }
                  break;
                case BackendMetricsTokenizer.TokenType.IndexLookupTimeInMs:
                  readOnlySpan = readOnlySpan.Slice(BackendMetricsTokenizer.TokenBuffers.IndexLookupTimeInMs.Length);
                  if (!BackendMetricsParser.TryParseTimeSpanField(readOnlySpan, out timeSpan6, out bytesConsumed))
                  {
                    backendMetrics = (BackendMetrics) null;
                    return false;
                  }
                  break;
                case BackendMetricsTokenizer.TokenType.QueryLogicalPlanBuildTimeInMs:
                  readOnlySpan = readOnlySpan.Slice(BackendMetricsTokenizer.TokenBuffers.QueryLogicalPlanBuildTimeInMs.Length);
                  if (!BackendMetricsParser.TryParseTimeSpanField(readOnlySpan, out timeSpan3, out bytesConsumed))
                  {
                    backendMetrics = (BackendMetrics) null;
                    return false;
                  }
                  break;
                case BackendMetricsTokenizer.TokenType.OutputDocumentCount:
                  readOnlySpan = readOnlySpan.Slice(BackendMetricsTokenizer.TokenBuffers.OutputDocumentCount.Length);
                  if (!BackendMetricsParser.TryParseLongField(readOnlySpan, out outputDocumentCount, out bytesConsumed))
                  {
                    backendMetrics = (BackendMetrics) null;
                    return false;
                  }
                  break;
                case BackendMetricsTokenizer.TokenType.OutputDocumentSize:
                  readOnlySpan = readOnlySpan.Slice(BackendMetricsTokenizer.TokenBuffers.OutputDocumentSize.Length);
                  if (!BackendMetricsParser.TryParseLongField(readOnlySpan, out outputDocumentSize, out bytesConsumed))
                  {
                    backendMetrics = (BackendMetrics) null;
                    return false;
                  }
                  break;
                case BackendMetricsTokenizer.TokenType.QueryPhysicalPlanBuildTimeInMs:
                  readOnlySpan = readOnlySpan.Slice(BackendMetricsTokenizer.TokenBuffers.QueryPhysicalPlanBuildTimeInMs.Length);
                  if (!BackendMetricsParser.TryParseTimeSpanField(readOnlySpan, out timeSpan4, out bytesConsumed))
                  {
                    backendMetrics = (BackendMetrics) null;
                    return false;
                  }
                  break;
                case BackendMetricsTokenizer.TokenType.QueryCompileTimeInMs:
                  readOnlySpan = readOnlySpan.Slice(BackendMetricsTokenizer.TokenBuffers.QueryCompileTimeInMs.Length);
                  if (!BackendMetricsParser.TryParseTimeSpanField(readOnlySpan, out timeSpan2, out bytesConsumed))
                  {
                    backendMetrics = (BackendMetrics) null;
                    return false;
                  }
                  break;
                case BackendMetricsTokenizer.TokenType.QueryOptimizationTimeInMs:
                  readOnlySpan = readOnlySpan.Slice(BackendMetricsTokenizer.TokenBuffers.QueryOptimizationTimeInMs.Length);
                  if (!BackendMetricsParser.TryParseTimeSpanField(readOnlySpan, out timeSpan5, out bytesConsumed))
                  {
                    backendMetrics = (BackendMetrics) null;
                    return false;
                  }
                  break;
                case BackendMetricsTokenizer.TokenType.RetrievedDocumentCount:
                  readOnlySpan = readOnlySpan.Slice(BackendMetricsTokenizer.TokenBuffers.RetrievedDocumentCount.Length);
                  if (!BackendMetricsParser.TryParseLongField(readOnlySpan, out retrievedDocumentCount, out bytesConsumed))
                  {
                    backendMetrics = (BackendMetrics) null;
                    return false;
                  }
                  break;
                case BackendMetricsTokenizer.TokenType.RetrievedDocumentSize:
                  readOnlySpan = readOnlySpan.Slice(BackendMetricsTokenizer.TokenBuffers.RetrievedDocumentSize.Length);
                  if (!BackendMetricsParser.TryParseLongField(readOnlySpan, out retrievedDocumentSize, out bytesConsumed))
                  {
                    backendMetrics = (BackendMetrics) null;
                    return false;
                  }
                  break;
                case BackendMetricsTokenizer.TokenType.SystemFunctionExecuteTimeInMs:
                  readOnlySpan = readOnlySpan.Slice(BackendMetricsTokenizer.TokenBuffers.SystemFunctionExecuteTimeInMs.Length);
                  if (!BackendMetricsParser.TryParseTimeSpanField(readOnlySpan, out timeSpan10, out bytesConsumed))
                  {
                    backendMetrics = (BackendMetrics) null;
                    return false;
                  }
                  break;
                case BackendMetricsTokenizer.TokenType.TotalExecutionTimeInMs:
                  readOnlySpan = readOnlySpan.Slice(BackendMetricsTokenizer.TokenBuffers.TotalExecutionTimeInMs.Length);
                  if (!BackendMetricsParser.TryParseTimeSpanField(readOnlySpan, out timeSpan1, out bytesConsumed))
                  {
                    backendMetrics = (BackendMetrics) null;
                    return false;
                  }
                  break;
                case BackendMetricsTokenizer.TokenType.UserFunctionExecuteTimeInMs:
                  readOnlySpan = readOnlySpan.Slice(BackendMetricsTokenizer.TokenBuffers.UserFunctionExecuteTimeInMs.Length);
                  if (!BackendMetricsParser.TryParseTimeSpanField(readOnlySpan, out timeSpan11, out bytesConsumed))
                  {
                    backendMetrics = (BackendMetrics) null;
                    return false;
                  }
                  break;
                case BackendMetricsTokenizer.TokenType.VMExecutionTimeInMs:
                  readOnlySpan = readOnlySpan.Slice(BackendMetricsTokenizer.TokenBuffers.VMExecutionTimeInMs.Length);
                  if (!BackendMetricsParser.TryParseTimeSpanField(readOnlySpan, out timeSpan8, out bytesConsumed))
                  {
                    backendMetrics = (BackendMetrics) null;
                    return false;
                  }
                  break;
                default:
                  backendMetrics = (BackendMetrics) null;
                  return false;
              }
            }
            readOnlySpan = readOnlySpan.Slice(bytesConsumed);
            if (!readOnlySpan.IsEmpty)
            {
              BackendMetricsTokenizer.TokenType? tokenType2 = BackendMetricsTokenizer.Read(readOnlySpan).tokenType;
              if (tokenType2.HasValue)
              {
                BackendMetricsTokenizer.TokenType? nullable = tokenType2;
                BackendMetricsTokenizer.TokenType tokenType3 = BackendMetricsTokenizer.TokenType.SemiColonDelimiter;
                if (nullable.GetValueOrDefault() == tokenType3 & nullable.HasValue)
                {
                  readOnlySpan = readOnlySpan.Slice(1);
                  continue;
                }
              }
              backendMetrics = (BackendMetrics) null;
              return false;
            }
          }
          backendMetrics = new BackendMetrics(retrievedDocumentCount, retrievedDocumentSize, outputDocumentCount, outputDocumentSize, indexHitRatio, timeSpan1, new QueryPreparationTimes(timeSpan2, timeSpan3, timeSpan4, timeSpan5), timeSpan6, timeSpan7, timeSpan8, new RuntimeExecutionTimes(timeSpan8 - timeSpan6 - timeSpan7 - timeSpan9, timeSpan10, timeSpan11), timeSpan9);
          return true;
      }
    }

    private static bool TryParseTimeSpanField(
      ReadOnlySpan<byte> corpus,
      out TimeSpan timeSpan,
      out int bytesConsumed)
    {
      BackendMetricsTokenizer.TokenType? tokenType = BackendMetricsTokenizer.Read(corpus).tokenType;
      if (!tokenType.HasValue || tokenType.Value != BackendMetricsTokenizer.TokenType.EqualsDelimiter)
      {
        timeSpan = new TimeSpan();
        bytesConsumed = 0;
        return false;
      }
      corpus = corpus.Slice(1);
      double num;
      if (!Utf8Parser.TryParse(corpus, out num, out bytesConsumed))
      {
        timeSpan = new TimeSpan();
        return false;
      }
      timeSpan = TimeSpan.FromTicks((long) (10000.0 * num));
      ++bytesConsumed;
      return true;
    }

    private static bool TryParseLongField(
      ReadOnlySpan<byte> corpus,
      out long value,
      out int bytesConsumed)
    {
      BackendMetricsTokenizer.TokenType? tokenType = BackendMetricsTokenizer.Read(corpus).tokenType;
      if (!tokenType.HasValue || tokenType.Value != BackendMetricsTokenizer.TokenType.EqualsDelimiter)
      {
        value = 0L;
        bytesConsumed = 0;
        return false;
      }
      corpus = corpus.Slice(1);
      if (!Utf8Parser.TryParse(corpus, out value, out bytesConsumed))
      {
        value = 0L;
        return false;
      }
      ++bytesConsumed;
      return true;
    }

    private static bool TryParseDoubleField(
      ReadOnlySpan<byte> corpus,
      out double value,
      out int bytesConsumed)
    {
      BackendMetricsTokenizer.TokenType? tokenType = BackendMetricsTokenizer.Read(corpus).tokenType;
      if (!tokenType.HasValue || tokenType.Value != BackendMetricsTokenizer.TokenType.EqualsDelimiter)
      {
        value = 0.0;
        bytesConsumed = 0;
        return false;
      }
      corpus = corpus.Slice(1);
      if (!Utf8Parser.TryParse(corpus, out value, out bytesConsumed))
      {
        value = 0.0;
        return false;
      }
      ++bytesConsumed;
      return true;
    }
  }
}
