// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.YamlPipelineProcess
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [ClientInternalUseOnly(false)]
  [DataContract]
  internal sealed class YamlPipelineProcess : PipelineProcess
  {
    [DataMember(Name = "Errors", EmitDefaultValue = false)]
    private List<string> errors;

    [JsonConstructor]
    public YamlPipelineProcess()
      : base(PipelineProcessTypes.Yaml)
    {
    }

    [DataMember]
    public string Filename { get; set; }

    [DataMember]
    public YamlFileSource FileSource { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public YamlPipelineProcessResources Resources { get; set; }

    public IList<string> Errors
    {
      get
      {
        if (this.errors == null)
          this.errors = new List<string>();
        return (IList<string>) this.errors;
      }
      set => this.errors = new List<string>((IEnumerable<string>) value);
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.FileSource?.SetSecuredObject(token, requiredPermissions);
      this.Resources?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
