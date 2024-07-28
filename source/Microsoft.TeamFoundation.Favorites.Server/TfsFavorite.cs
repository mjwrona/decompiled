// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Favorites.TfsFavorite
// Assembly: Microsoft.TeamFoundation.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4742830F-DF0E-4509-8C2D-2540DAED73F4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Favorites.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.TeamFoundation.Favorites
{
  [DataContract]
  public class TfsFavorite
  {
    [DataMember(Name = "projectId", EmitDefaultValue = false)]
    public Guid ProjectId { get; set; }

    [DataMember(Name = "scope", EmitDefaultValue = false)]
    public string Scope { get; set; }

    [DataMember(Name = "id", EmitDefaultValue = false)]
    public Guid? Id { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Name = "type", EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(Name = "data", EmitDefaultValue = false)]
    public string Data { get; set; }

    public bool IsFolder => string.IsNullOrEmpty(this.Type);

    public void Validate()
    {
      ArgumentUtility.CheckForEmptyGuid(this.ProjectId, "ProjectId", "Favorite");
      ArgumentUtility.CheckStringForNullOrEmpty(this.Type, "Type", "Favorite");
      if (this.Metadata == null)
        return;
      ArgumentUtility.CheckStringLength(this.Metadata, "Metadata", 8000, expectedServiceArea: "Favorite");
    }

    [DataMember(Name = "artifactIsDeleted", EmitDefaultValue = false)]
    public bool ArtifactIsDeleted { get; set; }

    [DataMember(Name = "parentId", EmitDefaultValue = false)]
    public Guid ParentId { get; set; }

    [DataMember(Name = "metadata", EmitDefaultValue = false)]
    public string Metadata { get; set; }

    public static TfsFavorite Deserialize(string value)
    {
      using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
        return new DataContractJsonSerializer(typeof (TfsFavorite)).ReadObject((Stream) memoryStream) as TfsFavorite;
    }
  }
}
