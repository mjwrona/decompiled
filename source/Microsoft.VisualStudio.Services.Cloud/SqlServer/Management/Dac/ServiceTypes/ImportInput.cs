// Decompiled with JetBrains decompiler
// Type: Microsoft.SqlServer.Management.Dac.ServiceTypes.ImportInput
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
  [DataContract(Name = "ImportInput", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.SqlServer.Management.Dac.ServiceTypes")]
  public class ImportInput : IExtensibleDataObject
  {
    private ExtensionDataObject extensionDataField;
    private string AzureEditionField;
    private BlobCredentials BlobCredentialsField;
    private ConnectionInfo ConnectionInfoField;
    private int DatabaseSizeInGBField;
    private string ServiceObjectiveField;

    public ExtensionDataObject ExtensionData
    {
      get => this.extensionDataField;
      set => this.extensionDataField = value;
    }

    [DataMember]
    public string AzureEdition
    {
      get => this.AzureEditionField;
      set => this.AzureEditionField = value;
    }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public BlobCredentials BlobCredentials
    {
      get => this.BlobCredentialsField;
      set => this.BlobCredentialsField = value;
    }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public ConnectionInfo ConnectionInfo
    {
      get => this.ConnectionInfoField;
      set => this.ConnectionInfoField = value;
    }

    [DataMember]
    public int DatabaseSizeInGB
    {
      get => this.DatabaseSizeInGBField;
      set => this.DatabaseSizeInGBField = value;
    }

    [DataMember]
    public string ServiceObjective
    {
      get => this.ServiceObjectiveField;
      set => this.ServiceObjectiveField = value;
    }
  }
}
