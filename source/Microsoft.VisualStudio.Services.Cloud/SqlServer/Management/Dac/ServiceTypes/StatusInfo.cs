// Decompiled with JetBrains decompiler
// Type: Microsoft.SqlServer.Management.Dac.ServiceTypes.StatusInfo
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.SqlServer.Management.Dac.ServiceTypes
{
  [DebuggerStepThrough]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DataContract(Name = "StatusInfo", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.SqlServer.Management.Dac.ServiceTypes")]
  public class StatusInfo : IExtensibleDataObject
  {
    private ExtensionDataObject extensionDataField;
    private string BlobUriField;
    private string DatabaseNameField;
    private string ErrorMessageField;
    private DateTime LastModifiedTimeField;
    private DateTime QueuedTimeField;
    private string RequestIdField;
    private string RequestTypeField;
    private string ServerNameField;
    private string StatusField;

    public ExtensionDataObject ExtensionData
    {
      get => this.extensionDataField;
      set => this.extensionDataField = value;
    }

    [DataMember]
    public string BlobUri
    {
      get => this.BlobUriField;
      set => this.BlobUriField = value;
    }

    [DataMember]
    public string DatabaseName
    {
      get => this.DatabaseNameField;
      set => this.DatabaseNameField = value;
    }

    [DataMember]
    public string ErrorMessage
    {
      get => this.ErrorMessageField;
      set => this.ErrorMessageField = value;
    }

    [DataMember]
    public DateTime LastModifiedTime
    {
      get => this.LastModifiedTimeField;
      set => this.LastModifiedTimeField = value;
    }

    [DataMember]
    public DateTime QueuedTime
    {
      get => this.QueuedTimeField;
      set => this.QueuedTimeField = value;
    }

    [DataMember]
    public string RequestId
    {
      get => this.RequestIdField;
      set => this.RequestIdField = value;
    }

    [DataMember]
    public string RequestType
    {
      get => this.RequestTypeField;
      set => this.RequestTypeField = value;
    }

    [DataMember]
    public string ServerName
    {
      get => this.ServerNameField;
      set => this.ServerNameField = value;
    }

    [DataMember]
    public string Status
    {
      get => this.StatusField;
      set => this.StatusField = value;
    }
  }
}
