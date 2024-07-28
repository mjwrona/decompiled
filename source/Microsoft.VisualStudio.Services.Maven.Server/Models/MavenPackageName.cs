// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.MavenPackageName
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models
{
  public class MavenPackageName : IPackageName
  {
    public MavenPackageName(string groupId, string artifactId)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(groupId, nameof (groupId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      this.GroupId = groupId;
      this.ArtifactId = artifactId;
    }

    public MavenPackageName(string name)
      : this(MavenPackageName.SplitName(name))
    {
    }

    private MavenPackageName(string[] names)
      : this(((IEnumerable<string>) names).First<string>(), ((IEnumerable<string>) names).Last<string>())
    {
    }

    public string GroupId { get; }

    public string NormalizedGroupId => this.GroupId.ToLowerInvariant();

    public string ArtifactId { get; }

    public string NormalizedArtifactId => this.ArtifactId.ToLowerInvariant();

    public string DisplayName => this.GroupId + ":" + this.ArtifactId;

    public string NormalizedName => this.NormalizedGroupId + ":" + this.NormalizedArtifactId;

    public IProtocol Protocol => (IProtocol) Microsoft.VisualStudio.Services.Maven.Server.Protocol.Maven;

    public override string ToString() => this.DisplayName;

    private static string[] SplitName(string name)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(name, nameof (name));
      string[] strArray = name.Split(':');
      return strArray.Length == 2 ? strArray : throw new ArgumentException(Microsoft.VisualStudio.Services.Maven.Server.Resources.Error_ArtifactFullNameNotValid((object) name));
    }
  }
}
