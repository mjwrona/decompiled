// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WebPlatform.Server.IRequestProjectService
// Assembly: Microsoft.TeamFoundation.WebPlatform.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BDF91478-A3ED-4B5B-AA51-9473C7AE6182
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WebPlatform.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.WebPlatform.Server
{
  [DefaultServiceImplementation(typeof (RequestProjectService))]
  public interface IRequestProjectService : IVssFrameworkService
  {
    ProjectInfo GetProject(IVssRequestContext requestContext);

    string GetRequestProjectIdentifier(IVssRequestContext requestContext);

    bool IsProjectSpecifiedById(IVssRequestContext requestContext);
  }
}
