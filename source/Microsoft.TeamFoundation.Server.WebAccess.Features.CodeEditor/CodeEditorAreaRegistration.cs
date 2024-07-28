// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Features.CodeEditor.CodeEditorAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Features.CodeEditor, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC93D3E8-2C8E-445B-91D5-3622EF15D37B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Features.CodeEditor.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Features.CodeEditor
{
  public class CodeEditorAreaRegistration : AreaRegistration
  {
    public override string AreaName => "VssCodeEditor";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea("CodeEditor", "CodeEditor/Resources", "VSS").RegisterResource("VssCodeEditor", (Func<ResourceManager>) (() => CodeEditorResources.ResourceManager));
  }
}
