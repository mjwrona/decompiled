// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.BindableOperationFinder
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Builder
{
  internal class BindableOperationFinder
  {
    private Dictionary<IEdmEntityType, List<IEdmOperation>> _map = new Dictionary<IEdmEntityType, List<IEdmOperation>>();
    private Dictionary<IEdmEntityType, List<IEdmOperation>> _collectionMap = new Dictionary<IEdmEntityType, List<IEdmOperation>>();

    public BindableOperationFinder(IEdmModel model)
    {
      foreach (IGrouping<IEdmType, IEdmOperation> grouping in model.SchemaElements.OfType<IEdmOperation>().Where<IEdmOperation>((Func<IEdmOperation, bool>) (op =>
      {
        if (!op.IsBound)
          return false;
        return op.Parameters.First<IEdmOperationParameter>().Type.TypeKind() == EdmTypeKind.Entity || op.Parameters.First<IEdmOperationParameter>().Type.TypeKind() == EdmTypeKind.Collection;
      })).GroupBy<IEdmOperation, IEdmType>((Func<IEdmOperation, IEdmType>) (op => op.Parameters.First<IEdmOperationParameter>().Type.Definition)))
      {
        if (grouping.Key is IEdmEntityType key1)
          this._map[key1] = grouping.ToList<IEdmOperation>();
        if (grouping.Key is IEdmCollectionType key2 && key2.ElementType.Definition is IEdmEntityType definition)
        {
          List<IEdmOperation> edmOperationList;
          if (this._collectionMap.TryGetValue(definition, out edmOperationList))
            edmOperationList.AddRange((IEnumerable<IEdmOperation>) grouping);
          else
            this._collectionMap[definition] = grouping.ToList<IEdmOperation>();
        }
      }
    }

    public virtual IEnumerable<IEdmOperation> FindOperations(IEdmEntityType entityType) => BindableOperationFinder.GetTypeHierarchy(entityType).SelectMany<IEdmEntityType, IEdmOperation>(new Func<IEdmEntityType, IEnumerable<IEdmOperation>>(this.FindDeclaredOperations));

    public virtual IEnumerable<IEdmOperation> FindOperationsBoundToCollection(
      IEdmEntityType entityType)
    {
      return BindableOperationFinder.GetTypeHierarchy(entityType).SelectMany<IEdmEntityType, IEdmOperation>(new Func<IEdmEntityType, IEnumerable<IEdmOperation>>(this.FindDeclaredOperationsBoundToCollection));
    }

    private static IEnumerable<IEdmEntityType> GetTypeHierarchy(IEdmEntityType entityType)
    {
      for (IEdmEntityType current = entityType; current != null; current = current.BaseEntityType())
        yield return current;
    }

    private IEnumerable<IEdmOperation> FindDeclaredOperations(IEdmEntityType entityType)
    {
      List<IEdmOperation> edmOperationList;
      return this._map.TryGetValue(entityType, out edmOperationList) ? (IEnumerable<IEdmOperation>) edmOperationList : (IEnumerable<IEdmOperation>) Enumerable.Empty<IEdmFunction>();
    }

    private IEnumerable<IEdmOperation> FindDeclaredOperationsBoundToCollection(
      IEdmEntityType entityType)
    {
      List<IEdmOperation> edmOperationList;
      return this._collectionMap.TryGetValue(entityType, out edmOperationList) ? (IEnumerable<IEdmOperation>) edmOperationList : (IEnumerable<IEdmOperation>) Enumerable.Empty<IEdmFunction>();
    }
  }
}
