// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.CIWorkflow.CIWorkflowAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.CIWorkflow, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 62B7B505-5C9A-4857-97F9-63D3356475DE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.CIWorkflow.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.CIWorkflow
{
  public class CIWorkflowAreaRegistration : AreaRegistration
  {
    public override string AreaName => "CIWorkflow";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea(this.AreaName, (Func<ResourceManager>) (() => CIWorkflowResources.ResourceManager), "TFS");
  }
}
