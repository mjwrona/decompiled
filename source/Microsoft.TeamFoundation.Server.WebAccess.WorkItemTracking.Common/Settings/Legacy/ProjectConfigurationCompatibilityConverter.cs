// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Settings.Legacy.ProjectConfigurationCompatibilityConverter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Settings.Legacy
{
  internal class ProjectConfigurationCompatibilityConverter
  {
    public static ProjectProcessConfiguration ConvertToV3(ProjectProcessConfiguration configuration)
    {
      ArgumentUtility.CheckForNull<ProjectProcessConfiguration>(configuration, nameof (configuration));
      ProjectProcessConfiguration v3 = ProjectConfigurationCompatibilityConverter.Clone(configuration);
      v3.Properties = (Property[]) null;
      return v3;
    }

    public static CommonProjectConfiguration ConvertToV1(CommonProjectConfiguration configuration)
    {
      ArgumentUtility.CheckForNull<CommonProjectConfiguration>(configuration, nameof (configuration));
      CommonProjectConfiguration v1 = ProjectConfigurationCompatibilityConverter.Clone(configuration);
      if (v1.TypeFields != null)
      {
        List<TypeField> typeFieldList = new List<TypeField>();
        foreach (TypeField typeField in v1.TypeFields)
        {
          if (typeField.Type != FieldTypeEnum.ApplicationLaunchInstructions && typeField.Type != FieldTypeEnum.ApplicationStartInformation && typeField.Type != FieldTypeEnum.ApplicationType && typeField.Type != FieldTypeEnum.FeedbackNotes)
            typeFieldList.Add(typeField);
        }
        v1.TypeFields = typeFieldList.ToArray();
        v1.FeedbackRequestWorkItems = (WorkItemCategory) null;
        v1.FeedbackResponseWorkItems = (WorkItemCategory) null;
      }
      return v1;
    }

    private static ProjectProcessConfiguration Clone(ProjectProcessConfiguration configuration) => new ProjectProcessConfiguration()
    {
      BugWorkItems = configuration.BugWorkItems,
      FeedbackRequestWorkItems = configuration.FeedbackResponseWorkItems,
      FeedbackResponseWorkItems = configuration.FeedbackResponseWorkItems,
      FeedbackWorkItems = configuration.FeedbackWorkItems,
      PortfolioBacklogs = configuration.PortfolioBacklogs,
      Properties = configuration.Properties,
      ReleaseStageWorkItems = configuration.ReleaseStageWorkItems,
      ReleaseWorkItems = configuration.ReleaseWorkItems,
      RequirementBacklog = configuration.RequirementBacklog,
      StageSignoffTaskWorkItems = configuration.StageSignoffTaskWorkItems,
      TaskBacklog = configuration.TaskBacklog,
      TestPlanWorkItems = configuration.TestPlanWorkItems,
      TestSuiteWorkItems = configuration.TestSuiteWorkItems,
      TypeFields = configuration.TypeFields,
      Weekends = configuration.Weekends,
      WorkItemColors = configuration.WorkItemColors
    };

    private static CommonProjectConfiguration Clone(CommonProjectConfiguration configuration) => new CommonProjectConfiguration()
    {
      BugWorkItems = configuration.BugWorkItems,
      FeedbackRequestWorkItems = configuration.FeedbackRequestWorkItems,
      FeedbackResponseWorkItems = configuration.FeedbackResponseWorkItems,
      FeedbackWorkItems = configuration.FeedbackWorkItems,
      RequirementWorkItems = configuration.RequirementWorkItems,
      TaskWorkItems = configuration.TaskWorkItems,
      TypeFields = configuration.TypeFields,
      Weekends = configuration.Weekends
    };
  }
}
