// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.BindingParameterConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData.Edm;

namespace Microsoft.AspNet.OData.Builder
{
  public class BindingParameterConfiguration : ParameterConfiguration
  {
    public const string DefaultBindingParameterName = "bindingParameter";

    public BindingParameterConfiguration(string name, IEdmTypeConfiguration parameterType)
      : base(name, parameterType)
    {
      EdmTypeKind kind = parameterType.Kind;
      if (kind == EdmTypeKind.Collection)
        kind = (parameterType as CollectionTypeConfiguration).ElementType.Kind;
      if (kind != EdmTypeKind.Entity)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (parameterType), SRResources.InvalidBindingParameterType, (object) parameterType.FullName);
    }
  }
}
