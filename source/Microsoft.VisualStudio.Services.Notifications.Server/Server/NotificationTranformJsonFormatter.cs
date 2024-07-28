// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationTranformJsonFormatter
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Formatting;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class NotificationTranformJsonFormatter : JsonMediaTypeFormatter
  {
    public NotificationTranformJsonFormatter()
    {
      this.SerializerSettings.ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver();
      this.SerializerSettings.Converters.Add((JsonConverter) new StringEnumConverter()
      {
        NamingStrategy = (NamingStrategy) new CamelCaseNamingStrategy()
      });
    }
  }
}
