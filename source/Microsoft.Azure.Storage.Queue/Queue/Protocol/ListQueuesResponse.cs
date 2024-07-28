// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.Protocol.ListQueuesResponse
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.Storage.Queue.Protocol
{
  public sealed class ListQueuesResponse
  {
    public IEnumerable<QueueEntry> Queues { get; private set; }

    public string NextMarker { get; private set; }

    private static async Task<QueueEntry> ParseQueueEntryAsync(
      XmlReader reader,
      Uri baseUri,
      CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      string name = (string) null;
      IDictionary<string, string> metadata = (IDictionary<string, string>) null;
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
              case "Name":
                name = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                continue;
              case "Metadata":
                metadata = await Response.ParseMetadataAsync(reader).ConfigureAwait(false);
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
      QueueEntry queueEntryAsync = new QueueEntry(name, NavigationHelper.AppendPathToSingleUri(baseUri, name), metadata ?? (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
      name = (string) null;
      metadata = (IDictionary<string, string>) null;
      return queueEntryAsync;
    }

    internal static async Task<ListQueuesResponse> ParseAsync(
      Stream stream,
      CancellationToken token)
    {
      ListQueuesResponse async;
      using (XmlReader reader = XMLReaderExtensions.CreateAsAsync(stream))
      {
        token.ThrowIfCancellationRequested();
        List<QueueEntry> entries = new List<QueueEntry>();
        string nextMarker = (string) null;
        ConfiguredTaskAwaitable<bool> configuredTaskAwaitable1 = reader.ReadToFollowingAsync("EnumerationResults").ConfigureAwait(false);
        if (await configuredTaskAwaitable1)
        {
          if (reader.IsEmptyElement)
          {
            await reader.SkipAsync().ConfigureAwait(false);
          }
          else
          {
            Uri baseUri = new Uri(reader.GetAttribute("ServiceEndpoint"));
            ConfiguredTaskAwaitable configuredTaskAwaitable2 = reader.ReadStartElementAsync().ConfigureAwait(false);
            await configuredTaskAwaitable2;
            while (true)
            {
              configuredTaskAwaitable1 = reader.IsStartElementAsync().ConfigureAwait(false);
              if (await configuredTaskAwaitable1)
              {
                token.ThrowIfCancellationRequested();
                if (reader.IsEmptyElement)
                {
                  configuredTaskAwaitable2 = reader.SkipAsync().ConfigureAwait(false);
                  await configuredTaskAwaitable2;
                }
                else
                {
                  switch (reader.Name)
                  {
                    case "Marker":
                      string str1 = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                      continue;
                    case "NextMarker":
                      nextMarker = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                      continue;
                    case "MaxResults":
                      int num = await reader.ReadElementContentAsInt32Async().ConfigureAwait(false);
                      continue;
                    case "Prefix":
                      string str2 = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                      continue;
                    case "Queues":
                      configuredTaskAwaitable2 = reader.ReadStartElementAsync().ConfigureAwait(false);
                      await configuredTaskAwaitable2;
                      while (true)
                      {
                        configuredTaskAwaitable1 = reader.IsStartElementAsync().ConfigureAwait(false);
                        if (await configuredTaskAwaitable1)
                        {
                          List<QueueEntry> queueEntryList = entries;
                          queueEntryList.Add(await ListQueuesResponse.ParseQueueEntryAsync(reader, baseUri, token).ConfigureAwait(false));
                          queueEntryList = (List<QueueEntry>) null;
                        }
                        else
                          break;
                      }
                      configuredTaskAwaitable2 = reader.ReadEndElementAsync().ConfigureAwait(false);
                      await configuredTaskAwaitable2;
                      continue;
                    default:
                      configuredTaskAwaitable2 = reader.SkipAsync().ConfigureAwait(false);
                      await configuredTaskAwaitable2;
                      continue;
                  }
                }
              }
              else
                break;
            }
            configuredTaskAwaitable2 = reader.ReadEndElementAsync().ConfigureAwait(false);
            await configuredTaskAwaitable2;
            baseUri = (Uri) null;
          }
        }
        async = new ListQueuesResponse()
        {
          Queues = (IEnumerable<QueueEntry>) entries,
          NextMarker = nextMarker
        };
      }
      return async;
    }
  }
}
