// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.PermissionsPolicyManagementService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public class PermissionsPolicyManagementService : IVssFrameworkService
  {
    private const string c_area = "PermissionsPolicy";
    private const string c_layer = "Service";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(15321100, "PermissionsPolicy", "Service", nameof (ServiceStart));
      systemRequestContext.TraceLeave(15321101, "PermissionsPolicy", "Service", nameof (ServiceStart));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(15321102, "PermissionsPolicy", "Service", nameof (ServiceEnd));
      systemRequestContext.TraceLeave(15321103, "PermissionsPolicy", "Service", nameof (ServiceEnd));
    }

    public void AddPermisionsPolicyHeader(
      IVssRequestContext requestContext,
      HttpContextBase context)
    {
      requestContext.TraceEnter(15321110, "PermissionsPolicy", "Service", nameof (AddPermisionsPolicyHeader));
      try
      {
        requestContext.To(TeamFoundationHostType.Deployment);
        if (!requestContext.IsFeatureEnabled(PermissionsPolicyFeatureFlags.PermissionsPolicyHeaderFeatureFlag))
          return;
        context.Response.AddHeader("Permissions-Policy", "interest-cohort=()");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15321125, "PermissionsPolicy", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(15321130, "PermissionsPolicy", "Service", nameof (AddPermisionsPolicyHeader));
      }
    }
  }
}
