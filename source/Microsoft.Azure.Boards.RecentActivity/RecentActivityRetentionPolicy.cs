// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.RecentActivity.RecentActivityRetentionPolicy
// Assembly: Microsoft.Azure.Boards.RecentActivity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 684DCFA4-4764-4794-94A6-960AF811434C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.RecentActivity.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.Azure.Boards.RecentActivity
{
  public class RecentActivityRetentionPolicy
  {
    public RecentActivityRetentionPolicy(
      Guid artifactKind,
      int retentionCountPerUser,
      int retentionCountPerProject)
    {
      ArgumentUtility.CheckGreaterThanZero((float) retentionCountPerUser, nameof (retentionCountPerUser));
      ArgumentUtility.CheckGreaterThanZero((float) retentionCountPerProject, nameof (retentionCountPerProject));
      this.ArtifactKind = artifactKind;
      this.RetentionCountPerUser = retentionCountPerUser;
      this.RetentionCountPerProject = retentionCountPerProject;
    }

    public Guid ArtifactKind { get; }

    public int RetentionCountPerUser { get; }

    public int RetentionCountPerProject { get; }
  }
}
