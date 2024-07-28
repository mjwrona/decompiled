// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.ServiceDeploymentType
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Admin
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum ServiceDeploymentType
  {
    None,
    Tfs,
    Sps,
    Els,
    Msdn,
    ServiceHooks,
    Ai,
    CodeLens,
    Rm,
    Ibiza,
    Search,
    Dtl,
    OSS,
    TRI,
    CloudWorkspace,
    CloudWorkbench,
    Artifact,
    BlobStore,
    SpsExtension,
    ExtMgmt,
    Market,
    Arcus,
    Feed,
    Packaging,
    Analytics,
    Compliance,
  }
}
