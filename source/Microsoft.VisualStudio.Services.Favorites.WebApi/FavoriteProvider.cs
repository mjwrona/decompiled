// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.WebApi.FavoriteProvider
// Assembly: Microsoft.VisualStudio.Services.Favorites.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86250CA5-6C66-4E9F-9014-5EE4DB12BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Favorites.WebApi
{
  [DataContract]
  public class FavoriteProvider : FavoritesSecuredObject
  {
    [DataMember(EmitDefaultValue = false, Name = "contributionId")]
    public string ContributionId { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "artifactType")]
    public string ArtifactType { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "artifactUri")]
    public string ArtifactUri { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "serviceIdentifier")]
    public Guid ServiceIdentifier { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "serviceUri")]
    public string ServiceUri { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "pluralName")]
    public string PluralName { get; set; }

    [DataMember(EmitDefaultValue = true, Name = "order")]
    public int Order { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "iconName")]
    public string IconName { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "iconClass")]
    public string IconClass { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "clientServiceIdentifier")]
    public string ClientServiceIdentifier { get; set; }
  }
}
