// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DataAccess.WidgetSqlError
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

namespace Microsoft.TeamFoundation.Dashboards.DataAccess
{
  internal static class WidgetSqlError
  {
    public const int SqlServerDefaultUserMessage = 50000;
    public const int GenericDatabaseFailure = 1600300;
    public const int WidgetExists = 1600301;
    public const int WidgetDoesNotExist = 1600302;
    public const int ETagConflict = 1600303;
    public const int WidgetConflictOnCopy = 1600304;
    public const int MAX_SQL_ERROR = 1600304;
  }
}
