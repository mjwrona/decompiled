// Decompiled with JetBrains decompiler
// Type: WebGrease.Extensions.InputSpecExtensions
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using WebGrease.Configuration;

namespace WebGrease.Extensions
{
  public static class InputSpecExtensions
  {
    public static IEnumerable<string> GetFiles(
      this IEnumerable<InputSpec> inputs,
      string rootPath,
      LogManager log = null,
      bool throwWhenMissingAndNotOptional = false)
    {
      return inputs.Where<InputSpec>((Func<InputSpec, bool>) (_ => _ != null && !string.IsNullOrWhiteSpace(_.Path))).SelectMany<InputSpec, string>((Func<InputSpec, IEnumerable<string>>) (i => i.GetFiles(rootPath, log, throwWhenMissingAndNotOptional)));
    }

    public static IEnumerable<string> GetFiles(
      this InputSpec input,
      string rootPath = null,
      LogManager log = null,
      bool throwWhenMissingAndNotOptional = false)
    {
      List<string> files = new List<string>();
      string str1 = Path.Combine(rootPath ?? string.Empty, input.Path);
      if (File.Exists(str1))
      {
        log?.Information("- {0}".InvariantFormat((object) str1));
        files.Add(str1);
      }
      else if (Directory.Exists(str1))
      {
        log?.Information("Folder: {0}, Pattern: {1}, Options: {2}".InvariantFormat((object) str1, (object) input.SearchPattern, (object) input.SearchOption));
        files.AddRange((IEnumerable<string>) Directory.EnumerateFiles(str1, string.IsNullOrWhiteSpace(input.SearchPattern) ? "*.*" : input.SearchPattern, input.SearchOption).OrderBy<string, string>((Func<string, string>) (name => name), (IComparer<string>) StringComparer.OrdinalIgnoreCase));
        if (log != null)
        {
          foreach (string str2 in files)
            log.Information("- {0}".InvariantFormat((object) str2));
        }
      }
      else if (!input.IsOptional && throwWhenMissingAndNotOptional)
        throw new FileNotFoundException("Could not find the file for non option input spec: Path:{0}, SearchPattern:{1}, Options:{2}".InvariantFormat((object) str1, (object) input.SearchPattern, (object) input.SearchOption), str1);
      return (IEnumerable<string>) files;
    }

    internal static void AddInputSpecs(
      this IList<InputSpec> inputSpecs,
      string sourceDirectory,
      XElement element)
    {
      foreach (XElement descendant in element.Descendants())
      {
        InputSpec inputSpec = new InputSpec(descendant, sourceDirectory);
        if (!string.IsNullOrWhiteSpace(inputSpec.Path))
          inputSpecs.Add(inputSpec);
      }
    }
  }
}
