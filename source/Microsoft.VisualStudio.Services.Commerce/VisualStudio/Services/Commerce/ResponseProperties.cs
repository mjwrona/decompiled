// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ResponseProperties
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class ResponseProperties
  {
    public TemplateLink templateLink { get; set; }

    public ParametersLink parametersLink { get; set; }

    public Parameters parameters { get; set; }

    public string mode { get; set; }

    public DebugSetting debugSetting { get; set; }

    public string provisioningState { get; set; }

    public string timestamp { get; set; }

    public string correlationId { get; set; }

    public List<Provider> providers { get; set; }

    public List<Dependency> dependencies { get; set; }
  }
}
