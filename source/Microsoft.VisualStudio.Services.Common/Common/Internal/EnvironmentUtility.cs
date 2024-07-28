// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Internal.EnvironmentUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.Win32;

namespace Microsoft.VisualStudio.Services.Common.Internal
{
  public static class EnvironmentUtility
  {
    public static void SetMachineEnvironmentVariable(string variable, string value)
    {
      EnvironmentUtility.CheckEnvironmentVariableName(variable);
      using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Session Manager\\Environment", true))
      {
        if (registryKey == null)
          return;
        if (value == null)
          registryKey.DeleteValue(variable, false);
        else
          registryKey.SetValue(variable, (object) value);
      }
    }

    private static void CheckEnvironmentVariableName(string variable)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(variable, nameof (variable));
      ArgumentUtility.CheckStringLength(variable, nameof (variable), 1024);
    }
  }
}
