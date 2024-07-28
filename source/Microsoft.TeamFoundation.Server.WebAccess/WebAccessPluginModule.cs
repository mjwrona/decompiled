// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WebAccessPluginModule
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class WebAccessPluginModule : WebSdkMetadata
  {
    [DataMember(Name = "namespace")]
    public string @namespace;
    [DataMember(Name = "loadAfter")]
    public string loadAfter;
    private Func<TfsWebContext, bool> m_isPluginEnabledFunc;

    public WebAccessPluginModule()
    {
    }

    public WebAccessPluginModule(
      string moduleNamespace,
      string loadAfterModule,
      Func<TfsWebContext, bool> isEnabledFunc)
    {
      this.@namespace = moduleNamespace;
      this.loadAfter = loadAfterModule;
      this.m_isPluginEnabledFunc = isEnabledFunc;
    }

    public bool IsEnabled(TfsWebContext webContext) => this.m_isPluginEnabledFunc == null || this.m_isPluginEnabledFunc(webContext);
  }
}
