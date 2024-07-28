// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RequestRestrictions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RequestRestrictions
  {
    internal static readonly string[] s_SingleLabelDelimiter = new string[1]
    {
      ", "
    };
    internal static readonly RequestRestrictions DefaultRequestRestrictions = new RequestRestrictions(RequiredAuthentication.ValidatedUser, AllowedHandler.All, "default");

    internal RequestRestrictions(
      RequiredAuthentication requiredAuthentication,
      AllowedHandler allowedHandlers,
      string description)
      : this((string) null, requiredAuthentication, allowedHandlers, description, false, true, AuthenticationMechanisms.All)
    {
    }

    internal RequestRestrictions(
      RequiredAuthentication requiredAuthentication,
      AllowedHandler allowedHandlers,
      string description,
      bool allowNonSsl)
      : this((string) null, requiredAuthentication, allowedHandlers, description, allowNonSsl, true, AuthenticationMechanisms.All)
    {
    }

    internal RequestRestrictions(
      string label,
      RequiredAuthentication requiredAuthentication,
      AllowedHandler allowedHandlers,
      string description)
      : this(label, requiredAuthentication, allowedHandlers, description, false, true, AuthenticationMechanisms.All)
    {
    }

    internal RequestRestrictions(
      string label,
      RequiredAuthentication requiredAuthentication,
      AllowedHandler allowedHandlers,
      string description,
      bool allowNonSsl)
      : this(label, requiredAuthentication, allowedHandlers, description, allowNonSsl, true, AuthenticationMechanisms.All)
    {
    }

    internal RequestRestrictions(
      RequiredAuthentication requiredAuthentication,
      AllowedHandler allowedHandlers,
      string description,
      bool allowNonSsl,
      AuthenticationMechanisms mechanismsToAdvertise)
      : this((string) null, requiredAuthentication, allowedHandlers, description, allowNonSsl, true, mechanismsToAdvertise)
    {
    }

    internal RequestRestrictions(
      string label,
      RequiredAuthentication requiredAuthentication,
      AllowedHandler allowedHandlers,
      string description,
      bool allowNonSsl,
      AuthenticationMechanisms mechanismsToAdvertise)
      : this(label, requiredAuthentication, allowedHandlers, description, allowNonSsl, true, mechanismsToAdvertise)
    {
    }

    internal RequestRestrictions(
      string label,
      RequiredAuthentication requiredAuthentication,
      AllowedHandler allowedHandlers,
      string description,
      bool allowNonSsl,
      bool allowCORS,
      AuthenticationMechanisms mechanismsToAdvertise)
    {
      this.Label = label;
      this.RequiredAuthentication = requiredAuthentication;
      this.AllowedHandlers = allowedHandlers;
      this.Description = description;
      this.AllowNonSsl = allowNonSsl;
      this.AllowCORS = allowCORS;
      this.MechanismsToAdvertise = mechanismsToAdvertise;
    }

    internal string Label { get; }

    internal RequiredAuthentication RequiredAuthentication { get; }

    internal AllowedHandler AllowedHandlers { get; }

    internal bool AllowNonSsl { get; }

    internal bool AllowCORS { get; }

    internal AuthenticationMechanisms MechanismsToAdvertise { get; }

    internal string Description { get; }

    internal bool HasAnyLabel(params string[] labels) => !string.IsNullOrWhiteSpace(this.Label) && ((IEnumerable<string>) this.Label.Split(RequestRestrictions.s_SingleLabelDelimiter, StringSplitOptions.RemoveEmptyEntries)).Any<string>((Func<string, bool>) (requestLabel => ((IEnumerable<string>) labels).Any<string>((Func<string, bool>) (label => StringComparer.OrdinalIgnoreCase.Equals(label, requestLabel)))));

    internal RequestRestrictions WithMechanismsToAdvertise(
      AuthenticationMechanisms newMechanismsToAdvertise)
    {
      return new RequestRestrictions(this.Label, this.RequiredAuthentication, this.AllowedHandlers, string.Format("{0} + MechanismsToAdvertise={1}", (object) this.Description, (object) newMechanismsToAdvertise), this.AllowNonSsl, this.AllowCORS, newMechanismsToAdvertise);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Auth: {0}; Handlers: {1} ({2})", (object) this.RequiredAuthentication, (object) this.AllowedHandlers, (object) this.Description);
  }
}
