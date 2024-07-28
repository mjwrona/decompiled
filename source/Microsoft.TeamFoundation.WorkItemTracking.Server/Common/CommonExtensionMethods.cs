// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.CommonExtensionMethods
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  internal static class CommonExtensionMethods
  {
    public static void SetNullableBool(this SqlDataRecord record, int ordinal, bool? value)
    {
      if (value.HasValue)
        record.SetBoolean(ordinal, value.Value);
      else
        record.SetDBNull(ordinal);
    }

    public static void SetNullableString(SqlDataRecord record, int ordinal, string value)
    {
      if (value != null)
        record.SetString(ordinal, value);
      else
        record.SetDBNull(ordinal);
    }
  }
}
