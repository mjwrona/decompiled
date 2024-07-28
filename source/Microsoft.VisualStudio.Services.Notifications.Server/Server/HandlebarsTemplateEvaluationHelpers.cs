// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.HandlebarsTemplateEvaluationHelpers
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal static class HandlebarsTemplateEvaluationHelpers
  {
    internal static readonly TimeSpan s_evaluationTimeout = TimeSpan.FromMilliseconds(2000.0);
    internal const int c_maximumDepth = 50;
    internal const int c_maximumLength = 5000000;

    public static MustacheOptions CreateEvaluationOptions(
      IVssRequestContext requestContext,
      bool disableInlinePartials = false)
    {
      if (!requestContext.IsFeatureEnabled("Notifications.ThirdPartyEventPublishing"))
        return (MustacheOptions) null;
      return new MustacheOptions()
      {
        CancellationToken = new CancellationTokenSource(HandlebarsTemplateEvaluationHelpers.s_evaluationTimeout).Token,
        DisableInlinePartials = disableInlinePartials,
        MaxDepth = 50,
        MaxResultLength = 5000000
      };
    }
  }
}
