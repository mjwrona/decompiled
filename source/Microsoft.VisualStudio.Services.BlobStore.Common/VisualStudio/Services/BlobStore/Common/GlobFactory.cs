// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.GlobFactory
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using DotNet.Globbing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class GlobFactory
  {
    private const string ArtifactIgnoreFileName = ".artifactignore";
    private StringComparer defaultStringComparer;
    private GlobOptions defaultGlobOptions = GlobOptions.Default;
    private bool globUsingArtifactIgnoreFile;
    private bool isWindowsPlatform;
    private string artifactIgnoreFilePath;
    private HashSet<string> files;
    private HashSet<string> directories;
    private HashSet<string> sourceDirectoryItems;
    private HashSet<string> nonEmptyDirectories;
    private HashSet<string> emptyDirectories;
    private HashSet<string> ignoreFileSet;
    private HashSet<string> ignoreFileGlobSet;
    private HashSet<string> doNotIgnoreFileGlobSet;
    private readonly Dictionary<string, string> GlobPatternsWindows = new Dictionary<string, string>()
    {
      {
        "Comment",
        "^#"
      },
      {
        "DoNotIgnoreWithEscaping",
        "^!{1}[\\\\]{1}(!|#)"
      },
      {
        "DoNotIgnore",
        "^!{1}"
      },
      {
        "IgnoreWithEscaping",
        "^\\\\{1}(!|#)"
      },
      {
        "Ignore",
        "(.*?)"
      }
    };
    private readonly Dictionary<string, string> GlobPatternsNix = new Dictionary<string, string>()
    {
      {
        "Comment",
        "^#"
      },
      {
        "DoNotIgnoreWithEscaping",
        "^!{1}[\\/]{1}(!|#)"
      },
      {
        "DoNotIgnore",
        "^!{1}"
      },
      {
        "IgnoreWithEscaping",
        "^\\/{1}(!|#)"
      },
      {
        "Ignore",
        "(.*?)"
      }
    };
    private readonly IEnumerable<string> DefaultGlobList = (IEnumerable<string>) new List<string>()
    {
      ".git"
    };

    public IFileSystem TargetFileSystem { get; }

    private IAppTraceSource Tracer { get; }

    private bool GlobEmptyDirectories { get; }

    private GlobFactory()
    {
    }

    public GlobFactory(IFileSystem fileSystem, IAppTraceSource tracer, bool globEmptyDirectories = false)
    {
      this.TargetFileSystem = fileSystem;
      this.Tracer = tracer;
      this.GlobEmptyDirectories = globEmptyDirectories;
    }

    private void Init(string sourceDirectory)
    {
      if (this.TargetFileSystem.FileExists(this.artifactIgnoreFilePath = Path.Combine(sourceDirectory, ".artifactignore")))
      {
        this.globUsingArtifactIgnoreFile = true;
        this.Tracer.Info("Using .artifactignore file located at: {0} for globbing", (object) this.artifactIgnoreFilePath);
      }
      this.defaultStringComparer = Helpers.FileSystemStringComparer(Environment.OSVersion);
      if (this.isWindowsPlatform = Helpers.IsWindowsPlatform(Environment.OSVersion))
        this.defaultGlobOptions = new GlobOptions()
        {
          Evaluation = new EvaluationOptions()
          {
            CaseInsensitive = true
          }
        };
      this.files = this.TargetFileSystem.EnumerateFiles(sourceDirectory, true).ToHashSet<string, string>((Func<string, string>) (x => x), (IEqualityComparer<string>) this.defaultStringComparer);
      this.ignoreFileGlobSet = new HashSet<string>((IEqualityComparer<string>) this.defaultStringComparer);
      this.doNotIgnoreFileGlobSet = new HashSet<string>((IEqualityComparer<string>) this.defaultStringComparer);
      this.ignoreFileSet = new HashSet<string>((IEqualityComparer<string>) this.defaultStringComparer);
      this.sourceDirectoryItems = new HashSet<string>((IEqualityComparer<string>) this.defaultStringComparer);
      this.sourceDirectoryItems.AddRange<string, HashSet<string>>((IEnumerable<string>) this.files);
      if (!this.GlobEmptyDirectories)
        return;
      this.directories = this.TargetFileSystem.EnumerateDirectories(sourceDirectory, true).ToHashSet<string, string>((Func<string, string>) (x => x), (IEqualityComparer<string>) this.defaultStringComparer);
      this.nonEmptyDirectories = new HashSet<string>((IEqualityComparer<string>) this.defaultStringComparer);
      this.emptyDirectories = new HashSet<string>((IEqualityComparer<string>) this.defaultStringComparer);
      foreach (string file in this.files)
        this.nonEmptyDirectories.Add(Path.GetDirectoryName(file));
      foreach (string directory in this.directories)
        this.nonEmptyDirectories.Add(Path.GetDirectoryName(directory));
      this.emptyDirectories.AddRange<string, HashSet<string>>(this.directories.Where<string>((Func<string, bool>) (d => !this.nonEmptyDirectories.Contains(d))));
      this.sourceDirectoryItems.AddRange<string, HashSet<string>>((IEnumerable<string>) this.emptyDirectories);
    }

    public IEnumerable<string> PerformGlobbing(string sourceDirectory)
    {
      this.Init(sourceDirectory);
      if (!this.globUsingArtifactIgnoreFile)
        this.ignoreFileGlobSet.AddRange<string, HashSet<string>>(this.DefaultGlobList);
      IEnumerable<string> globbingStation = this.DispatchToGlobbingStation(sourceDirectory);
      if (globbingStation != null && globbingStation.LongCount<string>() > 0L)
      {
        this.Tracer.Info(string.Format("Processing .artifactignore file surfaced {0} files. Total files under source directory: {1}", (object) globbingStation.LongCount<string>(), (object) this.sourceDirectoryItems.LongCount<string>()));
        this.Tracer.Verbose("Ignoring the following files/folders:");
        foreach (string str in globbingStation)
          this.Tracer.Verbose("{0}", (object) str);
      }
      return globbingStation;
    }

    private IEnumerable<string> DispatchToGlobbingStation(string sourceDirectory)
    {
      if (this.globUsingArtifactIgnoreFile)
        this.ParseArtifactIgnore();
      return this.GlobbingStation(sourceDirectory);
    }

    private void ParseArtifactIgnore()
    {
      using (StreamReader streamReader = this.TargetFileSystem.OpenText(this.artifactIgnoreFilePath))
      {
        Dictionary<string, string> dictionary = this.isWindowsPlatform ? this.GlobPatternsWindows : this.GlobPatternsNix;
        string str;
        while ((str = streamReader.ReadLine()) != null)
        {
          if (!string.IsNullOrWhiteSpace(str))
          {
            if (new Regex(dictionary["Comment"]).IsMatch(str))
              this.Tracer.Verbose("Skipping comment.");
            else if (new Regex(dictionary["DoNotIgnoreWithEscaping"]).IsMatch(str) && GlobFactory.IsSpecialExtentGlob(str))
            {
              this.GlobAccounting(str.Substring(2, str.Length - 2), false);
              this.Tracer.Verbose("Not ignoring file/folders matching: {0}", (object) str);
            }
            else if (new Regex(dictionary["DoNotIgnore"]).IsMatch(str) && GlobFactory.IsSpecialExtentGlob(str))
            {
              this.GlobAccounting(str.Substring(1, str.Length - 1), false);
              this.Tracer.Verbose("Not ignoring file/folders matching: {0}", (object) str);
            }
            else if (new Regex(dictionary["IgnoreWithEscaping"]).IsMatch(str) && GlobFactory.IsSpecialExtentGlob(str))
            {
              this.GlobAccounting(str.Substring(1, str.Length - 1));
              this.Tracer.Verbose("Ignoring file/folders matching: {0}", (object) str);
            }
            else if (GlobFactory.IsSpecialExtentGlob(str))
            {
              this.GlobAccounting(str);
              this.Tracer.Verbose("Ignoring file/folders matching: {0}", (object) str);
            }
            else if (new Regex(dictionary["DoNotIgnoreWithEscaping"]).IsMatch(str))
            {
              this.GlobAccounting(str.Substring(2, str.Length - 2), false);
              this.Tracer.Verbose("Not ignoring file/folder: {0}", (object) str);
            }
            else if (new Regex(dictionary["IgnoreWithEscaping"]).IsMatch(str))
            {
              this.GlobAccounting(str.Substring(1, str.Length - 1));
              this.Tracer.Verbose("Ignoring file/folder: {0}", (object) str);
            }
            else if (new Regex(dictionary["DoNotIgnore"]).IsMatch(str))
            {
              this.GlobAccounting(str.Substring(1, str.Length - 1), false);
              this.Tracer.Verbose("Not ignoring file/folder: {0}", (object) str);
            }
            else
            {
              this.GlobAccounting(str);
              this.Tracer.Verbose("Ignoring file/folder: {0}", (object) str);
            }
          }
        }
        streamReader.Close();
      }
    }

    private void GlobAccounting(string glob, bool ignore = true)
    {
      if (ignore)
      {
        if (this.ignoreFileGlobSet.Contains(glob))
          return;
        this.ignoreFileGlobSet.Add(glob);
      }
      else
      {
        if (this.doNotIgnoreFileGlobSet.Contains(glob))
          return;
        this.doNotIgnoreFileGlobSet.Add(glob);
      }
    }

    private IEnumerable<string> GlobbingStation(string sourceDirectory)
    {
      this.GlobFilter(sourceDirectory);
      this.GlobFilter(sourceDirectory, false);
      return (IEnumerable<string>) this.ignoreFileSet;
    }

    private void GlobFilter(string sourceDirectory, bool ignoreFiles = true)
    {
      foreach (string str1 in ignoreFiles ? this.ignoreFileGlobSet : this.doNotIgnoreFileGlobSet)
      {
        string str2 = string.Empty;
        string str3 = string.Empty;
        if (str1.Contains<char>(Path.DirectorySeparatorChar))
        {
          string str4 = string.Empty;
          string str5 = str1;
          char directorySeparatorChar = Path.DirectorySeparatorChar;
          string str6 = directorySeparatorChar.ToString();
          if (str5.StartsWith(str6))
          {
            string str7 = str1;
            directorySeparatorChar = Path.DirectorySeparatorChar;
            string str8 = directorySeparatorChar.ToString();
            if (str7.EndsWith(str8))
            {
              str4 = sourceDirectory + str1 + "**";
              goto label_20;
            }
          }
          string str9 = str1;
          directorySeparatorChar = Path.DirectorySeparatorChar;
          string str10 = directorySeparatorChar.ToString();
          if (str9.StartsWith(str10))
          {
            string str11 = str1;
            directorySeparatorChar = Path.DirectorySeparatorChar;
            string str12 = directorySeparatorChar.ToString();
            if (!str11.EndsWith(str12))
            {
              str4 = sourceDirectory + str1;
              if (!str1.EndsWith("**") && !str1.EndsWith("*"))
              {
                str3 = Path.Combine(str4, "**");
                goto label_20;
              }
              else
                goto label_20;
            }
          }
          string str13 = str1;
          directorySeparatorChar = Path.DirectorySeparatorChar;
          string str14 = directorySeparatorChar.ToString();
          if (str13.EndsWith(str14))
          {
            string str15 = str1;
            directorySeparatorChar = Path.DirectorySeparatorChar;
            string str16 = directorySeparatorChar.ToString();
            if (!str15.StartsWith(str16))
            {
              str4 = str1 + "**";
              if (!str1.StartsWith("**"))
              {
                str4 = Path.Combine(sourceDirectory, str4);
                goto label_20;
              }
              else
                goto label_20;
            }
          }
          string str17 = str1;
          directorySeparatorChar = Path.DirectorySeparatorChar;
          string str18 = directorySeparatorChar.ToString();
          if (!str17.EndsWith(str18))
          {
            string str19 = str1;
            directorySeparatorChar = Path.DirectorySeparatorChar;
            string str20 = directorySeparatorChar.ToString();
            if (!str19.StartsWith(str20))
            {
              if (!str1.StartsWith("**"))
                str4 = Path.Combine(sourceDirectory, str1);
              if (!str1.EndsWith("**") && !str1.EndsWith("*"))
                str3 = Path.Combine(string.IsNullOrWhiteSpace(str4) ? str1 : str4, "**");
            }
          }
label_20:
          this.PopulateFilter(Glob.Parse(string.IsNullOrWhiteSpace(str4) ? str1 : str4, this.defaultGlobOptions), !string.IsNullOrWhiteSpace(str3) ? Glob.Parse(str3, this.defaultGlobOptions) : (Glob) null, ignoreFiles);
        }
        else
        {
          Glob ignoreGlobWide = (Glob) null;
          if (!str1.StartsWith("**"))
            str2 = Path.Combine("**", str1);
          Glob ignoreGlob = Glob.Parse(string.IsNullOrWhiteSpace(str2) ? str1 : str2, this.defaultGlobOptions);
          if (!str1.EndsWith("**") && !str1.EndsWith("*"))
            ignoreGlobWide = Glob.Parse(Path.Combine(string.IsNullOrWhiteSpace(str2) ? str1 : str2, "**"), this.defaultGlobOptions);
          this.PopulateFilter(ignoreGlob, ignoreGlobWide, ignoreFiles);
        }
      }
    }

    private void PopulateFilter(Glob ignoreGlob, Glob ignoreGlobWide, bool ignoreFiles)
    {
      if (ignoreFiles)
      {
        this.ignoreFileSet.AddRangeIfRangeNotNull<string, HashSet<string>>(this.sourceDirectoryItems.Where<string>(new Func<string, bool>(ignoreGlob.IsMatch)));
        if (ignoreGlobWide == null)
          return;
        this.ignoreFileSet.AddRangeIfRangeNotNull<string, HashSet<string>>(this.sourceDirectoryItems.Where<string>(new Func<string, bool>(ignoreGlobWide.IsMatch)));
      }
      else
      {
        this.ignoreFileSet.ExceptWith(this.sourceDirectoryItems.Where<string>(new Func<string, bool>(ignoreGlob.IsMatch)));
        if (ignoreGlobWide == null)
          return;
        this.ignoreFileSet.ExceptWith(this.sourceDirectoryItems.Where<string>(new Func<string, bool>(ignoreGlobWide.IsMatch)));
      }
    }

    private static bool IsSpecialExtentGlob(string globLine) => globLine.Contains("*") || globLine.Contains("**") || globLine.Contains("?");
  }
}
