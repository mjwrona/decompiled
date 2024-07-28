// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.SyncService4
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using System;

namespace Microsoft.TeamFoundation.Server
{
  internal class SyncService4 : SyncService
  {
    public SyncService4(TfsTeamProjectCollection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("3b4c924f-113e-41c7-af6a-3b75d165c636");

    protected override string ComponentName => "vstfs";

    protected override string ServiceType => nameof (SyncService4);
  }
}
