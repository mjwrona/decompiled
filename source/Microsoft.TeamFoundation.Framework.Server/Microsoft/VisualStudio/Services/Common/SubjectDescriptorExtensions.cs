// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.SubjectDescriptorExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class SubjectDescriptorExtensions
  {
    public static IdentityDescriptor ToIdentityDescriptor(
      this SubjectDescriptor subjectDescriptor,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (subjectDescriptor == new SubjectDescriptor())
        return (IdentityDescriptor) null;
      if (subjectDescriptor.IsCuidBased())
        return requestContext.GetService<IGraphIdentifierConversionService>().GetLegacyDescriptorByCuidBasedDescriptor(requestContext, subjectDescriptor);
      string subjectType = subjectDescriptor.SubjectType;
      if (subjectType != null)
      {
        switch (subjectType.Length)
        {
          case 3:
            switch (subjectType[1])
            {
              case '2':
                if (subjectType == "s2s")
                  return new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", subjectDescriptor.Identifier);
                break;
              case 'c':
                switch (subjectType)
                {
                  case "scp":
                    return new IdentityDescriptor("Microsoft.VisualStudio.Services.Graph.GraphScope", subjectDescriptor.Identifier);
                  case "acs":
                    return new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", subjectDescriptor.Identifier);
                }
                break;
              case 'g':
                if (subjectType == "agg")
                  return new IdentityDescriptor("Microsoft.TeamFoundation.AggregateIdentity", subjectDescriptor.Identifier);
                break;
              case 'i':
                if (subjectType == "win")
                  return new IdentityDescriptor("System.Security.Principal.WindowsIdentity", subjectDescriptor.Identifier);
                break;
              case 'k':
                if (subjectType == "ukn")
                  return new IdentityDescriptor("Microsoft.VisualStudio.Services.Identity.UnknownIdentity", subjectDescriptor.Identifier);
                break;
              case 'm':
                if (subjectType == "imp")
                  return new IdentityDescriptor("Microsoft.TeamFoundation.ImportedIdentity", subjectDescriptor.Identifier);
                break;
              case 'n':
                if (subjectType == "bnd")
                  return new IdentityDescriptor("Microsoft.TeamFoundation.BindPendingIdentity", subjectDescriptor.Identifier);
                break;
              case 'p':
                if (subjectType == "spa")
                  return new IdentityDescriptor("System:PublicAccess", subjectDescriptor.Identifier);
                break;
              case 's':
                if (subjectType == "tst")
                  return new IdentityDescriptor("Microsoft.VisualStudio.Services.Identity.ServerTestIdentity", subjectDescriptor.Identifier);
                break;
              case 'v':
                if (subjectType == "svc")
                  return new IdentityDescriptor("Microsoft.TeamFoundation.ServiceIdentity", subjectDescriptor.Identifier);
                break;
            }
            break;
          case 4:
            switch (subjectType[1])
            {
              case 'a':
                if (subjectType == "sace")
                  return new IdentityDescriptor("System:AccessControl", subjectDescriptor.Identifier);
                break;
              case 'c':
                if (subjectType == "scsp")
                  return new IdentityDescriptor("System:CspPartner", subjectDescriptor.Identifier);
                break;
              case 'l':
                if (subjectType == "slic")
                  return new IdentityDescriptor("System:License", subjectDescriptor.Identifier);
                break;
              case 's':
                if (subjectType == "sscp")
                  return new IdentityDescriptor("System:Scope", subjectDescriptor.Identifier);
                break;
            }
            break;
          case 5:
            switch (subjectType[2])
            {
              case 'd':
                if (subjectType == "aadgp")
                  break;
                goto label_45;
              case 'g':
                if (subjectType == "ungrp")
                  break;
                goto label_45;
              case 's':
                if (subjectType == "vssgp")
                  break;
                goto label_45;
              case 'u':
                switch (subjectType)
                {
                  case "unusr":
                    return new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", subjectDescriptor.Identifier);
                  case "uauth":
                    return new IdentityDescriptor("Microsoft.TeamFoundation.UnauthenticatedIdentity", subjectDescriptor.Identifier);
                  default:
                    goto label_45;
                }
              default:
                goto label_45;
            }
            return new IdentityDescriptor("Microsoft.TeamFoundation.Identity", subjectDescriptor.Identifier);
        }
      }
label_45:
      throw new InvalidSubjectTypeException(subjectDescriptor.SubjectType);
    }

    internal static SubjectDescriptor CreateSubjectDescriptor(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(identity.Descriptor, "Descriptor");
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!identity.Descriptor.IsCuidBased())
        return new SubjectDescriptor(identity.Descriptor.GetSubjectTypeForNonCuidBasedIdentity(requestContext), identity.Descriptor.Identifier);
      return identity.Cuid() == new Guid() ? new SubjectDescriptor() : new SubjectDescriptor(identity.GetSubjectTypeForCuidBasedIdentity(requestContext), identity.Cuid().ToString());
    }

    internal static SubjectDescriptor GetSubjectDescriptor(
      this IReadOnlyVssIdentity identity,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IReadOnlyVssIdentity>(identity, nameof (identity));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      switch (identity)
      {
        case Microsoft.VisualStudio.Services.Identity.Identity identity1:
          return identity1.SubjectDescriptor;
        case IVssIdentityServer vssIdentityServer:
          return vssIdentityServer.SubjectDescriptor;
        default:
          ArgumentUtility.CheckForNull<IdentityDescriptor>(identity.Descriptor, "Descriptor");
          return identity.Descriptor.ToSubjectDescriptor(requestContext);
      }
    }
  }
}
