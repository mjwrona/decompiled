// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ConfigurationContextPaths
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Runtime.Serialization;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class ConfigurationContextPaths : WebSdkMetadata
  {
    public ConfigurationContextPaths()
      : this((IVssRequestContext) null)
    {
    }

    public ConfigurationContextPaths(IVssRequestContext vssRequestContext)
    {
      this.RootPath = VirtualPathUtility.ToAbsolute("~/");
      this.StaticContentVersion = StaticResources.Versioned.Version;
      this.ResourcesPath = StaticResources.Versioned.Content.GetLocation(string.Empty, vssRequestContext);
      this.StaticRootTfs = StaticResources.Versioned.GetLocation(string.Empty, vssRequestContext);
      this.CdnFallbackStaticRootTfs = StaticResources.Versioned.GetLocalLocation(string.Empty, vssRequestContext);
      this.StaticRoot3rdParty = StaticResources.ThirdParty.GetLocation(string.Empty, vssRequestContext);
      this.StaticContentRootPath = vssRequestContext != null ? vssRequestContext.WebApplicationPath() : this.RootPath;
    }

    [DataMember]
    public string RootPath { get; private set; }

    [DataMember]
    public string StaticContentRootPath { get; private set; }

    [DataMember]
    public string StaticContentVersion { get; private set; }

    [DataMember]
    public string ResourcesPath { get; private set; }

    [DataMember]
    public string StaticRootTfs { get; private set; }

    [DataMember]
    public string CdnFallbackStaticRootTfs { get; private set; }

    [DataMember]
    public string StaticRoot3rdParty { get; private set; }
  }
}
