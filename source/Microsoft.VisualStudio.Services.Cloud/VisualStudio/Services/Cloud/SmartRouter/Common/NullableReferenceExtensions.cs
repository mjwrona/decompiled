// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common.NullableReferenceExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common
{
  internal static class NullableReferenceExtensions
  {
    public static T CheckArgumentIsNotNull<T>(this T? value, string paramName) where T : class => (object) value != null ? value : throw new ArgumentNullException(paramName);

    public static string CheckArgumentIsNotNullOrEmpty(this string? value, string paramName)
    {
      value = value.CheckArgumentIsNotNull<string>(paramName);
      if (value.Length == 0)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(value, paramName);
        throw new ArgumentException();
      }
      return value;
    }

    public static IVssRequestContext CheckRequestContext(
      this IVssRequestContext? requestContext,
      bool requireDeploymentContext = false)
    {
      requestContext = requestContext.CheckArgumentIsNotNull<IVssRequestContext>(nameof (requestContext));
      if (requireDeploymentContext)
        requestContext.CheckServiceHostType(TeamFoundationHostType.Deployment);
      return requestContext;
    }
  }
}
