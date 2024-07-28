// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.JsonProperty
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public sealed class JsonProperty : IEquatable<JsonProperty>
  {
    public readonly string Key;
    public readonly JsonToken TokenType;
    public readonly object Value;
    public readonly JsonProperty Parent;

    public JsonProperty(JsonProperty parent, string key, JsonToken tokenType, object value)
    {
      this.Parent = parent;
      this.Key = key;
      this.TokenType = tokenType;
      this.Value = value;
    }

    public bool Equals(JsonProperty other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      if (!(this.Key == other.Key) || this.TokenType != other.TokenType || (this.Value == null ? (other.Value == null ? 1 : 0) : (this.Value.Equals(other.Value) ? 1 : 0)) == 0)
        return false;
      return this.Parent != null ? this.Parent.Equals(other.Parent) : other.Parent == null;
    }

    public override bool Equals(object obj) => this.Equals(obj as JsonProperty);

    public override int GetHashCode() => (((13 * 397 ^ (this.Key == null ? 0 : this.Key.GetHashCode())) * 397 ^ this.TokenType.GetHashCode()) * 397 ^ (this.Value == null ? 0 : this.Value.GetHashCode())) * 397 ^ (this.Parent == null ? 0 : this.Parent.GetHashCode());

    public override string ToString() => string.Format("[{0}][{1}]", (object) this.GetHashCode(), (object) this.TokenType) + string.Format("{0}: {1}:{2} (Parent:{3})", (object) (this.Key ?? "{null}"), (object) this.Value?.GetType()?.Name, this.Value ?? (object) "{null}", (object) this.Parent?.GetHashCode());
  }
}
