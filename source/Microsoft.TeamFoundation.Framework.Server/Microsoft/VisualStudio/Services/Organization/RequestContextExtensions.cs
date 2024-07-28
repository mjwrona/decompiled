// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.RequestContextExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Organization
{
  public static class RequestContextExtensions
  {
    private const string c_Area = "Organization";
    private const string c_Layer = "RequestContextExtensions";
    private static readonly Guid c_MicrosoftTenantid = new Guid("72f988bf-86f1-41af-91ab-2d7cd011db47");
    private static readonly Guid c_PPEMicrosoftTenantid = new Guid("f686d426-8d16-42db-81b7-ab578e110ccd");
    internal const string c_IsOrganizationActivatedRequestContextItemKey = "IsOrganizationActivated";

    [Obsolete("This method is obsolete. Use GetOrganizationAadTenantId() instead.")]
    public static Guid GetAzureActiveDirectoryTenantId(this IVssRequestContext context) => context.GetOrganizationAadTenantId();

    public static Guid GetOrganizationAadTenantId(this IVssRequestContext context)
    {
      if (context.ExecutionEnvironment.IsOnPremisesDeployment)
        return new Guid();
      if (context.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return new Guid();
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Application);
      ServiceHostTags serviceHostTags = ServiceHostTags.FromString(context1.ServiceHost.Description);
      if (serviceHostTags != ServiceHostTags.EmptyServiceHostTags && serviceHostTags.HasTag(WellKnownServiceHostTags.NoOrgMetadata))
      {
        context.Trace(36001, TraceLevel.Info, "Organization", nameof (RequestContextExtensions), "No data found for Organization: {0}.", (object) context1.ServiceHost.Name);
        return new Guid();
      }
      Microsoft.VisualStudio.Services.Organization.Organization organization = context1.GetService<IOrganizationService>().GetOrganization(context1.Elevate(), (IEnumerable<string>) null);
      if (organization == null)
        throw new OrganizationNotFoundException(FrameworkResources.OrganizationNotFoundByContext((object) context1.ServiceHost.ToString()));
      if (organization.TenantId == Guid.Empty)
        context.Trace(36002, TraceLevel.Info, "Organization", nameof (RequestContextExtensions), "Empty Tenant ID for Organization: {0}.", (object) organization.Id);
      return organization.TenantId;
    }

    public static bool IsOrganizationAadBacked(this IVssRequestContext context) => context.GetOrganizationAadTenantId() != Guid.Empty;

    public static bool IsOrganizationActivated(this IVssRequestContext context)
    {
      if (context.ExecutionEnvironment.IsOnPremisesDeployment || context.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return false;
      bool flag1;
      if (context.TryGetItem<bool>(nameof (IsOrganizationActivated), out flag1))
        return flag1;
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Application).Elevate();
      IOrganizationService service = context1.GetService<IOrganizationService>();
      bool flag2 = false;
      try
      {
        Microsoft.VisualStudio.Services.Organization.Organization organization = service.GetOrganization(context1, (IEnumerable<string>) null);
        flag2 = organization != null && organization.IsActivated;
      }
      catch (Exception ex)
      {
        context.TraceException(666666, "Organization", nameof (RequestContextExtensions), ex);
      }
      context.Items[nameof (IsOrganizationActivated)] = (object) flag2;
      return flag2;
    }

    public static bool IsOrganizationInReadOnlyMode(this IVssRequestContext context)
    {
      if (context.ExecutionEnvironment.IsOnPremisesDeployment || context.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return false;
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Application).Elevate();
      bool flag = false;
      try
      {
        flag = context1.GetService<IOrganizationPolicyService>().GetPolicy<bool>(context1.Elevate(), "Policy.IsReadOnly", false).EffectiveValue;
      }
      catch (Exception ex)
      {
        context.TraceException(666667, "Organization", nameof (RequestContextExtensions), ex);
      }
      return flag;
    }

    public static bool IsMicrosoftTenant(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return requestContext.ExecutionEnvironment.IsDevFabricDeployment || requestContext.GetOrganizationAadTenantId() == RequestContextExtensions.c_MicrosoftTenantid;
    }

    public static bool IsMicrosoftTenant(this IVssRequestContext requestContext, Guid tenantId)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return tenantId == RequestContextExtensions.c_MicrosoftTenantid || tenantId == RequestContextExtensions.c_PPEMicrosoftTenantid;
    }

    public static Guid GetMicrosoftTenantId(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      return requestContext.ExecutionEnvironment.IsDevFabricDeployment ? RequestContextExtensions.c_PPEMicrosoftTenantid : RequestContextExtensions.c_MicrosoftTenantid;
    }
  }
}
