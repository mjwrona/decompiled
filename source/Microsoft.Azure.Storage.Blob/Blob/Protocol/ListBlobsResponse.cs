// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.ListBlobsResponse
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core;
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

namespace Microsoft.Azure.Storage.Blob.Protocol
{
  public sealed class ListBlobsResponse
  {
    public IEnumerable<IListBlobEntry> Blobs { get; private set; }

    public string NextMarker { get; private set; }

    private ListBlobsResponse()
    {
    }

    private static async Task<IListBlobEntry> ParseBlobEntryAsync(
      XmlReader reader,
      Uri baseUri,
      CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      BlobAttributes blob = new BlobAttributes();
      string name = (string) null;
      string copyId = (string) null;
      string copyStatus = (string) null;
      string copyCompletionTime = (string) null;
      string copyProgress = (string) null;
      string copySource = (string) null;
      string copyStatusDescription = (string) null;
      string copyDestinationSnapshotTime = (string) null;
      string blobTierString = (string) null;
      bool? blobTierInferred = new bool?();
      string rehydrationStatusString = (string) null;
      DateTimeOffset? blobTierLastModifiedTime = new DateTimeOffset?();
      await reader.ReadStartElementAsync().ConfigureAwait(false);
      while (true)
      {
        if (await reader.IsStartElementAsync().ConfigureAwait(false))
        {
          token.ThrowIfCancellationRequested();
          BlobAttributes blobAttributes;
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
                blobAttributes = blob;
                configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                blobAttributes.SnapshotTime = new DateTimeOffset?((DateTimeOffset) (await configuredTaskAwaitable).ToUTCTime());
                blobAttributes = (BlobAttributes) null;
                continue;
              case "Deleted":
                blobAttributes = blob;
                configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                blobAttributes.IsDeleted = BlobHttpResponseParsers.GetDeletionStatus(await configuredTaskAwaitable);
                blobAttributes = (BlobAttributes) null;
                continue;
              case "Properties":
                await reader.ReadStartElementAsync().ConfigureAwait(false);
                while (true)
                {
                  if (await reader.IsStartElementAsync().ConfigureAwait(false))
                  {
                    token.ThrowIfCancellationRequested();
                    BlobProperties blobProperties;
                    Checksum checksum;
                    if (reader.IsEmptyElement)
                    {
                      await reader.SkipAsync().ConfigureAwait(false);
                    }
                    else
                    {
                      switch (reader.Name)
                      {
                        case "AccessTier":
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobTierString = await configuredTaskAwaitable;
                          continue;
                        case "AccessTierChangeTime":
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobTierLastModifiedTime = new DateTimeOffset?(DateTimeOffset.Parse(await configuredTaskAwaitable, (IFormatProvider) CultureInfo.InvariantCulture));
                          continue;
                        case "AccessTierInferred":
                          blobTierInferred = new bool?(await reader.ReadElementContentAsBooleanAsync().ConfigureAwait(false));
                          continue;
                        case "ArchiveStatus":
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          rehydrationStatusString = await configuredTaskAwaitable;
                          continue;
                        case "BlobType":
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          switch (await configuredTaskAwaitable)
                          {
                            case "BlockBlob":
                              blob.Properties.BlobType = BlobType.BlockBlob;
                              continue;
                            case "PageBlob":
                              blob.Properties.BlobType = BlobType.PageBlob;
                              continue;
                            case "AppendBlob":
                              blob.Properties.BlobType = BlobType.AppendBlob;
                              continue;
                            default:
                              continue;
                          }
                        case "Cache-Control":
                          blobProperties = blob.Properties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobProperties.CacheControl = await configuredTaskAwaitable;
                          blobProperties = (BlobProperties) null;
                          continue;
                        case "Content-CRC64":
                          checksum = blob.Properties.ContentChecksum;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          checksum.CRC64 = await configuredTaskAwaitable;
                          checksum = (Checksum) null;
                          continue;
                        case "Content-Disposition":
                          blobProperties = blob.Properties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobProperties.ContentDisposition = await configuredTaskAwaitable;
                          blobProperties = (BlobProperties) null;
                          continue;
                        case "Content-Encoding":
                          blobProperties = blob.Properties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobProperties.ContentEncoding = await configuredTaskAwaitable;
                          blobProperties = (BlobProperties) null;
                          continue;
                        case "Content-Language":
                          blobProperties = blob.Properties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobProperties.ContentLanguage = await configuredTaskAwaitable;
                          blobProperties = (BlobProperties) null;
                          continue;
                        case "Content-Length":
                          blobProperties = blob.Properties;
                          blobProperties.Length = await reader.ReadElementContentAsInt64Async().ConfigureAwait(false);
                          blobProperties = (BlobProperties) null;
                          continue;
                        case "Content-MD5":
                          checksum = blob.Properties.ContentChecksum;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          checksum.MD5 = await configuredTaskAwaitable;
                          checksum = (Checksum) null;
                          continue;
                        case "Content-Type":
                          blobProperties = blob.Properties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobProperties.ContentType = await configuredTaskAwaitable;
                          blobProperties = (BlobProperties) null;
                          continue;
                        case "CopyCompletionTime":
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          copyCompletionTime = await configuredTaskAwaitable;
                          continue;
                        case "CopyDestinationSnapshot":
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          copyDestinationSnapshotTime = await configuredTaskAwaitable;
                          continue;
                        case "CopyId":
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          copyId = await configuredTaskAwaitable;
                          continue;
                        case "CopyProgress":
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          copyProgress = await configuredTaskAwaitable;
                          continue;
                        case "CopySource":
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          copySource = await configuredTaskAwaitable;
                          continue;
                        case "CopyStatus":
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          copyStatus = await configuredTaskAwaitable;
                          continue;
                        case "CopyStatusDescription":
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          copyStatusDescription = await configuredTaskAwaitable;
                          continue;
                        case "Creation-Time":
                          blobProperties = blob.Properties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobProperties.Created = new DateTimeOffset?((DateTimeOffset) (await configuredTaskAwaitable).ToUTCTime());
                          blobProperties = (BlobProperties) null;
                          continue;
                        case "DeletedTime":
                          blobProperties = blob.Properties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobProperties.DeletedTime = new DateTimeOffset?((DateTimeOffset) (await configuredTaskAwaitable).ToUTCTime());
                          blobProperties = (BlobProperties) null;
                          continue;
                        case "EncryptionScope":
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          string str = await configuredTaskAwaitable;
                          blob.Properties.EncryptionScope = !string.IsNullOrEmpty(str) ? str : (string) null;
                          continue;
                        case "Etag":
                          blobProperties = blob.Properties;
                          IFormatProvider provider = (IFormatProvider) CultureInfo.InvariantCulture;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobProperties.ETag = string.Format(provider, "\"{0}\"", (object) await configuredTaskAwaitable);
                          blobProperties = (BlobProperties) null;
                          provider = (IFormatProvider) null;
                          continue;
                        case "IncrementalCopy":
                          blobProperties = blob.Properties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobProperties.IsIncrementalCopy = BlobHttpResponseParsers.GetIncrementalCopyStatus(await configuredTaskAwaitable);
                          blobProperties = (BlobProperties) null;
                          continue;
                        case "Last-Modified":
                          blobProperties = blob.Properties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobProperties.LastModified = new DateTimeOffset?((DateTimeOffset) (await configuredTaskAwaitable).ToUTCTime());
                          blobProperties = (BlobProperties) null;
                          continue;
                        case "LeaseDuration":
                          blobProperties = blob.Properties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobProperties.LeaseDuration = BlobHttpResponseParsers.GetLeaseDuration(await configuredTaskAwaitable);
                          blobProperties = (BlobProperties) null;
                          continue;
                        case "LeaseState":
                          blobProperties = blob.Properties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobProperties.LeaseState = BlobHttpResponseParsers.GetLeaseState(await configuredTaskAwaitable);
                          blobProperties = (BlobProperties) null;
                          continue;
                        case "LeaseStatus":
                          blobProperties = blob.Properties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobProperties.LeaseStatus = BlobHttpResponseParsers.GetLeaseStatus(await configuredTaskAwaitable);
                          blobProperties = (BlobProperties) null;
                          continue;
                        case "RemainingRetentionDays":
                          blobProperties = blob.Properties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobProperties.RemainingDaysBeforePermanentDelete = new int?(int.Parse(await configuredTaskAwaitable));
                          blobProperties = (BlobProperties) null;
                          continue;
                        case "ServerEncrypted":
                          blobProperties = blob.Properties;
                          configuredTaskAwaitable = reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                          blobProperties.IsServerEncrypted = BlobHttpResponseParsers.GetServerEncrypted(await configuredTaskAwaitable);
                          blobProperties = (BlobProperties) null;
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
                blobAttributes = blob;
                blobAttributes.Metadata = await Response.ParseMetadataAsync(reader).ConfigureAwait(false);
                blobAttributes = (BlobAttributes) null;
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
      Uri uri = NavigationHelper.AppendPathToSingleUri(baseUri, name);
      if (blob.SnapshotTime.HasValue)
      {
        UriQueryBuilder uriQueryBuilder = new UriQueryBuilder();
        uriQueryBuilder.Add("snapshot", Request.ConvertDateTimeToSnapshotString(blob.SnapshotTime.Value));
        uri = uriQueryBuilder.AddToUri(uri);
      }
      blob.StorageUri = new StorageUri(uri);
      if (!string.IsNullOrEmpty(copyStatus))
        blob.CopyState = BlobHttpResponseParsers.GetCopyAttributes(copyStatus, copyId, copySource, copyProgress, copyCompletionTime, copyStatusDescription, copyDestinationSnapshotTime);
      if (!string.IsNullOrEmpty(blobTierString))
      {
        StandardBlobTier? standardBlobTier;
        PremiumPageBlobTier? premiumPageBlobTier;
        BlobHttpResponseParsers.GetBlobTier(blob.Properties.BlobType, blobTierString, out standardBlobTier, out premiumPageBlobTier);
        blob.Properties.StandardBlobTier = standardBlobTier;
        blob.Properties.PremiumPageBlobTier = premiumPageBlobTier;
      }
      blob.Properties.RehydrationStatus = BlobHttpResponseParsers.GetRehydrationStatus(rehydrationStatusString);
      blob.Properties.BlobTierLastModifiedTime = blobTierLastModifiedTime;
      blob.Properties.BlobTierInferred = blobTierInferred;
      IListBlobEntry blobEntryAsync = (IListBlobEntry) new ListBlobEntry(name, blob);
      blob = (BlobAttributes) null;
      name = (string) null;
      copyId = (string) null;
      copyStatus = (string) null;
      copyCompletionTime = (string) null;
      copyProgress = (string) null;
      copySource = (string) null;
      copyStatusDescription = (string) null;
      copyDestinationSnapshotTime = (string) null;
      blobTierString = (string) null;
      rehydrationStatusString = (string) null;
      return blobEntryAsync;
    }

    private static async Task<ListBlobPrefixEntry> ParseBlobPrefixEntryAsync(
      XmlReader reader,
      CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      ListBlobPrefixEntry commonPrefix = new ListBlobPrefixEntry();
      await reader.ReadStartElementAsync().ConfigureAwait(false);
      while (true)
      {
        if (await reader.IsStartElementAsync().ConfigureAwait(false))
        {
          token.ThrowIfCancellationRequested();
          if (reader.IsEmptyElement)
            await reader.SkipAsync().ConfigureAwait(false);
          else if (reader.Name == "Name")
          {
            ListBlobPrefixEntry listBlobPrefixEntry = commonPrefix;
            listBlobPrefixEntry.Name = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
            listBlobPrefixEntry = (ListBlobPrefixEntry) null;
          }
          else
            await reader.SkipAsync().ConfigureAwait(false);
        }
        else
          break;
      }
      await reader.ReadEndElementAsync().ConfigureAwait(false);
      ListBlobPrefixEntry prefixEntryAsync = commonPrefix;
      commonPrefix = (ListBlobPrefixEntry) null;
      return prefixEntryAsync;
    }

    internal static async Task<ListBlobsResponse> ParseAsync(Stream stream, CancellationToken token)
    {
      ListBlobsResponse async;
      using (XmlReader reader = XMLReaderExtensions.CreateAsAsync(stream))
      {
        token.ThrowIfCancellationRequested();
        List<IListBlobEntry> entries = new List<IListBlobEntry>();
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
            string attribute = reader.GetAttribute("ServiceEndpoint");
            Uri baseUri = string.IsNullOrEmpty(attribute) ? new Uri(reader.GetAttribute("ContainerName")) : NavigationHelper.AppendPathToSingleUri(new Uri(attribute), reader.GetAttribute("ContainerName"));
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
                    case "Delimiter":
                      string str1 = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                      continue;
                    case "Marker":
                      string str2 = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                      continue;
                    case "NextMarker":
                      nextMarker = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                      continue;
                    case "MaxResults":
                      int num = await reader.ReadElementContentAsInt32Async().ConfigureAwait(false);
                      continue;
                    case "Prefix":
                      string str3 = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                      continue;
                    case "Blobs":
                      await reader.ReadStartElementAsync().ConfigureAwait(false);
                      while (true)
                      {
                        configuredTaskAwaitable = reader.IsStartElementAsync().ConfigureAwait(false);
                        List<IListBlobEntry> listBlobEntryList;
                        if (await configuredTaskAwaitable)
                        {
                          switch (reader.Name)
                          {
                            case "Blob":
                              listBlobEntryList = entries;
                              listBlobEntryList.Add(await ListBlobsResponse.ParseBlobEntryAsync(reader, baseUri, token).ConfigureAwait(false));
                              listBlobEntryList = (List<IListBlobEntry>) null;
                              continue;
                            case "BlobPrefix":
                              listBlobEntryList = entries;
                              listBlobEntryList.Add((IListBlobEntry) await ListBlobsResponse.ParseBlobPrefixEntryAsync(reader, token).ConfigureAwait(false));
                              listBlobEntryList = (List<IListBlobEntry>) null;
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
        async = new ListBlobsResponse()
        {
          Blobs = (IEnumerable<IListBlobEntry>) entries,
          NextMarker = nextMarker
        };
      }
      return async;
    }
  }
}
