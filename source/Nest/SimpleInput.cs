// Decompiled with JetBrains decompiler
// Type: Nest.SimpleInput
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class SimpleInput : 
    InputBase,
    ISimpleInput,
    IInput,
    IEnumerable<KeyValuePair<string, object>>,
    IEnumerable
  {
    public SimpleInput()
    {
    }

    public SimpleInput(IDictionary<string, object> payload)
      : this()
    {
      this.Payload = payload;
    }

    public IDictionary<string, object> Payload { get; private set; }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => this.Payload?.GetEnumerator() ?? Enumerable.Empty<KeyValuePair<string, object>>().GetEnumerator();

    public void Add(string key, object value)
    {
      if (this.Payload == null)
        this.Payload = (IDictionary<string, object>) new Dictionary<string, object>();
      this.Payload.Add(key, value);
    }

    public void Remove(string key) => this.Payload?.Remove(key);

    internal override void WrapInContainer(IInputContainer container) => container.Simple = (ISimpleInput) this;
  }
}
