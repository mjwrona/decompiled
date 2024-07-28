// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.GetPageRangesResponse
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Util;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.Storage.Blob.Protocol
{
  public static class GetPageRangesResponse
  {
    private static async Task<PageRange> ParsePageRangeAsync(
      XmlReader reader,
      CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      long start = 0;
      long end = 0;
      await reader.ReadStartElementAsync().ConfigureAwait(false);
      while (true)
      {
        if (await reader.IsStartElementAsync().ConfigureAwait(false))
        {
          token.ThrowIfCancellationRequested();
          if (reader.IsEmptyElement)
          {
            await reader.SkipAsync().ConfigureAwait(false);
          }
          else
          {
            switch (reader.Name)
            {
              case "Start":
                start = await reader.ReadElementContentAsInt64Async().ConfigureAwait(false);
                continue;
              case "End":
                end = await reader.ReadElementContentAsInt64Async().ConfigureAwait(false);
                continue;
              default:
                await reader.SkipAsync().ConfigureAwait(false);
                continue;
            }
          }
        }
        else
          break;
      }
      await reader.ReadEndElementAsync().ConfigureAwait(false);
      return new PageRange(start, end);
    }

    internal static async Task<IEnumerable<PageRange>> ParseAsync(
      Stream stream,
      CancellationToken token)
    {
      IEnumerable<PageRange> async;
      using (XmlReader reader = XMLReaderExtensions.CreateAsAsync(stream))
      {
        token.ThrowIfCancellationRequested();
        List<PageRange> ranges = new List<PageRange>();
        ConfiguredTaskAwaitable<bool> configuredTaskAwaitable = reader.ReadToFollowingAsync("PageList").ConfigureAwait(false);
        if (await configuredTaskAwaitable)
        {
          if (reader.IsEmptyElement)
          {
            await reader.SkipAsync().ConfigureAwait(false);
          }
          else
          {
            await reader.ReadStartElementAsync().ConfigureAwait(false);
            while (true)
            {
              configuredTaskAwaitable = reader.IsStartElementAsync("PageRange").ConfigureAwait(false);
              if (await configuredTaskAwaitable)
              {
                List<PageRange> pageRangeList = ranges;
                pageRangeList.Add(await GetPageRangesResponse.ParsePageRangeAsync(reader, token).ConfigureAwait(false));
                pageRangeList = (List<PageRange>) null;
              }
              else
                break;
            }
            await reader.ReadEndElementAsync().ConfigureAwait(false);
          }
        }
        async = (IEnumerable<PageRange>) ranges;
      }
      return async;
    }
  }
}
