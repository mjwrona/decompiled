// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication.ResourceOutput
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication
{
  [DataContract(Name = "Resource", Namespace = "http://schemas.microsoft.com/windowsazure")]
  [ExcludeFromCodeCoverage]
  public class ResourceOutput : IExtensibleDataObject
  {
    [DataMember(Order = 1)]
    public CloudServiceSettings CloudServiceSettings { get; set; }

    [DataMember(Order = 2)]
    public string ETag { get; set; }

    [DataMember(Order = 3)]
    public XmlNode[] IntrinsicSettings { get; set; }

    [DataMember(Order = 4)]
    public string Name { get; set; }

    [DataMember(Order = 5)]
    public OperationStatus OperationStatus { get; set; }

    [DataMember(Order = 6)]
    public OutputItemList OutputItems { get; set; }

    [DataMember(Order = 7)]
    public string Plan { get; set; }

    [DataMember(Order = 8)]
    public string PromotionCode { get; set; }

    [DataMember(Order = 9)]
    public string SchemaVersion { get; set; }

    [DataMember(Order = 10)]
    public string State { get; set; }

    [DataMember(Order = 11)]
    public string SubState { get; set; }

    [DataMember(Order = 12)]
    public string Type { get; set; }

    [DataMember(Order = 13)]
    public UsageMeterCollection UsageMeters { get; set; }

    [DataMember(Order = 14)]
    public string Label { get; set; }

    public ExtensionDataObject ExtensionData { get; set; }
  }
}
