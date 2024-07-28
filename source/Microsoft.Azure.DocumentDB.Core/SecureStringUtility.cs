// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SecureStringUtility
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace Microsoft.Azure.Documents
{
  internal static class SecureStringUtility
  {
    [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Disposable object returned by method")]
    public static SecureString ConvertToSecureString(string unsecureStr)
    {
      if (string.IsNullOrEmpty(unsecureStr))
        throw new ArgumentNullException(nameof (unsecureStr));
      SecureString secureString = new SecureString();
      foreach (char c in unsecureStr.ToCharArray())
        secureString.AppendChar(c);
      return secureString;
    }
  }
}
