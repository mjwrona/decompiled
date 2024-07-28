// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.StorageLogStats.StorageAccountAdapter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Analytics;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.StorageLogStats
{
  public class StorageAccountAdapter : IStorageAccountAdapter
  {
    private const string containerName = "$logs";

    public IEnumerable<IListBlobItem> GetCloudBlobs(string connectionString, string prefix) => CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient().GetContainerReference("$logs").ListBlobs(prefix, true, BlobListingDetails.All);

    public string GetStorageAccountName(string connectionString) => StorageAccountUtilities.GetAccountInfo(connectionString).Name;

    public IEnumerable<string> PopulateStorageAccountList(IVssRequestContext requestContext) => StorageAccountConfigurationFacade.ReadAllStorageAccounts(requestContext.To(TeamFoundationHostType.Deployment)).Select<StrongBoxConnectionString, string>((Func<StrongBoxConnectionString, string>) (s => s.ConnectionString));

    public IEnumerable<StorageLogRecord> GetLogRecords(ICloudBlob cloudBlob, string filterString)
    {
      List<StorageLogRecord> logRecords = new List<StorageLogRecord>();
      if (cloudBlob is CloudBlockBlob cloudBlockBlob)
      {
        string str1 = cloudBlockBlob.DownloadText();
        string[] separator = new string[1]{ "\n" };
        foreach (string str2 in str1.Split(separator, StringSplitOptions.None))
        {
          if (!string.IsNullOrWhiteSpace(str2) && (string.IsNullOrWhiteSpace(filterString) || str2.Contains(filterString)))
          {
            byte[] bytes = Encoding.UTF8.GetBytes(str2 + "\n");
            using (MemoryStream memoryStream = new MemoryStream())
            {
              memoryStream.Write(bytes, 0, bytes.Length);
              memoryStream.Position = 0L;
              logRecords.AddRange(CloudAnalyticsClient.ParseLogStream((Stream) memoryStream).Select<LogRecord, StorageLogRecord>((Func<LogRecord, StorageLogRecord>) (logRecord => new StorageLogRecord(logRecord))));
            }
          }
        }
      }
      return (IEnumerable<StorageLogRecord>) logRecords;
    }

    public async Task WriteLogBlobToDiskAsync(
      ICloudBlob cloudBlob,
      string filterString,
      string defaultDownloadDirectory)
    {
      if (!(cloudBlob is CloudBlockBlob cloudBlockBlob))
        return;
      string[] strArray1 = cloudBlockBlob.DownloadText().Split(new string[1]
      {
        "\n"
      }, StringSplitOptions.None);
      if (!new DirectoryInfo(defaultDownloadDirectory).Exists)
        Directory.CreateDirectory(defaultDownloadDirectory);
      using (StreamWriter sourceStream = new StreamWriter(defaultDownloadDirectory + "\\log.log"))
      {
        string[] strArray = strArray1;
        for (int index = 0; index < strArray.Length; ++index)
        {
          string str = strArray[index];
          if (!string.IsNullOrWhiteSpace(str) && (string.IsNullOrWhiteSpace(filterString) || str.Contains(filterString)))
            await sourceStream.WriteLineAsync(str);
        }
        strArray = (string[]) null;
      }
    }
  }
}
