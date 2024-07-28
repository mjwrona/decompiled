// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.UnqualifiedODataUriResolver
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  public class UnqualifiedODataUriResolver : ODataUriResolver
  {
    public override IEnumerable<IEdmOperation> ResolveUnboundOperations(
      IEdmModel model,
      string identifier)
    {
      return identifier.Contains(".") ? base.ResolveUnboundOperations(model, identifier) : UnqualifiedODataUriResolver.FindAcrossModels<IEdmOperation>(model, identifier, this.EnableCaseInsensitive).Where<IEdmOperation>((Func<IEdmOperation, bool>) (operation => !operation.IsBound));
    }

    public override IEnumerable<IEdmOperation> ResolveBoundOperations(
      IEdmModel model,
      string identifier,
      IEdmType bindingType)
    {
      return identifier.Contains(".") ? base.ResolveBoundOperations(model, identifier, bindingType) : UnqualifiedODataUriResolver.FindAcrossModels<IEdmOperation>(model, identifier, this.EnableCaseInsensitive).Where<IEdmOperation>((Func<IEdmOperation, bool>) (operation => operation.IsBound && operation.Parameters.Any<IEdmOperationParameter>() && operation.HasEquivalentBindingType(bindingType)));
    }

    private static IEnumerable<T> FindAcrossModels<T>(
      IEdmModel model,
      string qualifiedName,
      bool caseInsensitive)
      where T : IEdmSchemaElement
    {
      Func<IEdmModel, IEnumerable<T>> func = (Func<IEdmModel, IEnumerable<T>>) (refModel => refModel.SchemaElements.OfType<T>().Where<T>((Func<T, bool>) (e => string.Equals(qualifiedName, e.Name, caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))));
      IEnumerable<T> first = func(model);
      foreach (IEdmModel referencedModel in model.ReferencedModels)
        first.Concat<T>(func(referencedModel));
      return first;
    }
  }
}
