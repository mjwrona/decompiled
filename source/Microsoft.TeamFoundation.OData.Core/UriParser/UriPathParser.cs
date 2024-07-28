// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.UriPathParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  public class UriPathParser
  {
    private readonly int maxSegments;

    public UriPathParser(ODataUriParserSettings settings) => this.maxSegments = settings.PathLimit;

    public virtual ICollection<string> ParsePathIntoSegments(Uri fullUri, Uri serviceBaseUri)
    {
      if (serviceBaseUri == (Uri) null)
      {
        serviceBaseUri = UriUtils.CreateMockAbsoluteUri();
        fullUri = UriUtils.CreateMockAbsoluteUri(fullUri);
      }
      if (!UriUtils.UriInvariantInsensitiveIsBaseOf(serviceBaseUri, fullUri))
        throw new ODataException(Strings.UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri((object) fullUri, (object) serviceBaseUri));
      try
      {
        Uri uri = fullUri;
        int num = serviceBaseUri.AbsolutePath.Split('/').Length - 1;
        string[] strArray = uri.AbsolutePath.Split('/');
        int startIndex = -1;
        List<string> stringList = new List<string>();
        for (int index = num; index < strArray.Length; ++index)
        {
          string str1 = strArray[index];
          if (str1.Length != 0 && !(str1 == "/"))
          {
            if (str1[str1.Length - 1] == '/')
              str1 = str1.Substring(0, str1.Length - 1);
            if (stringList.Count == this.maxSegments)
              throw new ODataException(Strings.UriQueryPathParser_TooManySegments);
            if (str1.Length >= 2 && str1.EndsWith("::", StringComparison.Ordinal))
            {
              if (startIndex == -1)
                throw new ODataException(Strings.UriQueryPathParser_InvalidEscapeUri((object) str1));
              string str2 = string.Join("/", strArray, startIndex, index - startIndex + 1);
              stringList.Add(":" + str2.Substring(0, str2.Length - 1));
              startIndex = index + 1;
            }
            else if (str1.Length >= 1 && str1[str1.Length - 1] == ':')
            {
              if (startIndex == -1)
              {
                if (str1 != ":")
                  stringList.Add(str1.Substring(0, str1.Length - 1));
                startIndex = index + 1;
              }
              else
              {
                string str3 = ":" + string.Join("/", strArray, startIndex, index - startIndex + 1);
                stringList.Add(str3);
                startIndex = -1;
              }
            }
            else if (startIndex == -1)
              stringList.Add(Uri.UnescapeDataString(str1));
          }
        }
        if (startIndex != -1 && startIndex < strArray.Length)
        {
          string str = ":" + string.Join("/", strArray, startIndex, strArray.Length - startIndex);
          stringList.Add(str);
        }
        return (ICollection<string>) stringList.ToArray();
      }
      catch (FormatException ex)
      {
        throw new ODataException(Strings.UriQueryPathParser_SyntaxError, (Exception) ex);
      }
    }
  }
}
