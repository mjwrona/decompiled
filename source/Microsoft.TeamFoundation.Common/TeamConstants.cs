// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TeamConstants
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TeamConstants
  {
    public static readonly string TeamPropertyName = "Microsoft.TeamFoundation.Team";
    public static readonly string DefaultTeamPropertyName = "System.Microsoft.TeamFoundation.Team.Default";
    public static readonly string TeamCountProperty = "System.Microsoft.TeamFoundation.Team.Count";
    public static readonly string TeamSettingsPropertyName = TeamConstants.TeamPropertyName + ".Settings";
    public static readonly string TileZoneTileOrderPropertyName = TeamConstants.TeamPropertyName + ".TileZoneTileOrder";
    public static readonly string DefaultValueIndexPropertyName = TeamConstants.TeamSettingsPropertyName + ".DefaultValueIndex";
    public static readonly bool DefaultIncludeChildrenValue = false;
    public static readonly string TeamFieldSettingsPropertyName = TeamConstants.TeamSettingsPropertyName + ".TeamFieldSettings";
    public static readonly string TeamFieldDefaultValueIndexPropertyName = TeamConstants.TeamFieldSettingsPropertyName + ".DefaultValueIndex";
    public static readonly string TeamFieldValuesPropertyName = TeamConstants.TeamFieldSettingsPropertyName + ".TeamFieldValues";
    public const string TeamFieldValueFormat = "<ArrayOfTeamFieldValue xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><TeamFieldValue><Value>{0}</Value><IncludeChildren>true</IncludeChildren></TeamFieldValue></ArrayOfTeamFieldValue>";
    public static readonly string BacklogIterationPropertyName = TeamConstants.TeamSettingsPropertyName + ".BacklogIterationId";
    public static readonly string TeamIterationPropertyName = TeamConstants.TeamSettingsPropertyName + ".TeamIteration";
    public static readonly string TeamIterationIdPropertyNameFormat = TeamConstants.TeamIterationPropertyName + ".{0}.iterationId";
    public static readonly string TeamChartCachePropertyName = TeamConstants.TeamPropertyName + "ChartCache";
    public static readonly string WIPLimitsPropertyName = TeamConstants.TeamPropertyName + "WIPLimits";
    public static readonly string CumulativeFlowDiagramStartDate = TeamConstants.TeamPropertyName + ".CFDStartDate";
    public static readonly string CumulativeFlowDiagramHideIncoming = TeamConstants.TeamPropertyName + ".CFDHideIncoming";
    public static readonly string CalledFromTeamPlatformService = nameof (CalledFromTeamPlatformService);
  }
}
