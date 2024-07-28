// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CollectionMoveConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class CollectionMoveConstants
  {
    public static readonly string SnapshotStateExtendedProperty = "TFS_SNAPSHOT_STATE";
    public static readonly string SnapshotStateStarted = "Started";
    public static readonly string SnapshotStateComplete = "Complete";
    public static readonly string DeleteCollectionExtendedProperty = "TFS_CURRENTLY_DELETING_COLLECTION";
    public static readonly string AttachCloneCollectionId = "TFS_ATTACH_CLONE_ID";
    public static readonly string HostedSnapshotBlobNameFormat = "bacpac/{0}.zip";
    public static readonly string HostedSnapshotBacpacNameFormat = "bacpac/{0}.bacpac";
  }
}
