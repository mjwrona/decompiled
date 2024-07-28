// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsLocalePropertyProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class WindowsLocalePropertyProvider : IPropertyProvider
  {
    private const string SystemLocaleRegistryPath = "SYSTEM\\CurrentControlSet\\Control\\Nls\\Language";
    private const string SystemLocaleRegistryKey = "Default";
    private readonly Lazy<CultureInfo> systemInfo;
    private readonly IRegistryTools registryTools;

    public WindowsLocalePropertyProvider(IRegistryTools regTools)
    {
      regTools.RequiresArgumentNotNull<IRegistryTools>(nameof (regTools));
      this.registryTools = regTools;
      this.systemInfo = new Lazy<CultureInfo>((Func<CultureInfo>) (() => this.InitializeSystemInformation()), false);
    }

    public void AddSharedProperties(
      List<KeyValuePair<string, object>> sharedProperties,
      TelemetryContext telemetryContext)
    {
    }

    public void PostProperties(TelemetryContext telemetryContext, CancellationToken token)
    {
      if (token.IsCancellationRequested)
        return;
      if (this.systemInfo.Value != null)
      {
        telemetryContext.PostProperty("VS.Core.Locale.System", (object) this.systemInfo.Value.EnglishName);
        if (token.IsCancellationRequested)
          return;
      }
      CultureInfo currentCulture = CultureInfo.CurrentCulture;
      telemetryContext.PostProperty("VS.Core.Locale.User", (object) currentCulture.EnglishName);
      CultureInfo currentUiCulture = CultureInfo.CurrentUICulture;
      telemetryContext.PostProperty("VS.Core.Locale.UserUI", (object) currentUiCulture.EnglishName);
    }

    private CultureInfo InitializeSystemInformation()
    {
      object keyValue = this.registryTools.GetRegistryValueFromLocalMachineRoot("SYSTEM\\CurrentControlSet\\Control\\Nls\\Language", "Default");
      if (keyValue != null && keyValue is string)
      {
        CultureInfo[] source = (CultureInfo[]) null;
        try
        {
          source = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        }
        catch (AccessViolationException ex)
        {
        }
        if (source != null)
        {
          CultureInfo cultureInfo = ((IEnumerable<CultureInfo>) source).FirstOrDefault<CultureInfo>((Func<CultureInfo, bool>) (item => item.LCID.ToString("X4", (IFormatProvider) CultureInfo.InvariantCulture) == (string) keyValue));
          if (cultureInfo != null)
            return cultureInfo;
        }
      }
      return (CultureInfo) null;
    }
  }
}
