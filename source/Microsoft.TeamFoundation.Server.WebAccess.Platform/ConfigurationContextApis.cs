// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ConfigurationContextApis
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Configuration;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class ConfigurationContextApis : WebSdkMetadata
  {
    public ConfigurationContextApis()
    {
      this.AreaPrefix = "_";
      this.ControllerPrefix = "_";
      this.WebApiVersion = ConfigurationManager.AppSettings["webApiVersion"];
      if (!string.IsNullOrEmpty(this.WebApiVersion))
        return;
      this.WebApiVersion = "1";
    }

    [DataMember]
    public string WebApiVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string AreaPrefix { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ControllerPrefix { get; set; }
  }
}
