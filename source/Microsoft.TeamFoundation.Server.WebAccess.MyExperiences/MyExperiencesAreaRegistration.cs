// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.MyExperiences.MyExperiencesAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.MyExperiences, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD340A61-D28F-4435-96FD-F6CA1BCEA981
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.MyExperiences.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.MyExperiences
{
  public class MyExperiencesAreaRegistration : AreaRegistration
  {
    public override string AreaName => "MyExperiences";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea(this.AreaName, (Func<ResourceManager>) (() => MyExperiencesResources.ResourceManager), "TFS");
  }
}
