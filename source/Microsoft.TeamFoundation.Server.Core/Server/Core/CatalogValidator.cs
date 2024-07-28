// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogValidator
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class CatalogValidator : IDisposable
  {
    private Dictionary<Guid, CatalogResourceTypeConstraints> m_resourceTypeConstraints = new Dictionary<Guid, CatalogResourceTypeConstraints>();
    private Dictionary<Guid, List<ValidateNodeDelegate>> m_nodeValidators = new Dictionary<Guid, List<ValidateNodeDelegate>>();
    private Dictionary<Guid, List<ValidateResourceDelegate>> m_resourceValidators = new Dictionary<Guid, List<ValidateResourceDelegate>>();
    private Dictionary<Guid, List<ValidateDeleteDelegate>> m_deleteValidators = new Dictionary<Guid, List<ValidateDeleteDelegate>>();
    private CatalogRuleRegistrar m_registrar;

    public void Initialize(IVssRequestContext requestContext)
    {
      if (this.m_registrar != null)
        this.m_registrar.Dispose();
      this.m_registrar = new CatalogRuleRegistrar();
      this.m_registrar.LoadResourceTypeConstraints(requestContext);
      foreach (CatalogResourceTypeConstraints resourceTypeConstraint in this.m_registrar.ResourceTypeConstraints)
        this.AddConstraints(resourceTypeConstraint);
      foreach (NodeValidationSpec nodeValidationSpec in this.m_registrar.NodeValidationSpecs)
      {
        List<ValidateNodeDelegate> validateNodeDelegateList;
        if (!this.m_nodeValidators.TryGetValue(nodeValidationSpec.ResourceType, out validateNodeDelegateList))
        {
          validateNodeDelegateList = new List<ValidateNodeDelegate>();
          this.m_nodeValidators[nodeValidationSpec.ResourceType] = validateNodeDelegateList;
        }
        validateNodeDelegateList.Add(nodeValidationSpec.ValidateNodeDelegate);
      }
      foreach (ResourceValidationSpec resourceValidationSpec in this.m_registrar.ResourceValidationSpecs)
      {
        List<ValidateResourceDelegate> resourceDelegateList;
        if (!this.m_resourceValidators.TryGetValue(resourceValidationSpec.ResourceType, out resourceDelegateList))
        {
          resourceDelegateList = new List<ValidateResourceDelegate>();
          this.m_resourceValidators[resourceValidationSpec.ResourceType] = resourceDelegateList;
        }
        resourceDelegateList.Add(resourceValidationSpec.ValidateResourceDelegate);
      }
      foreach (DeleteValidationSpec deleteValidationSpec in this.m_registrar.DeleteValidationSpecs)
      {
        List<ValidateDeleteDelegate> validateDeleteDelegateList;
        if (!this.m_deleteValidators.TryGetValue(deleteValidationSpec.ResourceType, out validateDeleteDelegateList))
        {
          validateDeleteDelegateList = new List<ValidateDeleteDelegate>();
          this.m_deleteValidators[deleteValidationSpec.ResourceType] = validateDeleteDelegateList;
        }
        validateDeleteDelegateList.Add(deleteValidationSpec.ValidateDeleteDelegate);
      }
    }

    public void AddConstraints(CatalogResourceTypeConstraints constraints)
    {
      this.m_resourceTypeConstraints[constraints.ResourceType] = constraints;
      if (constraints.ParentChildConstraints == null)
        return;
      ParentChildConstraintValidator.AddConstraints(constraints.ResourceType, constraints.ParentChildConstraints);
    }

    public void ValidateChanges(
      IVssRequestContext requestContext,
      ITeamFoundationCatalogService catalogService,
      CatalogRuleValidationUtility utility,
      IEnumerable<CatalogNode> nodeChanges,
      IEnumerable<CatalogResource> resourceChanges,
      IEnumerable<KeyValuePair<CatalogNode, bool>> deletes)
    {
      foreach (CatalogResource resourceChange in resourceChanges)
        this.ValidateResource(requestContext, catalogService, utility, resourceChange);
      foreach (CatalogNode nodeChange in nodeChanges)
        this.ValidateNode(requestContext, catalogService, utility, nodeChange);
      foreach (KeyValuePair<CatalogNode, bool> delete in deletes)
        this.ValidateDelete(requestContext, catalogService, utility, delete.Key, delete.Value);
    }

    public void Dispose()
    {
      if (this.m_registrar == null)
        return;
      this.m_registrar.Dispose();
      this.m_registrar = (CatalogRuleRegistrar) null;
    }

    private void ValidateNode(
      IVssRequestContext requestContext,
      ITeamFoundationCatalogService catalogSerivce,
      CatalogRuleValidationUtility utility,
      CatalogNode node)
    {
      CatalogResourceTypeConstraints resourceTypeConstraints;
      if (this.m_resourceTypeConstraints.TryGetValue(node.Resource.ResourceType.Identifier, out resourceTypeConstraints))
      {
        ParentChildConstraintValidator.ValidateParentChildRelationship(requestContext, catalogSerivce, utility, node);
        utility.CheckDependenciesConstraints(requestContext, node, resourceTypeConstraints.DependencyConstraints);
        utility.CheckExclusiveNodeReference(requestContext, node, resourceTypeConstraints.SingleNodeReferenceConstraint);
        utility.CheckExclusiveTypePerRootExistence(requestContext, node, resourceTypeConstraints.ExistencePerRootConstraints);
        utility.CheckExclusiveTypePerParentExistence(requestContext, node, resourceTypeConstraints.ExistencePerParentTypeConstraints);
      }
      List<ValidateNodeDelegate> validateNodeDelegateList;
      if (!this.m_nodeValidators.TryGetValue(node.Resource.ResourceType.Identifier, out validateNodeDelegateList))
        return;
      foreach (ValidateNodeDelegate validateNodeDelegate in validateNodeDelegateList)
        validateNodeDelegate(requestContext, catalogSerivce, utility, node, string.IsNullOrEmpty(node.FullPath));
    }

    private void ValidateResource(
      IVssRequestContext requestContext,
      ITeamFoundationCatalogService catalogSerivce,
      CatalogRuleValidationUtility utility,
      CatalogResource resource)
    {
      CatalogResourceTypeConstraints resourceTypeConstraints;
      if (this.m_resourceTypeConstraints.TryGetValue(resource.ResourceType.Identifier, out resourceTypeConstraints))
      {
        utility.CheckPropertyConstraints(requestContext, resource, resourceTypeConstraints.PropertyConstraints);
        utility.CheckServiceReferenceConstraints(resource, resourceTypeConstraints.ServiceReferenceConstraints);
      }
      List<ValidateResourceDelegate> resourceDelegateList;
      if (!this.m_resourceValidators.TryGetValue(resource.ResourceType.Identifier, out resourceDelegateList))
        return;
      foreach (ValidateResourceDelegate resourceDelegate in resourceDelegateList)
        resourceDelegate(requestContext, catalogSerivce, utility, resource);
    }

    private void ValidateDelete(
      IVssRequestContext requestContext,
      ITeamFoundationCatalogService catalogSerivce,
      CatalogRuleValidationUtility utility,
      CatalogNode node,
      bool recurse)
    {
      CatalogResourceTypeConstraints resourceTypeConstraints;
      if (this.m_resourceTypeConstraints.TryGetValue(node.Resource.ResourceType.Identifier, out resourceTypeConstraints))
        utility.CheckDeleteConstraints(requestContext, node, recurse, resourceTypeConstraints.DeleteConstraints);
      List<ValidateDeleteDelegate> validateDeleteDelegateList;
      if (!this.m_deleteValidators.TryGetValue(node.Resource.ResourceType.Identifier, out validateDeleteDelegateList))
        return;
      foreach (ValidateDeleteDelegate validateDeleteDelegate in validateDeleteDelegateList)
        validateDeleteDelegate(requestContext, catalogSerivce, utility, node, recurse);
    }
  }
}
