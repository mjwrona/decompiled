// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.BlobReferenceExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public static class BlobReferenceExtensions
  {
    public static void CheckReferencesBatchCreatePermission(
      this IEnumerable<BlobReference> references,
      IVssRequestContext requestContext,
      IClock clock)
    {
      foreach (BlobReference reference in references)
        reference.CheckReferenceCreatePermission(requestContext, clock);
    }

    public static void CheckReferencesBatchDeletePermission(
      this IEnumerable<IdBlobReference> idReferences,
      IVssRequestContext requestContext)
    {
      foreach (IdBlobReference idReference in idReferences)
        idReference.CheckIdReferenceDeletePermission(requestContext);
    }

    public static void CheckReferenceCreatePermission(
      this BlobReference reference,
      IVssRequestContext requestContext,
      IClock clock)
    {
      reference.Match((Action<IdBlobReference>) (idRef => idRef.CheckIdReferenceCreatePermission(requestContext)), (Action<KeepUntilBlobReference>) (keepUntilRef =>
      {
        TimeSpan timeSpan = keepUntilRef.KeepUntil.Subtract(clock.Now.UtcDateTime);
        timeSpan = timeSpan.Subtract(TimeSpan.FromHours(1.0));
        double requestedDays = Math.Max(0.0, timeSpan.TotalDays);
        requestContext.AssertPermission(KeepUntilSecurityNamespace.Token, (int) KeepUntilSecurityNamespace.GetPermissionForDays(requestedDays), KeepUntilSecurityNamespace.NamespaceId);
      }));
    }

    public static void CheckIdReferenceDeletePermission(
      this IdBlobReference idReference,
      IVssRequestContext requestContext)
    {
      string referenceSecurityToken = BlobReferenceExtensions.ComputeIdReferenceSecurityToken(idReference);
      requestContext.AssertPermission(referenceSecurityToken, 2, BlobNamespace.NamespaceId);
    }

    public static void CheckIdReferenceCreatePermission(
      this IdBlobReference idReference,
      IVssRequestContext requestContext)
    {
      string referenceSecurityToken = BlobReferenceExtensions.ComputeIdReferenceSecurityToken(idReference);
      requestContext.AssertPermission(referenceSecurityToken, 4, BlobNamespace.NamespaceId);
    }

    private static string ComputeIdReferenceSecurityToken(IdBlobReference idReference)
    {
      if (idReference.Scope == null)
        throw new ArgumentException("Id reference without scope. Reference: " + idReference.ToString());
      string str1 = BlobNamespace.ReferencesToken.Trim(BlobNamespace.PathSeparator);
      string str2 = idReference.Scope.Trim(BlobNamespace.PathSeparator);
      return string.Format("{0}{1}{2}", (object) str1, (object) BlobNamespace.PathSeparator, (object) str2);
    }
  }
}
