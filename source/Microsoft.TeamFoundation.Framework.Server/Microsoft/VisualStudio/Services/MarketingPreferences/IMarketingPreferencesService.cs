// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MarketingPreferences.IMarketingPreferencesService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.MarketingPreferences
{
  [DefaultServiceImplementation(typeof (IInternalMarketingPreferencesService))]
  public interface IMarketingPreferencesService : IVssFrameworkService
  {
    bool GetContactUserWithOffersSetting(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor);

    void SetContactUserWithOffersSetting(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      bool value);

    Microsoft.VisualStudio.Services.MarketingPreferences.MarketingPreferences GetMarketingPreferences(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor);

    void SetMarketingPreferences(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      Microsoft.VisualStudio.Services.MarketingPreferences.MarketingPreferences preferences);
  }
}
