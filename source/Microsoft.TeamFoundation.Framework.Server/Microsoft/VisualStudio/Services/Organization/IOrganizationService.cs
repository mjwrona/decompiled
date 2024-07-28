// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.IOrganizationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization.Client;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Organization
{
  [DefaultServiceImplementation(typeof (FrameworkOrganizationService))]
  public interface IOrganizationService : IVssFrameworkService
  {
    Microsoft.VisualStudio.Services.Organization.Organization GetOrganization(
      IVssRequestContext context,
      IEnumerable<string> propertyNames);

    bool RenameOrganization(IVssRequestContext context, string newName);

    bool ActivateOrganization(IVssRequestContext context);

    bool UpdateProperties(IVssRequestContext context, PropertyBag properties);

    bool DeleteProperties(IVssRequestContext context, IEnumerable<string> propertyNames);

    bool UpdateLogo(IVssRequestContext context, Logo logo);

    CollectionRef CreateCollection(
      IVssRequestContext context,
      CollectionCreationContext creationContext);

    IList<CollectionRef> GetCollections(IVssRequestContext context);

    bool DeleteCollection(
      IVssRequestContext context,
      Guid collectionId,
      Dictionary<string, object> deletionContext = null);

    bool RestoreCollection(IVssRequestContext context, Guid collectionId, string collectionName = null);
  }
}
