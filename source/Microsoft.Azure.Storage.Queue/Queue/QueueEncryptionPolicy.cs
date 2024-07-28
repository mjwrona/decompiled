// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.QueueEncryptionPolicy
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using Microsoft.Azure.KeyVault.Core;
using Microsoft.Azure.Storage.Core.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;

namespace Microsoft.Azure.Storage.Queue
{
  public sealed class QueueEncryptionPolicy
  {
    public IKey Key { get; private set; }

    public IKeyResolver KeyResolver { get; private set; }

    public QueueEncryptionPolicy(IKey key, IKeyResolver keyResolver)
    {
      this.Key = key;
      this.KeyResolver = keyResolver;
    }

    internal string EncryptMessage(byte[] inputMessage)
    {
      CommonUtility.AssertNotNull(nameof (inputMessage), (object) inputMessage);
      if (this.Key == null)
        throw new InvalidOperationException("Key is not initialized. Encryption requires it to be initialized.", (Exception) null);
      CloudQueueEncryptedMessage encryptedMessage = new CloudQueueEncryptedMessage();
      EncryptionData encryptionData = new EncryptionData()
      {
        EncryptionAgent = new EncryptionAgent("1.0", EncryptionAlgorithm.AES_CBC_256),
        KeyWrappingMetadata = (IDictionary<string, string>) new Dictionary<string, string>()
      };
      encryptionData.KeyWrappingMetadata["EncryptionLibrary"] = ".NET 11.2.3";
      using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider())
      {
        encryptionData.ContentEncryptionIV = myAes.IV;
        Tuple<byte[], string> tuple = CommonUtility.RunWithoutSynchronizationContext<Tuple<byte[], string>>((Func<Tuple<byte[], string>>) (() => this.Key.WrapKeyAsync(myAes.Key, (string) null, CancellationToken.None).GetAwaiter().GetResult()));
        encryptionData.WrappedContentKey = new WrappedKey(this.Key.Kid, tuple.Item1, tuple.Item2);
        using (ICryptoTransform encryptor = myAes.CreateEncryptor())
          encryptedMessage.EncryptedMessageContents = Convert.ToBase64String(encryptor.TransformFinalBlock(inputMessage, 0, inputMessage.Length));
        encryptedMessage.EncryptionData = encryptionData;
        return JsonConvert.SerializeObject((object) encryptedMessage);
      }
    }

    internal byte[] DecryptMessage(string inputMessage, bool? requireEncryption)
    {
      CommonUtility.AssertNotNull(nameof (inputMessage), (object) inputMessage);
      try
      {
        CloudQueueEncryptedMessage encryptedMessage = JsonConvert.DeserializeObject<CloudQueueEncryptedMessage>(inputMessage);
        if (requireEncryption.HasValue && requireEncryption.Value && encryptedMessage.EncryptionData == null)
          throw new StorageException("Encryption data does not exist. If you do not want to decrypt the data, please do not set the RequireEncryption flag on request options.", (Exception) null)
          {
            IsRetryable = false
          };
        if (encryptedMessage.EncryptionData == null)
          return Convert.FromBase64String(encryptedMessage.EncryptedMessageContents);
        EncryptionData encryptionData = encryptedMessage.EncryptionData;
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
          CommonUtility.AssertNotNull("keyEncryptionKey", (object) keyEncryptionKey);
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
            cryptoServiceProvider.Key = numArray;
            cryptoServiceProvider.IV = encryptionData.ContentEncryptionIV;
            byte[] inputBuffer = Convert.FromBase64String(encryptedMessage.EncryptedMessageContents);
            using (ICryptoTransform decryptor = cryptoServiceProvider.CreateDecryptor())
              return decryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
          }
        }
        else
          throw new StorageException("Invalid Encryption Algorithm found on the resource. This version of the client library does not support the specified encryption algorithm.", (Exception) null)
          {
            IsRetryable = false
          };
      }
      catch (JsonException ex)
      {
        throw new StorageException("Error while de-serializing the encrypted queue message string from the wire. Please check inner exception for more details.", (Exception) ex)
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
  }
}
