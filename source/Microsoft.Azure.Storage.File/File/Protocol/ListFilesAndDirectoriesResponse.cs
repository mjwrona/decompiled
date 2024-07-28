// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.ListFilesAndDirectoriesResponse
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.Storage.File.Protocol
{
  public sealed class ListFilesAndDirectoriesResponse
  {
    public IEnumerable<IListFileEntry> Files { get; private set; }

    public string NextMarker { get; private set; }

    private ListFilesAndDirectoriesResponse()
    {
    }

    private static async Task<IListFileEntry> ParseFileEntryAsync(
      XmlReader reader,
      Uri baseUri,
      CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      CloudFileAttributes file = new CloudFileAttributes();
      string name = (string) null;
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
              case "Properties":
                await reader.ReadStartElementAsync().ConfigureAwait(false);
                while (true)
                {
                  if (await reader.IsStartElementAsync().ConfigureAwait(false))
                  {
                    token.ThrowIfCancellationRequested();
                    if (reader.IsEmptyElement)
                      await reader.SkipAsync().ConfigureAwait(false);
                    else if (reader.Name == "Content-Length")
                    {
                      FileProperties fileProperties = file.Properties;
                      fileProperties.Length = await reader.ReadElementContentAsInt64Async().ConfigureAwait(false);
                      fileProperties = (FileProperties) null;
                    }
                    else
                      await reader.SkipAsync().ConfigureAwait(false);
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
      file.StorageUri = new StorageUri(NavigationHelper.AppendPathToSingleUri(baseUri, name));
      IListFileEntry fileEntryAsync = (IListFileEntry) new ListFileEntry(name, file);
      file = (CloudFileAttributes) null;
      name = (string) null;
      return fileEntryAsync;
    }

    private static async Task<IListFileEntry> ParseFileDirectoryEntryAsync(
      XmlReader reader,
      Uri baseUri,
      CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      FileDirectoryProperties properties = new FileDirectoryProperties();
      string name = (string) null;
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
              case "Properties":
                await reader.ReadStartElementAsync().ConfigureAwait(false);
                while (true)
                {
                  if (await reader.IsStartElementAsync().ConfigureAwait(false))
                  {
                    token.ThrowIfCancellationRequested();
                    FileDirectoryProperties directoryProperties;
                    if (reader.IsEmptyElement)
                    {
                      await reader.SkipAsync().ConfigureAwait(false);
                    }
                    else
                    {
                      switch (reader.Name)
                      {
                        case "Last-Modified":
                          directoryProperties = properties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          directoryProperties.LastModified = new DateTimeOffset?((DateTimeOffset) (await configuredTaskAwaitable).ToUTCTime());
                          directoryProperties = (FileDirectoryProperties) null;
                          continue;
                        case "Etag":
                          directoryProperties = properties;
                          IFormatProvider provider = (IFormatProvider) CultureInfo.InvariantCulture;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          directoryProperties.ETag = string.Format(provider, "\"{0}\"", (object) await configuredTaskAwaitable);
                          directoryProperties = (FileDirectoryProperties) null;
                          provider = (IFormatProvider) null;
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
      IListFileEntry directoryEntryAsync = (IListFileEntry) new ListFileDirectoryEntry(name, NavigationHelper.AppendPathToSingleUri(baseUri, name), properties);
      properties = (FileDirectoryProperties) null;
      name = (string) null;
      return directoryEntryAsync;
    }

    internal static async Task<ListFilesAndDirectoriesResponse> ParseAsync(
      Stream stream,
      CancellationToken token)
    {
      ListFilesAndDirectoriesResponse async;
      using (XmlReader reader = XMLReaderExtensions.CreateAsAsync(stream))
      {
        token.ThrowIfCancellationRequested();
        List<IListFileEntry> entries = new List<IListFileEntry>();
        string nextMarker = (string) null;
        ConfiguredTaskAwaitable<bool> configuredTaskAwaitable = reader.ReadToFollowingAsync("EnumerationResults").ConfigureAwait(false);
        if (await configuredTaskAwaitable)
        {
          if (reader.IsEmptyElement)
          {
            await reader.SkipAsync().ConfigureAwait(false);
          }
          else
          {
            Uri baseUri = new Uri(reader.GetAttribute("ServiceEndpoint"));
            baseUri = NavigationHelper.AppendPathToSingleUri(baseUri, reader.GetAttribute("ShareName"));
            baseUri = NavigationHelper.AppendPathToSingleUri(baseUri, reader.GetAttribute("DirectoryPath"));
            await reader.ReadStartElementAsync().ConfigureAwait(false);
            while (true)
            {
              configuredTaskAwaitable = reader.IsStartElementAsync().ConfigureAwait(false);
              if (await configuredTaskAwaitable)
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
                    case "Marker":
                      string str = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                      continue;
                    case "NextMarker":
                      nextMarker = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                      continue;
                    case "MaxResults":
                      int num = await reader.ReadElementContentAsInt32Async().ConfigureAwait(false);
                      continue;
                    case "Entries":
                      await reader.ReadStartElementAsync().ConfigureAwait(false);
                      while (true)
                      {
                        configuredTaskAwaitable = reader.IsStartElementAsync().ConfigureAwait(false);
                        List<IListFileEntry> listFileEntryList;
                        if (await configuredTaskAwaitable)
                        {
                          switch (reader.Name)
                          {
                            case "File":
                              listFileEntryList = entries;
                              listFileEntryList.Add(await ListFilesAndDirectoriesResponse.ParseFileEntryAsync(reader, baseUri, token).ConfigureAwait(false));
                              listFileEntryList = (List<IListFileEntry>) null;
                              continue;
                            case "Directory":
                              listFileEntryList = entries;
                              listFileEntryList.Add(await ListFilesAndDirectoriesResponse.ParseFileDirectoryEntryAsync(reader, baseUri, token).ConfigureAwait(false));
                              listFileEntryList = (List<IListFileEntry>) null;
                              continue;
                            default:
                              continue;
                          }
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
            baseUri = (Uri) null;
          }
        }
        async = new ListFilesAndDirectoriesResponse()
        {
          Files = (IEnumerable<IListFileEntry>) entries,
          NextMarker = nextMarker
        };
      }
      return async;
    }
  }
}
