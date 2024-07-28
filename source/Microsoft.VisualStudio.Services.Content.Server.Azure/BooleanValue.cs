// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.BooleanValue
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class BooleanValue : IValue, IComparable<IValue>, IEquatable<IValue>
  {
    public BooleanValue(bool? value) => this.Value = value;

    public EdmType EdmType => EdmType.Boolean;

    public bool? Value { get; private set; }

    public int CompareTo(IValue other)
    {
      if (other == null)
        return 1;
      if (this.EdmType != other.EdmType)
        throw new InvalidOperationException(string.Format("cannot compare different types {0} to {1}", (object) this.EdmType, (object) other.EdmType));
      BooleanValue booleanValue = (BooleanValue) other;
      if (this.Value.HasValue)
      {
        bool? nullable = booleanValue.Value;
        if (!nullable.HasValue)
          return -1;
        nullable = this.Value;
        bool flag = nullable.Value;
        ref bool local = ref flag;
        nullable = booleanValue.Value;
        int num = nullable.Value ? 1 : 0;
        return local.CompareTo(num != 0);
      }
      return booleanValue.Value.HasValue ? 1 : 0;
    }

    public bool Equals(IValue other)
    {
      if (other == null)
        return false;
      if (this.EdmType != other.EdmType)
        throw new InvalidOperationException(string.Format("cannot compare different types {0} to {1}", (object) this.EdmType, (object) other.EdmType));
      return this.Value.Equals((object) ((BooleanValue) other).Value);
    }
  }
}
