// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.Hubs.HubTypeExtensions
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.SignalR.Hubs
{
  internal static class HubTypeExtensions
  {
    internal static string GetHubName(this Type type) => !typeof (IHub).IsAssignableFrom(type) ? (string) null : type.GetHubAttributeName() ?? HubTypeExtensions.GetHubTypeName(type);

    internal static string GetHubAttributeName(this Type type) => !typeof (IHub).IsAssignableFrom(type) ? (string) null : ReflectionHelper.GetAttributeValue<HubNameAttribute, string>((ICustomAttributeProvider) type, (Func<HubNameAttribute, string>) (attr => attr.HubName));

    private static string GetHubTypeName(Type type)
    {
      int length = type.Name.LastIndexOf('`');
      return length == -1 ? type.Name : type.Name.Substring(0, length);
    }
  }
}
