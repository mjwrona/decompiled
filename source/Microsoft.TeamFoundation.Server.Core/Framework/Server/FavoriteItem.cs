// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FavoriteItem
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DataContract]
  public class FavoriteItem
  {
    [DataMember(Name = "id", EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(Name = "parentId", EmitDefaultValue = false)]
    public Guid ParentId { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Name = "type", EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(Name = "data", EmitDefaultValue = false)]
    public string Data { get; set; }

    [DataMember(Name = "metadata", EmitDefaultValue = false)]
    public string Metadata { get; set; }

    [DataMember(Name = "artifactIsDeleted", EmitDefaultValue = false)]
    public bool ArtifactIsDeleted { get; set; }

    public bool IsFolder => string.IsNullOrEmpty(this.Type);

    internal string Serialize()
    {
      MemoryStream memoryStream = new MemoryStream();
      new DataContractJsonSerializer(typeof (FavoriteItem)).WriteObject((Stream) memoryStream, (object) this);
      return Encoding.UTF8.GetString(memoryStream.ToArray()).ToString();
    }

    internal static FavoriteItem Deserialize(string value)
    {
      using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
        return new DataContractJsonSerializer(typeof (FavoriteItem)).ReadObject((Stream) memoryStream) as FavoriteItem;
    }
  }
}
