// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DataImport.DataImportStatusMessage
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.DataImport
{
  [DataContract]
  [ServiceEventObject]
  public class DataImportStatusMessage
  {
    [DataMember]
    public DateTime MessageCreated { get; set; }

    [DataMember]
    public Guid ImportId { get; set; }

    [DataMember]
    public string Service { get; set; }

    [DataMember]
    public string StepName { get; set; }

    [DataMember]
    public string GroupName { get; set; }

    [DataMember]
    public string OperationName { get; set; }

    [DataMember]
    public int CurrentStepNumber { get; set; }

    [DataMember]
    public int TotalStepNumber { get; set; }

    [DataMember]
    public string InfoMessage { get; set; }

    [DataMember]
    public string ErrorMessage { get; set; }
  }
}
