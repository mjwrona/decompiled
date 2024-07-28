// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.BlobRequest
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.Azure.Storage.Blob.Protocol
{
  public static class BlobRequest
  {
    public static void WriteSharedAccessIdentifiers(
      SharedAccessBlobPolicies sharedAccessPolicies,
      Stream outputStream)
    {
      Request.WriteSharedAccessIdentifiers<SharedAccessBlobPolicy>((IDictionary<string, SharedAccessBlobPolicy>) sharedAccessPolicies, outputStream, (Action<SharedAccessBlobPolicy, XmlWriter>) ((policy, writer) =>
      {
        writer.WriteElementString("Start", SharedAccessSignatureHelper.GetDateTimeOrEmpty(policy.SharedAccessStartTime));
        writer.WriteElementString("Expiry", SharedAccessSignatureHelper.GetDateTimeOrEmpty(policy.SharedAccessExpiryTime));
        writer.WriteElementString("Permission", SharedAccessBlobPolicy.PermissionsToString(policy.Permissions));
      }));
    }

    public static void WriteBlockListBody(IEnumerable<PutBlockListItem> blocks, Stream outputStream)
    {
      CommonUtility.AssertNotNull(nameof (blocks), (object) blocks);
      using (XmlWriter xmlWriter = XmlWriter.Create(outputStream, new XmlWriterSettings()
      {
        Encoding = Encoding.UTF8
      }))
      {
        xmlWriter.WriteStartElement("BlockList");
        foreach (PutBlockListItem block in blocks)
        {
          if (block.SearchMode == BlockSearchMode.Committed)
            xmlWriter.WriteElementString("Committed", block.Id);
          else if (block.SearchMode == BlockSearchMode.Uncommitted)
            xmlWriter.WriteElementString("Uncommitted", block.Id);
          else if (block.SearchMode == BlockSearchMode.Latest)
            xmlWriter.WriteElementString("Latest", block.Id);
        }
        xmlWriter.WriteEndDocument();
      }
    }

    internal static void ApplyCustomerProvidedKey(
      StorageRequestMessage request,
      BlobCustomerProvidedKey customerProvidedKey,
      bool isSource)
    {
      if (customerProvidedKey == null)
        return;
      if (isSource)
      {
        request.Headers.Add("x-ms-source-encryption-key", customerProvidedKey.Key);
        request.Headers.Add("x-ms-source-encryption-key-sha256", customerProvidedKey.KeySHA256);
        request.Headers.Add("x-ms-source-encryption-algorithm", customerProvidedKey.EncryptionAlgorithm);
      }
      else
      {
        request.Headers.Add("x-ms-encryption-key", customerProvidedKey.Key);
        request.Headers.Add("x-ms-encryption-key-sha256", customerProvidedKey.KeySHA256);
        request.Headers.Add("x-ms-encryption-algorithm", customerProvidedKey.EncryptionAlgorithm);
      }
    }

    internal static void ApplyCustomerProvidedKeyOrEncryptionScope(
      StorageRequestMessage request,
      BlobRequestOptions options,
      bool isSource)
    {
      BlobCustomerProvidedKey customerProvidedKey = options?.CustomerProvidedKey;
      string encryptionScope = options?.EncryptionScope;
      if (customerProvidedKey != null)
      {
        BlobRequest.ApplyCustomerProvidedKey(request, customerProvidedKey, isSource);
      }
      else
      {
        if (encryptionScope == null)
          return;
        request.Headers.Add("x-ms-encryption-scope", encryptionScope);
      }
    }

    internal static void VerifyHttpsCustomerProvidedKey(Uri uri, BlobRequestOptions options)
    {
      if (options?.CustomerProvidedKey != null && !string.Equals(uri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException("Cannot use client-provided key requests without HTTPS.");
    }
  }
}
