// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitObjectEqualityComparer
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TfsGitObjectEqualityComparer : IEqualityComparer<TfsGitObject>
  {
    private static readonly TfsGitObjectEqualityComparer s_instance = new TfsGitObjectEqualityComparer();

    public bool Equals(TfsGitObject x, TfsGitObject y) => x.ObjectId == y.ObjectId;

    public int GetHashCode(TfsGitObject obj) => obj.ObjectId.GetHashCode();

    public static IEqualityComparer<TfsGitObject> Instance => (IEqualityComparer<TfsGitObject>) TfsGitObjectEqualityComparer.s_instance;
  }
}
