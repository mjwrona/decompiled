// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.WebApi.ArtifactScope
// Assembly: Microsoft.VisualStudio.Services.Favorites.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86250CA5-6C66-4E9F-9014-5EE4DB12BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Favorites.WebApi
{
  [DataContract]
  public class ArtifactScope
  {
    public ArtifactScope(string type, string id, string name = null)
    {
      this.Type = type;
      this.Id = id;
      this.Name = name;
    }

    public ArtifactScope()
    {
    }

    public static ArtifactScope Default() => new ArtifactScope()
    {
      Type = "Collection",
      Id = string.Empty
    };

    [DataMember]
    public string Type { get; set; }

    [DataMember]
    public string Id { get; set; }

    [DataMember(IsRequired = false)]
    public string Name { get; set; }

    public void Validate()
    {
      ArgumentUtility.CheckStringForNullOrEmpty(this.Type, "ArtifactScope.Type", "Favorite");
      if (this.Type != "Collection" && this.Type != "Project" && this.Type != "Organization")
        throw new UnsupportedScopeTypeException(FavoritesWebApiResources.UnrecognizedStorageScopeTypeExceptionMessage());
      if (!(this.Type == "Project"))
        return;
      ArgumentUtility.CheckStringForNullOrEmpty(this.Id, "ArtifactScope.Id", "Favorite");
    }

    internal bool IsSame(ArtifactScope artifactScope)
    {
      bool flag = false;
      if (string.Equals(artifactScope.Type, this.Type, StringComparison.InvariantCultureIgnoreCase))
      {
        if (string.Equals(this.Type, "Collection", StringComparison.InvariantCultureIgnoreCase))
          flag = true;
        else if (this.Type == "Project" && string.Equals(this.Id, artifactScope.Id, StringComparison.InvariantCultureIgnoreCase))
          flag = true;
      }
      return flag;
    }
  }
}
