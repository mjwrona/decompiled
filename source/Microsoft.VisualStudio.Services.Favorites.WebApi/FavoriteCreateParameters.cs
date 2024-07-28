// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.WebApi.FavoriteCreateParameters
// Assembly: Microsoft.VisualStudio.Services.Favorites.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86250CA5-6C66-4E9F-9014-5EE4DB12BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Favorites.WebApi
{
  [DataContract]
  public class FavoriteCreateParameters
  {
    [DataMember]
    public ArtifactScope ArtifactScope { get; set; }

    [DataMember]
    public string ArtifactName { get; set; }

    [DataMember]
    public string ArtifactType { get; set; }

    [DataMember]
    public string ArtifactId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ArtifactProperties ArtifactProperties { get; set; }

    internal Favorite ToFavorite() => new Favorite()
    {
      ArtifactName = this.ArtifactName,
      ArtifactScope = this.ArtifactScope,
      ArtifactType = this.ArtifactType,
      ArtifactId = this.ArtifactId,
      ArtifactProperties = this.ArtifactProperties
    };
  }
}
