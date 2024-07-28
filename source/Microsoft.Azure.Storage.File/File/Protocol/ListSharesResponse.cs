// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.ListSharesResponse
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.Storage.File.Protocol
{
  public sealed class ListSharesResponse
  {
    public IEnumerable<FileShareEntry> Shares { get; private set; }

    public string NextMarker { get; private set; }

    private static async Task<FileShareEntry> ParseShareEntryAsync(
      XmlReader reader,
      Uri baseUri,
      CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      string name = (string) null;
      DateTimeOffset? snapshotTime = new DateTimeOffset?();
      IDictionary<string, string> metadata = (IDictionary<string, string>) null;
      FileShareProperties shareProperties = new FileShareProperties();
      await reader.ReadStartElementAsync().ConfigureAwait(false);
      while (true)
      {
        if (await reader.IsStartElementAsync().ConfigureAwait(false))
        {
          token.ThrowIfCancellationRequested();
          ConfiguredTaskAwaitable<string> configuredTaskAwaitable;
          if (reader.IsEmptyElement)
          {
            await reader.SkipAsync().ConfigureAwait(false);
          }
          else
          {
            switch (reader.Name)
            {
              case "Name":
                configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                name = await configuredTaskAwaitable;
                continue;
              case "Snapshot":
                configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                snapshotTime = new DateTimeOffset?((DateTimeOffset) (await configuredTaskAwaitable).ToUTCTime());
                continue;
              case "Properties":
                await reader.ReadStartElementAsync().ConfigureAwait(false);
                while (true)
                {
                  if (await reader.IsStartElementAsync().ConfigureAwait(false))
                  {
                    token.ThrowIfCancellationRequested();
                    FileShareProperties fileShareProperties;
                    if (reader.IsEmptyElement)
                    {
                      await reader.SkipAsync().ConfigureAwait(false);
                    }
                    else
                    {
                      switch (reader.Name)
                      {
                        case "Last-Modified":
                          fileShareProperties = shareProperties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          fileShareProperties.LastModified = new DateTimeOffset?((DateTimeOffset) (await configuredTaskAwaitable).ToUTCTime());
                          fileShareProperties = (FileShareProperties) null;
                          continue;
                        case "Etag":
                          fileShareProperties = shareProperties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          fileShareProperties.ETag = await configuredTaskAwaitable;
                          fileShareProperties = (FileShareProperties) null;
                          continue;
                        case "Quota":
                          fileShareProperties = shareProperties;
                          fileShareProperties.Quota = new int?(await reader.ReadElementContentAsInt32Async().ConfigureAwait(false));
                          fileShareProperties = (FileShareProperties) null;
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
      FileShareEntry shareEntryAsync = new FileShareEntry()
      {
        Properties = shareProperties,
        Name = name,
        Uri = NavigationHelper.AppendPathToSingleUri(baseUri, name),
        Metadata = metadata ?? (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase),
        SnapshotTime = snapshotTime
      };
      name = (string) null;
      metadata = (IDictionary<string, string>) null;
      shareProperties = (FileShareProperties) null;
      return shareEntryAsync;
    }

    internal static async Task<ListSharesResponse> ParseAsync(
      Stream stream,
      CancellationToken token)
    {
      ListSharesResponse async;
      using (XmlReader reader = XMLReaderExtensions.CreateAsAsync(stream))
      {
        token.ThrowIfCancellationRequested();
        List<FileShareEntry> shares = new List<FileShareEntry>();
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
                    case "Shares":
                      configuredTaskAwaitable2 = reader.ReadStartElementAsync().ConfigureAwait(false);
                      await configuredTaskAwaitable2;
                      while (true)
                      {
                        configuredTaskAwaitable1 = reader.IsStartElementAsync("Share").ConfigureAwait(false);
                        if (await configuredTaskAwaitable1)
                        {
                          List<FileShareEntry> fileShareEntryList = shares;
                          fileShareEntryList.Add(await ListSharesResponse.ParseShareEntryAsync(reader, baseUri, token).ConfigureAwait(false));
                          fileShareEntryList = (List<FileShareEntry>) null;
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
        async = new ListSharesResponse()
        {
          Shares = (IEnumerable<FileShareEntry>) shares,
          NextMarker = nextMarker
        };
      }
      return async;
    }
  }
}
