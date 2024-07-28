// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Features.Preview.PreviewAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Features.Preview, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 62B18CCA-729B-4701-ABEA-C59682320C65
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Features.Preview.dll

using System;
using System.Resources;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Features.Preview
{
  public class PreviewAreaRegistration : AreaRegistration
  {
    public override string AreaName => "VSSPreview";

    public override void RegisterArea(AreaRegistrationContext context) => ScriptRegistration.RegisterBundledArea("Preview", "VSSPreview/Resources", "VSS").RegisterResource("Preview", (Func<ResourceManager>) (() => PreviewResources.ResourceManager));
  }
}
