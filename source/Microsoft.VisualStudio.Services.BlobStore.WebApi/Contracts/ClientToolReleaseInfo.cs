// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.ClientToolReleaseInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts
{
  [DataContract]
  public class ClientToolReleaseInfo
  {
    private ClientTool clientName;
    private Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.RuntimeIdentifier runtime;

    public ClientToolReleaseInfo()
    {
    }

    public ClientToolReleaseInfo(
      string name,
      string runtimeIdentifier,
      string version,
      string filePath,
      Uri uri)
    {
      this.Name = name;
      this.RuntimeIdentifier = runtimeIdentifier;
      this.Version = version;
      this.FilePath = filePath;
      this.Uri = uri;
    }

    public ClientToolReleaseInfo(
      ClientTool clientName,
      Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.RuntimeIdentifier runtimeIdentifier,
      string version,
      string filePath,
      Uri uri)
    {
      this.clientName = clientName;
      this.runtime = runtimeIdentifier;
      this.Version = version;
      this.FilePath = filePath;
      this.Uri = uri;
    }

    [DataMember(Name = "name")]
    public string Name
    {
      get => this.clientName.ToString();
      set => this.clientName = (ClientTool) Enum.Parse(typeof (ClientTool), value, true);
    }

    [DataMember(Name = "rid")]
    public string RuntimeIdentifier
    {
      get => RuntimeIdentifierHelper.GetRuntimeName(this.runtime);
      set => this.runtime = RuntimeIdentifierHelper.GetRuntimeIdentifier(value);
    }

    [DataMember(Name = "version")]
    public string Version { get; set; }

    [IgnoreDataMember]
    public string FilePath { get; set; }

    [DataMember(Name = "uri")]
    public Uri Uri { get; set; }
  }
}
