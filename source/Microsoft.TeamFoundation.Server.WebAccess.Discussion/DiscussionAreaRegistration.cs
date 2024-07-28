// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Discussion.DiscussionAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Discussion, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3C57FAFE-4971-4BBB-A484-416136CA3D02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Discussion.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Discussion
{
  public class DiscussionAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Discussion";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea(this.AreaName + "/Scripts", this.AreaName + "/Scripts/Resources", "TFS").RegisterResource("DiscussionLibrary", (Func<ResourceManager>) (() => DiscussionResources.ResourceManager));
  }
}
