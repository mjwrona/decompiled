// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.VssfSoapExtension
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;
using System.Web;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class VssfSoapExtension : SoapExtension
  {
    public override object GetInitializer(
      LogicalMethodInfo methodInfo,
      SoapExtensionAttribute attribute)
    {
      return (object) null;
    }

    public override object GetInitializer(Type serviceType) => (object) null;

    public override void Initialize(object initializer)
    {
    }

    public override void ProcessMessage(SoapMessage message)
    {
      if (message.Stage != SoapMessageStage.BeforeDeserialize)
        return;
      TeamFoundationApplication applicationInstance = HttpContextFactory.Current.ApplicationInstance as TeamFoundationApplication;
      IVssRequestContext vssRequestContext = applicationInstance.VssRequestContext;
      IdentityValidationResult validationResult = vssRequestContext.IsValidIdentity();
      if (validationResult.IsSuccess)
        return;
      TeamFoundationApplicationCore.CompleteRequest(vssRequestContext, (IHttpApplication) new HttpApplicationWrapper((HttpApplication) applicationInstance), HttpStatusCode.Unauthorized, validationResult.Exception);
    }
  }
}
