// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.EmailNotification.EmailAbExperiment`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.EmailNotification
{
  public abstract class EmailAbExperiment<T> where T : INotificationEmailData
  {
    public const string InteractionTrackingFeatureFlag = "VisualStudio.EmailNotificationService.InteractionTracking";
    public const float DefaultThrottle = 0.8f;
    private static Random s_randomizer = new Random();
    private static object s_randomizerLock = new object();

    protected IVssRequestContext RequestContext { get; private set; }

    public float Throttle { get; private set; }

    protected abstract T CreateOriginalEmail();

    protected abstract T CreateEmailForA();

    protected abstract T CreateEmailForB();

    public EmailAbExperiment(IVssRequestContext requestContext, float throttle = 0.8f)
    {
      this.RequestContext = requestContext;
      this.Throttle = throttle;
    }

    public T GetEmail()
    {
      if (!this.RequestContext.IsFeatureEnabled("VisualStudio.EmailNotificationService.InteractionTracking"))
        return this.CreateOriginalEmail();
      double num;
      lock (EmailAbExperiment<T>.s_randomizerLock)
        num = EmailAbExperiment<T>.s_randomizer.NextDouble();
      return num < (double) this.Throttle ? this.CreateEmailForB() : this.CreateEmailForA();
    }
  }
}
