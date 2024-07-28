// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.JustInTimeProcessExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class JustInTimeProcessExtensions
  {
    public static Microsoft.TeamFoundation.Build2.Server.JustInTimeProcess ToServerJustInTimeProcess(
      this Microsoft.TeamFoundation.Build.WebApi.JustInTimeProcess source)
    {
      return source == null ? (Microsoft.TeamFoundation.Build2.Server.JustInTimeProcess) null : new Microsoft.TeamFoundation.Build2.Server.JustInTimeProcess();
    }

    public static Microsoft.TeamFoundation.Build.WebApi.JustInTimeProcess ToWebApiJustInTimeProcess(
      this Microsoft.TeamFoundation.Build2.Server.JustInTimeProcess source,
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      if (source == null)
        return (Microsoft.TeamFoundation.Build.WebApi.JustInTimeProcess) null;
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      return new Microsoft.TeamFoundation.Build.WebApi.JustInTimeProcess(securedObject);
    }
  }
}
