// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheScopedIdKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  [TypeConverter(typeof (ImsCacheScopedIdKeyTypeConverter))]
  internal class ImsCacheScopedIdKey : ImsCacheKey<ScopedId>
  {
    internal ImsCacheScopedIdKey()
    {
    }

    internal ImsCacheScopedIdKey(ScopedId id)
      : base(id)
    {
    }

    internal override string Serialize()
    {
      Guid guid = this.Id.ScopeId;
      string str1 = guid.ToString();
      // ISSUE: variable of a boxed type
      __Boxed<char> local = (ValueType) ',';
      guid = this.Id.Id;
      string str2 = guid.ToString();
      return str1 + (object) local + str2;
    }

    public override object Clone() => (object) new ImsCacheScopedIdKey((ScopedId) this.Id?.Clone());

    internal static bool TryParse(string input, out ImsCacheScopedIdKey result)
    {
      result = (ImsCacheScopedIdKey) null;
      try
      {
        string[] strArray = input.Split(new char[1]{ ',' }, 2);
        if (strArray.Length == 2)
        {
          Guid result1;
          if (Guid.TryParse(strArray[0], out result1))
          {
            Guid result2;
            if (Guid.TryParse(strArray[1], out result2))
            {
              result = new ImsCacheScopedIdKey(new ScopedId(result1, result2));
              return true;
            }
          }
        }
      }
      catch (Exception ex)
      {
        return false;
      }
      return false;
    }
  }
}
