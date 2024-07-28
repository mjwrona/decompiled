// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.BlobActionMessage
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  [DataContract]
  public class BlobActionMessage
  {
    [DataMember(IsRequired = true)]
    public BlobAction Action { get; set; }

    [DataMember(IsRequired = true)]
    public string ContainerName { get; set; }

    [DataMember(IsRequired = true)]
    public string BlobName { get; set; }

    public string ToJson() => this.Serialize<BlobActionMessage>(true);

    public static BlobActionMessage FromJson(string json) => JsonUtilities.Deserialize<BlobActionMessage>(json, true);
  }
}
