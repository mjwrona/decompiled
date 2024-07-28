// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.MachinePoolDefaults
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.Build.Common
{
  public static class MachinePoolDefaults
  {
    public static KeyValuePair<string, string>[] DefaultProperties = new KeyValuePair<string, string>[11]
    {
      new KeyValuePair<string, string>("Description", string.Empty),
      new KeyValuePair<string, string>("CleanupPolicy", "DeleteProfile"),
      new KeyValuePair<string, string>("DisconnectTimeout", 900000000L.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
      new KeyValuePair<string, string>("IdleTimeout", 60000000L.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
      new KeyValuePair<string, string>("IsDefault", false.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
      new KeyValuePair<string, string>("LeaseTimeout", 3600000000L.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
      new KeyValuePair<string, string>("MaxQueuedDuration", 300000000L.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
      new KeyValuePair<string, string>("MinRunDuration", 10000000L.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
      new KeyValuePair<string, string>("ServiceLevel", 0.99.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
      new KeyValuePair<string, string>("StatusReportInterval", 900000000L.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
      new KeyValuePair<string, string>("TargetQueuedDuration", 10000000L.ToString((IFormatProvider) CultureInfo.InvariantCulture))
    };
    public const long DefaultLeaseTimeout = 3600000000;
    public const long DefaultStatusReportInterval = 900000000;
    public const long DefaultIdleTimeout = 60000000;
    public const long DefaultMinRunDuration = 10000000;
    public const long DefaultMaxQueuedDuration = 300000000;
    public const long DefaultTargetQueuedDuration = 10000000;
    public const double DefaultServiceLevel = 0.99;
    public const long DefaultDisconnectionTimeout = 900000000;
    private const long MicrosecondsPerSecond = 1000000;
  }
}
