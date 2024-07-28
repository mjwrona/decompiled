// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.S2SCommunicator
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Remoting.Messaging;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class S2SCommunicator
  {
    private const string ContextDataName = "6AE593A0-7818-4FA8-AF7D-66F199E7BFB2";
    private static readonly Guid s_searchPrincipal = new Guid("00000010-0000-8888-8000-000000000000");

    public static void SetRequestContext(IVssRequestContext requestContext) => CallContext.LogicalSetData("6AE593A0-7818-4FA8-AF7D-66F199E7BFB2", (object) requestContext);

    public static void ClearRequestContext() => CallContext.FreeNamedDataSlot("6AE593A0-7818-4FA8-AF7D-66F199E7BFB2");

    public static VssCredentials GetS2SCredentials()
    {
      IVssRequestContext vssRequestContext = S2SCommunicator.GetRequestContext().To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<S2SCredentialsService>().GetS2SCredentials(vssRequestContext, S2SCommunicator.s_searchPrincipal);
    }

    private static IVssRequestContext GetRequestContext() => CallContext.LogicalGetData("6AE593A0-7818-4FA8-AF7D-66F199E7BFB2") as IVssRequestContext;
  }
}
