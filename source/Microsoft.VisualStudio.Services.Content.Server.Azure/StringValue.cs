// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.StringValue
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class StringValue : IValue, IComparable<IValue>, IEquatable<IValue>
  {
    public StringValue(string value) => this.Value = value != null ? value : throw new InvalidOperationException("azure does not support null comparisons");

    public EdmType EdmType => EdmType.String;

    public string Value { get; private set; }

    public int CompareTo(IValue other)
    {
      if (other == null)
        return 1;
      if (this.EdmType != other.EdmType)
        throw new InvalidOperationException(string.Format("cannot compare different types {0} to {1}", (object) this.EdmType, (object) other.EdmType));
      return string.Compare(this.Value, ((StringValue) other).Value, StringComparison.Ordinal);
    }

    public bool Equals(IValue other)
    {
      if (other == null)
        return false;
      if (this.EdmType != other.EdmType)
        throw new InvalidOperationException(string.Format("cannot compare different types {0} to {1}", (object) this.EdmType, (object) other.EdmType));
      return this.Value.Equals(((StringValue) other).Value);
    }
  }
}
