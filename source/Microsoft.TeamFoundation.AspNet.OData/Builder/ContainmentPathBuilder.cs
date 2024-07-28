// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.ContainmentPathBuilder
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Builder
{
  internal class ContainmentPathBuilder
  {
    private List<ODataPathSegment> _segments;

    public Microsoft.AspNet.OData.Routing.ODataPath TryComputeCanonicalContainingPath(Microsoft.AspNet.OData.Routing.ODataPath path)
    {
      this._segments = path.Segments.ToList<ODataPathSegment>();
      this.RemoveAllTypeCasts();
      this.RemovePathSegmentsAfterTheLastNavigationProperty();
      this.RemoveRedundantContainingPathSegments();
      this.AddTypeCastsIfNecessary();
      if (this._segments.Count > 0)
        this._segments.RemoveAt(this._segments.Count - 1);
      return new Microsoft.AspNet.OData.Routing.ODataPath((IEnumerable<ODataPathSegment>) this._segments);
    }

    private void RemovePathSegmentsAfterTheLastNavigationProperty()
    {
      ODataPathSegment odataPathSegment = (ODataPathSegment) this._segments.OfType<NavigationPropertySegment>().LastOrDefault<NavigationPropertySegment>();
      List<ODataPathSegment> odataPathSegmentList = new List<ODataPathSegment>();
      foreach (ODataPathSegment segment in this._segments)
      {
        odataPathSegmentList.Add(segment);
        if (segment == odataPathSegment)
          break;
      }
      this._segments = odataPathSegmentList;
    }

    private void RemoveRedundantContainingPathSegments()
    {
      this._segments.Reverse();
      navigationPropertySegment = (NavigationPropertySegment) null;
      List<ODataPathSegment> odataPathSegmentList = new List<ODataPathSegment>();
      foreach (ODataPathSegment segment in this._segments)
      {
        if (segment is NavigationPropertySegment navigationPropertySegment)
        {
          EdmNavigationSourceKind navigationSourceKind = navigationPropertySegment.NavigationSource.NavigationSourceKind();
          if (navigationPropertySegment.NavigationProperty.TargetMultiplicity() == EdmMultiplicity.Many)
          {
            if (navigationSourceKind == EdmNavigationSourceKind.EntitySet)
              break;
          }
          if (navigationSourceKind == EdmNavigationSourceKind.Singleton)
            break;
        }
        odataPathSegmentList.Insert(0, segment);
      }
      if (navigationPropertySegment != null)
      {
        IEdmNavigationSource navigationSource = navigationPropertySegment.NavigationSource;
        if (navigationSource.NavigationSourceKind() == EdmNavigationSourceKind.Singleton)
        {
          SingletonSegment singletonSegment = new SingletonSegment((IEdmSingleton) navigationSource);
          odataPathSegmentList.Insert(0, (ODataPathSegment) singletonSegment);
        }
        else
        {
          EntitySetSegment entitySetSegment = new EntitySetSegment((IEdmEntitySet) navigationSource);
          odataPathSegmentList.Insert(0, (ODataPathSegment) entitySetSegment);
        }
      }
      this._segments = odataPathSegmentList;
    }

    private void RemoveAllTypeCasts()
    {
      List<ODataPathSegment> odataPathSegmentList = new List<ODataPathSegment>();
      foreach (ODataPathSegment segment in this._segments)
      {
        if (!(segment is TypeSegment))
          odataPathSegmentList.Add(segment);
      }
      this._segments = odataPathSegmentList;
    }

    private void AddTypeCastsIfNecessary()
    {
      IEdmEntityType edmEntityType = (IEdmEntityType) null;
      List<ODataPathSegment> odataPathSegmentList = new List<ODataPathSegment>();
      foreach (ODataPathSegment segment in this._segments)
      {
        if (segment is NavigationPropertySegment navigationPropertySegment && edmEntityType != null && edmEntityType.FindProperty(navigationPropertySegment.NavigationProperty.Name) == null)
        {
          TypeSegment typeSegment = new TypeSegment((IEdmType) navigationPropertySegment.NavigationProperty.DeclaringType, (IEdmNavigationSource) null);
          odataPathSegmentList.Add((ODataPathSegment) typeSegment);
        }
        odataPathSegmentList.Add(segment);
        IEdmEntityType targetEntityType = ContainmentPathBuilder.GetTargetEntityType(segment);
        if (targetEntityType != null)
          edmEntityType = targetEntityType;
      }
      this._segments = odataPathSegmentList;
    }

    private static IEdmEntityType GetTargetEntityType(ODataPathSegment segment)
    {
      switch (segment)
      {
        case EntitySetSegment entitySetSegment:
          return entitySetSegment.EntitySet.EntityType();
        case SingletonSegment singletonSegment:
          return singletonSegment.Singleton.EntityType();
        case NavigationPropertySegment navigationPropertySegment:
          return navigationPropertySegment.NavigationSource.EntityType();
        default:
          return (IEdmEntityType) null;
      }
    }
  }
}
