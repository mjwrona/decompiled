// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.MicrosoftAjaxConfig
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class MicrosoftAjaxConfig : WebSdkMetadata
  {
    private ClientCultureInfo m_clientCultureInfo;

    internal ClientCultureInfo ClientCultureInfo
    {
      get
      {
        if (this.m_clientCultureInfo == null)
          this.m_clientCultureInfo = ClientCultureInfo.GetClientCulture(System.Globalization.CultureInfo.CurrentCulture);
        return this.m_clientCultureInfo;
      }
    }

    [DataMember]
    public JObject CultureInfo => this.ClientCultureInfo.ToJson();
  }
}
