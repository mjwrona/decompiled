// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobEncryptionPolicy
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.KeyVault.Core;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace Microsoft.Azure.Storage.Blob
{
  public sealed class BlobEncryptionPolicy
  {
    internal BlobEncryptionMode EncryptionMode { get; set; }

    public IKey Key { get; private set; }

    public IKeyResolver KeyResolver { get; private set; }

    public BlobEncryptionPolicy(IKey key, IKeyResolver keyResolver)
    {
      this.Key = key;
      this.KeyResolver = keyResolver;
      this.EncryptionMode = BlobEncryptionMode.FullBlob;
    }

    internal Stream DecryptBlob(
      Stream userProvidedStream,
      IDictionary<string, string> metadata,
      out ICryptoTransform transform,
      bool? requireEncryption,
      byte[] iv = null,
      bool noPadding = false)
    {
      CommonUtility.AssertNotNull(nameof (metadata), (object) metadata);
      string str = (string) null;
      bool flag = metadata.TryGetValue("encryptiondata", out str);
      if (requireEncryption.HasValue && requireEncryption.Value && !flag)
        throw new StorageException("Encryption data does not exist. If you do not want to decrypt the data, please do not set the RequireEncryption flag on request options.", (Exception) null)
        {
          IsRetryable = false
        };
      try
      {
        if (str != null)
        {
          BlobEncryptionData encryptionData = JsonConvert.DeserializeObject<BlobEncryptionData>(str);
          CommonUtility.AssertNotNull("ContentEncryptionIV", (object) encryptionData.ContentEncryptionIV);
          CommonUtility.AssertNotNull("EncryptedKey", (object) encryptionData.WrappedContentKey.EncryptedKey);
          if (encryptionData.EncryptionAgent.Protocol != "1.0")
            throw new StorageException("Invalid Encryption Agent. This version of the client library does not understand the Encryption Agent set on the blob.", (Exception) null)
            {
              IsRetryable = false
            };
          if (this.Key == null && this.KeyResolver == null)
            throw new StorageException("Key and Resolver are not initialized. Decryption requires either of them to be initialized.", (Exception) null)
            {
              IsRetryable = false
            };
          byte[] numArray;
          if (this.KeyResolver != null)
          {
            IKey keyEncryptionKey = CommonUtility.RunWithoutSynchronizationContext<IKey>((Func<IKey>) (() => this.KeyResolver.ResolveKeyAsync(encryptionData.WrappedContentKey.KeyId, CancellationToken.None).GetAwaiter().GetResult()));
            CommonUtility.AssertNotNull("KeyEncryptionKey", (object) keyEncryptionKey);
            numArray = CommonUtility.RunWithoutSynchronizationContext<byte[]>((Func<byte[]>) (() => keyEncryptionKey.UnwrapKeyAsync(encryptionData.WrappedContentKey.EncryptedKey, encryptionData.WrappedContentKey.Algorithm, CancellationToken.None).GetAwaiter().GetResult()));
          }
          else if (this.Key.Kid == encryptionData.WrappedContentKey.KeyId)
            numArray = CommonUtility.RunWithoutSynchronizationContext<byte[]>((Func<byte[]>) (() => this.Key.UnwrapKeyAsync(encryptionData.WrappedContentKey.EncryptedKey, encryptionData.WrappedContentKey.Algorithm, CancellationToken.None).GetAwaiter().GetResult()));
          else
            throw new StorageException("Key mismatch. The key id stored on the service does not match the specified key.", (Exception) null)
            {
              IsRetryable = false
            };
          if (encryptionData.EncryptionAgent.EncryptionAlgorithm == EncryptionAlgorithm.AES_CBC_256)
          {
            using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
            {
              cryptoServiceProvider.IV = iv != null ? iv : encryptionData.ContentEncryptionIV;
              cryptoServiceProvider.Key = numArray;
              if (noPadding)
                cryptoServiceProvider.Padding = PaddingMode.None;
              transform = cryptoServiceProvider.CreateDecryptor();
              return (Stream) new CryptoStream(userProvidedStream, transform, CryptoStreamMode.Write);
            }
          }
          else
            throw new StorageException("Invalid Encryption Algorithm found on the resource. This version of the client library does not support the specified encryption algorithm.", (Exception) null)
            {
              IsRetryable = false
            };
        }
        else
        {
          transform = (ICryptoTransform) null;
          return userProvidedStream;
        }
      }
      catch (JsonException ex)
      {
        throw new StorageException("Error while de-serializing the encryption metadata string from the wire.", (Exception) ex)
        {
          IsRetryable = false
        };
      }
      catch (StorageException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new StorageException("Decryption logic threw error. Please check the inner exception for more details.", ex)
        {
          IsRetryable = false
        };
      }
    }

    internal static Stream WrapUserStreamWithDecryptStream(
      CloudBlob blob,
      Stream userProvidedStream,
      BlobRequestOptions options,
      BlobAttributes attributes,
      bool rangeRead,
      out ICryptoTransform transform,
      long? endOffset = null,
      long? userSpecifiedLength = null,
      int discardFirst = 0,
      bool bufferIV = false)
    {
      if (!rangeRead)
        return options.EncryptionPolicy.DecryptBlob((Stream) new NonCloseableStream(userProvidedStream), attributes.Metadata, out transform, options.RequireEncryption, noPadding: blob.BlobType == BlobType.PageBlob);
      bool noPadding = blob.BlobType == BlobType.PageBlob || endOffset.HasValue && endOffset.Value < attributes.Properties.Length - 16L;
      transform = (ICryptoTransform) null;
      return (Stream) new BlobDecryptStream(userProvidedStream, attributes.Metadata, userSpecifiedLength, discardFirst, bufferIV, noPadding, options.EncryptionPolicy, options.RequireEncryption);
    }

    internal ICryptoTransform CreateAndSetEncryptionContext(
      IDictionary<string, string> metadata,
      bool noPadding)
    {
      CommonUtility.AssertNotNull(nameof (metadata), (object) metadata);
      if (this.Key == null)
        throw new InvalidOperationException("Key is not initialized. Encryption requires it to be initialized.", (Exception) null);
      using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
      {
        if (noPadding)
          aesProvider.Padding = PaddingMode.None;
        BlobEncryptionData blobEncryptionData = new BlobEncryptionData();
        blobEncryptionData.EncryptionAgent = new EncryptionAgent("1.0", EncryptionAlgorithm.AES_CBC_256);
        Tuple<byte[], string> tuple = CommonUtility.RunWithoutSynchronizationContext<Tuple<byte[], string>>((Func<Tuple<byte[], string>>) (() => this.Key.WrapKeyAsync(aesProvider.Key, (string) null, CancellationToken.None).GetAwaiter().GetResult()));
        blobEncryptionData.WrappedContentKey = new WrappedKey(this.Key.Kid, tuple.Item1, tuple.Item2);
        blobEncryptionData.EncryptionMode = this.EncryptionMode.ToString();
        blobEncryptionData.KeyWrappingMetadata = (IDictionary<string, string>) new Dictionary<string, string>();
        blobEncryptionData.KeyWrappingMetadata["EncryptionLibrary"] = ".NET 11.2.3";
        blobEncryptionData.ContentEncryptionIV = aesProvider.IV;
        metadata["encryptiondata"] = JsonConvert.SerializeObject((object) blobEncryptionData, Formatting.None);
        return aesProvider.CreateEncryptor();
      }
    }

    internal long GetEncryptedLength(long unencryptedLength, bool noPadding) => noPadding ? unencryptedLength : unencryptedLength + (16L - unencryptedLength % 16L);
  }
}
