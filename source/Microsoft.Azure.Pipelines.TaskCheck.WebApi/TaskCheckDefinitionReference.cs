// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.TaskCheck.WebApi.TaskCheckDefinitionReference
// Assembly: Microsoft.Azure.Pipelines.TaskCheck.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E88E420-FA63-4A56-A903-50B247686E79
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.TaskCheck.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.TaskCheck.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public sealed class TaskCheckDefinitionReference
  {
    [DataMember(IsRequired = true)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = true)]
    public string Version { get; set; }
  }
}
