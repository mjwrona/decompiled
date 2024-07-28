// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactsDownloadInput
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class ArtifactsDownloadInput : 
    ReleaseManagementSecuredObject,
    IEquatable<ArtifactsDownloadInput>
  {
    [DataMember(EmitDefaultValue = false, Name = "DownloadInputs")]
    private IList<ArtifactDownloadInputBase> downloadInputs;

    public IList<ArtifactDownloadInputBase> DownloadInputs
    {
      get
      {
        if (this.downloadInputs == null)
          this.downloadInputs = (IList<ArtifactDownloadInputBase>) new List<ArtifactDownloadInputBase>();
        return this.downloadInputs;
      }
      set => this.downloadInputs = value;
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      IList<ArtifactDownloadInputBase> downloadInputs = this.DownloadInputs;
      if (downloadInputs == null)
        return;
      downloadInputs.ForEach<ArtifactDownloadInputBase>((Action<ArtifactDownloadInputBase>) (i => i?.SetSecuredObject(token, requiredPermissions)));
    }

    public bool Equals(ArtifactsDownloadInput other) => other != null && this.Equals(this.DownloadInputs, other.DownloadInputs);

    private bool Equals(
      IList<ArtifactDownloadInputBase> value,
      IList<ArtifactDownloadInputBase> otherValue)
    {
      if (value.Count != otherValue.Count)
        return false;
      foreach (ArtifactDownloadInputBase downloadInputBase in (IEnumerable<ArtifactDownloadInputBase>) value)
      {
        ArtifactDownloadInputBase artifact = downloadInputBase;
        ArtifactDownloadInputBase other = otherValue.SingleOrDefault<ArtifactDownloadInputBase>((Func<ArtifactDownloadInputBase, bool>) (i => i.Alias.Equals(artifact.Alias, StringComparison.OrdinalIgnoreCase)));
        if (other == null || !artifact.Equals(other))
          return false;
      }
      return true;
    }
  }
}
