// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitUtils
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class GitUtils
  {
    public static readonly Encoding SafeUnicodeLENoBOM = (Encoding) new UnicodeEncoding(false, false, true);
    private const char c_slash = '/';
    private static readonly char[] s_slashSeparator = new char[1]
    {
      '/'
    };

    public static bool ByteArraysAreEqual(byte[] x, byte[] y, int expectedLength)
    {
      if (x == null || y == null || expectedLength != x.Length || expectedLength != y.Length)
        throw new InvalidOperationException();
      if (x == y)
        return true;
      for (int index = 0; index < expectedLength; ++index)
      {
        if ((int) x[index] != (int) y[index])
          return false;
      }
      return true;
    }

    public static bool CompareByteArrays(
      byte[] b1,
      int b1start,
      byte[] b2,
      int b2start,
      int length)
    {
      if (b1start < 0 || b2start < 0 || length < 0 || b1.Length - b1start < length || b2.Length - b2start < length)
        return false;
      for (int index = 0; index < length; ++index)
      {
        if ((int) b1[b1start + index] != (int) b2[b2start + index])
          return false;
      }
      return true;
    }

    public static bool TryGetByteArrayFromString(
      string objectId,
      int expectedStringLength,
      out byte[] parsedObjectId)
    {
      parsedObjectId = (byte[]) null;
      if (expectedStringLength % 2 != 0)
        return false;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(objectId, nameof (objectId));
      if (objectId.Length != expectedStringLength)
        return false;
      byte num1 = 0;
      parsedObjectId = new byte[expectedStringLength / 2];
      for (int index = 0; index < expectedStringLength; ++index)
      {
        short num2 = (short) objectId[index];
        bool flag = 1 == (index & 1);
        byte num3;
        if (num2 >= (short) 48 && num2 <= (short) 57)
          num3 = (byte) 48;
        else if (num2 >= (short) 65 && num2 <= (short) 70)
        {
          num3 = (byte) 55;
        }
        else
        {
          if (num2 < (short) 97 || num2 > (short) 102)
            return false;
          num3 = (byte) 87;
        }
        if (flag)
        {
          num1 |= (byte) ((uint) num2 - (uint) num3);
          parsedObjectId[index >> 1] = num1;
        }
        else
          num1 = (byte) ((int) num2 - (int) num3 << 4);
      }
      return true;
    }

    public static byte[] ObjectIdFromUTF8Bytes(byte[] bytes, int start = 0)
    {
      if (bytes == null)
        throw new ArgumentNullException(nameof (bytes));
      if (bytes.Length - start < 40)
        throw new ArgumentException(string.Format("There is not enough data to decode an object ID. 40 bytes are required, but only {0} were found.", (object) (bytes.Length - start)), nameof (bytes));
      byte[] numArray = new byte[20];
      byte num1 = 0;
      for (int index = 0; index < 40; ++index)
      {
        byte num2 = bytes[start + index];
        int num3 = 1 == (index & 1) ? 1 : 0;
        byte num4;
        if (num2 >= (byte) 48 && num2 <= (byte) 57)
          num4 = (byte) 48;
        else if (num2 >= (byte) 65 && num2 <= (byte) 70)
        {
          num4 = (byte) 55;
        }
        else
        {
          if (num2 < (byte) 97 || num2 > (byte) 102)
            throw new ArgumentException(string.Format("While decoding an object ID, expected to find a hex digit in ASCII form; found {0} instead.", (object) num2.ToString("x2")), nameof (bytes));
          num4 = (byte) 87;
        }
        if (num3 != 0)
        {
          num1 |= (byte) ((uint) num2 - (uint) num4);
          numArray[index >> 1] = num1;
        }
        else
          num1 = (byte) ((int) num2 - (int) num4 << 4);
      }
      return numArray;
    }

    public static string StringFromByteArray(byte[] byteArray)
    {
      ArgumentUtility.CheckForNull<byte[]>(byteArray, nameof (byteArray));
      StringBuilder builder = new StringBuilder(byteArray.Length * 2);
      GitUtils.AppendBytesToStringBuilder(builder, new ArraySegment<byte>(byteArray));
      return builder.ToString();
    }

    public static void AppendBytesToStringBuilder(StringBuilder builder, ArraySegment<byte> bytes)
    {
      ArgumentUtility.CheckForNull<StringBuilder>(builder, nameof (builder));
      ArgumentUtility.CheckForNull<byte[]>(bytes.Array, "bytes.Array");
      for (int index = 0; index < bytes.Count; ++index)
      {
        int num = (int) bytes.Array[bytes.Offset + index];
        char ch1 = (char) ((num >> 4 & 15) + 48);
        char ch2 = (char) ((num & 15) + 48);
        builder.Append(ch1 >= ':' ? (char) ((uint) ch1 + 39U) : ch1);
        builder.Append(ch2 >= ':' ? (char) ((uint) ch2 + 39U) : ch2);
      }
    }

    public static string CalculateSecurable(Guid projectId, Guid repositoryId, string refName)
    {
      GitUtils.ValidateSecurableInputs(projectId, repositoryId, refName);
      StringBuilder stringBuilder = new StringBuilder("repoV2/");
      if (projectId != Guid.Empty)
      {
        stringBuilder.Append((object) projectId);
        stringBuilder.Append("/");
        if (repositoryId != Guid.Empty)
        {
          stringBuilder.Append(repositoryId.ToString());
          stringBuilder.Append("/");
          if (!string.IsNullOrEmpty(refName))
          {
            if (refName.Length > GitConstants.MaxGitRefNameLength || !refName.StartsWith("refs/", StringComparison.Ordinal))
              throw new InvalidOperationException("Invalid ref name (" + refName + "). " + string.Format("Must be less that {0} characters long, ", (object) GitConstants.MaxGitRefNameLength) + "and start with 'refs/'.");
            refName = refName.TrimEnd('/');
            int num1 = refName.IndexOf('/');
            if (num1 < 1 || num1 == refName.Length - 1)
              throw new InvalidOperationException("The ref name must contain at least two '/'s and not start with a '/'");
            int num2 = refName.IndexOf('/', num1 + 1);
            if (num2 < 0 || num2 == refName.Length - 1)
            {
              stringBuilder.Append(refName);
              stringBuilder.Append('/');
            }
            else
            {
              string[] strArray = refName.Substring(num2 + 1).Split(GitUtils.s_slashSeparator);
              stringBuilder.Append(refName.Substring(0, num2 + 1));
              stringBuilder.EnsureCapacity(stringBuilder.Length + 4 * (refName.Length - (num2 + 1)) - 3 * (strArray.Length - 1) + 1);
              foreach (string s in strArray)
              {
                try
                {
                  stringBuilder.Append(GitUtils.StringFromByteArray(GitUtils.SafeUnicodeLENoBOM.GetBytes(s)));
                }
                catch (EncoderFallbackException ex)
                {
                  throw new ArgumentException("The ref name is invalid an invalid Unicode sequence", nameof (refName), (Exception) ex);
                }
                stringBuilder.Append('/');
              }
            }
          }
        }
      }
      return stringBuilder.ToString();
    }

    public static string CalculateSecurable(
      string teamProjectUri,
      Guid repositoryId,
      string refName)
    {
      Guid projectId;
      GitUtils.ParseSecurableInputs(teamProjectUri, repositoryId, out projectId);
      return GitUtils.CalculateSecurable(projectId, repositoryId, refName);
    }

    private static void ParseSecurableInputs(
      string teamProjectUri,
      Guid repositoryId,
      out Guid projectId)
    {
      if (string.IsNullOrEmpty(teamProjectUri) && repositoryId != Guid.Empty)
        throw new ArgumentException("If you provide a repository Id, then you must provide a team project Uri.", nameof (teamProjectUri));
      ArtifactId artifactId = LinkingUtilities.DecodeUri(teamProjectUri);
      projectId = Guid.Empty;
      Guid.TryParse(artifactId.ToolSpecificId, out projectId);
    }

    private static void ValidateSecurableInputs(Guid projectId, Guid repositoryId, string refName)
    {
      if (projectId == Guid.Empty && repositoryId != Guid.Empty)
        throw new ArgumentException("If you provide a repository Id, then you must provide a project Id.", nameof (projectId));
      if (!string.IsNullOrEmpty(refName) && repositoryId == Guid.Empty)
        throw new ArgumentException("If you provide a ref name, then you must provide a repository Id.", nameof (repositoryId));
    }

    public static string AppendUrl(string url, string pathToAppend) => UriUtility.Combine(UriUtility.EnsureEndsWithPathSeparator(url), pathToAppend, true).ToString();

    public static GitRepositoryPermissions GetPermissionFlags(string[] permissionList)
    {
      GitRepositoryPermissions permissionFlags = GitRepositoryPermissions.None;
      foreach (string permission in permissionList)
        permissionFlags |= (GitRepositoryPermissions) Enum.Parse(typeof (GitRepositoryPermissions), permission);
      return permissionFlags;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetFriendlyBranchName(string refName) => !string.IsNullOrEmpty(refName) && refName.StartsWith("refs/heads/", StringComparison.Ordinal) ? refName.Substring("refs/heads/".Length) : refName;
  }
}
