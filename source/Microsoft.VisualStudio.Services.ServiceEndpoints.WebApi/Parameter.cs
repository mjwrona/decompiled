// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.Parameter
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [DataContract]
  public class Parameter
  {
    public Parameter()
    {
    }

    public Parameter(Parameter parameter)
      : this(parameter.Value, parameter.IsSecret)
    {
    }

    public Parameter(string value, bool isSecret)
    {
      this.Value = value;
      this.IsSecret = isSecret;
    }

    [DataMember(EmitDefaultValue = true)]
    public string Value { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool IsSecret { get; set; }

    public static implicit operator Parameter(string value) => new Parameter(value, true);
  }
}
