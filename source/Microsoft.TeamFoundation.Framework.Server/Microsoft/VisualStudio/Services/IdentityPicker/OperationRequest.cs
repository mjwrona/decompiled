// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.OperationRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.IdentityPicker.Extensions;
using System;

namespace Microsoft.VisualStudio.Services.IdentityPicker
{
  public abstract class OperationRequest
  {
    public virtual IdentityPickerServiceExtensionData ExtensionData { get; set; }

    internal OperationResponse Invoke(IVssRequestContext requestContext)
    {
      this.Validate(requestContext);
      return this.Process(requestContext);
    }

    protected virtual void Validate(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && !requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
      {
        InvalidOperationException ex = new InvalidOperationException("Identity Picker service operations cannot be called exclusively at deployment level");
        Tracing.TraceException(requestContext, 584, (Exception) ex);
        throw ex;
      }
    }

    protected abstract OperationResponse Process(IVssRequestContext requestContext);
  }
}
