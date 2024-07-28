// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.AcceptNavigationLevelsAttribute
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Reflection;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public sealed class AcceptNavigationLevelsAttribute : ActionMethodSelectorAttribute
  {
    public AcceptNavigationLevelsAttribute(NavigationContextLevels levels) => this.Levels = levels;

    public NavigationContextLevels Levels { get; set; }

    public override bool IsValidForRequest(
      ControllerContext controllerContext,
      MethodInfo methodInfo)
    {
      return this.Levels.HasFlag((Enum) NavigationHelpers.CalculateTopMostLevel(controllerContext.RequestContext));
    }
  }
}
