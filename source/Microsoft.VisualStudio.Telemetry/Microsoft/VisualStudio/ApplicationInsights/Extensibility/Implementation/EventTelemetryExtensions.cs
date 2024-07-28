// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.EventTelemetryExtensions
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.DataContracts;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  internal static class EventTelemetryExtensions
  {
    public static bool TryGetBoolean(
      this IDictionary<string, string> properties,
      string propertyName,
      out bool result)
    {
      string str;
      if (properties.TryGetValue(propertyName, out str) && bool.TryParse(str, out result))
        return true;
      result = false;
      return false;
    }

    public static bool IsMicrosoftInternal(this EventTelemetry telemetry)
    {
      bool result = false;
      IDictionary<string, string> properties = telemetry.Properties;
      string str;
      if (properties.TryGetBoolean("Context.Default.VS.Core.User.IsVSLoginInternal", out result) & result || properties.TryGetBoolean("Context.Default.VS.Core.User.IsMicrosoftAADJoined", out result) & result || properties.TryGetBoolean("Context.Default.VS.Core.User.IsMicrosoftInternal", out result) || !properties.TryGetValue("Context.Default.VS.Core.User.Type", out str))
        return result;
      return str == "Internal" || str == "Lab";
    }
  }
}
