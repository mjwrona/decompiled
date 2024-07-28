// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ReparentCollectionV1Controller
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.Organization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "ReparentCollection", ResourceName = "Requests", ResourceVersion = 1)]
  public class ReparentCollectionV1Controller : 
    ServicingOrchestrationController<FrameworkReparentCollectionRequest, FrameworkReparentCollectionJobManager>
  {
    protected override void ValidateRequest(
      IVssRequestContext requestContext,
      FrameworkReparentCollectionRequest request)
    {
      base.ValidateRequest(requestContext, request);
      TeamFoundationServiceHostProperties localProperties;
      if (!requestContext.GetService<IInternalHostSyncService>().LocalHostExistsAndWellFormed(requestContext, request.HostId, out localProperties) && localProperties != null)
        throw new HostDoesNotExistException(request.HostId);
    }
  }
}
