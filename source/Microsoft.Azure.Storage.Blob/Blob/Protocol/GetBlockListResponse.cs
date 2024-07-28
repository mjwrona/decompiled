// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.GetBlockListResponse
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
  public static class GetBlockListResponse
  {
    internal static async Task<IEnumerable<ListBlockItem>> ParseAsync(
      Stream stream,
      CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      List<ListBlockItem> blocks = new List<ListBlockItem>();
      IEnumerable<ListBlockItem> async;
      using (XmlReader reader = XMLReaderExtensions.CreateAsAsync(stream))
      {
        token.ThrowIfCancellationRequested();
        if (await reader.ReadToFollowingAsync("BlockList").ConfigureAwait(false))
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
              if (await reader.IsStartElementAsync().ConfigureAwait(false))
              {
                token.ThrowIfCancellationRequested();
                List<ListBlockItem> listBlockItemList;
                ConfiguredTaskAwaitable<ListBlockItem> configuredTaskAwaitable;
                if (reader.IsEmptyElement)
                {
                  await reader.SkipAsync().ConfigureAwait(false);
                }
                else
                {
                  switch (reader.Name)
                  {
                    case "CommittedBlocks":
                      await reader.ReadStartElementAsync().ConfigureAwait(false);
                      while (true)
                      {
                        if (await reader.IsStartElementAsync("Block").ConfigureAwait(false))
                        {
                          token.ThrowIfCancellationRequested();
                          listBlockItemList = blocks;
                          configuredTaskAwaitable = GetBlockListResponse.ParseBlockItemAsync(true, reader, token).ConfigureAwait(false);
                          listBlockItemList.Add(await configuredTaskAwaitable);
                          listBlockItemList = (List<ListBlockItem>) null;
                        }
                        else
                          break;
                      }
                      await reader.ReadEndElementAsync().ConfigureAwait(false);
                      continue;
                    case "UncommittedBlocks":
                      await reader.ReadStartElementAsync().ConfigureAwait(false);
                      while (true)
                      {
                        if (await reader.IsStartElementAsync("Block").ConfigureAwait(false))
                        {
                          token.ThrowIfCancellationRequested();
                          listBlockItemList = blocks;
                          configuredTaskAwaitable = GetBlockListResponse.ParseBlockItemAsync(false, reader, token).ConfigureAwait(false);
                          listBlockItemList.Add(await configuredTaskAwaitable);
                          listBlockItemList = (List<ListBlockItem>) null;
                        }
                        else
                          break;
                      }
                      await reader.ReadEndElementAsync().ConfigureAwait(false);
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
          }
        }
        async = (IEnumerable<ListBlockItem>) blocks;
      }
      blocks = (List<ListBlockItem>) null;
      return async;
    }

    private static async Task<ListBlockItem> ParseBlockItemAsync(
      bool committed,
      XmlReader reader,
      CancellationToken token)
    {
      ListBlockItem block = new ListBlockItem()
      {
        Committed = committed
      };
      await reader.ReadStartElementAsync().ConfigureAwait(false);
      while (true)
      {
        if (await reader.IsStartElementAsync().ConfigureAwait(false))
        {
          token.ThrowIfCancellationRequested();
          ListBlockItem listBlockItem;
          if (reader.IsEmptyElement)
          {
            await reader.SkipAsync().ConfigureAwait(false);
          }
          else
          {
            switch (reader.Name)
            {
              case "Size":
                listBlockItem = block;
                listBlockItem.Length = await reader.ReadElementContentAsInt64Async().ConfigureAwait(false);
                listBlockItem = (ListBlockItem) null;
                continue;
              case "Name":
                listBlockItem = block;
                listBlockItem.Name = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                listBlockItem = (ListBlockItem) null;
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
      ListBlockItem blockItemAsync = block;
      block = (ListBlockItem) null;
      return blockItemAsync;
    }
  }
}
