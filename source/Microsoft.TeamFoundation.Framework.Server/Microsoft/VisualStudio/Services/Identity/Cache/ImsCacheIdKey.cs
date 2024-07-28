// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheIdKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  [TypeConverter(typeof (ImsCacheIdKeyTypeConverter))]
  internal class ImsCacheIdKey : ImsCacheKey<Guid>
  {
    internal ImsCacheIdKey()
    {
    }

    internal ImsCacheIdKey(Guid id)
      : base(id)
    {
    }

    internal override string Serialize() => this.Id.ToString();

    public override object Clone() => (object) new ImsCacheIdKey(this.Id);

    internal static bool TryParse(string input, out ImsCacheIdKey result)
    {
      result = (ImsCacheIdKey) null;
      Guid result1;
      if (!Guid.TryParse(input, out result1))
        return false;
      result = new ImsCacheIdKey(result1);
      return true;
    }
  }
}
