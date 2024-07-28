// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.BinderBase
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  internal abstract class BinderBase
  {
    protected MetadataBinder.QueryTokenVisitor bindMethod;
    protected BindingState state;

    protected BinderBase(MetadataBinder.QueryTokenVisitor bindMethod, BindingState state)
    {
      ExceptionUtils.CheckArgumentNotNull<MetadataBinder.QueryTokenVisitor>(bindMethod, nameof (bindMethod));
      ExceptionUtils.CheckArgumentNotNull<BindingState>(state, nameof (state));
      this.bindMethod = bindMethod;
      this.state = state;
    }

    protected ODataUriResolver Resolver => this.state.Configuration.Resolver;
  }
}
