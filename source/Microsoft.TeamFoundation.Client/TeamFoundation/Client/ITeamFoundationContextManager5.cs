// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ITeamFoundationContextManager5
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Client
{
  [CLSCompliant(false)]
  [Guid("0CFE311C-3DEF-4EFF-8E33-6414C2339725")]
  public interface ITeamFoundationContextManager5 : 
    ITeamFoundationContextManager4,
    ITeamFoundationContextManager3,
    ITeamFoundationContextManager2,
    ITeamFoundationContextManager
  {
    void SetContext(
      TfsTeamProjectCollection teamProjectCollection,
      string project,
      Guid teamId,
      IEnumerable<Guid> repositoryIds,
      bool connectAsynchronously,
      ActiveContextChangeReason reason,
      bool promptOnError);

    Task SetContextAsync(
      TfsTeamProjectCollection teamProjectCollection,
      string project,
      Guid teamId,
      IEnumerable<Guid> repositoryIds,
      bool connectAsynchronously,
      ActiveContextChangeReason reason,
      bool promptOnError);
  }
}
