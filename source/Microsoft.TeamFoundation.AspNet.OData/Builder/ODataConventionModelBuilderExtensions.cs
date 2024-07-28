// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.ODataConventionModelBuilderExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.ComponentModel;

namespace Microsoft.AspNet.OData.Builder
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ODataConventionModelBuilderExtensions
  {
    public static ODataConventionModelBuilder EnableLowerCamelCase(
      this ODataConventionModelBuilder builder)
    {
      return builder != null ? builder.EnableLowerCamelCase(NameResolverOptions.ProcessReflectedPropertyNames | NameResolverOptions.ProcessDataMemberAttributePropertyNames | NameResolverOptions.ProcessExplicitPropertyNames) : throw Error.ArgumentNull(nameof (builder));
    }

    public static ODataConventionModelBuilder EnableLowerCamelCase(
      this ODataConventionModelBuilder builder,
      NameResolverOptions options)
    {
      if (builder == null)
        throw Error.ArgumentNull(nameof (builder));
      builder.OnModelCreating += new Action<ODataConventionModelBuilder>(new LowerCamelCaser(options).ApplyLowerCamelCase);
      return builder;
    }
  }
}
