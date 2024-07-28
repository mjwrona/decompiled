// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.MentionActionManager
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Mention.Server
{
  public class MentionActionManager
  {
    public IEnumerable<IMentionAction> Actions { get; set; }

    public MentionActionManager(IEnumerable<IMentionAction> actions) => this.Actions = actions;

    public virtual void PerformActions(object requestContext, Microsoft.TeamFoundation.Mention.Server.Mention mention) => throw new NotImplementedException();
  }
}
