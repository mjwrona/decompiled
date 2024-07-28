// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JObjectExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class JObjectExtensions
  {
    public static bool TryGetValue<T>(this JObject container, string parameterName, out T value)
    {
      JToken jtoken;
      if (container.TryGetValue(parameterName, out jtoken))
      {
        value = jtoken.ToObject<T>();
        return true;
      }
      value = default (T);
      return false;
    }
  }
}
