// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Lfs.LfsPointerFile
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server.Lfs
{
  internal class LfsPointerFile
  {
    public static readonly string VersionMarker = "version https://git-lfs.github.com/spec/v1\n";
    private Sha256Id m_id;
    private const char c_unixLineEnding = '\n';
    private const char c_keyValueSeparator = ' ';
    private const char c_hashingMethodSeparator = ':';
    private const string c_objectIdKey = "oid";
    private const int c_pointerReaderBufferSize = 4096;

    public IReadOnlyDictionary<string, string> Values { get; private set; }

    public Sha256Id ObjectId
    {
      get
      {
        if (this.m_id != (Sha256Id) null)
          return this.m_id;
        string str;
        if (!this.Values.TryGetValue("oid", out str))
          return (Sha256Id) null;
        int num = str.IndexOf(':');
        if (num == -1)
          return (Sha256Id) null;
        this.m_id = new Sha256Id(str.Substring(num + 1));
        return this.m_id;
      }
    }

    public static LfsPointerFile TryParse(Stream contentStream)
    {
      ArgumentUtility.CheckForNull<Stream>(contentStream, nameof (contentStream));
      string ifPointerDetected = LfsPointerFile.ReadToEndIfPointerDetected(contentStream);
      if (ifPointerDetected == null)
        return (LfsPointerFile) null;
      Dictionary<string, string> values = LfsPointerFile.ParseValues(ifPointerDetected);
      return values == null ? (LfsPointerFile) null : new LfsPointerFile((IDictionary<string, string>) values);
    }

    private LfsPointerFile(IDictionary<string, string> values) => this.Values = (IReadOnlyDictionary<string, string>) new ReadOnlyDictionary<string, string>(values);

    private static string ReadToEndIfPointerDetected(Stream contentStream)
    {
      using (StreamReader streamReader = new StreamReader(contentStream, GitEncodingUtil.SafeUtf8NoBom, false, 4096, true))
      {
        try
        {
          char[] buffer = new char[LfsPointerFile.VersionMarker.Length];
          streamReader.ReadBlock(buffer, 0, LfsPointerFile.VersionMarker.Length);
          string str = new string(buffer);
          return str != LfsPointerFile.VersionMarker ? (string) null : str + streamReader.ReadToEnd();
        }
        catch (DecoderFallbackException ex)
        {
          return (string) null;
        }
      }
    }

    private static Dictionary<string, string> ParseValues(string content)
    {
      string[] strArray = content.Split('\n');
      if (strArray[strArray.Length - 1] != string.Empty)
        return (Dictionary<string, string>) null;
      Dictionary<string, string> values = new Dictionary<string, string>();
      int num = strArray.Length - 1;
      for (int index = 0; index < num; ++index)
      {
        string str = strArray[index];
        int length = str.IndexOf(' ');
        string key = str.Substring(0, length);
        if (values.ContainsKey(key))
          return (Dictionary<string, string>) null;
        values.Add(key, str.Substring(length + 1));
      }
      return values;
    }
  }
}
