// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineConfiguration
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class MachineConfiguration
  {
    public MachineConfiguration(X509Certificate2 settings, string url)
      : this(settings)
    {
      this.Url = url;
    }

    public MachineConfiguration(X509Certificate2 settings)
    {
      this.Data = settings.Export(X509ContentType.Pfx);
      this.Thumbprint = settings.Thumbprint;
      this.ThumbprintAlgorithm = "sha1";
    }

    public MachineConfiguration(string json, X509Certificate2 settings)
      : this(settings)
    {
      this.SettingsJSON = json;
    }

    public MachineConfiguration()
    {
    }

    [DataMember(IsRequired = true)]
    public byte[] Data { get; set; }

    [DataMember(IsRequired = true)]
    public string Thumbprint { get; set; }

    [DataMember(IsRequired = true)]
    public string ThumbprintAlgorithm { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string SettingsJSON { get; set; }

    [DataMember(IsRequired = true)]
    public string Url { get; set; }
  }
}
