// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.RecentActivity.IRecentActivityArtifactProvider
// Assembly: Microsoft.Azure.Boards.RecentActivity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 684DCFA4-4764-4794-94A6-960AF811434C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.RecentActivity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.Azure.Boards.RecentActivity
{
  [InheritedExport]
  public interface IRecentActivityArtifactProvider
  {
    Guid ArtifactKind { get; }

    RecentActivityRetentionPolicy GetRetentionPolicy(IVssRequestContext requestContext);

    IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> Filter(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Microsoft.Azure.Boards.RecentActivity.RecentActivity> activities);
  }
}
