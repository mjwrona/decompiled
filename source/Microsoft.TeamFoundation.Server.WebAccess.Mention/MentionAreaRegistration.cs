// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Mention.MentionAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Mention, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EBF937EB-CE65-404A-9E2A-85B6514F6A76
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Mention.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Mention
{
  public class MentionAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Mention";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea(this.AreaName, (Func<ResourceManager>) (() => MentionResources.ResourceManager), "TFS");
  }
}
