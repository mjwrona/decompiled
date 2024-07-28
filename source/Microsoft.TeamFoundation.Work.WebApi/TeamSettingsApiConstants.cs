// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.TeamSettingsApiConstants
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TeamSettingsApiConstants
  {
    public static readonly Guid LocationId = new Guid("c3c1012b-bea7-49d7-b45e-1664e566f84c");
    public static readonly Guid TeamFieldValuesLocationId = new Guid("07ced576-58ed-49e6-9c1e-5cb53ab8bf2a");
    public static readonly Guid IterationsLocationId = new Guid("c9175577-28a1-4b06-9197-8636af9f64ad");
    public static readonly Guid TeamDaysOffLocationId = new Guid("2d4faa2e-9150-4cbf-a47a-932b1b4a0773");
    public static readonly Guid CapacityLocationId = new Guid("74412d15-8c1a-4352-a48d-ef1ed5587d57");
    public static readonly Guid IterationCapacityLocationId = new Guid("1e385ce0-396b-4273-8171-d64562c18d37");
    public static readonly Guid IterationWorkItemsLocationId = new Guid("5b3ef1a6-d3ab-44cd-bafd-c7f45db850fa");
    public const string TeamIterationsLink = "teamIterations";
    public const string TeamIterationLink = "teamIteration";
    public const string TeamFieldValuesLink = "teamFieldValues";
    public const string TeamSettingsLink = "teamSettings";
    public const string ClassificationNodesRootAreaLink = "areaPathClassificationNodes";
    public const string SpecificClassificationNodeLink = "classificationNode";
    public const string TeamDaysOffLink = "teamDaysOff";
    public const string CapacityLink = "capacity";
    public const string IterationCapacityLink = "iterationCapacity";
    public const string IterationWorkItemsLink = "workitems";
  }
}
