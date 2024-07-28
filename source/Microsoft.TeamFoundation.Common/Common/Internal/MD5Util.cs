// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.MD5Util
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class MD5Util
  {
    private static readonly byte[] s_nullHash = Array.Empty<byte>();
    private static int s_fipsAlgorithmPolicyEnabled = -1;
    private const int c_enabled = 1;
    private const int c_disabled = 0;
    private const int c_unknown = -1;

    public static bool CanCreateMD5Provider
    {
      get
      {
        if (MD5Util.s_fipsAlgorithmPolicyEnabled == -1)
        {
          using (MD5Util.TryCreateMD5Provider())
            ;
        }
        return MD5Util.s_fipsAlgorithmPolicyEnabled == 0;
      }
    }

    public static byte[] CalculateMD5(string fileName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(fileName, nameof (fileName));
      if (!MD5Util.CanCreateMD5Provider)
        return MD5Util.s_nullHash;
      using (FileStream fileStream = new FileStream(fileName, FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
        return MD5Util.CalculateMD5((Stream) fileStream);
    }

    public static byte[] CalculateMD5(byte[] content)
    {
      ArgumentUtility.CheckForNull<byte[]>(content, nameof (content));
      using (MD5 md5Provider = MD5Util.TryCreateMD5Provider())
        return md5Provider != null ? md5Provider.ComputeHash(content) : MD5Util.s_nullHash;
    }

    public static byte[] CalculateMD5(Stream stream) => MD5Util.CalculateMD5(stream, false);

    public static byte[] CalculateMD5(Stream stream, bool rewind)
    {
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      MD5 md5Provider = MD5Util.TryCreateMD5Provider();
      byte[] md5;
      if (md5Provider == null)
      {
        md5 = MD5Util.s_nullHash;
      }
      else
      {
        using (md5Provider)
        {
          long offset = 0;
          if (rewind)
            offset = stream.Position;
          md5 = md5Provider.ComputeHash(stream);
          if (rewind)
            stream.Seek(offset, SeekOrigin.Begin);
        }
      }
      return md5;
    }

    public static MD5 TryCreateMD5Provider()
    {
      MD5 md5Provider = (MD5) null;
      if (MD5Util.s_fipsAlgorithmPolicyEnabled == 0 || MD5Util.s_fipsAlgorithmPolicyEnabled == -1)
      {
        if (Environment.GetEnvironmentVariable("DD_SUITES_FIPS") != null)
        {
          MD5Util.s_fipsAlgorithmPolicyEnabled = 1;
        }
        else
        {
          try
          {
            TeamFoundationTrace.Verbose("Creating an MD5 provider");
            md5Provider = (MD5) new MD5CryptoServiceProvider();
            MD5Util.s_fipsAlgorithmPolicyEnabled = 0;
            TeamFoundationTrace.Verbose("MD5 provider has been created");
          }
          catch (InvalidOperationException ex)
          {
            MD5Util.s_fipsAlgorithmPolicyEnabled = 1;
            TeamFoundationTrace.Error("Failed to create an MD5 crypto service provider. Please check if Fips policy is disabled.");
          }
        }
      }
      else
        TeamFoundationTrace.Error("Cannot create an MD5 crypto service provider. Please check if Fips policy is disabled.");
      return md5Provider;
    }
  }
}
