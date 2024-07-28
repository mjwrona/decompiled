// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.FipsCompliantSha
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Security.Cryptography;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class FipsCompliantSha
  {
    public static readonly ThreadLocal<HashAlgorithm> Sha256 = new ThreadLocal<HashAlgorithm>(new Func<HashAlgorithm>(FipsCompliantSha.CreateEncryptor));

    private static HashAlgorithm CreateEncryptor() => HashAlgorithm.Create(typeof (SHA256CryptoServiceProvider).AssemblyQualifiedName);
  }
}
