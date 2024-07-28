// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Json.JRawValue
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Microsoft.AspNet.SignalR.Json
{
  internal class JRawValue : IJsonValue
  {
    private readonly string _value;

    public JRawValue(JRaw value) => this._value = value.ToString();

    public object ConvertTo(Type type)
    {
      using (StringReader reader = new StringReader(this._value))
        return JsonUtility.CreateDefaultSerializer().Deserialize((TextReader) reader, type);
    }

    public bool CanConvertTo(Type type) => true;
  }
}
