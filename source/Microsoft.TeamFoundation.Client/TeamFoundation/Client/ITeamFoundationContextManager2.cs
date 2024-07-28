// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ITeamFoundationContextManager2
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Client
{
  [CLSCompliant(false)]
  [Guid("9CCFE978-9849-4338-AB18-194EA5A646C5")]
  public interface ITeamFoundationContextManager2 : ITeamFoundationContextManager
  {
    void SetContext(
      TfsTeamProjectCollection teamProjectCollection,
      string project,
      Guid teamId,
      bool connectAsynchronously,
      ActiveContextChangeReason reason,
      bool promptOnError);

    void SetContext(
      string teamProjectCollectionUri,
      string project,
      Guid teamId,
      bool connectAsynchronously,
      ActiveContextChangeReason reason,
      bool promptOnError);
  }
}
