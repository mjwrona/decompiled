// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.BuiltInExtensions.CodeEditor.CodeEditorAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.BuiltInExtensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AFF996E-AF68-479F-B1B4-F4F26C62129C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.BuiltInExtensions.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.BuiltInExtensions.CodeEditor
{
  public class CodeEditorAreaRegistration : AreaRegistration
  {
    public override string AreaName => "CodeEditor";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterArea("BuiltInExtensions/Scripts", "BuiltInExtensions/Scripts/Resources").RegisterResource("CodeEditor", (Func<ResourceManager>) (() => CodeEditorResources.ResourceManager));
  }
}
