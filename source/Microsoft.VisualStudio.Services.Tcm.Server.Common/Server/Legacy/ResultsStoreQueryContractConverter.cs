// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.ResultsStoreQueryContractConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  public class ResultsStoreQueryContractConverter
  {
    public static Microsoft.TeamFoundation.TestManagement.Server.ResultsStoreQuery Convert(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query)
    {
      if (query == null)
        return (Microsoft.TeamFoundation.TestManagement.Server.ResultsStoreQuery) null;
      return new Microsoft.TeamFoundation.TestManagement.Server.ResultsStoreQuery()
      {
        DayPrecision = query.DayPrecision,
        QueryText = query.QueryText,
        TeamProjectName = query.TeamProjectName,
        TimeZone = query.TimeZone
      };
    }

    public static Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery Convert(
      Microsoft.TeamFoundation.TestManagement.Server.ResultsStoreQuery query)
    {
      if (query == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery()
      {
        DayPrecision = query.DayPrecision,
        QueryText = query.QueryText,
        TeamProjectName = query.TeamProjectName,
        TimeZone = query.TimeZone
      };
    }
  }
}
