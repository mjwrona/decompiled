// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Web.RemoteProjectAccessChecker
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.ServiceHooks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0ADA66F7-C61B-45D2-A394-67E5BF762451
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.ServiceHooks.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WebPlatform.Server;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Web
{
  public class RemoteProjectAccessChecker : IProjectAccessChecker
  {
    public void CheckProjectAccess(IVssRequestContext tfsRequestContext, ProjectInfo project)
    {
    }
  }
}
