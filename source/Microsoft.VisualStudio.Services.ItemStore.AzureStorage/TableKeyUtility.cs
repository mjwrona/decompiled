// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.AzureStorage.TableKeyUtility
// Assembly: Microsoft.VisualStudio.Services.ItemStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DF52255-B389-4C6F-82CF-18DDB4745F9C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.AzureStorage.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.ItemStore.AzureStorage
{
  public static class TableKeyUtility
  {
    private const char ElementSeperator = '{';
    private const char FinalElementSeperator = '}';
    private const char TerminatingSeperator = '~';

    public static string MustBeLessThanElement(
      string initialValue,
      TableKeyUtility.EqualityType searchType)
    {
      StringBuilder source = new StringBuilder(initialValue);
      source.ReplaceLast('}', '{');
      switch (searchType)
      {
        case TableKeyUtility.EqualityType.ImmediateChildren:
          source.Append('~');
          break;
        case TableKeyUtility.EqualityType.DeepChildren:
          source.Append('}');
          break;
      }
      return source.ToString();
    }

    public static string MustBeGreaterThanElement(
      string initialValue,
      TableKeyUtility.EqualityType searchType)
    {
      StringBuilder source = new StringBuilder(initialValue);
      source.ReplaceLast('}', '{');
      switch (searchType)
      {
        case TableKeyUtility.EqualityType.ImmediateChildren:
          source.Append('}');
          break;
        case TableKeyUtility.EqualityType.DeepChildren:
          source.Append('{');
          break;
      }
      return source.ToString();
    }

    public static string RowKeyFromLocator(Locator locator) => TableKeyUtility.GetEncodedStringBuilder(locator).ReplaceLast('{', '}').ToString();

    public static string RowKeyPrefixFromLocator(Locator locator) => TableKeyUtility.GetEncodedStringBuilder(locator).ToString();

    private static StringBuilder GetEncodedStringBuilder(Locator locator)
    {
      StringBuilder encodedStringBuilder = new StringBuilder(".");
      foreach (string pathSegment in (IEnumerable<string>) locator.PathSegments)
        encodedStringBuilder.Append('{').Append(pathSegment.EncodeStringToBase64Table());
      return encodedStringBuilder;
    }

    public static string LocatorPathFromRowKey(string rowKey)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string encodedString in ((IEnumerable<string>) rowKey.Split('{', '}')).ToList<string>().Skip<string>(1))
      {
        if (!string.IsNullOrEmpty(encodedString))
          stringBuilder.Append("/").Append(TableKeyUtility.DecodeStringFromBase64Table(encodedString));
      }
      return stringBuilder.ToString();
    }

    public static string DecodeStringFromBase64Table(string encodedString)
    {
      char[] charArray = encodedString.ToCharArray();
      TableKeyUtility.MutateBase64TableToBase64(charArray);
      return StrictEncodingWithBOM.UTF8.GetString(Convert.FromBase64String(new string(charArray)));
    }

    public static string EncodeStringToBase64Table(this string unencodedString)
    {
      char[] charArray = Convert.ToBase64String(StrictEncodingWithoutBOM.UTF8.GetBytes(unencodedString)).ToCharArray();
      TableKeyUtility.MutateBase64ToBase64Table(charArray);
      return new string(charArray);
    }

    private static void MutateBase64TableToBase64(char[] base64UrlData)
    {
      for (int index = 0; index < base64UrlData.Length; ++index)
      {
        if (base64UrlData[index] == '_')
          base64UrlData[index] = '/';
      }
    }

    private static void MutateBase64ToBase64Table(char[] base64Data)
    {
      for (int index = 0; index < base64Data.Length; ++index)
      {
        if (base64Data[index] == '/')
          base64Data[index] = '_';
      }
    }

    private static StringBuilder ReplaceLast(
      this StringBuilder source,
      char oldValue,
      char newValue)
    {
      int startIndex = source.ToString().LastIndexOf(oldValue);
      if (startIndex >= 0)
        source.Replace(oldValue, newValue, startIndex, 1);
      return source;
    }

    public static ulong GetIdFromPartitonKey(string partitonKey) => BitConverter.ToUInt64(((IEnumerable<byte>) partitonKey.GetUTF8Bytes().CalculateBlockHash((IBlobHasher) VsoHash.Instance).HashBytes).Take<byte>(8).Reverse<byte>().ToArray<byte>(), 0);

    public enum EqualityType
    {
      ImmediateChildren,
      DeepChildren,
    }
  }
}
