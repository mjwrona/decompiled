// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.DataImportModeUtils
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class DataImportModeUtils
  {
    public static bool IncludesPhase1(DataImportMode dataImportMode)
    {
      switch (dataImportMode)
      {
        case DataImportMode.Phase1Only:
        case DataImportMode.Phase2TipOobThenPhase1:
        case DataImportMode.Phase2AllOobsThenPhase1:
        case DataImportMode.Phase2TFS2012PlusOobsThenPhase1:
        case DataImportMode.Phase2ForceMatchOobThenPhase1:
        case DataImportMode.InheritedAndXML:
          return true;
        default:
          return false;
      }
    }

    public static bool IncludesPhase2(DataImportMode dataImportMode)
    {
      switch (dataImportMode)
      {
        case DataImportMode.Phase2TipOobOnly:
        case DataImportMode.Phase2AllOobsOnly:
        case DataImportMode.Phase2TipOobThenPhase1:
        case DataImportMode.Phase2AllOobsThenPhase1:
        case DataImportMode.Phase2TFS2012PlusOobsThenPhase1:
        case DataImportMode.Phase2ForceMatchOobThenPhase1:
        case DataImportMode.InheritedAndXML:
          return true;
        default:
          return false;
      }
    }
  }
}
