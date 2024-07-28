// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.TooManyRequestsExceptionHandler
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class TooManyRequestsExceptionHandler : IErrorPageHandler
  {
    public void HandleError(IVssRequestContext requestContext, ErrorContext context)
    {
      if (context.StatusCode != 429)
        return;
      context.ImageContentId = "Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Error.Content.Images.Error_429.png";
      context.StatusCode = 429;
      context.ErrorTitle = WebFrameworkResources.TooManyRequestsException_Title();
      ErrorAction errorAction = new ErrorAction()
      {
        Href = LinkUtilities.ForwardLink(328778),
        Text = WebFrameworkResources.ContactSupport_ActionText()
      };
      try
      {
        ILocationDataProvider locationData = requestContext.GetService<ILocationService>().GetLocationData(requestContext, ServiceInstanceTypes.TFS);
        string str = locationData.LocationForAccessMapping(requestContext, "UtilizationUsageSummary", FrameworkServiceIdentifiers.UtilizationUsageSummary, locationData.DetermineAccessMapping(requestContext));
        context.ErrorDescription = WebFrameworkResources.TooManyRequestsExceptionWithUsage_Desc();
        context.PrimaryAction = new ErrorAction()
        {
          Href = str,
          Text = WebFrameworkResources.ViewMyUsage_ActionText()
        };
        context.SecondaryActions.Add(errorAction);
      }
      catch
      {
        context.ErrorDescription = WebFrameworkResources.TooManyRequestsException_Desc();
        context.PrimaryAction = errorAction;
      }
    }
  }
}
