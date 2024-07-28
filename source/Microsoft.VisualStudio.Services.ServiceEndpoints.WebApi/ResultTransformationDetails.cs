// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ResultTransformationDetails
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public class ResultTransformationDetails
  {
    public ResultTransformationDetails()
    {
    }

    private ResultTransformationDetails(
      ResultTransformationDetails resultTransformationDetailsToClone)
    {
      this.ResultTemplate = resultTransformationDetailsToClone.ResultTemplate;
      this.CallbackContextTemplate = resultTransformationDetailsToClone.CallbackContextTemplate;
      this.CallbackRequiredTemplate = resultTransformationDetailsToClone.CallbackRequiredTemplate;
    }

    [DataMember(EmitDefaultValue = false)]
    public string ResultTemplate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string CallbackContextTemplate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string CallbackRequiredTemplate { get; set; }

    public ResultTransformationDetails Clone() => new ResultTransformationDetails(this);
  }
}
