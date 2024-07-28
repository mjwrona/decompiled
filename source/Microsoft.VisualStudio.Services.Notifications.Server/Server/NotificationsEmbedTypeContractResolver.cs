// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationsEmbedTypeContractResolver
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class NotificationsEmbedTypeContractResolver : 
    NotificationsCamelCasePropertyNamesContractResolver
  {
    protected override IList<JsonProperty> CreateProperties(
      Type type,
      MemberSerialization memberSerialization)
    {
      IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
      if (properties != null && properties.Count == 0 || type.GetCustomAttribute<NotificationsEmbedTypeAttribute>(true) == null)
        return properties;
      properties.Add(new JsonProperty()
      {
        DeclaringType = type,
        HasMemberAttribute = false,
        PropertyName = NotificationsSerialization.s_notifTypeProperty,
        PropertyType = typeof (string),
        Readable = true,
        ValueProvider = (IValueProvider) new TypeValueProvider()
        {
          Type = type
        }
      });
      return properties;
    }
  }
}
