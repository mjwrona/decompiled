// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ParentChildConstraintValidator
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ParentChildConstraintValidator
  {
    private static readonly ConcurrentDictionary<Guid, ParentChildConstraints> s_constraints = new ConcurrentDictionary<Guid, ParentChildConstraints>();

    internal static void AddConstraints(Guid resourceType, ParentChildConstraints constraints) => ParentChildConstraintValidator.s_constraints[resourceType] = constraints;

    public static void ValidateParentChildRelationship(
      IVssRequestContext requestContext,
      ITeamFoundationCatalogService catalogService,
      CatalogRuleValidationUtility utility,
      CatalogNode node)
    {
      CatalogNode parent = utility.FindParent(requestContext, node);
      if (parent == null && (node.Resource.ResourceType.Identifier == CatalogResourceTypes.OrganizationalRoot || node.Resource.ResourceType.Identifier == CatalogResourceTypes.InfrastructureRoot))
        return;
      if (parent == null)
        throw new InvalidCatalogSaveNodeException(FrameworkResources.IllegalAddToCatalogRoot((object) node.Resource.ResourceType.DisplayName));
      ParentChildConstraints childConstraints1;
      if (ParentChildConstraintValidator.s_constraints.TryGetValue(parent.Resource.ResourceType.Identifier, out childConstraints1))
        ParentChildConstraintValidator.VerifyAllowedRelationship(parent, childConstraints1.AllowedChildren, childConstraints1.DeniedChildren, node, true);
      ParentChildConstraints childConstraints2;
      if (!ParentChildConstraintValidator.s_constraints.TryGetValue(node.Resource.ResourceType.Identifier, out childConstraints2))
        return;
      ParentChildConstraintValidator.VerifyAllowedRelationship(node, childConstraints2.AllowedParents, childConstraints2.DeniedParents, parent, false);
    }

    private static void VerifyAllowedRelationship(
      CatalogNode node,
      Guid[] allows,
      Guid[] denies,
      CatalogNode relative,
      bool nodeIsParent)
    {
      if (denies == ParentChildConstraints.AllTypes)
      {
        foreach (Guid allow in allows)
        {
          if (object.Equals((object) allow, (object) relative.Resource.ResourceType.Identifier))
            return;
        }
      }
      else
      {
        bool flag = true;
        foreach (Guid deny in denies)
        {
          if (object.Equals((object) deny, (object) relative.Resource.ResourceType.Identifier))
            flag = false;
        }
        if (flag)
          return;
      }
      throw new InvalidCatalogSaveNodeException(nodeIsParent ? FrameworkResources.IllegalCatalogParentRelationship((object) node.Resource.ResourceType.DisplayName, (object) relative.Resource.ResourceType.DisplayName) : FrameworkResources.IllegalCatalogChildRelationship((object) node.Resource.ResourceType.DisplayName, (object) relative.Resource.ResourceType.DisplayName));
    }
  }
}
