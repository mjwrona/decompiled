// Decompiled with JetBrains decompiler
// Type: Nest.SimpleInputDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class SimpleInputDescriptor : ISimpleInput, IInput, IDescriptor
  {
    private IDictionary<string, object> _payload;

    public SimpleInputDescriptor()
    {
    }

    public SimpleInputDescriptor(IDictionary<string, object> payload) => this._payload = payload;

    IDictionary<string, object> ISimpleInput.Payload => this._payload;

    public SimpleInputDescriptor Add(string key, object value)
    {
      if (this._payload == null)
        this._payload = (IDictionary<string, object>) new Dictionary<string, object>();
      this._payload.Add(key, value);
      return this;
    }

    public SimpleInputDescriptor Remove(string key)
    {
      if (this._payload == null)
        return this;
      this._payload.Remove(key);
      return this;
    }
  }
}
