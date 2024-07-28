// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.WebApi.Favorite
// Assembly: Microsoft.VisualStudio.Services.Favorites.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86250CA5-6C66-4E9F-9014-5EE4DB12BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Favorites.WebApi
{
  [DataContract]
  public class Favorite
  {
    [DataMember(EmitDefaultValue = false)]
    public ArtifactScope ArtifactScope { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef Owner { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ArtifactName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ArtifactType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ArtifactId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ArtifactProperties ArtifactProperties { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? CreationDate { get; set; }

    [DataMember(Name = "_links")]
    public ReferenceLinks Links { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool ArtifactIsDeleted { get; set; }

    public void Validate()
    {
      ArgumentUtility.CheckForNull<ArtifactScope>(this.ArtifactScope, "ArtifactScope");
      this.ArtifactScope.Validate();
      ArgumentUtility.CheckStringForNullOrEmpty(this.ArtifactType, "ArtifactType", nameof (Favorite));
      ArgumentUtility.CheckStringLength(this.ArtifactType, "ArtifactType", 128, expectedServiceArea: nameof (Favorite));
      if (!string.IsNullOrEmpty(this.ArtifactId))
        ArgumentUtility.CheckStringLength(this.ArtifactId, "ArtifactId", 256, expectedServiceArea: nameof (Favorite));
      if (string.IsNullOrEmpty(this.ArtifactName))
        return;
      ArgumentUtility.CheckStringLength(this.ArtifactName, "ArtifactName", 256, expectedServiceArea: nameof (Favorite));
    }

    public static void VerifyArtifactPropertyString(string artifactProperties)
    {
      if (string.IsNullOrEmpty(artifactProperties))
        return;
      ArgumentUtility.CheckStringLength(artifactProperties, "ArtifactProperties", 8000, expectedServiceArea: nameof (Favorite));
    }
  }
}
