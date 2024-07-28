// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ITeamFoundationContextManager4
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Client
{
  [CLSCompliant(false)]
  [Guid("992CE4A5-D4ED-447E-BDEA-566A75BBB2CF")]
  public interface ITeamFoundationContextManager4 : 
    ITeamFoundationContextManager3,
    ITeamFoundationContextManager2,
    ITeamFoundationContextManager
  {
    void SetContext(
      string teamProjectCollectionUri,
      string project,
      Guid teamId,
      IEnumerable<Guid> repositoryIds);

    void SetContext(
      string teamProjectCollectionUri,
      string project,
      Guid teamId,
      IEnumerable<Guid> repositoryIds,
      bool connectAsynchronously,
      ActiveContextChangeReason reason,
      bool promptOnError);
  }
}
