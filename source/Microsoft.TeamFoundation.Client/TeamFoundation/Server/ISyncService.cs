// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.ISyncService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Server
{
  public interface ISyncService
  {
    SyncMapping GetSyncMapping(Guid serverId);

    bool GetSyncMappingChange(Guid serverId, int baselineRev, out SyncMapping mapping);

    SyncProperty[] GetSyncProperties(Guid serverId);

    SyncProperty GetSyncProperty(Guid serverId, string name);

    void SaveMapping(Guid serverId, int baselineRev, string mapping);

    void SaveSyncProperty(Guid serverId, int baselineRev, string name, string value);
  }
}
