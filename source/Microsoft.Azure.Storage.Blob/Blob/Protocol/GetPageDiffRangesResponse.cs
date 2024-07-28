// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.GetPageDiffRangesResponse
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
  public static class GetPageDiffRangesResponse
  {
    private static async Task<PageDiffRange> ParsePageDiffRangeAsync(
      XmlReader reader,
      bool isCleared,
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
      return new PageDiffRange(start, end, isCleared);
    }

    internal static async Task<IEnumerable<PageDiffRange>> ParseAsync(
      Stream stream,
      CancellationToken token)
    {
      IEnumerable<PageDiffRange> async;
      using (XmlReader reader = XMLReaderExtensions.CreateAsAsync(stream))
      {
        token.ThrowIfCancellationRequested();
        List<PageDiffRange> ranges = new List<PageDiffRange>();
        ConfiguredTaskAwaitable<bool> configuredTaskAwaitable1 = reader.ReadToFollowingAsync("PageList").ConfigureAwait(false);
        if (await configuredTaskAwaitable1)
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
              do
              {
                configuredTaskAwaitable1 = reader.IsStartElementAsync().ConfigureAwait(false);
                if (await configuredTaskAwaitable1)
                {
                  token.ThrowIfCancellationRequested();
                  configuredTaskAwaitable1 = reader.IsStartElementAsync("ClearRange").ConfigureAwait(false);
                  if (!await configuredTaskAwaitable1)
                    configuredTaskAwaitable1 = reader.IsStartElementAsync("PageRange").ConfigureAwait(false);
                  else
                    goto label_8;
                }
                else
                  goto label_16;
              }
              while (!await configuredTaskAwaitable1);
              goto label_12;
label_8:
              List<PageDiffRange> pageDiffRangeList = ranges;
              ConfiguredTaskAwaitable<PageDiffRange> configuredTaskAwaitable2 = GetPageDiffRangesResponse.ParsePageDiffRangeAsync(reader, true, token).ConfigureAwait(false);
              pageDiffRangeList.Add(await configuredTaskAwaitable2);
              pageDiffRangeList = (List<PageDiffRange>) null;
              continue;
label_12:
              pageDiffRangeList = ranges;
              configuredTaskAwaitable2 = GetPageDiffRangesResponse.ParsePageDiffRangeAsync(reader, false, token).ConfigureAwait(false);
              pageDiffRangeList.Add(await configuredTaskAwaitable2);
              pageDiffRangeList = (List<PageDiffRange>) null;
            }
label_16:
            await reader.ReadEndElementAsync().ConfigureAwait(false);
          }
        }
        async = (IEnumerable<PageDiffRange>) ranges;
      }
      return async;
    }
  }
}
