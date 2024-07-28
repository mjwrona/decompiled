// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.ListRangesResponse
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Util;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.Storage.File.Protocol
{
  public static class ListRangesResponse
  {
    private static async Task<FileRange> ParseRangeAsync(XmlReader reader, CancellationToken token)
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
      return new FileRange(start, end);
    }

    internal static async Task<IEnumerable<FileRange>> ParseAsync(
      Stream stream,
      CancellationToken token)
    {
      IEnumerable<FileRange> async;
      using (XmlReader reader = XMLReaderExtensions.CreateAsAsync(stream))
      {
        token.ThrowIfCancellationRequested();
        List<FileRange> ranges = new List<FileRange>();
        ConfiguredTaskAwaitable<bool> configuredTaskAwaitable = reader.ReadToFollowingAsync("Ranges").ConfigureAwait(false);
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
              configuredTaskAwaitable = reader.IsStartElementAsync("Range").ConfigureAwait(false);
              if (await configuredTaskAwaitable)
              {
                List<FileRange> fileRangeList = ranges;
                fileRangeList.Add(await ListRangesResponse.ParseRangeAsync(reader, token).ConfigureAwait(false));
                fileRangeList = (List<FileRange>) null;
              }
              else
                break;
            }
            await reader.ReadEndElementAsync().ConfigureAwait(false);
          }
        }
        async = (IEnumerable<FileRange>) ranges;
      }
      return async;
    }
  }
}
