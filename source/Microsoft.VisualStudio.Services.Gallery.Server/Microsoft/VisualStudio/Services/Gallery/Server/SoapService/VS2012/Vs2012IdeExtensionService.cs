// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VS2012.Vs2012IdeExtensionService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VStudio;
using System.ServiceModel.Activation;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VS2012
{
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  public class Vs2012IdeExtensionService : VsIdeExtensionService, IVs2012IdeService, IVsIdeService
  {
    public Vs2012IdeExtensionService()
      : base(VisualStudioIdeVersion.Dev11)
    {
    }
  }
}
