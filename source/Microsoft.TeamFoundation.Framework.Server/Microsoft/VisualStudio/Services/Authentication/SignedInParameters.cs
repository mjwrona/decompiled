// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Authentication.SignedInParameters
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Compliance;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.UrlValidation;
using System;
using System.Diagnostics;
using System.Web;

namespace Microsoft.VisualStudio.Services.Authentication
{
  public class SignedInParameters
  {
    private const string Area = "Authentication";
    private const string Layer = "SignedInParameters";

    private SignedInParameters()
    {
    }

    public static SignedInParameters BuildSignedInParameters(
      IVssRequestContext requestContext,
      string realm,
      string protocol,
      string reply_to,
      string hid,
      string visibility,
      Uri requestUri,
      SignInContext context)
    {
      requestContext.TraceEnter(1450720, "Authentication", nameof (SignedInParameters), nameof (BuildSignedInParameters));
      AccessMapping accessMapping = requestContext.GetService<ILocationService>().DetermineAccessMapping(requestContext);
      SecureFlowLocation location1;
      int num1 = (int) SecureFlowLocation.TryCreate(requestContext, reply_to, SecureFlowLocation.GetDefaultLocation(requestContext), out location1);
      SecureFlowLocation location2;
      int num2 = (int) SecureFlowLocation.TryCreate(requestContext, accessMapping.AccessPoint, SecureFlowLocation.GetDefaultLocation(requestContext), out location2);
      if (location1 == null)
      {
        requestContext.Trace(5500102, TraceLevel.Warning, "Authentication", nameof (SignedInParameters), "Signin URI does not have a reply_to. Check IIS logs for referrer that caused this.");
        location1 = SecureFlowLocation.GetDefaultLocation(requestContext);
      }
      realm = location1.GetInnermostReplyTo().Host;
      IUrlValidationService service = requestContext.GetService<IUrlValidationService>();
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.SignIn.ValidateRedirectUrls") && !service.IsValidUrl(requestContext, location1.ToString()))
        requestContext.TraceAlways(5500130, TraceLevel.Info, "Authentication", nameof (SignedInParameters), "Invalid ReplyToUrl: " + HttpUtility.HtmlEncode(reply_to));
      if (realm == null)
        throw new InvalidReplyToException(FrameworkResources.InvalidReplyToExceptionMessage((object) HttpUtility.HtmlEncode(reply_to)));
      SignedInProcessorParameters parameters = SignedInProcessorParameters.EvaluateParameters(requestContext, realm, reply_to, protocol, hid, requestUri, location2);
      requestContext.TraceLeave(1450729, "Authentication", nameof (SignedInParameters), nameof (BuildSignedInParameters));
      return new SignedInParameters()
      {
        RealmIsDeploymentHost = parameters.RealmIsDeploymentHost,
        ValidatedRealm = parameters.Realm,
        ValidDomain = parameters.Domain,
        FinalLocation = location1,
        CurrentLocation = location2,
        Protocol = protocol,
        Visibility = visibility,
        ProcessorParameters = parameters,
        RequestUri = SignedInProcessorParameters.RemoveSentitiveQueryParams(requestContext, requestUri),
        RealmSignedInLocation = parameters.RealmSignedInLocation,
        Context = context,
        FinalHop = parameters.FinalHop
      };
    }

    public SecureFlowLocation CurrentLocation { get; set; }

    public SecureFlowLocation FinalLocation { get; set; }

    public bool RealmIsDeploymentHost { get; set; }

    public string ValidDomain { get; set; }

    public string ValidatedRealm { get; set; }

    public string Protocol { get; set; }

    public string Visibility { get; set; }

    public SignedInProcessorParameters ProcessorParameters { get; set; }

    public Uri RequestUri { get; set; }

    public SecureFlowLocation RealmSignedInLocation { get; set; }

    public SignInContext Context { get; set; }

    public bool FinalHop { get; set; }
  }
}
