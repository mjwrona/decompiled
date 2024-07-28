// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.ReportingSqlError
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

namespace Microsoft.TeamFoundation.Reporting.DataServices.DataAccess
{
  internal static class ReportingSqlError
  {
    public const int SqlServerDefaultUserMessage = 50000;
    public const int InternalStoredProcedureError = 400017;
    public const int GenericDatabaseFailure = 1400000;
    public const int ChartConfigurationExists = 1400001;
    public const int TransformOptionsExists = 1400002;
    public const int InvalidChartConfiguration = 1400003;
    public const int TransformDoesNotExist = 1400004;
    public const int ChartDoesNotExist = 1400005;
    public const int InvalidColorConfiguration = 1400006;
    public const int TooManyChartsPerGroup = 1400007;
    public const int MAX_SQL_ERROR = 1400007;
  }
}
