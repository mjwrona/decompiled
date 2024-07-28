// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IdentityCuidHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class IdentityCuidHelper
  {
    private const string s_area = "IdentityService";
    private const string s_layer = "IdentityCuidHelper";

    public static Guid GetCuidByVsid(IVssRequestContext requestContext, Guid identityId)
    {
      if (identityId == Guid.Empty)
      {
        requestContext.Trace(80240, TraceLevel.Warning, "IdentityService", nameof (IdentityCuidHelper), "Skip exchange for CUID because input identity Id is empty Guid");
        return identityId;
      }
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        identityId
      }, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity != null)
        return identity.Cuid();
      requestContext.Trace(80241, TraceLevel.Warning, "IdentityService", nameof (IdentityCuidHelper), string.Format("Cannot find this identity with id {0}", (object) identityId));
      return Guid.Empty;
    }

    internal static Guid GenerateCuidByTenantIdOid(
      Guid tenantId,
      Guid oid,
      NameBasedGuidVersion nameBasedGuidVersion = NameBasedGuidVersion.Seven)
    {
      byte[] numArray = new byte[32];
      Array.Copy((Array) tenantId.ToByteArray(), 0, (Array) numArray, 0, 16);
      Array.Copy((Array) oid.ToByteArray(), 0, (Array) numArray, 16, 16);
      return NameBasedGuidGenerator.NewAadGuid(numArray, nameBasedGuidVersion);
    }

    internal static Guid ComputeCuid(
      IVssRequestContext requestContext,
      IReadOnlyVssIdentity vssIdentity)
    {
      return IdentityCuidHelper.ComputeCuid(requestContext, vssIdentity, NameBasedGuidVersion.Seven, out Guid _, out Guid _);
    }

    internal static Guid ComputeCuid(
      IVssRequestContext requestContext,
      IReadOnlyVssIdentity vssIdentity,
      NameBasedGuidVersion nameBasedGuidVersion,
      bool returnNewGuidIfCannotCreateConsistentVSID = false)
    {
      return IdentityCuidHelper.ComputeCuid(requestContext, vssIdentity, nameBasedGuidVersion, out Guid _, out Guid _, returnNewGuidIfCannotCreateConsistentVSID);
    }

    internal static Guid ComputeCuid(
      IVssRequestContext requestContext,
      IReadOnlyVssIdentity vssIdentity,
      NameBasedGuidVersion nameBasedGuidVersion,
      out Guid tenantId,
      out Guid providerId,
      bool returnNewGuidIfCannotCreateConsistentVSID = false)
    {
      tenantId = new Guid();
      providerId = new Guid();
      bool flag = false;
      try
      {
        if (vssIdentity.IsExternalUser)
        {
          if (Guid.TryParse(vssIdentity.GetProperty<string>("Domain", string.Empty), out tenantId))
          {
            if (vssIdentity.Descriptor.IsCspPartnerIdentityType())
            {
              flag = true;
              string property = vssIdentity.GetProperty<string>("PUID", string.Empty);
              Guid cuid;
              if (IdentityCuidHelper.TryGenerateCuidByTenantIdPuidForCspPartner(tenantId, property, out cuid, nameBasedGuidVersion))
              {
                providerId = IdentityCuidHelper.GetPuidBasedProviderId(property);
                return cuid;
              }
            }
            else
            {
              flag = true;
              providerId = vssIdentity.GetProperty<Guid>("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.Empty);
              if (providerId != new Guid())
                return IdentityCuidHelper.GenerateCuidByTenantIdOid(tenantId, providerId, nameBasedGuidVersion);
            }
          }
        }
        else if (vssIdentity.GetProperty<string>("Domain", string.Empty) == "Windows Live ID")
        {
          flag = true;
          string property = vssIdentity.GetProperty<string>("PUID", string.Empty);
          Guid cuid;
          if (IdentityCuidHelper.TryGenerateCuidByPuid(property, out cuid, nameBasedGuidVersion))
          {
            providerId = IdentityCuidHelper.GetPuidBasedProviderId(property);
            return cuid;
          }
        }
        if (flag)
        {
          if (requestContext == null)
            TeamFoundationTracingService.TraceRaw(80245, TraceLevel.Error, "IdentityService", nameof (IdentityCuidHelper), "Cannot generate Cuid for Cuid based identity {0}", (object) vssIdentity.Id);
          else
            requestContext.Trace(80245, TraceLevel.Error, "IdentityService", nameof (IdentityCuidHelper), "Cannot generate Cuid for Cuid based identity {0}", (object) vssIdentity.Id);
        }
      }
      catch (Exception ex)
      {
        if (requestContext == null)
          TeamFoundationTracingService.TraceExceptionRaw(80205, "IdentityService", nameof (IdentityCuidHelper), ex);
        else
          requestContext.TraceException(80205, TraceLevel.Error, "IdentityService", nameof (IdentityCuidHelper), ex);
      }
      return !returnNewGuidIfCannotCreateConsistentVSID ? Guid.Empty : Guid.NewGuid();
    }

    internal static Guid ComputeCuid(
      IVssRequestContext requestContext,
      Guid tenantId,
      string originId,
      NameBasedGuidVersion nameBasedGuidVersion = NameBasedGuidVersion.Seven)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(originId, nameof (originId));
      if (tenantId == Guid.Empty)
      {
        Guid cuid;
        if (!IdentityCuidHelper.TryGenerateCuidByPuid(originId, out cuid, nameBasedGuidVersion))
          throw new ArgumentException(string.Format("Failed to generate CUID based on tenantId: {0}, originId: {1}.", (object) tenantId, (object) originId));
        return cuid;
      }
      Guid result;
      if (Guid.TryParse(originId, out result))
        return IdentityCuidHelper.GenerateCuidByTenantIdOid(tenantId, result, nameBasedGuidVersion);
      Guid cuid1;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.UseSubjectDescriptorForDIMS") && IdentityCuidHelper.TryGenerateCuidByTenantIdPuidForCspPartner(tenantId, originId, out cuid1, nameBasedGuidVersion))
        return cuid1;
      throw new ArgumentException("Found invalid AAD originId: " + originId + ".");
    }

    internal static Guid ComputeCuidForCspPartner(
      Guid domainId,
      string puid,
      NameBasedGuidVersion nameBasedGuidVersion = NameBasedGuidVersion.Seven)
    {
      ArgumentUtility.CheckForEmptyGuid(domainId, nameof (domainId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(puid, nameof (puid));
      Guid cuid;
      if (!IdentityCuidHelper.TryGenerateCuidByTenantIdPuidForCspPartner(domainId, puid, out cuid, nameBasedGuidVersion))
        throw new ArgumentException(string.Format("Failed to generate CUID based on domainId: {0}, puid: {1}.", (object) domainId, (object) puid));
      return cuid;
    }

    private static bool TryGenerateCuidByTenantIdPuidForCspPartner(
      Guid tenantId,
      string puidString,
      out Guid cuid,
      NameBasedGuidVersion nameBasedGuidVersion = NameBasedGuidVersion.Seven)
    {
      cuid = Guid.Empty;
      byte[] puidBytes;
      if (!IdentityCuidHelper.TryGetPuidBytes(puidString, out puidBytes))
        return false;
      byte[] numArray = new byte[16 + puidBytes.Length];
      Array.Copy((Array) tenantId.ToByteArray(), 0, (Array) numArray, 0, 16);
      Array.Copy((Array) puidBytes, 0, (Array) numArray, 16, puidBytes.Length);
      cuid = NameBasedGuidGenerator.NewCspPartnerGuid(numArray, nameBasedGuidVersion);
      return true;
    }

    internal static bool TryGenerateCuidByPuid(
      string puidString,
      out Guid cuid,
      NameBasedGuidVersion nameBasedGuidVersion = NameBasedGuidVersion.Seven)
    {
      cuid = Guid.Empty;
      byte[] puidBytes;
      if (!IdentityCuidHelper.TryGetPuidBytes(puidString, out puidBytes))
        return false;
      cuid = NameBasedGuidGenerator.NewMsaGuid(puidBytes, nameBasedGuidVersion);
      return true;
    }

    private static bool TryGetPuidBytes(string puidString, out byte[] puidBytes)
    {
      puidBytes = (byte[]) null;
      long result;
      if (string.IsNullOrWhiteSpace(puidString) || !long.TryParse(puidString, NumberStyles.AllowHexSpecifier, (IFormatProvider) null, out result))
        return false;
      puidBytes = BitConverter.GetBytes(result);
      return true;
    }

    private static Guid GetPuidBasedProviderId(string puidString)
    {
      Guid result;
      if (Guid.TryParse(puidString.PadLeft(32, '0'), out result))
        return result;
      throw new ArgumentException("Found invalid puid string: " + puidString + ".");
    }
  }
}
