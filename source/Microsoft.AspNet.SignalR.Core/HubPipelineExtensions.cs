// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.HubPipelineExtensions
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Hubs;
using System;

namespace Microsoft.AspNet.SignalR
{
  public static class HubPipelineExtensions
  {
    public static void RequireAuthentication(this IHubPipeline pipeline)
    {
      if (pipeline == null)
        throw new ArgumentNullException(nameof (pipeline));
      AuthorizeAttribute authorizeAttribute = new AuthorizeAttribute();
      pipeline.AddModule((IHubPipelineModule) new AuthorizeModule((IAuthorizeHubConnection) authorizeAttribute, (IAuthorizeHubMethodInvocation) authorizeAttribute));
    }
  }
}
