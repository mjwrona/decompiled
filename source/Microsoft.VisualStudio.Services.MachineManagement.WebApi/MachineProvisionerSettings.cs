// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineProvisionerSettings
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public class MachineProvisionerSettings
  {
    public string ImageOperatingSystem { get; set; }

    public string PackageFileUri { get; set; }

    public string ScriptFileUri { get; set; }

    public string InstallCommand { get; set; }

    public string ProvisionerVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DevFabricRelaySettings DevFabricRelaySettings { get; set; }
  }
}
