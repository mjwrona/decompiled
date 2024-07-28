// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.RequestExtensions
// Assembly: Microsoft.AspNet.SignalR.SystemWeb, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 683C8AD1-A25F-40E2-A0AC-9A1047E77A7E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.SystemWeb.dll

using System.Web;

namespace Microsoft.AspNet.SignalR
{
  public static class RequestExtensions
  {
    public static HttpContextBase GetHttpContext(this IRequest request)
    {
      object obj;
      return request.Environment.TryGetValue(typeof (HttpContextBase).FullName, out obj) ? obj as HttpContextBase : (HttpContextBase) null;
    }
  }
}
