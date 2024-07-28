// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions.AbstractAction
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DE7D0F19-C193-43CC-9602-3C8794FE9CA0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Actions
{
  public abstract class AbstractAction : IAction
  {
    protected AbstractAction(ActionType healthAction, ActionContext actionContext)
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), healthAction, actionContext)
    {
    }

    protected AbstractAction(
      IDataAccessFactory dataAccessFactory,
      ActionType healthAction,
      ActionContext actionContext)
    {
      this.ActionType = healthAction;
      this.ActionContext = actionContext ?? throw new ArgumentNullException(nameof (actionContext));
      this.DataAccessFactory = dataAccessFactory ?? throw new ArgumentNullException(nameof (dataAccessFactory));
    }

    internal IDataAccessFactory DataAccessFactory { get; }

    public abstract bool IsLongRunning();

    public abstract bool IsCompleted(IVssRequestContext requestContext);

    public ActionType ActionType { get; set; }

    public ActionContext ActionContext { get; set; }

    public abstract void Invoke(IVssRequestContext requestContext, out string resultMessage);
  }
}
