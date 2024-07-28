// Decompiled with JetBrains decompiler
// Type: Azure.Core.AppContextSwitchHelper
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;


#nullable enable
namespace Azure.Core
{
  internal static class AppContextSwitchHelper
  {
    public static bool GetConfigValue(string appContexSwitchName, string environmentVariableName)
    {
      bool isEnabled;
      if (AppContext.TryGetSwitch(appContexSwitchName, out isEnabled))
        return isEnabled;
      string environmentVariable = Environment.GetEnvironmentVariable(environmentVariableName);
      return environmentVariable != null && (environmentVariable.Equals("true", StringComparison.OrdinalIgnoreCase) || environmentVariable.Equals("1"));
    }
  }
}
