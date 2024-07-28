// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.CisDictionary
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [Serializable]
  public class CisDictionary : Hashtable
  {
    public CisDictionary()
    {
    }

    protected CisDictionary(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public override object this[object index]
    {
      get => index is string ? base[(object) ((string) index).ToLower(CultureInfo.InvariantCulture)] : base[index];
      set
      {
        if (index is string)
          base[(object) ((string) index).ToLower(CultureInfo.InvariantCulture)] = value;
        else
          base[index] = value;
      }
    }

    public override bool Contains(object key) => this.ContainsKey(key);

    public override bool ContainsKey(object key) => key is string ? base.ContainsKey((object) ((string) key).ToLower(CultureInfo.InvariantCulture)) : base.ContainsKey(key);

    public override void Add(object key, object value)
    {
      if (key is string)
        base.Add((object) ((string) key).ToLower(CultureInfo.InvariantCulture), value);
      else
        base.Add(key, value);
    }
  }
}
