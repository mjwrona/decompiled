// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildLogReferenceExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class BuildLogReferenceExtensions
  {
    public static BuildLogReference ToWebApiLogReference(
      this LogReference srvLogReference,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvLogReference == null)
        return (BuildLogReference) null;
      return new BuildLogReference(securedObject)
      {
        Id = srvLogReference.Id,
        Type = srvLogReference.Type
      };
    }

    public static LogReference ToServerLogReference(this BuildLogReference webApiLogReference)
    {
      if (webApiLogReference == null)
        return (LogReference) null;
      return new LogReference()
      {
        Id = webApiLogReference.Id,
        Type = webApiLogReference.Type
      };
    }
  }
}
