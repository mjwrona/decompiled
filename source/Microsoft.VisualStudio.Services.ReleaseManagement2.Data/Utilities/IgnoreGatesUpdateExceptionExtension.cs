// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.IgnoreGatesUpdateExceptionExtension
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class IgnoreGatesUpdateExceptionExtension
  {
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Should catch all exceptions")]
    public static TeamFoundationServiceException GetException(
      int releaseId,
      int gateStepId,
      GateStatus gateStatus,
      string existingIgnoredGatesJson,
      string newlyIgnoredGatesJson)
    {
      string empty = string.Empty;
      if (gateStatus != GateStatus.InProgress)
        return (TeamFoundationServiceException) new GateUpdateFailedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.GatesAreNotInProgress, (object) releaseId, (object) gateStepId));
      string message;
      try
      {
        IList<IgnoredGate> ignoredGates1 = IgnoredGatesExtension.GetIgnoredGates(newlyIgnoredGatesJson);
        IList<IgnoredGate> ignoredGates2 = IgnoredGatesExtension.GetIgnoredGates(existingIgnoredGatesJson);
        List<string> values = new List<string>();
        List<string> stringList = new List<string>();
        foreach (IgnoredGate ignoredGate1 in (IEnumerable<IgnoredGate>) ignoredGates1)
        {
          IgnoredGate gate = ignoredGate1;
          IgnoredGate ignoredGate2 = ignoredGates2.FirstOrDefault<IgnoredGate>((Func<IgnoredGate, bool>) (g => string.Equals(g.Name, gate.Name, StringComparison.OrdinalIgnoreCase)));
          if (ignoredGate2 == null || !ignoredGate2.IsProcessed)
            values.Add(gate.Name);
          else
            stringList.Add(gate.Name);
        }
        message = !stringList.Any<string>() ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.IgnoreGateUpdateFailedMessage, (object) string.Join(",", (IEnumerable<string>) values)) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.DeploymentGateAlreadyIgnored, (object) string.Join(",", (IEnumerable<string>) stringList));
      }
      catch (Exception ex)
      {
        message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.IgnoreGateUpdateFailedExceptionMessage, (object) releaseId, (object) gateStepId);
      }
      return (TeamFoundationServiceException) new GateUpdateFailedException(message);
    }
  }
}
