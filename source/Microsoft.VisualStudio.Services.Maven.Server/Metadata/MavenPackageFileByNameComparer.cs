// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Metadata.MavenPackageFileByNameComparer
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Maven.Server.Metadata
{
  public class MavenPackageFileByNameComparer : IEqualityComparer<MavenPackageFileNew>
  {
    public static IEqualityComparer<MavenPackageFileNew> Instance { get; } = (IEqualityComparer<MavenPackageFileNew>) new MavenPackageFileByNameComparer();

    public bool Equals(MavenPackageFileNew x, MavenPackageFileNew y) => x == null || y == null ? x == y : MavenFileNameUtility.FileNameStringComparer.Equals(x.Path, y.Path);

    public int GetHashCode(MavenPackageFileNew obj) => obj == null ? 0 : MavenFileNameUtility.FileNameStringComparer.GetHashCode(obj.Path);
  }
}
