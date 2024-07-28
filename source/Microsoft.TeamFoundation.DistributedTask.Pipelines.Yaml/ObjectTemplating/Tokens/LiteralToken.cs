// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.LiteralToken
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens
{
  [DataContract]
  internal sealed class LiteralToken : ScalarToken
  {
    [DataMember(Name = "lit", EmitDefaultValue = false)]
    private string m_value;

    internal LiteralToken(int? fileId, int? line, int? column, string value, ScalarStyle? style = null)
      : base(0, fileId, line, column)
    {
      this.m_value = value;
      this.Style = style;
    }

    public string Value
    {
      get
      {
        if (this.m_value == null)
          this.m_value = string.Empty;
        return this.m_value;
      }
    }

    public override string ToString() => this.m_value;

    internal override IReadOnlyTemplateToken ToReadOnly() => (IReadOnlyTemplateToken) new ReadOnlyLiteralToken(this);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      string str = this.m_value;
      if ((str != null ? (str.Length == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_value = (string) null;
    }

    [DataMember(Name = "style", EmitDefaultValue = false)]
    internal ScalarStyle? Style { get; }
  }
}
