// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactDownloadInputBase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  [KnownType(typeof (BuildArtifactDownloadInput))]
  [KnownType(typeof (CustomArtifactDownloadInput))]
  [KnownType(typeof (GitArtifactDownloadInput))]
  [KnownType(typeof (GitHubArtifactDownloadInput))]
  [KnownType(typeof (JenkinsArtifactDownloadInput))]
  [KnownType(typeof (TfvcArtifactDownloadInput))]
  [JsonConverter(typeof (ArtifactJsonConverter))]
  public abstract class ArtifactDownloadInputBase : 
    ReleaseManagementSecuredObject,
    IEquatable<ArtifactDownloadInputBase>
  {
    private IList<string> artifactItems;

    [DataMember]
    public string Alias { get; set; }

    [DataMember]
    public string ArtifactType { get; set; }

    [DataMember]
    public string ArtifactDownloadMode { get; set; }

    [DataMember]
    public IList<string> ArtifactItems
    {
      get
      {
        if (this.artifactItems == null)
          this.artifactItems = (IList<string>) new List<string>();
        return this.artifactItems;
      }
      set => this.artifactItems = value;
    }

    public ArtifactDownloadInputBase(string artifactType) => this.ArtifactType = artifactType;

    private bool AreArtifactItemsEqual(ArtifactDownloadInputBase other)
    {
      if (other == null || this.ArtifactItems.Count != other.ArtifactItems.Count)
        return false;
      foreach (string artifactItem in (IEnumerable<string>) this.ArtifactItems)
      {
        string artifactName = artifactItem;
        if (!other.ArtifactItems.Any<string>((Func<string, bool>) (otherArtifactItem => otherArtifactItem != null && otherArtifactItem.Equals(artifactName, StringComparison.OrdinalIgnoreCase))))
          return false;
      }
      return true;
    }

    public bool Equals(ArtifactDownloadInputBase other) => other != null && this.ArtifactDownloadMode.Equals(other.ArtifactDownloadMode, StringComparison.OrdinalIgnoreCase) && this.Alias.Equals(other.Alias, StringComparison.OrdinalIgnoreCase) && this.ArtifactType.Equals(other.ArtifactType, StringComparison.OrdinalIgnoreCase) && this.AreArtifactItemsEqual(other);
  }
}
