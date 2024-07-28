// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Evaluation.ODataMetadataSelector
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace Microsoft.OData.Evaluation
{
  public abstract class ODataMetadataSelector
  {
    public virtual IEnumerable<IEdmNavigationProperty> SelectNavigationProperties(
      IEdmStructuredType type,
      IEnumerable<IEdmNavigationProperty> navigationProperties)
    {
      return navigationProperties;
    }

    public virtual IEnumerable<IEdmOperation> SelectBindableOperations(
      IEdmStructuredType type,
      IEnumerable<IEdmOperation> bindableOperations)
    {
      return bindableOperations;
    }

    public virtual IEnumerable<IEdmStructuralProperty> SelectStreamProperties(
      IEdmStructuredType type,
      IEnumerable<IEdmStructuralProperty> selectedStreamProperties)
    {
      return selectedStreamProperties;
    }
  }
}
