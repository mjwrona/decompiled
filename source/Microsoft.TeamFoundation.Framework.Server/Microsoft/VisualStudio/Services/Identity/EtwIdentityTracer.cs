// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.EtwIdentityTracer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class EtwIdentityTracer
  {
    public static readonly EtwIdentityTracer Instance = new EtwIdentityTracer();
    private ITeamFoundationTracingService TeamFoundationTracer;
    private const string LocalAuthorityDomain = "LOCAL AUTHORITY";
    private const string ClaimsProviderDomain = "ClaimsProvider";
    internal const string ServicePrincipalDomain = "@2c895908-04e0-4952-89fd-54b0046d6288";
    private const string Area = "Identity";
    private const string Layer = "EtwIdentityTracer";

    internal EtwIdentityTracer(ITeamFoundationTracingService tracer = null) => this.TeamFoundationTracer = tracer ?? (ITeamFoundationTracingService) TeamFoundationTracingServiceWrapper.Instance;

    public void Trace(
      IVssRequestContext requestContext,
      ITraceRequest tracer,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string fallbackIdentityProviderId = null)
    {
      try
      {
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
        if (identity.Descriptor == (IdentityDescriptor) null)
          tracer.TraceSerializedConditionally(80803, TraceLevel.Verbose, "Identity", nameof (EtwIdentityTracer), "Skipped tracing identity due to null descriptor: {0}", (object) new
          {
            identity = identity,
            fallbackIdentityProviderId = fallbackIdentityProviderId
          });
        else if (identity.Descriptor.IdentityType != "Microsoft.IdentityModel.Claims.ClaimsIdentity" && identity.Descriptor.IdentityType != "Microsoft.TeamFoundation.ServiceIdentity")
        {
          tracer.TraceSerializedConditionally(80804, TraceLevel.Verbose, "Identity", nameof (EtwIdentityTracer), "Skipped tracing non-claims and non-service identity: {0}", (object) new
          {
            identity = identity,
            fallbackIdentityProviderId = fallbackIdentityProviderId
          });
        }
        else
        {
          EtwIdentityTracer.IdentityTracingInfo identityTracingInfo = EtwIdentityTracer.CategorizeIdentityForSI(requestContext, tracer, identity, fallbackIdentityProviderId);
          if (identityTracingInfo.IdentityProviderName == "Vsts")
          {
            tracer.TraceSerializedConditionally(80810, TraceLevel.Verbose, "Identity", nameof (EtwIdentityTracer), "Skipped tracing VSTS and unexpected service identity: {0}", (object) new
            {
              identity = identity,
              fallbackIdentityProviderId = fallbackIdentityProviderId
            });
          }
          else
          {
            Guid vsid = identity.MasterId;
            if (vsid == Guid.Empty || vsid == IdentityConstants.LinkedId)
              vsid = identity.Id;
            this.TeamFoundationTracer.TraceIdentityChanges(vsid, identity.Cuid(), identityTracingInfo.IdentityCategory, identityTracingInfo.IdentityProviderName, identityTracingInfo.IdentityProviderId, identityTracingInfo.IdentityProviderTenantId, DateTime.UtcNow);
          }
        }
      }
      catch (Exception ex)
      {
        tracer.TraceSerializedConditionally(80808, TraceLevel.Error, "Identity", nameof (EtwIdentityTracer), "{0}", (object) new
        {
          Identity = identity,
          FallbackIdentityProviderId = fallbackIdentityProviderId,
          Exception = ex
        });
      }
    }

    private static EtwIdentityTracer.IdentityTracingInfo CategorizeIdentityForSI(
      IVssRequestContext requestContext,
      ITraceRequest tracer,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string fallbackIdentityProviderId)
    {
      string property = identity.GetProperty<string>("Domain", (string) null);
      if (string.IsNullOrWhiteSpace(property))
        tracer.TraceSerializedConditionally(80805, TraceLevel.Error, "Identity", nameof (EtwIdentityTracer), "Domain was missing, null, or whitespace: {0}", (object) identity);
      EtwIdentityTracer.IdentityTracingInfo identityTracingInfo = EtwIdentityTracer.TryCategorizeAsVstsService(identity, property) ?? EtwIdentityTracer.TryCategorizeAsMsa(tracer, identity, fallbackIdentityProviderId, property) ?? EtwIdentityTracer.TryCategorizeAsAad(requestContext, tracer, identity, fallbackIdentityProviderId, property) ?? EtwIdentityTracer.CateogrizeAsUnexpectedType(tracer, identity, fallbackIdentityProviderId);
      tracer.TraceSerializedConditionally(80807, TraceLevel.Verbose, "Identity", nameof (EtwIdentityTracer), "{0}", (object) new
      {
        identity = identity,
        fallbackIdentityProviderId = fallbackIdentityProviderId
      });
      return identityTracingInfo;
    }

    private static EtwIdentityTracer.IdentityTracingInfo TryCategorizeAsVstsService(
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string domain)
    {
      if (!string.Equals(domain, "LOCAL AUTHORITY", StringComparison.OrdinalIgnoreCase) && !(identity.Descriptor.IdentityType == "Microsoft.TeamFoundation.ServiceIdentity"))
        return (EtwIdentityTracer.IdentityTracingInfo) null;
      return new EtwIdentityTracer.IdentityTracingInfo()
      {
        IdentityId = identity.Id,
        IdentityCategory = "ServiceIdentity",
        IdentityProviderName = "Vsts",
        IdentityProviderId = identity.Id.ToString(),
        IdentityProviderTenantId = domain
      };
    }

    internal static EtwIdentityTracer.IdentityTracingInfo TryCategorizeAsMsa(
      ITraceRequest tracer,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string fallbackIdentityProviderId,
      string domain)
    {
      if (!string.Equals(domain, "Windows Live ID", StringComparison.OrdinalIgnoreCase))
        return (EtwIdentityTracer.IdentityTracingInfo) null;
      string str = identity.GetProperty<string>("PUID", (string) null) ?? fallbackIdentityProviderId;
      if (string.IsNullOrEmpty(str))
      {
        tracer.TraceSerializedConditionally(80802, TraceLevel.Verbose, "Identity", nameof (EtwIdentityTracer), "{0}", (object) identity);
        string identifier = identity.Descriptor?.Identifier;
        if (identifier != null && identifier.Length >= 16 && string.Equals("@Live.com", identifier.Substring(16), StringComparison.InvariantCultureIgnoreCase))
          str = identifier.Substring(0, 16);
      }
      return new EtwIdentityTracer.IdentityTracingInfo()
      {
        IdentityId = identity.Id,
        IdentityCategory = "AuthenticatedIdentity",
        IdentityProviderName = "Msa",
        IdentityProviderId = str ?? identity.Id.ToString(),
        IdentityProviderTenantId = domain
      };
    }

    private static EtwIdentityTracer.IdentityTracingInfo TryCategorizeAsAad(
      IVssRequestContext requestContext,
      ITraceRequest tracer,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string fallbackIdentityProviderId,
      string domain)
    {
      int num1 = Guid.TryParse(domain, out Guid _) ? 1 : 0;
      bool flag = domain == "ClaimsProvider" || domain == "@2c895908-04e0-4952-89fd-54b0046d6288";
      int num2 = flag ? 1 : 0;
      if ((num1 | num2) == 0)
        return (EtwIdentityTracer.IdentityTracingInfo) null;
      string str1 = flag || ServicePrincipals.IsServicePrincipal(requestContext, identity.Descriptor) ? "ServiceIdentity" : "AuthenticatedIdentity";
      string str2 = identity.GetProperty<string>("http://schemas.microsoft.com/identity/claims/objectidentifier", (string) null);
      if (string.IsNullOrWhiteSpace(str2))
      {
        if (!string.IsNullOrWhiteSpace(fallbackIdentityProviderId))
        {
          str2 = fallbackIdentityProviderId;
          tracer.TraceSerializedConditionally(80809, TraceLevel.Verbose, "Identity", nameof (EtwIdentityTracer), "Identity had no OID claim, using supplied fallback id '{0}' for identity: {1}", (object) fallbackIdentityProviderId, (object) identity);
        }
        else
        {
          tracer.TraceSerializedConditionally(80809, TraceLevel.Verbose, "Identity", nameof (EtwIdentityTracer), "Identity had no OID claim nor fallback id, so we are reporting the Id field for identity: {0}", (object) identity);
          str2 = identity.Id.ToString();
        }
      }
      return new EtwIdentityTracer.IdentityTracingInfo()
      {
        IdentityId = identity.Id,
        IdentityCategory = str1,
        IdentityProviderName = "Aad",
        IdentityProviderId = str2,
        IdentityProviderTenantId = domain
      };
    }

    private static EtwIdentityTracer.IdentityTracingInfo CateogrizeAsUnexpectedType(
      ITraceRequest tracer,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string fallbackIdentityProviderId)
    {
      tracer.TraceSerializedConditionally(80806, TraceLevel.Error, "Identity", nameof (EtwIdentityTracer), "Could not resolve the identity provider: {0}", (object) identity);
      return new EtwIdentityTracer.IdentityTracingInfo()
      {
        IdentityId = identity.Id,
        IdentityCategory = "UnexpectedIdentityType",
        IdentityProviderName = "Vsts",
        IdentityProviderId = fallbackIdentityProviderId ?? identity.Id.ToString()
      };
    }

    internal class IdentityTracingInfo
    {
      public Guid IdentityId { get; set; }

      public string IdentityCategory { get; set; }

      public string IdentityProviderName { get; set; }

      public string IdentityProviderId { get; set; }

      public string IdentityProviderTenantId { get; set; }
    }
  }
}
