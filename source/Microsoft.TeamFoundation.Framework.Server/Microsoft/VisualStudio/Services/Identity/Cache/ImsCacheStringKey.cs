// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.ImsCacheStringKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  internal class ImsCacheStringKey : ImsCacheKey<string>
  {
    internal ImsCacheStringKey()
    {
    }

    internal ImsCacheStringKey(string id)
      : base(id)
    {
    }

    internal override string Serialize() => this.Id;

    public override object Clone() => (object) new ImsCacheStringKey(this.Id);

    internal static bool TryParse(string input, out ImsCacheStringKey result)
    {
      result = new ImsCacheStringKey(input);
      return true;
    }
  }
}
