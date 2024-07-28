// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationFileRepositoryService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class TeamFoundationFileRepositoryService : IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public FileRepositoryProperties GetProxyProperties(IVssRequestContext requestContext)
    {
      ITeamFoundationSigningService service = requestContext.GetService<ITeamFoundationSigningService>();
      return new FileRepositoryProperties(new SigningInfo(ProxyConstants.ProxySigningKey)
      {
        DownloadKey = service.GetPublicKey(requestContext, ProxyConstants.ProxySigningKey, out int _)
      });
    }
  }
}
