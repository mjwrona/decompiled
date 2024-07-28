// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EvaluationContext
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class EvaluationContext
  {
    public bool Verify { get; set; }

    public TimeSpan RegexTimeout { get; set; } = new TimeSpan(10000L);

    public bool UseRegexMatch { get; set; }

    public int EvaluationsCount { get; set; }

    public Subscription Subscription { get; set; }

    public TeamFoundationEvent Event { get; set; }

    public Microsoft.VisualStudio.Services.Identity.Identity User { get; set; }

    public Dictionary<string, string> MacrosLookup { get; set; }

    public ISubscriptionObjectTrace Tracer { get; set; }
  }
}
