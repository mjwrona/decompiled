// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions.ActionFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DE7D0F19-C193-43CC-9602-3C8794FE9CA0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions
{
  public class ActionFactory : AbstractActionFactory
  {
    public override List<IAction> GetActions(List<ActionData> listOfActionData)
    {
      List<IAction> actions = new List<IAction>();
      foreach (ActionData actionData in listOfActionData)
      {
        if (actionData.ActionType == ActionType.CollectionFinalize)
          actions.Add((IAction) new CollectionFinalizeAction(actionData.ActionContext));
      }
      return actions;
    }
  }
}
