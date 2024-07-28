// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ISessionContainer
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Collections;

namespace Microsoft.Azure.Documents
{
  internal interface ISessionContainer
  {
    string ResolveGlobalSessionToken(DocumentServiceRequest entity);

    ISessionToken ResolvePartitionLocalSessionToken(
      DocumentServiceRequest entity,
      string partitionKeyRangeId);

    void ClearTokenByCollectionFullname(string collectionFullname);

    void ClearTokenByResourceId(string resourceId);

    void SetSessionToken(DocumentServiceRequest request, INameValueCollection responseHeader);

    void SetSessionToken(
      string collectionRid,
      string collectionFullname,
      INameValueCollection responseHeaders);
  }
}
