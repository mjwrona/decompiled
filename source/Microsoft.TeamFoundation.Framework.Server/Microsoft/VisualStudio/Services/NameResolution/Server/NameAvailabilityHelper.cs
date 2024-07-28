// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.Server.NameAvailabilityHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NameResolution.Server
{
  public class NameAvailabilityHelper
  {
    private const string c_area = "NameResolution";
    private const string c_layer = "NameAvailabilityHelper";

    public virtual void CheckCollectionNameReservation(
      IVssRequestContext context,
      string collectionName,
      Guid collectionId,
      bool devopsDomainUrls)
    {
      List<string> list = this.GetCollectionNamespaceReservations(devopsDomainUrls).Select<CollectionNamespaceReservation, string>((Func<CollectionNamespaceReservation, string>) (x => x.Namespace)).ToList<string>();
      this.CheckCollectionNameReservation(context, (IReadOnlyList<string>) list, collectionName, collectionId);
    }

    public virtual void CheckCollectionNameReservation(
      IVssRequestContext context,
      IReadOnlyList<string> collectionNamespaces,
      string collectionName,
      Guid collectionId)
    {
      List<string> namespaces = new List<string>()
      {
        "Forbidden",
        "MicrosoftReserved",
        "Deployment"
      };
      namespaces.AddRange((IEnumerable<string>) collectionNamespaces);
      NameResolutionEntry nameResolutionEntry = this.QueryFirstEntry(context, (IReadOnlyList<string>) namespaces, collectionName);
      if (nameResolutionEntry == null || collectionId != Guid.Empty && nameResolutionEntry.Value == collectionId)
        return;
      if (collectionNamespaces.Contains<string>(nameResolutionEntry.Namespace, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        throw new CollectionAlreadyExistsException(FrameworkResources.NameIsReserved((object) collectionName));
      if (nameResolutionEntry.Namespace == "Deployment")
        throw new CollectionNameException(FrameworkResources.NameIsReserved((object) collectionName));
      if (nameResolutionEntry.Namespace == "Forbidden")
        throw new CollectionNameException(FrameworkResources.NameIsForbidden((object) collectionName));
      if (nameResolutionEntry.Namespace == "MicrosoftReserved")
        throw new CollectionNameException(FrameworkResources.NameIsReserved((object) collectionName));
      context.Trace(5001568, TraceLevel.Error, "NameResolution", nameof (NameAvailabilityHelper), "An unexpected set of reservation were found for name " + collectionName + ". Reservation: " + nameResolutionEntry.Serialize<NameResolutionEntry>(true));
      throw new CollectionNameException(FrameworkResources.NameIsReserved((object) collectionName));
    }

    public virtual void CheckOrganizationNameReservation(
      IVssRequestContext context,
      string organizationName,
      Guid organizationId)
    {
      string[] namespaces = new string[4]
      {
        "Forbidden",
        "MicrosoftReserved",
        "Organization",
        "Deployment"
      };
      NameResolutionEntry nameResolutionEntry = this.QueryFirstEntry(context, (IReadOnlyList<string>) namespaces, organizationName);
      if (nameResolutionEntry == null || organizationId != Guid.Empty && nameResolutionEntry.Value == organizationId)
        return;
      if (nameResolutionEntry.Namespace == "Organization")
        throw new OrganizationAlreadyExistsException(organizationName);
      if (nameResolutionEntry.Namespace == "Deployment")
        throw new OrganizationNameException(FrameworkResources.NameIsReserved((object) organizationName));
      if (nameResolutionEntry.Namespace == "Forbidden")
        throw new OrganizationNameException(FrameworkResources.NameIsForbidden((object) organizationName));
      if (nameResolutionEntry.Namespace == "MicrosoftReserved")
        throw new OrganizationNameException(FrameworkResources.NameIsReserved((object) organizationName));
      context.Trace(5001578, TraceLevel.Error, "NameResolution", nameof (NameAvailabilityHelper), "An unexpected set of reservation were found for name " + organizationName + ". Reservations: " + nameResolutionEntry.Serialize<NameResolutionEntry>(true));
      throw new OrganizationNameException(FrameworkResources.NameIsReserved((object) organizationName));
    }

    public virtual NameResolutionEntry QueryEntry(
      IVssRequestContext context,
      string @namespace,
      string name)
    {
      if (this.IsPrecreatedOrDeletedNameFormat(name))
        return (NameResolutionEntry) null;
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<INameResolutionService>().QueryEntry(vssRequestContext, @namespace, name);
    }

    public virtual NameResolutionEntry QueryFirstEntry(
      IVssRequestContext context,
      IReadOnlyList<string> namespaces,
      string name)
    {
      if (this.IsPrecreatedOrDeletedNameFormat(name))
        return (NameResolutionEntry) null;
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IInternalNameResolutionService>().QueryFirstEntry(vssRequestContext, namespaces, name, (Predicate<NameResolutionEntry>) (_ => true));
    }

    public virtual IReadOnlyList<CollectionNamespaceReservation> GetCollectionNamespaceReservations(
      bool devopsDomainUrls)
    {
      return (IReadOnlyList<CollectionNamespaceReservation>) new List<CollectionNamespaceReservation>()
      {
        new CollectionNamespaceReservation()
        {
          Namespace = "Collection",
          IsPrimary = !devopsDomainUrls
        },
        new CollectionNamespaceReservation()
        {
          Namespace = "GlobalCollection",
          IsPrimary = devopsDomainUrls
        }
      };
    }

    public virtual IReadOnlyList<string> GetGlobalCollectionNamespaces(IVssRequestContext context) => (IReadOnlyList<string>) new string[2]
    {
      "Collection",
      "GlobalCollection"
    };

    private bool IsPrecreatedOrDeletedNameFormat(string name) => ServiceHostNameHelper.IsPGuid(name) || ServiceHostNameHelper.IsDGuid(name);
  }
}
