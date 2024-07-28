// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TFUtil
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TFUtil
  {
    private static readonly ICollection<string> UnsafeFileExtensions = (ICollection<string>) new HashSet<string>((IEnumerable<string>) new string[6]
    {
      ".exe",
      ".cmd",
      ".bat",
      ".ps1",
      ".com",
      ".wsf"
    }, (IEqualityComparer<string>) TFStringComparer.FileType);
    private const int cMaxDirLength = 247;
    private static readonly TFUtil.DomainInfo[] m_knownDomains = new TFUtil.DomainInfo[8]
    {
      new TFUtil.DomainInfo()
      {
        ExactMatch = "dev.azure.com",
        UrlFormatVersion = TFUtil.UrlFormat.Azure,
        IsDevFabric = false
      },
      new TFUtil.DomainInfo()
      {
        ExactMatch = "ssh.dev.azure.com",
        UrlFormatVersion = TFUtil.UrlFormat.Azure,
        IsDevFabric = false
      },
      new TFUtil.DomainInfo()
      {
        SuffixMatch = ".visualstudio.com",
        UrlFormatVersion = TFUtil.UrlFormat.VisualStudio,
        IsDevFabric = false
      },
      new TFUtil.DomainInfo()
      {
        ExactMatch = "codex.azure.com",
        UrlFormatVersion = TFUtil.UrlFormat.Azure,
        IsDevFabric = false
      },
      new TFUtil.DomainInfo()
      {
        ExactMatch = "ssh.codex.azure.com",
        UrlFormatVersion = TFUtil.UrlFormat.Azure,
        IsDevFabric = false
      },
      new TFUtil.DomainInfo()
      {
        ExactMatch = "codedev.ms",
        UrlFormatVersion = TFUtil.UrlFormat.Azure,
        IsDevFabric = true
      },
      new TFUtil.DomainInfo()
      {
        ExactMatch = "ssh.codedev.ms",
        UrlFormatVersion = TFUtil.UrlFormat.Azure,
        IsDevFabric = true
      },
      new TFUtil.DomainInfo()
      {
        SuffixMatch = ".vsts.me",
        UrlFormatVersion = TFUtil.UrlFormat.VisualStudio,
        IsDevFabric = true
      }
    };

    public static Uri GetApplicationInstanceUri(TfsConnection server)
    {
      if (!(server is TfsTeamProjectCollection projectCollection))
        return server.Uri;
      return projectCollection.CatalogNode != null ? projectCollection.ConfigurationServer.Uri : projectCollection.Uri;
    }

    public static System.Diagnostics.Process BeginRunApp(string appExe, string arguments) => System.Diagnostics.Process.Start(new ProcessStartInfo(appExe, arguments));

    public static void SafeLaunchApp(string app)
    {
      try
      {
        System.Diagnostics.Process.Start(app);
      }
      catch (Exception ex)
      {
        if ((ex is Win32Exception win32Exception ? (win32Exception.ErrorCode == -2147467259 ? 1 : 0) : 0) != 0)
        {
          int num1 = (int) UIHost.ShowError(ClientResources.InternetExplorerNotFound((object) app));
        }
        else
        {
          int num2 = (int) UIHost.ShowError(ex.Message);
        }
      }
    }

    public static bool SafeOpenFile(string filename)
    {
      ArgumentUtility.CheckForNull<string>(filename, nameof (filename));
      if (!File.Exists(filename))
        throw new InvalidOperationException(ClientResources.FileNotFound((object) filename));
      bool flag = true;
      string extension = Path.GetExtension(filename);
      if (TFUtil.UnsafeFileExtensions.Contains(extension))
        flag = UIHost.ShowMessageBox(ClientResources.UnsafeFileOpenPrompt(), (string) null, (string) null, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
      if (flag)
        System.Diagnostics.Process.Start(filename);
      return flag;
    }

    public static void CheckAndLaunchUri(string uriString) => TFUtil.SafeLaunchApp(UriUtility.GetAbsoluteUriFromString(uriString).AbsoluteUri);

    public static void CheckForNull(object var, string varName)
    {
      if (var == null)
        throw new ArgumentNullException(varName);
    }

    public static void CheckStringForNullOrEmpty(string stringVar, string stringVarName)
    {
      TFUtil.CheckForNull((object) stringVar, stringVarName);
      if (stringVar.Length == 0)
        throw new ArgumentException(ClientResources.EmptyStringNotAllowed(), stringVarName);
    }

    public static string GetDirectoryName(string path)
    {
      TFUtil.CheckStringForNullOrEmpty(path, nameof (path));
      return Path.GetDirectoryName(path) ?? path;
    }

    public static void AddXmlAttribute(XmlNode node, string attrName, string value)
    {
      if (value == null)
        return;
      XmlAttribute attribute = node.OwnerDocument.CreateAttribute((string) null, attrName, (string) null);
      node.Attributes.Append(attribute);
      attribute.InnerText = value;
    }

    public static string PrepareForFormat(string s)
    {
      if (s.IndexOfAny(new char[2]{ '{', '}' }) >= 0)
        s = s.Replace("{", "{{").Replace("}", "}}");
      return s;
    }

    private static string AdjustFileName(string name, int maxLength)
    {
      TFUtil.CheckStringForNullOrEmpty(name, nameof (name));
      if (maxLength < 1)
        throw new ArgumentOutOfRangeException(nameof (maxLength));
      if (name.Length <= maxLength)
        return name;
      string extension = Path.GetExtension(name);
      int length1 = name.Length;
      int length2 = extension.Length;
      int startIndex = maxLength - extension.Length;
      return startIndex <= 0 ? (string) null : name.Remove(startIndex) + extension;
    }

    public static string GetTempFileName(string dir, string template, string baseName)
    {
      string empty = string.Empty;
      bool flag = true;
      int num = 256;
      TFUtil.CheckStringForNullOrEmpty(dir, nameof (dir));
      string fullPath = Path.GetFullPath(dir);
      int length = fullPath.Length;
      if (length > 247)
        throw new ArgumentOutOfRangeException(nameof (dir));
      baseName = TFUtil.PrepareForFormat(baseName);
      string path = (string) null;
      string str = (string) null;
      FileStream fileStream = (FileStream) null;
      for (int index = 1; index <= num; ++index)
      {
        try
        {
          if (flag)
            str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, template, (object) baseName, (object) empty);
          int maxLength = 259 - length - 1 - empty.Length;
          str = TFUtil.AdjustFileName(str, maxLength);
          path = str != null ? Path.Combine(fullPath, str) : throw new TeamFoundationServerException(ClientResources.CannotGetTempFileName((object) baseName));
          fileStream = new FileStream(path, FileMode.CreateNew);
        }
        catch (IOException ex1)
        {
          if (File.Exists(path))
          {
            if (DateTime.Compare(DateTime.Now, File.GetCreationTime(path).AddHours(1.0)) > 0)
            {
              flag = false;
              try
              {
                File.Delete(path);
              }
              catch (Exception ex2)
              {
                flag = true;
              }
            }
          }
          else
            throw;
        }
        finally
        {
          fileStream?.Close();
        }
        if (fileStream != null)
          return path;
        empty = index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      throw new TeamFoundationServerException(ClientResources.CannotGetTempFileName((object) baseName));
    }

    public static string RemoveInvalidFileNameChars(string fileName)
    {
      char[] anyOf = new char[41]
      {
        char.MinValue,
        '\u0001',
        '\u0002',
        '\u0003',
        '\u0004',
        '\u0005',
        '\u0006',
        '\a',
        '\b',
        '\t',
        '\n',
        '\v',
        '\f',
        '\r',
        '\u000E',
        '\u000F',
        '\u0010',
        '\u0011',
        '\u0012',
        '\u0013',
        '\u0014',
        '\u0015',
        '\u0016',
        '\u0017',
        '\u0018',
        '\u0019',
        '\u001A',
        '\u001B',
        '\u001C',
        '\u001D',
        '\u001E',
        '\u001F',
        '"',
        '/',
        ':',
        '<',
        '>',
        '\\',
        '|',
        '*',
        '?'
      };
      int startIndex = 0;
      int num;
      if ((num = fileName.IndexOfAny(anyOf)) < 0)
        return fileName;
      StringBuilder stringBuilder = new StringBuilder();
      for (; num >= 0; num = fileName.IndexOfAny(anyOf, startIndex))
      {
        if (num > startIndex)
          stringBuilder.Append(fileName.Substring(startIndex, num - startIndex));
        startIndex = num + 1;
      }
      if (startIndex < fileName.Length)
        stringBuilder.Append(fileName.Substring(startIndex));
      return stringBuilder.ToString();
    }

    public static byte[] CalculateMD5(string fileName) => MD5Util.CalculateMD5(fileName);

    public static byte[] CalculateMD5(Stream stream) => MD5Util.CalculateMD5(stream);

    public static long CopyStream(Stream source, Stream dest, byte[] buffer) => TFUtil.CopyStream(source, dest, buffer, (TFUtil.CopyStreamCallback) null);

    public static long CopyStream(
      Stream source,
      Stream dest,
      byte[] buffer,
      TFUtil.CopyStreamCallback progressCallback)
    {
      Stopwatch stopwatch = (Stopwatch) null;
      if (progressCallback != null)
      {
        stopwatch = new Stopwatch();
        stopwatch.Start();
      }
      long bytesWritten = 0;
      long num = 0;
      int count;
      while ((count = source.Read(buffer, 0, buffer.Length)) > 0)
      {
        dest.Write(buffer, 0, count);
        bytesWritten += (long) count;
        if (progressCallback != null && stopwatch.ElapsedMilliseconds > num + 1000L)
        {
          progressCallback(bytesWritten);
          num = stopwatch.ElapsedMilliseconds;
        }
      }
      if (progressCallback != null && num != 0L)
        progressCallback(bytesWritten);
      return bytesWritten;
    }

    public static string LocaleSensitiveToUpper(string original)
    {
      if (string.IsNullOrEmpty(original))
        return string.Empty;
      CultureInfo currentCulture = CultureInfo.CurrentCulture;
      return currentCulture.LCID == 31 || currentCulture.LCID == 1055 || currentCulture.LCID == 44 || currentCulture.LCID == 30764 || currentCulture.LCID == 1068 ? original.ToUpperInvariant() : original.ToUpper(currentCulture);
    }

    public static bool IsHostedServer(string hostName) => !string.IsNullOrEmpty(hostName) && ((IEnumerable<TFUtil.DomainInfo>) TFUtil.m_knownDomains).Any<TFUtil.DomainInfo>((Func<TFUtil.DomainInfo, bool>) (kd => kd.IsMatch(hostName)));

    public static bool IsHostedServer(Uri uri) => TFUtil.IsHostedServer(uri?.Host);

    public static bool IsLocalDevFabric(string hostName) => !string.IsNullOrEmpty(hostName) && ((IEnumerable<TFUtil.DomainInfo>) TFUtil.m_knownDomains).Any<TFUtil.DomainInfo>((Func<TFUtil.DomainInfo, bool>) (kd => kd.IsMatch(hostName) && kd.IsDevFabric));

    public static bool IsLegacyHostedServer(string hostName) => !string.IsNullOrEmpty(hostName) && ((IEnumerable<TFUtil.DomainInfo>) TFUtil.m_knownDomains).Any<TFUtil.DomainInfo>((Func<TFUtil.DomainInfo, bool>) (kd => kd.IsMatch(hostName) && kd.UrlFormatVersion == TFUtil.UrlFormat.VisualStudio));

    public static bool IsLegacyHostedServer(Uri uri) => TFUtil.IsLegacyHostedServer(uri.Host);

    public static string GetUniqueAccountKey(Uri uri)
    {
      string lowerInvariant1 = uri.Host.ToLowerInvariant();
      if (TFUtil.IsLegacyHostedServer(uri) || !TFUtil.IsHostedServer(uri))
        return lowerInvariant1;
      string[] strArray = uri.AbsolutePath.Split(new string[1]
      {
        "/"
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length == 0)
        return lowerInvariant1;
      string lowerInvariant2 = strArray[0].ToLowerInvariant();
      return lowerInvariant1 + "/" + lowerInvariant2;
    }

    internal static void CopyRegistryBranch(RegistryKey source, RegistryKey destination)
    {
      if (source == null || destination == null)
        return;
      foreach (string subKeyName in source.GetSubKeyNames())
      {
        using (RegistryKey source1 = source.OpenSubKey(subKeyName))
        {
          using (RegistryKey subKey = destination.CreateSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree))
            TFUtil.CopyRegistryBranch(source1, subKey);
        }
      }
      foreach (string valueName in source.GetValueNames())
      {
        RegistryValueKind valueKind = source.GetValueKind(valueName);
        object obj = source.GetValue(valueName, (object) null, RegistryValueOptions.DoNotExpandEnvironmentNames);
        destination.SetValue(valueName, obj, valueKind);
      }
    }

    internal static bool CanUseMD5 => MD5Util.CanCreateMD5Provider;

    public delegate void CopyStreamCallback(long bytesWritten);

    internal class AccessControlEntryComparer : 
      IComparer<AccessControlEntry>,
      IEqualityComparer<AccessControlEntry>
    {
      public static TFUtil.AccessControlEntryComparer Instance = new TFUtil.AccessControlEntryComparer();

      private AccessControlEntryComparer()
      {
      }

      public int Compare(AccessControlEntry x, AccessControlEntry y)
      {
        if (x == y)
          return 0;
        if (x == null && y != null)
          return -1;
        if (x != null && y == null)
          return 1;
        int num;
        if ((num = IdentityDescriptorComparer.Instance.Compare(x.Descriptor, y.Descriptor)) != 0)
          return num;
        if (x.Allow > y.Allow)
          return 1;
        if (x.Allow < y.Allow)
          return -1;
        if (x.Deny > y.Deny)
          return 1;
        if (x.Deny < y.Deny)
          return -1;
        if (x.ExtendedInfo == null && y.ExtendedInfo == null)
          return 0;
        if (x.ExtendedInfo == null && y.ExtendedInfo != null)
          return -1;
        if (x.ExtendedInfo != null && y.ExtendedInfo == null || x.ExtendedInfo.EffectiveAllow > y.ExtendedInfo.EffectiveAllow)
          return 1;
        if (x.ExtendedInfo.EffectiveAllow < y.ExtendedInfo.EffectiveAllow)
          return -1;
        if (x.ExtendedInfo.EffectiveDeny > y.ExtendedInfo.EffectiveDeny)
          return 1;
        if (x.ExtendedInfo.EffectiveDeny < y.ExtendedInfo.EffectiveDeny)
          return -1;
        if (x.ExtendedInfo.InheritedAllow > y.ExtendedInfo.InheritedAllow)
          return 1;
        if (x.ExtendedInfo.InheritedAllow < y.ExtendedInfo.InheritedAllow)
          return -1;
        if (x.ExtendedInfo.InheritedDeny > y.ExtendedInfo.InheritedDeny)
          return 1;
        return x.ExtendedInfo.InheritedDeny < y.ExtendedInfo.InheritedDeny ? -1 : 0;
      }

      public bool Equals(AccessControlEntry x, AccessControlEntry y) => this.Compare(x, y) == 0;

      public int GetHashCode(AccessControlEntry obj) => obj.GetHashCode();
    }

    private enum UrlFormat
    {
      VisualStudio,
      Azure,
    }

    private struct DomainInfo
    {
      public string ExactMatch;
      public string SuffixMatch;
      public TFUtil.UrlFormat UrlFormatVersion;
      public bool IsDevFabric;

      public bool IsMatch(string hostName)
      {
        if (this.SuffixMatch != null && hostName.EndsWith(this.SuffixMatch, StringComparison.OrdinalIgnoreCase))
          return true;
        return this.ExactMatch != null && hostName.Equals(this.ExactMatch, StringComparison.OrdinalIgnoreCase);
      }
    }
  }
}
