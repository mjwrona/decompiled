// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence.CiEventUrlUtility
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence
{
  public static class CiEventUrlUtility
  {
    public const string CiEventEndPoint = "/_cievent";

    public static string GetUrl(CiEvent evt) => "/_cievent?" + CiEventUrlUtility.GetQueryString(evt);

    private static string GetQueryString(CiEvent evt)
    {
      if (Attribute.GetCustomAttribute((MemberInfo) evt.GetType(), typeof (ClientSupportedCiEventAttribute)) == null)
        throw new InvalidOperationException(string.Format("The CI event {0} is not supported.", (object) evt.GetType()));
      List<string> stringList = new List<string>();
      stringList.Add(CiEventUrlUtility.FormatQueryParam("area", evt.Area));
      stringList.Add(CiEventUrlUtility.FormatQueryParam("feature", evt.Feature));
      foreach (KeyValuePair<string, object> property in evt.Properties)
        stringList.Add(CiEventUrlUtility.FormatQueryParam(property.Key, property.Value.ToString()));
      return string.Join("&", stringList.ToArray());
    }

    private static string FormatQueryParam(string name, string value) => string.Format("{0}={1}", (object) name, (object) Uri.EscapeDataString(value));
  }
}
