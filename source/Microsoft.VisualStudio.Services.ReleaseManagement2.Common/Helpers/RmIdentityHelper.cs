// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.RmIdentityHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  public static class RmIdentityHelper
  {
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Handling null check in the implementation")]
    public static IEnumerable<string> GetEmailAddresses(
      IVssRequestContext requestContext,
      Guid[] identities,
      bool throwExceptionOnFailure = true)
    {
      List<string> source = new List<string>();
      if (!((IEnumerable<Guid>) identities).IsNullOrEmpty<Guid>())
      {
        foreach (Guid identity in identities)
          source.AddRange((IEnumerable<string>) RmIdentityHelper.GetEmailAddresses(requestContext, identity, throwExceptionOnFailure));
      }
      return source.Distinct<string>();
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public static IList<string> GetEmailAddresses(
      IVssRequestContext requestContext,
      Guid identityId,
      bool throwExceptionOnFailure = true)
    {
      if (requestContext == null || object.Equals((object) identityId, (object) Guid.Empty))
        return (IList<string>) new List<string>();
      List<string> emailAddresses = new List<string>();
      try
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = service.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          identityId
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity1 != null)
        {
          IEnumerable<string> preferredEmailAddress = RmIdentityHelper.GetPreferredEmailAddress(requestContext, identity1);
          if (!preferredEmailAddress.IsNullOrEmpty<string>())
            emailAddresses.AddRange(preferredEmailAddress);
          else if (identity1.IsContainer)
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity2 = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
            {
              identity1.Descriptor
            }, QueryMembership.ExpandedDown, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
            IdentityDescriptor[] array = identity2 == null ? (IdentityDescriptor[]) null : identity2.Members.ToArray<IdentityDescriptor>();
            foreach (Microsoft.VisualStudio.Services.Identity.Identity identity3 in !((IEnumerable<IdentityDescriptor>) array).IsNullOrEmpty<IdentityDescriptor>() ? (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) array, QueryMembership.None, (IEnumerable<string>) null) : (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) Array.Empty<Microsoft.VisualStudio.Services.Identity.Identity>())
              emailAddresses.AddRange(RmIdentityHelper.GetPreferredEmailAddress(requestContext, identity3));
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(1900024, TraceLevel.Warning, "ReleaseManagementService", nameof (RmIdentityHelper), "GetEmailAddresses for identityId: {0}. Exception: {1}", (object) identityId.ToString(), (object) ex.ToString());
        if (throwExceptionOnFailure)
          throw;
      }
      return (IList<string>) emailAddresses;
    }

    private static IEnumerable<string> GetPreferredEmailAddress(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string preferredEmailAddress = IdentityHelper.GetPreferredEmailAddress(requestContext, identity.Id);
      if (preferredEmailAddress == null)
        return (IEnumerable<string>) new List<string>();
      return (IEnumerable<string>) ((IEnumerable<string>) preferredEmailAddress.Split(new char[2]
      {
        ';',
        ','
      }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
    }
  }
}
