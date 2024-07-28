// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.DistributedTasksLibrary.DistributedTasksLibraryAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.DistributedTasksLibrary, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6468BB3E-B5A8-4A52-B7B4-DAFA38A047D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.DistributedTasksLibrary.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.DistributedTasksLibrary
{
  public class DistributedTasksLibraryAreaRegistration : AreaRegistration
  {
    private const string c_baseModulePath = "DistributedTasksCommon";

    public override string AreaName => "DistributedTasksLibrary";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea("DistributedTasksCommon", "DistributedTasksCommon/Resources", "TFS").RegisterResource(this.AreaName, (Func<ResourceManager>) (() => Microsoft.TeamFoundation.Server.WebAccess.DistributedTasksLibrary.Resources.ResourceManager));
  }
}
