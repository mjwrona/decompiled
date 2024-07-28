// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationsCamelCasePropertyNamesContractResolver
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Newtonsoft.Json.Serialization;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class NotificationsCamelCasePropertyNamesContractResolver : 
    CamelCasePropertyNamesContractResolver
  {
    protected override 
    #nullable disable
    JsonDictionaryContract CreateDictionaryContract(Type type)
    {
      JsonDictionaryContract dictionaryContract = base.CreateDictionaryContract(type);
      dictionaryContract.DictionaryKeyResolver = (Func<string, string>) (name => name);
      return dictionaryContract;
    }
  }
}
