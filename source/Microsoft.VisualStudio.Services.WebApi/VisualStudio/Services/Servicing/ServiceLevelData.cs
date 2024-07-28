// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Servicing.ServiceLevelData
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Servicing
{
  [DataContract]
  public class ServiceLevelData
  {
    [DataMember(Order = 0, EmitDefaultValue = false, IsRequired = false)]
    public string ServicingAreas { get; set; }

    [DataMember(Order = 10)]
    public string ConfigurationDatabaseServiceLevel { get; set; }

    [DataMember(Order = 20)]
    public string DeploymentHostServiceLevel { get; set; }

    [DataMember(Order = 30, EmitDefaultValue = false, IsRequired = false)]
    public string AccountDatabaseServiceLevel { get; set; }

    [DataMember(Order = 40, EmitDefaultValue = false, IsRequired = false)]
    public string AccountHostServiceLevel { get; set; }

    [DataMember(Order = 50, EmitDefaultValue = false, IsRequired = false)]
    public string CollectionDatabaseServiceLevel { get; set; }

    [DataMember(Order = 60, EmitDefaultValue = false, IsRequired = false)]
    public string CollectionHostServiceLevel { get; set; }

    [DataMember(Order = 70, EmitDefaultValue = false, IsRequired = false)]
    public string BuildServiceLevel { get; set; }
  }
}
