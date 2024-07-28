// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.DiagnosticTelemetry
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class DiagnosticTelemetry : IDiagnosticTelemetry
  {
    private readonly ConcurrentDictionary<string, string> registrySettings = new ConcurrentDictionary<string, string>();
    private const string RegistrySettingsPrefix = "VS.TelemetryApi.RegistrySettings.";

    public void LogRegistrySettings(string settingsName, string value)
    {
      settingsName.RequiresArgumentNotNullAndNotEmpty(nameof (settingsName));
      value.RequiresArgumentNotNullAndNotEmpty(nameof (value));
      this.registrySettings["VS.TelemetryApi.RegistrySettings." + settingsName] = value;
    }

    public void PostDiagnosticTelemetryWhenSessionInitialized(
      TelemetrySession telemetrySession,
      IEnumerable<KeyValuePair<string, object>> propertyBag)
    {
      telemetrySession.RequiresArgumentNotNull<TelemetrySession>(nameof (telemetrySession));
      propertyBag.RequiresArgumentNotNull<IEnumerable<KeyValuePair<string, object>>>(nameof (propertyBag));
      TelemetryEvent telemetryEvent = !telemetrySession.IsSessionCloned ? new TelemetryEvent("VS/TelemetryApi/Session/Initialized") : new TelemetryEvent("VS/TelemetryApi/Session/Cloned");
      telemetryEvent.Properties["vs.reliability.session.processid"] = (object) telemetrySession.ProcessPid;
      ulong? creationFileTime = NativeMethods.GetProcessCreationFileTime();
      if (creationFileTime.HasValue)
        telemetryEvent.Properties["vs.reliability.session.processstarttime"] = (object) creationFileTime.Value.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
      if (Platform.IsWindows)
      {
        string assemblyPath;
        telemetryEvent.Properties["vs.telemetryapi.binarylocation"] = DiagnosticTelemetry.ShouldHashTelemetryAssemblyPath(out assemblyPath) ? (object) new TelemetryPiiProperty((object) assemblyPath) : (object) assemblyPath;
      }
      if (this.registrySettings.Count > 0)
      {
        foreach (KeyValuePair<string, string> registrySetting in this.registrySettings)
          telemetryEvent.Properties[registrySetting.Key] = (object) registrySetting.Value;
      }
      foreach (KeyValuePair<string, object> keyValuePair in propertyBag)
        telemetryEvent.Properties[keyValuePair.Key] = keyValuePair.Value;
      telemetrySession.PostEvent(telemetryEvent);
    }

    private static bool ShouldHashTelemetryAssemblyPath(out string assemblyPath)
    {
      string lowerInvariant = Assembly.GetExecutingAssembly().Location.ToLowerInvariant();
      if (lowerInvariant.EndsWith("\\common7\\ide\\privateassemblies\\microsoft.visualstudio.telemetry.dll", StringComparison.OrdinalIgnoreCase))
      {
        assemblyPath = "PrivateAssemblies";
        return false;
      }
      assemblyPath = lowerInvariant;
      return true;
    }
  }
}
