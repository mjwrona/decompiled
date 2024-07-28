// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IGitConflictExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class IGitConflictExtensions
  {
    internal static bool IsSameConflict(this IGitConflict objA, IGitConflict objB) => objA.ConflictType == objB.ConflictType && objA.ConflictPath == objB.ConflictPath && objA.SourcePath == objB.SourcePath && objA.TargetPath == objB.TargetPath && objA.BaseObjectId == objB.BaseObjectId && objA.BaseObjectIdForTarget == objB.BaseObjectIdForTarget && objA.SourceObjectId == objB.SourceObjectId && objA.TargetObjectId == objB.TargetObjectId;

    public static IEqualityComparer<IGitConflict> SameConflictComparer { get; } = (IEqualityComparer<IGitConflict>) new IGitConflictExtensions.SameConflictComparerClass();

    private class SameConflictComparerClass : IEqualityComparer<IGitConflict>
    {
      public bool Equals(IGitConflict x, IGitConflict y) => x.IsSameConflict(y);

      public int GetHashCode(IGitConflict obj)
      {
        int hashCode = HashCodeUtil.GetHashCode<Sha1Id, Sha1Id, Sha1Id, Sha1Id>(obj.BaseObjectId, obj.SourceObjectId, obj.TargetObjectId, obj.BaseObjectIdForTarget);
        return HashCodeUtil.GetHashCode<GitConflictType, NormalizedGitPath, int>(obj.ConflictType, obj.ConflictPath, hashCode);
      }
    }
  }
}
