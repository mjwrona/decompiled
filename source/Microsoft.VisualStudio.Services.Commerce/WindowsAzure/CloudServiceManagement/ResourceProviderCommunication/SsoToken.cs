// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication.SsoToken
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication
{
  [DataContract(Namespace = "http://schemas.microsoft.com/windowsazure")]
  [ExcludeFromCodeCoverage]
  public class SsoToken : IExtensibleDataObject
  {
    [DataMember(IsRequired = true)]
    public string Token { get; set; }

    [DataMember(IsRequired = true)]
    public string TimeStamp { get; set; }

    public ExtensionDataObject ExtensionData { get; set; }
  }
}
