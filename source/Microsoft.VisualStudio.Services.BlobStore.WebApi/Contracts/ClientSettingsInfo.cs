// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.ClientSettingsInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts
{
  public class ClientSettingsInfo
  {
    public Client Client { get; set; }

    public Dictionary<string, string> Properties { get; set; }

    public ClientSettingsInfo()
    {
    }

    public ClientSettingsInfo(Client client, Dictionary<string, string> properties)
    {
      this.Client = client;
      this.Properties = properties;
    }
  }
}
