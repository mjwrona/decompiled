// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Common.Internal.WorkItemFieldNames
// Assembly: Microsoft.TeamFoundation.TestManagement.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1401105B-6771-499A-8DF3-F3CBE1BB3AE4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.TestManagement.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [GenerateAllConstants(null)]
  public static class WorkItemFieldNames
  {
    public static readonly string Actions = "Microsoft.VSTS.TCM.Steps";
    public static readonly string ActivatedBy = "Microsoft.VSTS.Common.ActivatedBy";
    public static readonly string ActivatedDate = "Microsoft.VSTS.Common.ActivatedDate";
    public static readonly string AutomationStatus = "Microsoft.VSTS.TCM.AutomationStatus";
    public static readonly string ClosedBy = "Microsoft.VSTS.Common.ClosedBy";
    public static readonly string ClosedDate = "Microsoft.VSTS.Common.ClosedDate";
    public static readonly string DataField = "Microsoft.VSTS.TCM.LocalDataSource";
    public static readonly string Description = "System.Description";
    public static readonly string IntegrationBuild = "Microsoft.VSTS.Build.IntegrationBuild";
    public static readonly string Owner = "System.AssignedTo";
    public static readonly string Parameters = "Microsoft.VSTS.TCM.Parameters";
    public static readonly string Priority = "Microsoft.VSTS.Common.Priority";
    public static readonly string StateChangeDate = "Microsoft.VSTS.Common.StateChangeDate";
    public static readonly string Storage = "Microsoft.VSTS.TCM.AutomatedTestStorage";
    public static readonly string TestId = "Microsoft.VSTS.TCM.AutomatedTestId";
    public static readonly string TestName = "Microsoft.VSTS.TCM.AutomatedTestName";
    public static readonly string TestType = "Microsoft.VSTS.TCM.AutomatedTestType";
    public static readonly string WorkItemType = "System.WorkItemType";
    public static readonly string Title = "System.Title";
    public static readonly string State = "System.State";
    public static readonly string TestIdIsNotNull = "Microsoft.VSTS.TCM.AutomatedTestId.IsNotNull";
  }
}
