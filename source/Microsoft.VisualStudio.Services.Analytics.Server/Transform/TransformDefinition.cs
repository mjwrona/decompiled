// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TransformDefinition
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  public abstract class TransformDefinition
  {
    public int MinServiceVersion { get; }

    public int? MaxServiceVersion { get; }

    public string TriggerTable { get; }

    public string TriggerOperation { get; }

    public Guid? TriggerGroup { get; }

    public int? IntervalMinutes { get; }

    public int TransformPriority { get; }

    public string TargetTable { get; }

    public string Operation { get; }

    public string SprocName { get; }

    public int SprocVersion { get; }

    public string OperationScope { get; }

    public List<string> EnableWithFeatureNames { get; }

    public List<string> DisableWithFeatureNames { get; }

    protected TransformDefinition(
      int minServiceVersion,
      int? maxServiceVersion,
      string triggerTable,
      string triggerOp,
      Guid? triggerGroup,
      int? intervalMinutes,
      int transformPriority,
      string targetTable,
      string operation,
      string operationScope,
      string sprocName,
      int sprocVersion,
      string[] enableFeatureNames,
      string[] disableFeatureNames = null)
    {
      this.MinServiceVersion = minServiceVersion;
      this.MaxServiceVersion = maxServiceVersion;
      this.TriggerTable = triggerTable;
      this.TriggerOperation = triggerOp;
      this.TriggerGroup = triggerGroup;
      this.IntervalMinutes = intervalMinutes;
      this.TransformPriority = transformPriority;
      this.TargetTable = targetTable;
      this.Operation = operation;
      this.OperationScope = operationScope;
      this.SprocName = sprocName;
      this.SprocVersion = sprocVersion;
      if ((enableFeatureNames != null ? enableFeatureNames.Length : 0) > 0)
        this.EnableWithFeatureNames = new List<string>((IEnumerable<string>) enableFeatureNames);
      if ((disableFeatureNames != null ? disableFeatureNames.Length : 0) <= 0)
        return;
      this.DisableWithFeatureNames = new List<string>((IEnumerable<string>) disableFeatureNames);
    }
  }
}
