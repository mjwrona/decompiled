// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OwnerId
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public enum OwnerId : byte
  {
    Generic,
    VersionControl,
    WorkItemTracking,
    TeamBuild,
    TeamTest,
    Servicing,
    UnitTest,
    WebAccess,
    ProcessTemplate,
    StrongBox,
    FileContainer,
    CodeSense,
    Profile,
    Aad,
    Gallery,
    BlobStore,
    GroupLicensing,
    MMS,
    DistributedTask,
    DISCO,
    Status,
    Deployment,
    PermissionsReport,
    ImsRemoteCache,
  }
}
