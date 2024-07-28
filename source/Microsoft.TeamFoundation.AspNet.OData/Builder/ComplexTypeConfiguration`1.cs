// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.ComplexTypeConfiguration`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

namespace Microsoft.AspNet.OData.Builder
{
  public class ComplexTypeConfiguration<TComplexType> : StructuralTypeConfiguration<TComplexType> where TComplexType : class
  {
    private ComplexTypeConfiguration _configuration;
    private ODataModelBuilder _modelBuilder;

    internal ComplexTypeConfiguration(ComplexTypeConfiguration configuration)
      : base((StructuralTypeConfiguration) configuration)
    {
    }

    internal ComplexTypeConfiguration(ODataModelBuilder modelBuilder)
      : this(modelBuilder, new ComplexTypeConfiguration(modelBuilder, typeof (TComplexType)))
    {
    }

    internal ComplexTypeConfiguration(
      ODataModelBuilder modelBuilder,
      ComplexTypeConfiguration configuration)
      : base((StructuralTypeConfiguration) configuration)
    {
      this._modelBuilder = modelBuilder;
      this._configuration = configuration;
    }

    public ComplexTypeConfiguration<TComplexType> Abstract()
    {
      this._configuration.IsAbstract = new bool?(true);
      return this;
    }

    public ComplexTypeConfiguration BaseType => this._configuration.BaseType;

    public ComplexTypeConfiguration<TComplexType> DerivesFromNothing()
    {
      this._configuration.DerivesFromNothing();
      return this;
    }

    public ComplexTypeConfiguration<TComplexType> DerivesFrom<TBaseType>() where TBaseType : class
    {
      this._configuration.DerivesFrom(this._modelBuilder.ComplexType<TBaseType>()._configuration);
      return this;
    }
  }
}
