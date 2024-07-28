// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories.WorkItemStateInputModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories
{
  internal static class WorkItemStateInputModelFactory
  {
    public static WorkItemStateDeclaration ToWorkItemStateDeclaration(
      this WorkItemStateInputModel state)
    {
      return new WorkItemStateDeclaration()
      {
        Name = state.Name,
        Color = state.Color,
        StateCategory = WorkItemStateInputModelFactory.ConvertStateCategoryStringToEnum(state.StateCategory).Value,
        Order = state.Order.HasValue ? state.Order.Value : 0
      };
    }

    public static WorkItemStateCategory? ConvertStateCategoryStringToEnum(string stateCategory)
    {
      if (string.IsNullOrEmpty(stateCategory))
        return new WorkItemStateCategory?();
      StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
      if (ordinalIgnoreCase.Equals(stateCategory, "Proposed"))
        return new WorkItemStateCategory?(WorkItemStateCategory.Proposed);
      if (ordinalIgnoreCase.Equals(stateCategory, "InProgress"))
        return new WorkItemStateCategory?(WorkItemStateCategory.InProgress);
      if (ordinalIgnoreCase.Equals(stateCategory, "Resolved"))
        return new WorkItemStateCategory?(WorkItemStateCategory.Resolved);
      if (ordinalIgnoreCase.Equals(stateCategory, "Completed"))
        return new WorkItemStateCategory?(WorkItemStateCategory.Completed);
      if (ordinalIgnoreCase.Equals(stateCategory, "Removed"))
        return new WorkItemStateCategory?(WorkItemStateCategory.Removed);
      throw new ArgumentException(ResourceStrings.WorkItemStateCategoryInvalid((object) stateCategory));
    }
  }
}
