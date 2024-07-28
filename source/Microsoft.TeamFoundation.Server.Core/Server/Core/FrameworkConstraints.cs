// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.FrameworkConstraints
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class FrameworkConstraints
  {
    internal static readonly CatalogResourceTypeConstraints TeamFoundationServiceInstanceConstraints;
    internal static readonly CatalogResourceTypeConstraints ProjectCollectionConstraints;
    internal static readonly CatalogResourceTypeConstraints ResourceFolderConstraints;
    internal static readonly CatalogResourceTypeConstraints GenericLinkConstraints;
    internal static readonly CatalogResourceTypeConstraints MachineConstraints;
    internal static readonly CatalogResourceTypeConstraints TeamFoundationWebApplicationConstraints;
    internal static readonly CatalogResourceTypeConstraints SQLAnalysisInstanceConstraints;
    internal static readonly CatalogResourceTypeConstraints SQLReportingInstanceConstraints;
    internal static readonly CatalogResourceTypeConstraints OrganizationalRootConstraints;
    internal static readonly CatalogResourceTypeConstraints InfrastructureRootConstraints;

    static FrameworkConstraints()
    {
      CatalogResourceTypeConstraints resourceTypeConstraints1 = new CatalogResourceTypeConstraints();
      resourceTypeConstraints1.ResourceType = CatalogResourceTypes.TeamFoundationServerInstance;
      resourceTypeConstraints1.ExistencePerRootConstraints = new ExistencePerRootConstraint[1]
      {
        new ExistencePerRootConstraint()
        {
          CatalogTree = CatalogTree.Organizational
        }
      };
      resourceTypeConstraints1.DependencyConstraints = new DependencyConstraint[1]
      {
        new DependencyConstraint("WebApplication", false, new Guid[1]
        {
          CatalogResourceTypes.TeamFoundationWebApplication
        })
      };
      resourceTypeConstraints1.SingleNodeReferenceConstraint = true;
      resourceTypeConstraints1.ServiceReferenceConstraints = new ServiceReferenceConstraint[1]
      {
        new ServiceReferenceConstraint()
        {
          ReferenceName = "Location",
          ServiceType = "LocationService"
        }
      };
      resourceTypeConstraints1.DeleteConstraints = new DeleteConstraints()
      {
        CanDeleteRecursively = false,
        CanDeleteNonRecursively = false
      };
      resourceTypeConstraints1.ParentChildConstraints = new ParentChildConstraints()
      {
        AllowedChildren = ParentChildConstraints.AllTypes,
        DeniedChildren = ParentChildConstraints.NoTypes,
        AllowedParents = new Guid[1]
        {
          CatalogResourceTypes.OrganizationalRoot
        },
        DeniedParents = ParentChildConstraints.AllTypes
      };
      FrameworkConstraints.TeamFoundationServiceInstanceConstraints = resourceTypeConstraints1;
      CatalogResourceTypeConstraints resourceTypeConstraints2 = new CatalogResourceTypeConstraints();
      resourceTypeConstraints2.ResourceType = CatalogResourceTypes.ProjectCollection;
      resourceTypeConstraints2.DependencyConstraints = new DependencyConstraint[0];
      resourceTypeConstraints2.SingleNodeReferenceConstraint = true;
      resourceTypeConstraints2.PropertyConstraints = new PropertyConstraint[1]
      {
        new PropertyConstraint()
        {
          PropertyName = "InstanceId",
          ValueExclusivity = PropertyValueExclusivity.None
        }
      };
      resourceTypeConstraints2.ServiceReferenceConstraints = new ServiceReferenceConstraint[1]
      {
        new ServiceReferenceConstraint()
        {
          ReferenceName = "Location",
          ServiceType = "LocationService"
        }
      };
      resourceTypeConstraints2.DeleteConstraints = new DeleteConstraints()
      {
        CanDeleteRecursively = true,
        CanDeleteNonRecursively = false
      };
      resourceTypeConstraints2.ParentChildConstraints = new ParentChildConstraints()
      {
        AllowedChildren = ParentChildConstraints.AllTypes,
        DeniedChildren = ParentChildConstraints.NoTypes,
        AllowedParents = new Guid[2]
        {
          CatalogResourceTypes.TeamFoundationServerInstance,
          CatalogResourceTypes.ResourceFolder
        },
        DeniedParents = ParentChildConstraints.AllTypes
      };
      FrameworkConstraints.ProjectCollectionConstraints = resourceTypeConstraints2;
      FrameworkConstraints.ResourceFolderConstraints = new CatalogResourceTypeConstraints()
      {
        ResourceType = CatalogResourceTypes.ResourceFolder,
        SingleNodeReferenceConstraint = false,
        DeleteConstraints = new DeleteConstraints()
        {
          CanDeleteRecursively = true,
          CanDeleteNonRecursively = true
        },
        ParentChildConstraints = new ParentChildConstraints()
        {
          AllowedChildren = ParentChildConstraints.AllTypes,
          DeniedChildren = ParentChildConstraints.NoTypes,
          AllowedParents = ParentChildConstraints.AllTypes,
          DeniedParents = ParentChildConstraints.NoTypes
        }
      };
      FrameworkConstraints.GenericLinkConstraints = new CatalogResourceTypeConstraints()
      {
        ResourceType = CatalogResourceTypes.GenericLink,
        SingleNodeReferenceConstraint = false,
        DeleteConstraints = new DeleteConstraints()
        {
          CanDeleteRecursively = true,
          CanDeleteNonRecursively = false
        },
        ParentChildConstraints = new ParentChildConstraints()
        {
          AllowedChildren = ParentChildConstraints.NoTypes,
          DeniedChildren = ParentChildConstraints.AllTypes,
          AllowedParents = ParentChildConstraints.AllTypes,
          DeniedParents = ParentChildConstraints.NoTypes
        }
      };
      CatalogResourceTypeConstraints resourceTypeConstraints3 = new CatalogResourceTypeConstraints();
      resourceTypeConstraints3.ResourceType = CatalogResourceTypes.Machine;
      resourceTypeConstraints3.PropertyConstraints = new PropertyConstraint[1]
      {
        new PropertyConstraint()
        {
          PropertyName = "MachineName",
          ValueExclusivity = PropertyValueExclusivity.EntireCatalog
        }
      };
      resourceTypeConstraints3.SingleNodeReferenceConstraint = false;
      resourceTypeConstraints3.DeleteConstraints = new DeleteConstraints()
      {
        CanDeleteRecursively = true,
        CanDeleteNonRecursively = false
      };
      resourceTypeConstraints3.ParentChildConstraints = new ParentChildConstraints()
      {
        AllowedChildren = ParentChildConstraints.AllTypes,
        DeniedChildren = ParentChildConstraints.NoTypes,
        AllowedParents = new Guid[1]
        {
          CatalogResourceTypes.InfrastructureRoot
        },
        DeniedParents = ParentChildConstraints.AllTypes
      };
      FrameworkConstraints.MachineConstraints = resourceTypeConstraints3;
      FrameworkConstraints.TeamFoundationWebApplicationConstraints = new CatalogResourceTypeConstraints()
      {
        ResourceType = CatalogResourceTypes.TeamFoundationWebApplication,
        SingleNodeReferenceConstraint = false,
        DeleteConstraints = new DeleteConstraints()
        {
          CanDeleteRecursively = true,
          CanDeleteNonRecursively = false
        },
        ParentChildConstraints = new ParentChildConstraints()
        {
          AllowedChildren = ParentChildConstraints.NoTypes,
          DeniedChildren = ParentChildConstraints.AllTypes,
          AllowedParents = new Guid[1]
          {
            CatalogResourceTypes.Machine
          },
          DeniedParents = ParentChildConstraints.AllTypes
        }
      };
      CatalogResourceTypeConstraints resourceTypeConstraints4 = new CatalogResourceTypeConstraints();
      resourceTypeConstraints4.ResourceType = CatalogResourceTypes.SqlAnalysisInstance;
      resourceTypeConstraints4.PropertyConstraints = new PropertyConstraint[2]
      {
        new PropertyConstraint()
        {
          PropertyName = "InstanceName",
          ValueExclusivity = PropertyValueExclusivity.None
        },
        new PropertyConstraint()
        {
          PropertyName = "IsClustered",
          ValueExclusivity = PropertyValueExclusivity.None
        }
      };
      resourceTypeConstraints4.SingleNodeReferenceConstraint = false;
      resourceTypeConstraints4.DeleteConstraints = new DeleteConstraints()
      {
        CanDeleteRecursively = true,
        CanDeleteNonRecursively = false
      };
      resourceTypeConstraints4.ParentChildConstraints = new ParentChildConstraints()
      {
        AllowedChildren = ParentChildConstraints.AllTypes,
        DeniedChildren = ParentChildConstraints.NoTypes,
        AllowedParents = new Guid[1]
        {
          CatalogResourceTypes.Machine
        },
        DeniedParents = ParentChildConstraints.AllTypes
      };
      FrameworkConstraints.SQLAnalysisInstanceConstraints = resourceTypeConstraints4;
      CatalogResourceTypeConstraints resourceTypeConstraints5 = new CatalogResourceTypeConstraints();
      resourceTypeConstraints5.ResourceType = CatalogResourceTypes.SqlReportingInstance;
      resourceTypeConstraints5.PropertyConstraints = new PropertyConstraint[2]
      {
        new PropertyConstraint()
        {
          PropertyName = "InstanceName",
          ValueExclusivity = PropertyValueExclusivity.None
        },
        new PropertyConstraint()
        {
          PropertyName = "IsClustered",
          ValueExclusivity = PropertyValueExclusivity.None
        }
      };
      resourceTypeConstraints5.SingleNodeReferenceConstraint = false;
      resourceTypeConstraints5.DeleteConstraints = new DeleteConstraints()
      {
        CanDeleteRecursively = true,
        CanDeleteNonRecursively = false
      };
      resourceTypeConstraints5.ParentChildConstraints = new ParentChildConstraints()
      {
        AllowedChildren = ParentChildConstraints.AllTypes,
        DeniedChildren = ParentChildConstraints.NoTypes,
        AllowedParents = new Guid[1]
        {
          CatalogResourceTypes.Machine
        },
        DeniedParents = ParentChildConstraints.AllTypes
      };
      FrameworkConstraints.SQLReportingInstanceConstraints = resourceTypeConstraints5;
      FrameworkConstraints.OrganizationalRootConstraints = new CatalogResourceTypeConstraints()
      {
        ResourceType = CatalogResourceTypes.OrganizationalRoot,
        SingleNodeReferenceConstraint = true,
        DeleteConstraints = new DeleteConstraints()
        {
          CanDeleteRecursively = false,
          CanDeleteNonRecursively = false
        },
        ParentChildConstraints = new ParentChildConstraints()
        {
          AllowedChildren = ParentChildConstraints.AllTypes,
          DeniedChildren = ParentChildConstraints.NoTypes,
          AllowedParents = ParentChildConstraints.NoTypes,
          DeniedParents = ParentChildConstraints.AllTypes
        }
      };
      FrameworkConstraints.InfrastructureRootConstraints = new CatalogResourceTypeConstraints()
      {
        ResourceType = CatalogResourceTypes.InfrastructureRoot,
        SingleNodeReferenceConstraint = true,
        DeleteConstraints = new DeleteConstraints()
        {
          CanDeleteRecursively = false,
          CanDeleteNonRecursively = false
        },
        ParentChildConstraints = new ParentChildConstraints()
        {
          AllowedChildren = ParentChildConstraints.AllTypes,
          DeniedChildren = ParentChildConstraints.NoTypes,
          AllowedParents = ParentChildConstraints.NoTypes,
          DeniedParents = ParentChildConstraints.AllTypes
        }
      };
    }
  }
}
