// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.ParameterConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using System;

namespace Microsoft.AspNet.OData.Builder
{
  public abstract class ParameterConfiguration
  {
    protected ParameterConfiguration(string name, IEdmTypeConfiguration parameterType)
    {
      if (name == null)
        throw Error.ArgumentNull(nameof (name));
      if (parameterType == null)
        throw Error.ArgumentNull(nameof (parameterType));
      this.Name = name;
      this.TypeConfiguration = parameterType;
      Type elementType;
      this.Nullable = TypeHelper.IsCollection(parameterType.ClrType, out elementType) ? EdmLibHelpers.IsNullable(elementType) : EdmLibHelpers.IsNullable(parameterType.ClrType);
    }

    public string Name { get; protected set; }

    public IEdmTypeConfiguration TypeConfiguration { get; protected set; }

    public bool Nullable { get; set; }

    public bool IsOptional { get; protected set; }

    public string DefaultValue { get; protected set; }

    public ParameterConfiguration Optional()
    {
      this.IsOptional = true;
      return this;
    }

    public ParameterConfiguration Required()
    {
      this.IsOptional = false;
      return this;
    }

    public ParameterConfiguration HasDefaultValue(string defaultValue)
    {
      this.IsOptional = true;
      this.DefaultValue = defaultValue;
      return this;
    }
  }
}
