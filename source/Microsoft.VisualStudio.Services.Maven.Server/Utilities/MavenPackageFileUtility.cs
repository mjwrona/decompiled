// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.MavenPackageFileUtility
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  public static class MavenPackageFileUtility
  {
    public static IdBlobReference GetBlobReferenceId(
      Guid feedId,
      MavenPackageName name,
      string fileName)
    {
      return new IdBlobReference(string.Join("/", new List<string>()
      {
        "feed",
        feedId.ToString(),
        name.GroupId,
        name.ArtifactId,
        fileName
      }.Where<string>((Func<string, bool>) (x => !string.IsNullOrWhiteSpace(x)))).ToLowerInvariant(), "maven".ToLowerInvariant());
    }
  }
}
