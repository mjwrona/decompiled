// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DistributedTask.WebAccess.TaskGroup.TaskGroupAreaRegistration
// Assembly: Microsoft.VisualStudio.Services.DistributedTask.WebAccess.TaskGroup, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DF5615EA-8DE2-4CBC-82B1-CDC63F632564
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.DistributedTask.WebAccess.TaskGroup.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.DistributedTask.WebAccess.TaskGroup
{
  public class TaskGroupAreaRegistration : AreaRegistration
  {
    public override string AreaName => "TaskGroup";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea("TaskGroup/Scripts", "TaskGroup/Scripts/Resources", "TFS").RegisterResource(this.AreaName, (Func<ResourceManager>) (() => TaskGroupResources.ResourceManager));
  }
}
