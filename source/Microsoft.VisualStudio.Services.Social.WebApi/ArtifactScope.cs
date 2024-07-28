// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Social.WebApi.ArtifactScope
// Assembly: Microsoft.VisualStudio.Services.Social.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0D2A928F-A131-41A8-A9E6-C3C26BFE105A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Social.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
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
      ArgumentUtility.CheckStringForNullOrEmpty(this.Type, "ArtifactScope.Type", "Social");
      if (this.Type != "Collection" && this.Type != "Project")
        throw new Exception();
      if (!(this.Type == "Project"))
        return;
      ArgumentUtility.CheckStringForNullOrEmpty(this.Id, "ArtifactScope.Id", "Social");
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
