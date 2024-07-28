// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.EnvironmentVariableHelper
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public static class EnvironmentVariableHelper
  {
    public static void SetMachineEnvVarIfNeeded(string variable, string value)
    {
      if (string.Equals(Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine), value, StringComparison.OrdinalIgnoreCase))
        return;
      Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.Machine);
    }
  }
}
