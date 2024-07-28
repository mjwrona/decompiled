// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.ArgumentValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class ArgumentValidator
  {
    public static void ValidateCreateContext(CollectionCreationContext creationContext)
    {
      try
      {
        ArgumentUtility.CheckForNull<CollectionCreationContext>(creationContext, nameof (creationContext));
        ArgumentUtility.CheckStringForNullOrWhiteSpace(creationContext.Name, "Name");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(creationContext.PreferredRegion, "PreferredRegion");
      }
      catch (Exception ex)
      {
        throw new OrganizationBadRequestException(ex.Message);
      }
    }

    public static void ValidateQueryContext(CollectionQueryContext queryContext)
    {
      try
      {
        ArgumentUtility.CheckForNull<CollectionQueryContext>(queryContext, nameof (queryContext));
        ArgumentUtility.CheckStringForNullOrWhiteSpace(queryContext.SearchValue, "SearchValue");
        ArgumentUtility.CheckForOutOfRange((int) queryContext.SearchKind, "SearchKind", 1, 3);
      }
      catch (Exception ex)
      {
        throw new OrganizationBadRequestException(ex.Message);
      }
    }

    public static void ValidateCreateContext(OrganizationCreationContext creationContext)
    {
      try
      {
        ArgumentUtility.CheckForNull<OrganizationCreationContext>(creationContext, nameof (creationContext));
        if (!creationContext.AutoGenerateName)
          ArgumentUtility.CheckStringForNullOrWhiteSpace(creationContext.Name, "Name");
        if (creationContext.PrimaryCollection == null)
          return;
        ArgumentValidator.ValidateCreateContext(creationContext.PrimaryCollection);
      }
      catch (Exception ex)
      {
        throw new OrganizationBadRequestException(ex.Message);
      }
    }

    public static void ValidateQueryContext(OrganizationQueryContext queryContext)
    {
      try
      {
        ArgumentUtility.CheckForNull<OrganizationQueryContext>(queryContext, nameof (queryContext));
        ArgumentUtility.CheckStringForNullOrWhiteSpace(queryContext.SearchValue, "SearchValue");
        ArgumentUtility.CheckForOutOfRange((int) queryContext.SearchKind, "SearchKind", 1, 3);
      }
      catch (Exception ex)
      {
        throw new OrganizationBadRequestException(ex.Message);
      }
    }
  }
}
