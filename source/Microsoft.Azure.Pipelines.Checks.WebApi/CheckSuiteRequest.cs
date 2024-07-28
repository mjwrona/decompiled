// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.CheckSuiteRequest
// Assembly: Microsoft.Azure.Pipelines.Checks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 381241F9-9196-42AF-BB4C-5187E3EFE32E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.WebApi.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Checks.WebApi
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CheckSuiteRequest
  {
    private List<Resource> m_resources;

    [DataMember(EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<Resource> Resources
    {
      get => this.m_resources ?? (this.m_resources = new List<Resource>());
      set => this.m_resources = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public JObject Context { get; set; }
  }
}
