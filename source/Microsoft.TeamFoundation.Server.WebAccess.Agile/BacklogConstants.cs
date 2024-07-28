// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.BacklogConstants
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  [GenerateAllConstants(null)]
  public static class BacklogConstants
  {
    public const string SprintForecastVelocity = "AgileBacklog.SprintForecastVelocity";
    public const int DefaultSprintForecastVelocity = 10;
    public const string ShowForecastFilter = "AgileBacklog.ShowForecastFilter";
    public const string ActionNameIteration = "Iteration";
    public const int NumberOfIterationsInVelocity = 5;
    public const string ShowInProgressFilter = "AgileBacklog.ShowInProgressFilter";
    public const string ShowCompletedChildItemsFilter = "AgileBacklog.ShowCompletedChildItemsFilter";
    public const string ParentColumnRefName = "System.Backlog.Parent";
  }
}
