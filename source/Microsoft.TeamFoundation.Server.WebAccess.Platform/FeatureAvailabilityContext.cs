// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.FeatureAvailabilityContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class FeatureAvailabilityContext : WebSdkMetadata
  {
    private WebContext m_webContext;

    public FeatureAvailabilityContext(WebContext webContext, bool populateFrameworkFlags)
    {
      this.m_webContext = webContext;
      if (!populateFrameworkFlags)
        return;
      this.IncludeFeatureFlags((IEnumerable<string>) WebPlatformBootstrapFeatureFlags.FeatureFlagNames);
    }

    public void IncludeFeatureFlag(string featureFlag) => this.IncludeFeatureFlags((IEnumerable<string>) new string[1]
    {
      featureFlag
    });

    public void IncludeFeatureFlags(IEnumerable<string> featureFlags)
    {
      if (this.m_webContext == null)
        return;
      if (this.FeatureStates == null)
        this.FeatureStates = new Dictionary<string, bool>();
      foreach (string featureFlag in featureFlags)
        this.FeatureStates[featureFlag] = this.m_webContext.TfsRequestContext.IsFeatureEnabled(featureFlag);
    }

    [DataMember]
    public Dictionary<string, bool> FeatureStates { get; private set; }
  }
}
