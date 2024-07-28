// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Exceptions.Helpers.MavenExceptionMappings
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Maven.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Maven.Server.Exceptions.Helpers
{
  public static class MavenExceptionMappings
  {
    public static readonly Dictionary<Type, HttpStatusCode> HttpExceptionMapping = PackagingExceptionMappings.WithOverrides((IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (MavenFileNotFoundException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (MavenPomParsingException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (MavenPomSizeLimitExceededException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (MavenFileExtensionException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (MavenGavException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (MavenInvalidFilenameException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (MavenInvalidFilePathException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArtifactsWithNonUniqueSnapshotVersionNotSupportedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (MavenServiceCanceledDueToClientException),
        HttpStatusCode.RequestTimeout
      },
      {
        typeof (PackageVersionDeletedException),
        HttpStatusCode.Conflict
      },
      {
        typeof (MavenArtifactVersionFileExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (MavenPackageDeletedException),
        HttpStatusCode.Conflict
      },
      {
        typeof (UnrecognizedMavenFilePathException),
        HttpStatusCode.InternalServerError
      }
    }).ToDictionary<KeyValuePair<Type, HttpStatusCode>, Type, HttpStatusCode>((Func<KeyValuePair<Type, HttpStatusCode>, Type>) (kv => kv.Key), (Func<KeyValuePair<Type, HttpStatusCode>, HttpStatusCode>) (kv => kv.Value));
  }
}
