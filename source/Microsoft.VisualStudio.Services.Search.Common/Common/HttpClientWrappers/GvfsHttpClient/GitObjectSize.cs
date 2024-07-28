// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.GvfsHttpClient.GitObjectSize
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.GvfsHttpClient
{
  public class GitObjectSize
  {
    public string Id { get; set; }

    public long Size { get; set; }

    [JsonConstructor]
    public GitObjectSize(string id, long size)
    {
      this.Id = id;
      this.Size = size;
    }
  }
}
