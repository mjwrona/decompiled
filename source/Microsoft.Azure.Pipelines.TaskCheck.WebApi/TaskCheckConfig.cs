// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TaskCheck.WebApi.TaskCheckConfig
// Assembly: Microsoft.Azure.Pipelines.TaskCheck.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E88E420-FA63-4A56-A903-50B247686E79
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.TaskCheck.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.TaskCheck.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public sealed class TaskCheckConfig
  {
    private IDictionary<string, string> m_inputs;

    [DataMember(IsRequired = true)]
    public string DisplayName { get; set; }

    [DataMember(IsRequired = true)]
    public TaskCheckDefinitionReference DefinitionRef { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> Inputs
    {
      get
      {
        if (this.m_inputs == null)
          this.m_inputs = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_inputs;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public int? RetryInterval { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string LinkedVariableGroup { get; set; }
  }
}
