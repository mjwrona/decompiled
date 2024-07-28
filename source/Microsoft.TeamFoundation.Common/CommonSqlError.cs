// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CommonSqlError
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class CommonSqlError
  {
    public const int DateTimeShiftDetected = 480000;
    public const int ISleepIfBusyInTransaction = 480001;
    public const int SqlFaultInjection = 480002;
  }
}
