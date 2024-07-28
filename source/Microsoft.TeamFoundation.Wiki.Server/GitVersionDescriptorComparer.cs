// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.GitVersionDescriptorComparer
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class GitVersionDescriptorComparer : IEqualityComparer<GitVersionDescriptor>
  {
    public bool Equals(GitVersionDescriptor version1, GitVersionDescriptor version2)
    {
      if (version1 == version2)
        return true;
      return version1 != null && version2 != null && version1.Version == version2.Version && version1.VersionType == version2.VersionType && version1.VersionOptions == version2.VersionOptions;
    }

    public int GetHashCode(GitVersionDescriptor versionDesc) => versionDesc == null ? 0 : (versionDesc.Version == null ? 0 : versionDesc.Version.GetHashCode()) ^ versionDesc.VersionType.GetHashCode();
  }
}
