// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.ResponseFileOptionReader
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  public class ResponseFileOptionReader : OptionReader
  {
    private static readonly char[] trimChars = new char[3]
    {
      '@',
      '\'',
      '"'
    };

    public override IEnumerable<string> GetOptions(string value)
    {
      string validPath;
      ResponseFileOptionReader.ValidateResponseFileOption(value, out validPath);
      return (IEnumerable<string>) ResponseFileOptionReader.ParseResponseFileLines(this.ReadResponseFileLines(this.ExpandFilePath(validPath)));
    }

    protected virtual string ExpandFilePath(string filePath)
    {
      string str = (string) null;
      if (!string.IsNullOrEmpty(filePath))
        str = Path.GetFullPath(Environment.ExpandEnvironmentVariables(filePath.Trim(ResponseFileOptionReader.trimChars)));
      return str;
    }

    protected virtual IEnumerable<string> ReadResponseFileLines(string filePath) => File.Exists(filePath) ? (IEnumerable<string>) File.ReadAllLines(filePath) : throw new FileNotFoundException(CommonResources.ErrorResponseFileNotFound((object) filePath));

    private static Collection<string> ParseResponseFileLines(IEnumerable<string> responseFileLines)
    {
      Collection<string> responseFileLines1 = (Collection<string>) null;
      if (responseFileLines != null && responseFileLines.Any<string>())
      {
        foreach (string responseFileLine in responseFileLines)
        {
          if (responseFileLine != null)
          {
            IEnumerable<string> source = responseFileLine.Trim().Lex();
            if (source != null && source.Any<string>() && !source.First<string>().StartsWith("#", StringComparison.OrdinalIgnoreCase))
            {
              foreach (string str1 in source)
              {
                string str2 = str1.Trim();
                if (!string.IsNullOrWhiteSpace(str2))
                {
                  if (responseFileLines1 == null)
                    responseFileLines1 = new Collection<string>();
                  responseFileLines1.Add(str2);
                }
              }
            }
          }
        }
      }
      return responseFileLines1;
    }

    private static void ValidateResponseFileOption(string value, out string validPath)
    {
      bool flag = false;
      validPath = (string) null;
      try
      {
        if (value != null)
        {
          string str = value.Trim();
          if (str.StartsWith("@", StringComparison.OrdinalIgnoreCase))
          {
            string path = str.Trim(ResponseFileOptionReader.trimChars);
            Path.GetFullPath(path);
            validPath = path;
            flag = true;
          }
        }
      }
      catch (Exception ex)
      {
        throw new ArgumentException(CommonResources.ErrorInvalidResponseFileOption((object) value), ex);
      }
      if (!flag)
        throw new ArgumentException(CommonResources.ErrorInvalidResponseFileOption((object) value));
    }
  }
}
