// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common.SmartRouterFrameworkServiceBase
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common
{
  public abstract class SmartRouterFrameworkServiceBase : SmartRouterBase, IVssFrameworkService
  {
    protected SmartRouterFrameworkServiceBase(
      SmartRouterBase.TraceLayer traceLayer,
      bool requireDeploymentContext = false)
      : base(traceLayer)
    {
      this.RequireDeploymentContext = requireDeploymentContext;
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      requestContext = this.CheckRequestContext(requestContext);
      this.ServiceStopped = false;
      this.OnServiceStart(requestContext);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext = this.CheckRequestContext(requestContext);
      this.ServiceStopped = true;
      this.OnServiceEnd(requestContext);
    }

    protected virtual void OnServiceStart(IVssRequestContext requestContext)
    {
    }

    protected virtual void OnServiceEnd(IVssRequestContext requestContext)
    {
    }

    protected virtual bool IsEnabled(IVssRequestContext requestContext) => !this.ServiceStopped && requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsSmartRouterFeatureEnabled();

    protected virtual IVssRequestContext CheckRequestContext(IVssRequestContext requestContext) => requestContext.CheckRequestContext(this.RequireDeploymentContext);

    protected virtual ILockName CreateLockName(IVssRequestContext requestContext, string name) => requestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}/{1}", (object) this.GetType().FullName, (object) name));

    protected bool ServiceStopped { get; private set; }

    private bool RequireDeploymentContext { get; }
  }
}
