// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IEncryptionHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.IO;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal interface IEncryptionHelper
  {
    byte[] GetBytes(IVssRequestContext requestContext, StrongBoxItemInfo item);

    Stream PrepareBytes(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      byte[] plainBytes);

    Stream EncryptContent(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      Aes fileEncrypt,
      Stream data,
      BinaryWriter keyWriter,
      bool useFileService);

    void CheckPermission(IVssRequestContext requestContext, string token, int permissions);

    Stream RetrieveFileInternal(
      IVssRequestContext requestContext,
      StrongBoxItemInfo itemInfo,
      out long encryptedLength,
      out long unencryptedLength);

    void DeleteFileNoThrow(IVssRequestContext requestContext, int fileId);
  }
}
