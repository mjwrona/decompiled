// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ParameterizedLazy`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ParameterizedLazy<TValue, TParameter>
  {
    private object _syncObj;
    private Func<TParameter, TValue> _valueFactory;
    private TValue _value;

    public ParameterizedLazy(TValue value)
    {
      this.IsInitialized = true;
      this._value = value;
    }

    public ParameterizedLazy(Func<TParameter, TValue> valueFactory)
    {
      this._valueFactory = valueFactory ?? throw new ArgumentNullException(nameof (valueFactory));
      this._syncObj = new object();
    }

    public bool IsInitialized { get; private set; }

    public TValue GetValue(TParameter factoryParameter)
    {
      if (!this.IsInitialized)
      {
        lock (this._syncObj)
        {
          if (!this.IsInitialized)
          {
            this._value = this._valueFactory(factoryParameter);
            this.IsInitialized = true;
          }
        }
      }
      return this._value;
    }
  }
}
