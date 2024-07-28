// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.DistributedTask.DistributedTaskAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.DistributedTask, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C76C69E8-B1FC-4B4A-9692-7C4D17856C34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.DistributedTask.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.DistributedTask
{
  public class DistributedTaskAreaRegistration : AreaRegistration
  {
    public override string AreaName => "DistributedTask";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea("DistributedTask/Scripts", "DistributedTask/Scripts/Resources", "TFS").RegisterResource(this.AreaName, (Func<ResourceManager>) (() => DistributedTaskResources.ResourceManager));
  }
}
