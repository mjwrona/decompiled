// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsCommon.IGlobalMessageService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TfsCommon, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3690C2EA-1623-4663-B65B-BB4B63BFE368
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TfsCommon.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;

namespace Microsoft.TeamFoundation.Server.WebAccess.TfsCommon
{
  [DefaultServiceImplementation(typeof (GlobalMessageService))]
  public interface IGlobalMessageService : IVssFrameworkService
  {
    void AddDialog(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      GlobalDialog dialog);

    void AddMessageBanner(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      string message,
      WebMessageLevel messageLevel);

    void AddMessageBanner(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      GlobalMessageBanner banner);

    void AddMessageAction(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      GlobalMessageAction action);

    object CreateGlobalDialogFromContribution(
      IVssRequestContext requestContext,
      ContributionNode contributionNode);

    object CreateGlobalMessageFromContribution(
      IVssRequestContext requestContext,
      ContributionNode contributionNode);

    void ProcessMessageLinks(IVssRequestContext requestContext, GlobalMessage message);
  }
}
