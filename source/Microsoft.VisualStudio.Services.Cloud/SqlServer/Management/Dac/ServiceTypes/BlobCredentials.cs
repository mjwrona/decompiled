// Decompiled with JetBrains decompiler
// Type: Microsoft.SqlServer.Management.Dac.ServiceTypes.BlobCredentials
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Dac.ServiceTypes
{
  [DebuggerStepThrough]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DataContract(Name = "BlobCredentials", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.SqlServer.Management.Dac.ServiceTypes")]
  [KnownType(typeof (BlobSharedAccessKeyCredentials))]
  [KnownType(typeof (BlobStorageAccessKeyCredentials))]
  public class BlobCredentials : IExtensibleDataObject
  {
    private ExtensionDataObject extensionDataField;
    private string UriField;

    public ExtensionDataObject ExtensionData
    {
      get => this.extensionDataField;
      set => this.extensionDataField = value;
    }

    [DataMember]
    public string Uri
    {
      get => this.UriField;
      set => this.UriField = value;
    }
  }
}
