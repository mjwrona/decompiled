// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Pipeline.WebAccess.PipelineWorkflow.PipelineWorkflowAreaRegistration
// Assembly: Microsoft.VisualStudio.Services.Pipeline.WebAccess.Workflow, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1E933B72-76BD-4873-B54A-5566593DDEA1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Pipeline.WebAccess.Workflow.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.VisualStudio.Services.Pipeline.WebAccess.Workflow;
using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.Pipeline.WebAccess.PipelineWorkflow
{
  public class PipelineWorkflowAreaRegistration : AreaRegistration
  {
    public override string AreaName => "PipelineWorkflow";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea("PipelineWorkflow/Scripts", "PipelineWorkflow/Scripts/Resources", "TFS").RegisterResource(this.AreaName, (Func<ResourceManager>) (() => PipelineWorkflowResources.ResourceManager));
  }
}
