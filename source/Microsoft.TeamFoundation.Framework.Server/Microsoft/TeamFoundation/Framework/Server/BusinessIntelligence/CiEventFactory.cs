// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence.CiEventFactory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Specialized;

namespace Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence
{
  public static class CiEventFactory
  {
    public static CiEvent CreateEvent(string area, string feature, NameValueCollection properties = null)
    {
      if (string.IsNullOrWhiteSpace(area))
        throw new ArgumentNullException(nameof (area));
      if (string.IsNullOrWhiteSpace(feature))
        throw new ArgumentNullException(nameof (feature));
      CiEvent ciEvent = (CiEvent) null;
      if (area.Equals(CustomerIntelligenceArea.Authentication, StringComparison.InvariantCultureIgnoreCase))
        ciEvent = (CiEvent) CiEventFactory.CreateAuthenticationEvent(feature, properties);
      else if (area.Equals(CustomerIntelligenceArea.Exceptions, StringComparison.InvariantCultureIgnoreCase))
        ciEvent = (CiEvent) CiEventFactory.CreateExceptionEvent(feature, properties);
      return ciEvent;
    }

    private static AuthenticationCiEvent CreateAuthenticationEvent(
      string feature,
      NameValueCollection properties = null)
    {
      AuthenticationCiEvent authenticationEvent = (AuthenticationCiEvent) null;
      if (feature.Equals(CustomerIntelligenceFeature.SignIn, StringComparison.InvariantCultureIgnoreCase))
        authenticationEvent = (AuthenticationCiEvent) new SignInCiEvent();
      else if (feature.Equals(CustomerIntelligenceFeature.SignedIn, StringComparison.InvariantCultureIgnoreCase))
        authenticationEvent = (AuthenticationCiEvent) new SignedInCiEvent();
      else if (feature.Equals(CustomerIntelligenceFeature.SignOut, StringComparison.InvariantCultureIgnoreCase))
        authenticationEvent = (AuthenticationCiEvent) new SignOutCiEvent();
      if (authenticationEvent != null && properties != null)
      {
        string property1 = properties["Source"];
        string property2 = properties["Flow"];
        try
        {
          if (!string.IsNullOrWhiteSpace(property1))
            authenticationEvent.Source = (AuthenticationCiEvent.Sources) int.Parse(property1);
          if (!string.IsNullOrWhiteSpace(property2))
            authenticationEvent.Flow = (AuthenticationCiEvent.Flows) int.Parse(property2);
        }
        catch (Exception ex)
        {
          return (AuthenticationCiEvent) null;
        }
      }
      return authenticationEvent;
    }

    private static ExceptionCiEvent CreateExceptionEvent(
      string feature,
      NameValueCollection properties = null)
    {
      ExceptionCiEvent exceptionEvent = (ExceptionCiEvent) null;
      if (feature.Equals(CustomerIntelligenceFeature.HttpException, StringComparison.InvariantCultureIgnoreCase))
      {
        exceptionEvent = (ExceptionCiEvent) new HttpExceptionCiEvent();
        if (properties != null)
        {
          string property = properties["HttpCode"];
          ((HttpExceptionCiEvent) exceptionEvent).HttpCode = int.Parse(property);
        }
      }
      if (exceptionEvent != null && properties != null)
      {
        string property1 = properties["ErrorMessage"];
        string property2 = properties["ActivityId"];
        try
        {
          if (!string.IsNullOrWhiteSpace(property1))
            exceptionEvent.ErrorMessage = property1;
          if (!string.IsNullOrWhiteSpace(property2))
          {
            Guid result;
            Guid.TryParse(property2, out result);
            exceptionEvent.ActivityId = result;
          }
        }
        catch (Exception ex)
        {
          return (ExceptionCiEvent) null;
        }
      }
      return exceptionEvent;
    }
  }
}
