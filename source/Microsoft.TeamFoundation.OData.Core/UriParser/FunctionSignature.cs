// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.FunctionSignature
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  internal sealed class FunctionSignature
  {
    private readonly IEdmTypeReference[] argumentTypes;
    private FunctionSignature.CreateArgumentTypeWithFacets[] createArgumentTypesWithFacets;

    internal FunctionSignature(
      IEdmTypeReference[] argumentTypes,
      FunctionSignature.CreateArgumentTypeWithFacets[] createArgumentTypesWithFacets)
    {
      this.argumentTypes = argumentTypes;
      this.createArgumentTypesWithFacets = createArgumentTypesWithFacets;
    }

    internal IEdmTypeReference[] ArgumentTypes => this.argumentTypes;

    internal IEdmTypeReference GetArgumentTypeWithFacets(int index, int? precision, int? scale)
    {
      if (this.createArgumentTypesWithFacets == null)
        return this.argumentTypes[index];
      FunctionSignature.CreateArgumentTypeWithFacets argumentTypesWithFacet = this.createArgumentTypesWithFacets[index];
      return argumentTypesWithFacet == null ? this.argumentTypes[index] : argumentTypesWithFacet(precision, scale);
    }

    internal delegate IEdmTypeReference CreateArgumentTypeWithFacets(int? precision, int? scale);
  }
}
