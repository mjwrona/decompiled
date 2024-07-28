// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogRuleRegistrar
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class CatalogRuleRegistrar : IDisposable
  {
    public void LoadResourceTypeConstraints(IVssRequestContext requestContext)
    {
      if (this.RuleSets != null)
        this.RuleSets.Dispose();
      this.RuleSets = requestContext.GetExtensions<ICatalogResourceTypeRuleSet>();
      this.ResourceTypeConstraints = new List<CatalogResourceTypeConstraints>();
      this.NodeValidationSpecs = new List<NodeValidationSpec>();
      this.ResourceValidationSpecs = new List<ResourceValidationSpec>();
      this.DeleteValidationSpecs = new List<DeleteValidationSpec>();
      foreach (ICatalogResourceTypeRuleSet ruleSet in (IEnumerable<ICatalogResourceTypeRuleSet>) this.RuleSets)
      {
        this.ResourceTypeConstraints.AddRange(ruleSet.GetResourceTypeConstraints());
        this.NodeValidationSpecs.AddRange(ruleSet.GetNodeValidationSpecs());
        this.ResourceValidationSpecs.AddRange(ruleSet.GetResourceValidationSpecs());
        this.DeleteValidationSpecs.AddRange(ruleSet.GetDeleteValidationSpecs());
      }
      this.ResourceTypeConstraints.Add(FrameworkConstraints.OrganizationalRootConstraints);
      this.ResourceTypeConstraints.Add(FrameworkConstraints.InfrastructureRootConstraints);
      this.ResourceTypeConstraints.Add(FrameworkConstraints.GenericLinkConstraints);
      this.ResourceTypeConstraints.Add(FrameworkConstraints.MachineConstraints);
      this.ResourceTypeConstraints.Add(FrameworkConstraints.ProjectCollectionConstraints);
      this.ResourceTypeConstraints.Add(FrameworkConstraints.ResourceFolderConstraints);
      this.ResourceTypeConstraints.Add(FrameworkConstraints.SQLReportingInstanceConstraints);
      this.ResourceTypeConstraints.Add(FrameworkConstraints.SQLAnalysisInstanceConstraints);
      this.ResourceTypeConstraints.Add(FrameworkConstraints.TeamFoundationServiceInstanceConstraints);
      this.ResourceTypeConstraints.Add(FrameworkConstraints.TeamFoundationWebApplicationConstraints);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.ResourceValidationSpecs.Add(new ResourceValidationSpec(CatalogResourceTypes.ProjectCollection, CatalogRuleRegistrar.\u003C\u003EO.\u003C0\u003E__ValidateCollectionExists ?? (CatalogRuleRegistrar.\u003C\u003EO.\u003C0\u003E__ValidateCollectionExists = new ValidateResourceDelegate(CollectionCatalogRules.ValidateCollectionExists))));
    }

    public void Dispose()
    {
      if (this.RuleSets == null)
        return;
      this.RuleSets.Dispose();
      this.RuleSets = (IDisposableReadOnlyList<ICatalogResourceTypeRuleSet>) null;
    }

    public List<CatalogResourceTypeConstraints> ResourceTypeConstraints { get; private set; }

    public List<NodeValidationSpec> NodeValidationSpecs { get; private set; }

    public List<ResourceValidationSpec> ResourceValidationSpecs { get; private set; }

    public List<DeleteValidationSpec> DeleteValidationSpecs { get; private set; }

    private IDisposableReadOnlyList<ICatalogResourceTypeRuleSet> RuleSets { get; set; }
  }
}
