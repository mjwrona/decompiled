// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.AzureFrontDoorUrlSigner
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class AzureFrontDoorUrlSigner : IUrlSigner
  {
    private const string AlgorithmIdSha256 = "2";
    private readonly ITimeProvider TimeProvider;
    private readonly HMAC HashAlgorithm;

    internal AzureFrontDoorUrlSigner(SecureString key, string keyId, ITimeProvider timeProvider)
    {
      this.KeyId = keyId;
      this.TimeProvider = timeProvider;
      this.HashAlgorithm = (HMAC) new HMACSHA256(key.ToByteArray());
    }

    public AzureFrontDoorUrlSigner(SecureString key, string keyId)
      : this(key, keyId, (ITimeProvider) new DefaultTimeProvider())
    {
    }

    public string KeyId { get; private set; }

    public Uri Sign(Uri uri, DateTime expiryTimeUtc)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      if (!uri.IsAbsoluteUri)
        throw new ArgumentException("Expected the parameter uri to be absolute uri");
      if (expiryTimeUtc.Kind != DateTimeKind.Utc)
        throw new ArgumentException("Must be a UTC date.", nameof (expiryTimeUtc));
      string expiry = !(expiryTimeUtc <= PrimitiveExtensions.UnixEpoch) ? expiryTimeUtc.ToUnixEpochTime().ToString("D") : throw new ArgumentException(string.Format("Expiry times must be later than {0:u}", (object) PrimitiveExtensions.UnixEpoch));
      string hash = this.CreateHash(uri.AbsolutePath, expiry);
      string str = this.AppendTluValues(uri, expiry, hash);
      return new Uri(uri.AbsoluteUri + str);
    }

    private string CreateHash(string absolutePath, string expiry) => Convert.ToBase64String(this.HashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(absolutePath + expiry)));

    private string AppendTluValues(Uri uri, string expiry, string messageAuthticationCode) => string.Format("{0}P1={1}", (object) (char) (uri.Query == "" ? (int) '?' : (int) '&'), (object) HttpUtility.UrlEncode(expiry)) + "&P2=" + HttpUtility.UrlEncode(this.KeyId) + "&P3=" + HttpUtility.UrlEncode("2") + "&P4=" + HttpUtility.UrlEncode(messageAuthticationCode);
  }
}
