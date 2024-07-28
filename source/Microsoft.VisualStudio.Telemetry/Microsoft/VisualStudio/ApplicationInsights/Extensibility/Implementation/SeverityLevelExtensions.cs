// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.SeverityLevelExtensions
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  internal static class SeverityLevelExtensions
  {
    public static Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel? TranslateSeverityLevel(
      this Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel? sdkSeverityLevel)
    {
      if (!sdkSeverityLevel.HasValue)
        return new Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel?();
      switch (sdkSeverityLevel.Value)
      {
        case Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel.Information:
          return new Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel?(Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel.Information);
        case Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel.Warning:
          return new Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel?(Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel.Warning);
        case Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel.Error:
          return new Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel?(Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel.Error);
        case Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel.Critical:
          return new Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel?(Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel.Critical);
        default:
          return new Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel?(Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel.Verbose);
      }
    }

    public static Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel? TranslateSeverityLevel(
      this Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel? dataPlatformSeverityLevel)
    {
      if (!dataPlatformSeverityLevel.HasValue)
        return new Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel?();
      switch (dataPlatformSeverityLevel.Value)
      {
        case Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel.Information:
          return new Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel?(Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel.Information);
        case Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel.Warning:
          return new Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel?(Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel.Warning);
        case Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel.Error:
          return new Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel?(Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel.Error);
        case Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel.Critical:
          return new Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel?(Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel.Critical);
        default:
          return new Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel?(Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SeverityLevel.Verbose);
      }
    }
  }
}
