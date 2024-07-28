// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.DefaultExperimentationFilterProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Telemetry.Services;
using Microsoft.VisualStudio.Utilities.Internal;

namespace Microsoft.VisualStudio.Experimentation
{
  public sealed class DefaultExperimentationFilterProvider : IExperimentationFilterProvider
  {
    internal const string Unknown = "unknown";
    private readonly TelemetrySession telemetrySession;
    private readonly IProcessInformationProvider processInformation;

    public DefaultExperimentationFilterProvider(TelemetrySession telemetrySession)
      : this(telemetrySession, (IProcessInformationProvider) new ProcessInformationProvider())
    {
    }

    internal DefaultExperimentationFilterProvider(
      TelemetrySession telemetrySession,
      IProcessInformationProvider processInformation)
    {
      telemetrySession.RequiresArgumentNotNull<TelemetrySession>(nameof (telemetrySession));
      this.telemetrySession = telemetrySession;
      processInformation.RequiresArgumentNotNull<IProcessInformationProvider>(nameof (processInformation));
      this.processInformation = processInformation;
    }

    public string GetFilterValue(Filters filter)
    {
      switch (filter)
      {
        case Filters.UserId:
          return this.telemetrySession.UserId.ToString("N");
        case Filters.ApplicationName:
          return this.processInformation.GetExeName() ?? "unknown";
        case Filters.ApplicationVersion:
          FileVersion processVersionInfo = this.processInformation.GetProcessVersionInfo();
          string filterValue = "unknown";
          if (processVersionInfo != null)
            filterValue = processVersionInfo.ToString();
          return filterValue;
        case Filters.IsInternal:
          return !this.telemetrySession.IsUserMicrosoftInternal ? "0" : "1";
        default:
          return string.Empty;
      }
    }
  }
}
