// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ClientRightsQueryContext
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class ClientRightsQueryContext
  {
    public string Canary { get; set; }

    public bool IncludeCertificate { get; set; }

    public string MachineId { get; set; }

    public string ProductEdition { get; set; }

    public string ProductFamily { get; set; }

    public string ProductVersion { get; set; }

    public string ReleaseType { get; set; }
  }
}
