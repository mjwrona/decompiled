// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ISasTokenRequestService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DefaultServiceImplementation(typeof (SasTokenRequestService))]
  public interface ISasTokenRequestService : IVssFrameworkService
  {
    string GetSasToken(
      IVssRequestContext requestContext,
      Uri resourceUri,
      SasRequestPermissions permissions,
      TimeSpan expiration);

    string GetSasToken(
      IVssRequestContext requestContext,
      Uri resourceUri,
      SasRequestPermissions permissions,
      TimeSpan expiration,
      SasTokenVersion version);

    string GetSasToken(
      IVssRequestContext requestContext,
      string resourceUri,
      SasRequestPermissions permissions,
      TimeSpan expiration);

    string GetSasToken(
      IVssRequestContext requestContext,
      string resourceUri,
      SasRequestPermissions permissions,
      TimeSpan expiration,
      SasTokenVersion version);

    string EncryptToken(IVssRequestContext requestContext, string sasToken);

    string DecryptToken(IVssRequestContext requestContext, string encryptedSasToken);
  }
}
