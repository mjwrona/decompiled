// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement.IContributedFeatureStateChangedListener
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using Microsoft.VisualStudio.Services.Settings;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement
{
  [InheritedExport]
  public interface IContributedFeatureStateChangedListener
  {
    string Name { get; }

    void OnFeatureStateChanged(
      IVssRequestContext requestContext,
      string contributionId,
      ContributedFeatureEnabledValue state,
      SettingsUserScope userScope,
      string scopeName,
      string scopeValue,
      IDictionary<string, object> properties);
  }
}
