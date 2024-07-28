// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheScopedNameKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  [TypeConverter(typeof (ImsCacheScopedNameKeyTypeConverter))]
  internal class ImsCacheScopedNameKey : ImsCacheKey<ScopedKey>
  {
    internal ImsCacheScopedNameKey()
    {
    }

    internal ImsCacheScopedNameKey(ScopedKey id)
      : base(id)
    {
    }

    internal override string Serialize() => this.Id.ScopeId.ToString() + (object) ',' + this.Id.Key;

    public override object Clone() => (object) new ImsCacheScopedNameKey(this.Id == null ? this.Id : (ScopedKey) this.Id.Clone());

    internal static bool TryParse(string input, out ImsCacheScopedNameKey result)
    {
      result = (ImsCacheScopedNameKey) null;
      try
      {
        string[] strArray = input.Split(new char[1]{ ',' }, 2);
        if (strArray.Length == 2)
        {
          Guid result1;
          if (Guid.TryParse(strArray[0], out result1))
          {
            result = new ImsCacheScopedNameKey(new ScopedKey(result1, strArray[1]));
            return true;
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
