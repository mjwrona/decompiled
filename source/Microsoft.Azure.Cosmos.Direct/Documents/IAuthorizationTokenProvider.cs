// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IAuthorizationTokenProvider
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Collections;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal interface IAuthorizationTokenProvider
  {
    ValueTask<(string token, string payload)> GetUserAuthorizationAsync(
      string resourceAddress,
      string resourceType,
      string requestVerb,
      INameValueCollection headers,
      AuthorizationTokenType tokenType);

    Task AddSystemAuthorizationHeaderAsync(
      DocumentServiceRequest request,
      string federationId,
      string verb,
      string resourceId);
  }
}
