// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ResourceResponse
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class ResourceResponse
  {
    public string id { get; set; }

    public string name { get; set; }

    public string type { get; set; }

    public string location { get; set; }

    public IDictionary<string, string> tags { get; set; }

    public IDictionary<string, string> properties { get; set; }

    public InsightsStatusMessageError error { get; set; }
  }
}
