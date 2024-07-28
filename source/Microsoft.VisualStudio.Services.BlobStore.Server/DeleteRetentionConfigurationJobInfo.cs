// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DeleteRetentionConfigurationJobInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [Serializable]
  public class DeleteRetentionConfigurationJobInfo
  {
    public string DomainId { get; set; }

    [JsonConverter(typeof (StringEnumConverter))]
    public DeleteRetentionOperationMode JobRunMode { get; set; }

    public int DeleteRetentionRequestedInDays { get; set; }

    public int TotalShardsDiscovered { get; set; }

    public int TotalShardsServiced { get; set; }
  }
}
