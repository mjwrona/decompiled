// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.RecentActivity.RecentActivity
// Assembly: Microsoft.Azure.Boards.RecentActivity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 684DCFA4-4764-4794-94A6-960AF811434C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.RecentActivity.dll

using System;

namespace Microsoft.Azure.Boards.RecentActivity
{
  public class RecentActivity
  {
    public RecentActivityScope Scope { get; set; }

    public Guid ProjectId { get; set; }

    public Guid ArtifactKind { get; set; }

    public string ArtifactId { get; set; }

    public Guid IdentityId { get; set; }

    public string ActivityDetails { get; set; }

    public DateTime ActivityDate { get; set; }
  }
}
