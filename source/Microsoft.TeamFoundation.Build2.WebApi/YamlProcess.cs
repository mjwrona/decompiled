// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.YamlProcess
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class YamlProcess : BuildProcess
  {
    [DataMember(Name = "Errors", EmitDefaultValue = false)]
    private List<string> m_errors;

    public YamlProcess()
      : this((ISecuredObject) null)
    {
    }

    internal YamlProcess(ISecuredObject securedObject)
      : base(2, securedObject)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public BuildProcessResources Resources { get; set; }

    public IList<string> Errors
    {
      get
      {
        if (this.m_errors == null)
          this.m_errors = new List<string>();
        return (IList<string>) this.m_errors;
      }
      set => this.m_errors = new List<string>((IEnumerable<string>) value);
    }

    [DataMember]
    public string YamlFilename { get; set; }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      List<string> errors = this.m_errors;
      // ISSUE: explicit non-virtual call
      if ((errors != null ? (__nonvirtual (errors.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_errors = (List<string>) null;
    }
  }
}
