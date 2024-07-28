// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.AcquisitionRequest.ExtensionAcquisitionRequest
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Newtonsoft.Json.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.AcquisitionRequest
{
  public class ExtensionAcquisitionRequest
  {
    public string ItemId { get; set; }

    public AcquisitionOperationType OperationType { get; set; }

    public string BillingId { get; set; }

    public int Quantity { get; set; }

    public AcquisitionAssignmentType AssignmentType { get; set; }

    public JObject Properties { get; set; }
  }
}
