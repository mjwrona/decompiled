// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ManualInterventionIdentityHandler
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class ManualInterventionIdentityHandler
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Used for extensibility purpose")]
    public static void PopulateIdentities(
      ManualIntervention manualIntervention,
      IVssRequestContext requestContext,
      bool throwExceptionOnFailure = false)
    {
      if (manualIntervention == null)
        throw new ArgumentNullException(nameof (manualIntervention));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      ManualInterventionIdentityHandler.PopulateIdentities((IList<ManualIntervention>) new List<ManualIntervention>()
      {
        manualIntervention
      }, requestContext, throwExceptionOnFailure);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Used for extensibility purpose")]
    public static void PopulateIdentities(
      IList<ManualIntervention> manualInterventions,
      IVssRequestContext requestContext,
      bool throwExceptionOnFailure = false)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      try
      {
        using (ReleaseManagementTimer.Create(requestContext, "Service", "ManualInterventionIdentityHandler.PopulateIdentities", 1900043))
        {
          List<ManualIntervention> list1 = manualInterventions != null ? manualInterventions.Where<ManualIntervention>((Func<ManualIntervention, bool>) (mi => mi?.Approver != null && !string.IsNullOrWhiteSpace(mi.Approver.Id))).ToList<ManualIntervention>() : (List<ManualIntervention>) null;
          List<string> list2 = list1 != null ? list1.Select<ManualIntervention, string>((Func<ManualIntervention, string>) (mi => mi.Approver.Id)).ToList<string>() : (List<string>) null;
          if (list2 == null || !list2.Any<string>())
            return;
          IdentityHelper identityHelper = IdentityHelper.GetIdentityHelper(requestContext, (ICollection<string>) list2, true);
          ManualInterventionIdentityHandler.PopulateIdentities((IList<ManualIntervention>) list1, identityHelper);
        }
      }
      catch (Exception ex)
      {
        if (!throwExceptionOnFailure)
          return;
        throw;
      }
    }

    public static void PopulateIdentities(
      IList<ManualIntervention> manualInterventions,
      IdentityHelper identityHelper)
    {
      if (identityHelper == null)
        throw new ArgumentNullException(nameof (identityHelper));
      if (manualInterventions == null || !manualInterventions.Any<ManualIntervention>())
        return;
      foreach (ManualIntervention manualIntervention in (IEnumerable<ManualIntervention>) manualInterventions)
        manualIntervention.Approver = identityHelper.GetIdentity(manualIntervention.Approver);
    }
  }
}
