// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TriggeredTransformDefinition
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  public class TriggeredTransformDefinition : TransformDefinition
  {
    public TriggeredTransformDefinition(
      int minServiceVersion,
      int? maxServiceVersion,
      string triggerTable,
      string triggerOp,
      int transformPriority,
      string targetTable,
      string operation,
      string operationScope,
      string sprocName,
      int sprocVersion,
      string[] enableFeatureNames,
      string[] disableFeatureNames = null)
      : base(minServiceVersion, maxServiceVersion, triggerTable, triggerOp, new Guid?(), new int?(), transformPriority, targetTable, operation, operationScope, sprocName, sprocVersion, enableFeatureNames, disableFeatureNames)
    {
    }

    public TriggeredTransformDefinition(
      int minServiceVersion,
      int? maxServiceVersion,
      string triggerTable,
      string triggerOp,
      int transformPriority,
      string targetTable,
      string operation,
      string operationScope,
      string sprocName,
      int sprocVersion,
      string enableFeatureName = null,
      string disableFeatureName = null)
    {
      int minServiceVersion1 = minServiceVersion;
      int? maxServiceVersion1 = maxServiceVersion;
      string triggerTable1 = triggerTable;
      string triggerOp1 = triggerOp;
      Guid? triggerGroup = new Guid?();
      int? intervalMinutes = new int?();
      int transformPriority1 = transformPriority;
      string targetTable1 = targetTable;
      string operation1 = operation;
      string operationScope1 = operationScope;
      string sprocName1 = sprocName;
      int sprocVersion1 = sprocVersion;
      string[] enableFeatureNames;
      if (enableFeatureName == null)
        enableFeatureNames = (string[]) null;
      else
        enableFeatureNames = new string[1]
        {
          enableFeatureName
        };
      string[] disableFeatureNames;
      if (disableFeatureName == null)
        disableFeatureNames = (string[]) null;
      else
        disableFeatureNames = new string[1]
        {
          disableFeatureName
        };
      // ISSUE: explicit constructor call
      base.\u002Ector(minServiceVersion1, maxServiceVersion1, triggerTable1, triggerOp1, triggerGroup, intervalMinutes, transformPriority1, targetTable1, operation1, operationScope1, sprocName1, sprocVersion1, enableFeatureNames, disableFeatureNames);
    }

    public TriggeredTransformDefinition(
      int minServiceVersion,
      int? maxServiceVersion,
      string triggerTable,
      string triggerOp,
      Guid triggerGroup,
      int transformPriority,
      string targetTable,
      string operation,
      string operationScope,
      string sprocName,
      int sprocVersion,
      string[] enableFeatureNames,
      string[] disableFeatureNames = null)
      : base(minServiceVersion, maxServiceVersion, triggerTable, triggerOp, new Guid?(triggerGroup), new int?(), transformPriority, targetTable, operation, operationScope, sprocName, sprocVersion, enableFeatureNames, disableFeatureNames)
    {
    }

    public TriggeredTransformDefinition(
      int minServiceVersion,
      int? maxServiceVersion,
      string triggerTable,
      string triggerOp,
      Guid triggerGroup,
      int transformPriority,
      string targetTable,
      string operation,
      string operationScope,
      string sprocName,
      int sprocVersion,
      string enableFeatureName = null,
      string disableFeatureName = null)
    {
      int minServiceVersion1 = minServiceVersion;
      int? maxServiceVersion1 = maxServiceVersion;
      string triggerTable1 = triggerTable;
      string triggerOp1 = triggerOp;
      Guid? triggerGroup1 = new Guid?(triggerGroup);
      int? intervalMinutes = new int?();
      int transformPriority1 = transformPriority;
      string targetTable1 = targetTable;
      string operation1 = operation;
      string operationScope1 = operationScope;
      string sprocName1 = sprocName;
      int sprocVersion1 = sprocVersion;
      string[] enableFeatureNames;
      if (enableFeatureName == null)
        enableFeatureNames = (string[]) null;
      else
        enableFeatureNames = new string[1]
        {
          enableFeatureName
        };
      string[] disableFeatureNames;
      if (disableFeatureName == null)
        disableFeatureNames = (string[]) null;
      else
        disableFeatureNames = new string[1]
        {
          disableFeatureName
        };
      // ISSUE: explicit constructor call
      base.\u002Ector(minServiceVersion1, maxServiceVersion1, triggerTable1, triggerOp1, triggerGroup1, intervalMinutes, transformPriority1, targetTable1, operation1, operationScope1, sprocName1, sprocVersion1, enableFeatureNames, disableFeatureNames);
    }
  }
}
