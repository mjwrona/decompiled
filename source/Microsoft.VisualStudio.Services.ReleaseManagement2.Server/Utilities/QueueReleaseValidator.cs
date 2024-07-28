// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.QueueReleaseValidator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Exceptions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class QueueReleaseValidator
  {
    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Validate returning params doesn't fit well")]
    public static void Validate(
      Release serverRelease,
      int releaseEnvironmentId,
      out ReleaseEnvironment targetEnvironment)
    {
      if (serverRelease == null)
        throw new ArgumentNullException(nameof (serverRelease));
      targetEnvironment = serverRelease.Environments.SingleOrDefault<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (e => e.Id == releaseEnvironmentId));
      if (targetEnvironment == null)
        throw new ReleaseManagementObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.NotValidReleaseEnvironmentId, (object) serverRelease.Name, (object) serverRelease.Id, (object) releaseEnvironmentId));
      if (targetEnvironment.Status != ReleaseEnvironmentStatus.NotStarted)
        throw new QueueReleaseNotAllowedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.EnvironmentShouldBeInNotStartedState, (object) releaseEnvironmentId));
      int targetEnvironmentRank = targetEnvironment.Rank;
      ReleaseEnvironment releaseEnvironment = serverRelease.Environments.SingleOrDefault<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (e => e.Rank == targetEnvironmentRank - 1));
      if (releaseEnvironment == null || releaseEnvironment.Status != ReleaseEnvironmentStatus.Succeeded && releaseEnvironment.Status != ReleaseEnvironmentStatus.PartiallySucceeded)
        throw new QueueReleaseNotAllowedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.PreviousEnvironmentShouldBeSucceeded));
    }
  }
}
