// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MigrationEntry
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class MigrationEntry : MigrationState
  {
    [JsonProperty("P")]
    public string Protocol { get; set; }

    [JsonProperty("Host")]
    public Guid CollectionId { get; set; }

    [JsonProperty("Feed")]
    public Guid FeedId { get; set; }

    [JsonProperty("Del")]
    public bool FeedIsDeleted { get; set; }
  }
}
