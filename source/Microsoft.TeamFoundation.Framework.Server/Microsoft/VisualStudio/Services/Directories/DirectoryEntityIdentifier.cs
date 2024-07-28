// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryEntityIdentifier
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories
{
  internal abstract class DirectoryEntityIdentifier
  {
    protected readonly int version;
    internal static readonly DirectoryEntityIdentifierComparer Comparer = new DirectoryEntityIdentifierComparer();

    internal DirectoryEntityIdentifier(int version) => this.version = version;

    internal int Version => this.version;

    internal abstract string Encode();

    internal static bool TryParse(string encodedId, out DirectoryEntityIdentifier decodedId)
    {
      decodedId = (DirectoryEntityIdentifier) null;
      if (encodedId == null)
        return false;
      string[] strArray = encodedId.Split('.');
      if (strArray.Length < 4 || !strArray[0].Equals("vss") || !strArray[1].Equals("ds"))
        return false;
      string str = strArray[2];
      int result;
      if (!str.StartsWith("v") || !int.TryParse(str.Substring(1), out result) || result <= 0)
        return false;
      if (result == 1)
      {
        if (strArray.Length != 6)
          return false;
        string source = strArray[3];
        string type = strArray[4];
        string id = strArray[5];
        decodedId = (DirectoryEntityIdentifier) new DirectoryEntityIdentifierV1(source, type, id);
        return true;
      }
      if (result <= 1)
        return false;
      decodedId = (DirectoryEntityIdentifier) new FutureDirectoryEntityIdentifier(result, encodedId);
      return true;
    }

    internal static IList<KeyValuePair<string, DirectoryEntityIdentifier>> TryParse(
      IEnumerable<string> encodedIds)
    {
      DirectoryEntityIdentifier decodedId;
      return (IList<KeyValuePair<string, DirectoryEntityIdentifier>>) encodedIds.Select<string, KeyValuePair<string, DirectoryEntityIdentifier>>((Func<string, KeyValuePair<string, DirectoryEntityIdentifier>>) (encodedId =>
      {
        DirectoryEntityIdentifier.TryParse(encodedId, out decodedId);
        return new KeyValuePair<string, DirectoryEntityIdentifier>(encodedId, decodedId);
      })).ToList<KeyValuePair<string, DirectoryEntityIdentifier>>();
    }
  }
}
