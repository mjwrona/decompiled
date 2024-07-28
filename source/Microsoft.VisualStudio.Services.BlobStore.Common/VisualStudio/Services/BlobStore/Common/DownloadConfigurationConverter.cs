// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.DownloadConfigurationConverter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class DownloadConfigurationConverter : 
    JsonConverter<ParallelHttpDownload.DownloadConfiguration>
  {
    public override bool CanWrite => false;

    public override void WriteJson(
      JsonWriter writer,
      ParallelHttpDownload.DownloadConfiguration value,
      JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override ParallelHttpDownload.DownloadConfiguration ReadJson(
      JsonReader reader,
      Type objectType,
      ParallelHttpDownload.DownloadConfiguration existingValue,
      bool hasExistingValue,
      JsonSerializer serializer)
    {
      JObject jobject = JObject.Load(reader);
      ParallelHttpDownload.DownloadConfiguration downloadConfiguration = ParallelHttpDownload.DownloadConfiguration.ReadFromEnvironment();
      JToken jtoken1 = jobject["ReadBufferTimeout"];
      string s1 = jtoken1 != null ? jtoken1.Value<string>() : (string) null;
      JToken jtoken2 = jobject["SegmentDownloadTimeout"];
      string s2 = jtoken2 != null ? jtoken2.Value<string>() : (string) null;
      TimeSpan segmentDownloadTimeout = s2 != null ? TimeSpan.Parse(s2) : downloadConfiguration.SegmentDownloadTimeout;
      JToken jtoken3 = jobject["SegmentSizeInBytes"];
      long segmentSizeInBytes = (jtoken3 != null ? (long) new int?(jtoken3.Value<int>()) : (long) new int?()) ?? downloadConfiguration.SegmentSizeInBytes;
      JToken jtoken4 = jobject["MaxParallelSegmentDownloadsPerFile"];
      int maxParallelSegmentDownloadsPerFile = jtoken4 != null ? jtoken4.Value<int>() : downloadConfiguration.MaxParallelSegmentDownloadsPerFile;
      JToken jtoken5 = jobject["MaxSegmentDownloadRetries"];
      int maxSegmentDownloadRetries = jtoken5 != null ? jtoken5.Value<int>() : downloadConfiguration.MaxSegmentDownloadRetries;
      TimeSpan readBufferTimeout = s1 != null ? TimeSpan.Parse(s1) : downloadConfiguration.ReadBufferTimeout;
      JToken jtoken6 = jobject["ReadBufferSize"];
      int readBufferSize = jtoken6 != null ? jtoken6.Value<int>() : downloadConfiguration.ReadBufferSize;
      JToken jtoken7 = jobject["UseSparseFiles"];
      bool? useSparseFiles = new bool?(((jtoken7 != null ? (int) jtoken7.Value<bool?>() : (int) new bool?()) ?? (downloadConfiguration.UseSparseFiles ? 1 : 0)) != 0);
      return new ParallelHttpDownload.DownloadConfiguration(segmentDownloadTimeout, segmentSizeInBytes, maxParallelSegmentDownloadsPerFile, maxSegmentDownloadRetries, readBufferTimeout, readBufferSize, useSparseFiles);
    }
  }
}
