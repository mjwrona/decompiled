// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceReference
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class ResourceReference
  {
    protected ResourceReference()
    {
    }

    protected ResourceReference(ResourceReference referenceToCopy) => this.Name = referenceToCopy.Name;

    [DataMember(EmitDefaultValue = false)]
    [JsonConverter(typeof (ExpressionValueJsonConverter<string>))]
    public ExpressionValue<string> Name { get; set; }

    public override string ToString()
    {
      ExpressionValue<string> name = this.Name;
      if (name != (ExpressionValue<string>) null)
      {
        string literal = name.Literal;
        if (!string.IsNullOrEmpty(literal))
          return literal;
        string expression = name.Expression;
        if (!string.IsNullOrEmpty(expression))
          return expression;
      }
      return (string) null;
    }
  }
}
