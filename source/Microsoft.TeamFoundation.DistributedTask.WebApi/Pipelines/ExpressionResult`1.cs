// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ExpressionResult`1
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ExpressionResult<T>
  {
    public ExpressionResult(T value)
      : this(value, false)
    {
    }

    public ExpressionResult(T value, bool containsSecrets)
    {
      this.ContainsSecrets = containsSecrets;
      this.Value = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public bool ContainsSecrets { get; set; }

    [DataMember]
    public T Value { get; set; }
  }
}
