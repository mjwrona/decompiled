// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.ListContainersResponse
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.Storage.Blob.Protocol
{
  public sealed class ListContainersResponse
  {
    public string NextMarker { get; private set; }

    public IEnumerable<BlobContainerEntry> Containers { get; private set; }

    private ListContainersResponse()
    {
    }

    private static async Task<BlobContainerEntry> ParseContainerEntryAsync(
      XmlReader reader,
      Uri baseUri,
      CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      string name = (string) null;
      IDictionary<string, string> metadata = (IDictionary<string, string>) null;
      BlobContainerProperties containerProperties1 = new BlobContainerProperties();
      containerProperties1.PublicAccess = new BlobContainerPublicAccessType?(BlobContainerPublicAccessType.Off);
      await reader.ReadStartElementAsync().ConfigureAwait(false);
      while (true)
      {
        ConfiguredTaskAwaitable<bool> configuredTaskAwaitable1 = reader.IsStartElementAsync().ConfigureAwait(false);
        if (await configuredTaskAwaitable1)
        {
          token.ThrowIfCancellationRequested();
          ConfiguredTaskAwaitable<string> configuredTaskAwaitable2;
          if (reader.IsEmptyElement)
          {
            await reader.SkipAsync().ConfigureAwait(false);
          }
          else
          {
            switch (reader.Name)
            {
              case "Name":
                configuredTaskAwaitable2 = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                name = await configuredTaskAwaitable2;
                continue;
              case "Properties":
                await reader.ReadStartElementAsync().ConfigureAwait(false);
                while (true)
                {
                  configuredTaskAwaitable1 = reader.IsStartElementAsync().ConfigureAwait(false);
                  if (await configuredTaskAwaitable1)
                  {
                    token.ThrowIfCancellationRequested();
                    BlobContainerProperties containerProperties2;
                    if (reader.IsEmptyElement)
                    {
                      await reader.SkipAsync().ConfigureAwait(false);
                    }
                    else
                    {
                      switch (reader.Name)
                      {
                        case "Etag":
                          containerProperties2 = containerProperties1;
                          configuredTaskAwaitable2 = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          containerProperties2.ETag = await configuredTaskAwaitable2;
                          containerProperties2 = (BlobContainerProperties) null;
                          continue;
                        case "HasImmutabilityPolicy":
                          containerProperties2 = containerProperties1;
                          configuredTaskAwaitable1 = reader.ReadElementContentAsBooleanAsync().ConfigureAwait(false);
                          containerProperties2.HasImmutabilityPolicy = new bool?(await configuredTaskAwaitable1);
                          containerProperties2 = (BlobContainerProperties) null;
                          continue;
                        case "HasLegalHold":
                          containerProperties2 = containerProperties1;
                          configuredTaskAwaitable1 = reader.ReadElementContentAsBooleanAsync().ConfigureAwait(false);
                          containerProperties2.HasLegalHold = new bool?(await configuredTaskAwaitable1);
                          containerProperties2 = (BlobContainerProperties) null;
                          continue;
                        case "Last-Modified":
                          containerProperties2 = containerProperties1;
                          configuredTaskAwaitable2 = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          containerProperties2.LastModified = new DateTimeOffset?((DateTimeOffset) (await configuredTaskAwaitable2).ToUTCTime());
                          containerProperties2 = (BlobContainerProperties) null;
                          continue;
                        case "LeaseDuration":
                          containerProperties2 = containerProperties1;
                          configuredTaskAwaitable2 = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          containerProperties2.LeaseDuration = BlobHttpResponseParsers.GetLeaseDuration(await configuredTaskAwaitable2);
                          containerProperties2 = (BlobContainerProperties) null;
                          continue;
                        case "LeaseState":
                          containerProperties2 = containerProperties1;
                          configuredTaskAwaitable2 = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          containerProperties2.LeaseState = BlobHttpResponseParsers.GetLeaseState(await configuredTaskAwaitable2);
                          containerProperties2 = (BlobContainerProperties) null;
                          continue;
                        case "LeaseStatus":
                          containerProperties2 = containerProperties1;
                          configuredTaskAwaitable2 = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          containerProperties2.LeaseStatus = BlobHttpResponseParsers.GetLeaseStatus(await configuredTaskAwaitable2);
                          containerProperties2 = (BlobContainerProperties) null;
                          continue;
                        case "PublicAccess":
                          containerProperties2 = containerProperties1;
                          configuredTaskAwaitable2 = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          containerProperties2.PublicAccess = new BlobContainerPublicAccessType?(ContainerHttpResponseParsers.GetContainerAcl(await configuredTaskAwaitable2));
                          containerProperties2 = (BlobContainerProperties) null;
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
      if (metadata == null)
        metadata = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      BlobContainerEntry containerEntryAsync = new BlobContainerEntry()
      {
        Properties = containerProperties1,
        Name = name,
        Uri = NavigationHelper.AppendPathToSingleUri(baseUri, name),
        Metadata = metadata
      };
      name = (string) null;
      metadata = (IDictionary<string, string>) null;
      containerProperties1 = (BlobContainerProperties) null;
      return containerEntryAsync;
    }

    internal static async Task<ListContainersResponse> ParseAsync(
      Stream stream,
      CancellationToken token)
    {
      ListContainersResponse async;
      using (XmlReader reader = XMLReaderExtensions.CreateAsAsync(stream))
      {
        token.ThrowIfCancellationRequested();
        List<BlobContainerEntry> entries = new List<BlobContainerEntry>();
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
                    case "Containers":
                      configuredTaskAwaitable2 = reader.ReadStartElementAsync().ConfigureAwait(false);
                      await configuredTaskAwaitable2;
                      while (true)
                      {
                        configuredTaskAwaitable1 = reader.IsStartElementAsync("Container").ConfigureAwait(false);
                        if (await configuredTaskAwaitable1)
                        {
                          List<BlobContainerEntry> blobContainerEntryList = entries;
                          blobContainerEntryList.Add(await ListContainersResponse.ParseContainerEntryAsync(reader, baseUri, token).ConfigureAwait(false));
                          blobContainerEntryList = (List<BlobContainerEntry>) null;
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
        async = new ListContainersResponse()
        {
          Containers = (IEnumerable<BlobContainerEntry>) entries,
          NextMarker = nextMarker
        };
      }
      return async;
    }
  }
}
