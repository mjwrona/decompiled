// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.FileHasherActivity
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using WebGrease.Common;
using WebGrease.Extensions;

namespace WebGrease.Activities
{
  internal sealed class FileHasherActivity
  {
    private readonly IWebGreaseContext context;
    private readonly ConcurrentDictionary<string, string> renamedFilesLog = new ConcurrentDictionary<string, string>();

    internal FileHasherActivity(IWebGreaseContext context)
    {
      this.context = context;
      this.SourceDirectories = (IList<string>) new List<string>();
      this.ConfigType = context.Configuration.ConfigType;
    }

    internal string ConfigType { get; set; }

    internal IList<string> SourceDirectories { get; private set; }

    internal string DestinationDirectory { private get; set; }

    internal bool CreateExtraDirectoryLevelFromHashes { private get; set; }

    internal string BasePrefixToAddToOutputPath { get; set; }

    internal FileTypes FileType { private get; set; }

    internal string BasePrefixToRemoveFromOutputPathInLog { get; set; }

    internal string BasePrefixToRemoveFromInputPathInLog { get; set; }

    internal string LogFileName { get; set; }

    internal bool ShouldPreserveSourceDirectoryStructure { private get; set; }

    internal string FileTypeFilter { private get; set; }

    internal void Execute()
    {
      this.renamedFilesLog.Clear();
      this.context.SectionedAction(nameof (FileHasherActivity), this.FileType.ToString()).Execute((Action) (() =>
      {
        try
        {
          if (this.SourceDirectories == null || this.SourceDirectories.Count == 0)
          {
            Trace.TraceInformation("FileHasherActivity - No source directories passed and hence no action taken for the activity.");
          }
          else
          {
            if (string.IsNullOrWhiteSpace(this.DestinationDirectory))
              this.DestinationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            IEnumerable<string> filters = FileHasherActivity.GetFilters(this.FileTypeFilter);
            if (string.IsNullOrWhiteSpace(this.BasePrefixToRemoveFromOutputPathInLog))
              this.BasePrefixToRemoveFromOutputPathInLog = string.Empty;
            if (string.IsNullOrWhiteSpace(this.BasePrefixToRemoveFromInputPathInLog))
              this.BasePrefixToRemoveFromInputPathInLog = string.Empty;
            foreach (string sourceDirectory in (IEnumerable<string>) this.SourceDirectories)
            {
              if (!Directory.Exists(sourceDirectory))
                Trace.TraceWarning(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ResourceStrings.FileHasheActivityCouldNotLocateDirectory, new object[1]
                {
                  (object) sourceDirectory
                }));
              else
                this.Hash(sourceDirectory, this.DestinationDirectory, filters);
            }
            this.Save();
          }
        }
        catch (Exception ex)
        {
          throw new WorkflowException(ResourceStrings.FileHasherActivityErrorOccurred, ex);
        }
      }));
    }

    internal IEnumerable<ContentItem> Hash(
      ContentItem contentItem,
      IEnumerable<string> originalFiles)
    {
      List<ContentItem> contentItemList = new List<ContentItem>();
      if (originalFiles.Any<string>())
      {
        string relativeContentPath = originalFiles.FirstOrDefault<string>();
        ContentItem hashedContentItem = this.Hash(ContentItem.FromContentItem(contentItem, relativeContentPath));
        contentItemList.Add(hashedContentItem);
        contentItemList.AddRange(this.AppendToWorkLog(hashedContentItem, originalFiles.Skip<string>(1)));
      }
      return (IEnumerable<ContentItem>) contentItemList;
    }

    internal IEnumerable<ContentItem> AppendToWorkLog(
      ContentItem hashedContentItem,
      IEnumerable<string> originalFiles)
    {
      List<ContentItem> workLog = new List<ContentItem>();
      foreach (string originalFile in originalFiles)
      {
        ContentItem cacheResult = ContentItem.FromContentItem(hashedContentItem, originalFile);
        this.AppendToWorkLog(cacheResult);
        workLog.Add(cacheResult);
      }
      return (IEnumerable<ContentItem>) workLog;
    }

    internal ContentItem Hash(ContentItem contentItem)
    {
      string relativeContentPath = contentItem.RelativeContentPath;
      string destinationFilePath = this.GetDestinationFilePath(this.DestinationDirectory, contentItem.GetContentHash(this.context) + Path.GetExtension(relativeContentPath), contentItem.RelativeContentPath);
      string str1 = this.context.Configuration.DestinationDirectory ?? this.DestinationDirectory;
      string str2 = destinationFilePath;
      if (!string.IsNullOrWhiteSpace(str1) && Path.IsPathRooted(str2))
        str2 = str2.MakeRelativeToDirectory(str1);
      contentItem = ContentItem.FromContentItem(contentItem, relativeHashedContentPath: str2);
      contentItem.WriteToRelativeHashedPath(str1);
      this.AppendToWorkLog(contentItem);
      return contentItem;
    }

    internal void Save(bool append = true) => this.WriteLog(append);

    internal void AppendToWorkLog(IEnumerable<ContentItem> cacheResults)
    {
      foreach (ContentItem cacheResult in cacheResults)
        this.AppendToWorkLog(cacheResult);
    }

    internal void AppendToWorkLog(ContentItem cacheResult) => this.AppendToWorkLog(cacheResult.RelativeContentPath, cacheResult.RelativeHashedContentPath);

    private static IEnumerable<string> GetFilters(string filterType)
    {
      if (!string.IsNullOrWhiteSpace(filterType))
        return (IEnumerable<string>) filterType.Split(Strings.FileFilterSeparator, StringSplitOptions.RemoveEmptyEntries);
      return (IEnumerable<string>) new string[1]{ "*" };
    }

    private static string GetUrlPath(string key) => key.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    private IEnumerable<ContentItem> Hash(
      string sourceDirectory,
      string destinationDirectory,
      IEnumerable<string> filters,
      string rootSourceDirectory = null)
    {
      List<ContentItem> contentItemList = new List<ContentItem>();
      Directory.CreateDirectory(destinationDirectory);
      DirectoryInfo sourceDirectoryInfo = new DirectoryInfo(sourceDirectory);
      rootSourceDirectory = rootSourceDirectory ?? sourceDirectoryInfo.FullName;
      contentItemList.AddRange(filters.SelectMany<string, ContentItem>((Func<string, IEnumerable<ContentItem>>) (filter => sourceDirectoryInfo.EnumerateFiles(filter, SearchOption.TopDirectoryOnly).Select<FileInfo, ContentItem>((Func<FileInfo, ContentItem>) (sourceFileInfo => this.Hash(ContentItem.FromFile(sourceFileInfo.FullName, sourceFileInfo.FullName.MakeRelativeToDirectory(rootSourceDirectory), (string) null)))))));
      foreach (DirectoryInfo directory in sourceDirectoryInfo.GetDirectories())
      {
        string destinationDirectory1 = this.ShouldPreserveSourceDirectoryStructure ? Path.Combine(destinationDirectory, directory.Name) : destinationDirectory;
        contentItemList.AddRange(this.Hash(directory.FullName, destinationDirectory1, filters, rootSourceDirectory));
      }
      return (IEnumerable<ContentItem>) contentItemList;
    }

    private string GetDestinationFilePath(
      string destination,
      string hashedFileName,
      string relativePath)
    {
      string str;
      if (this.CreateExtraDirectoryLevelFromHashes)
      {
        string lowerInvariant = Path.Combine(destination, hashedFileName.Substring(0, 2)).ToLowerInvariant();
        if (!Directory.Exists(lowerInvariant))
          Directory.CreateDirectory(lowerInvariant);
        str = Path.Combine(lowerInvariant, hashedFileName.Remove(0, 2));
      }
      else
        str = !this.ShouldPreserveSourceDirectoryStructure ? Path.Combine(destination, hashedFileName) : Path.Combine(destination, Path.GetDirectoryName(relativePath), hashedFileName);
      return str.ToLowerInvariant();
    }

    private void AppendToWorkLog(
      string fileBeforeHashing,
      string fileAfterHashing,
      bool skipIfExists = false)
    {
      fileAfterHashing = Path.Combine(this.context.Configuration.DestinationDirectory ?? this.DestinationDirectory, fileAfterHashing);
      fileBeforeHashing = this.NormalizeFileForWorkLog(fileBeforeHashing, this.BasePrefixToRemoveFromInputPathInLog);
      fileAfterHashing = this.NormalizeFileForWorkLog(fileAfterHashing, this.BasePrefixToRemoveFromOutputPathInLog);
      if (Path.IsPathRooted(fileBeforeHashing))
        fileBeforeHashing = fileBeforeHashing.MakeRelativeToDirectory(this.BasePrefixToRemoveFromInputPathInLog);
      if (this.renamedFilesLog.ContainsKey(fileBeforeHashing) && !this.renamedFilesLog[fileBeforeHashing].Equals(fileAfterHashing))
      {
        if (skipIfExists)
        {
          if (!File.Exists(fileAfterHashing))
            return;
          File.Delete(fileAfterHashing);
          string directoryName = Path.GetDirectoryName(fileAfterHashing);
          if (Directory.EnumerateFiles(directoryName).Any<string>())
            return;
          Directory.Delete(directoryName);
        }
        else
          throw new BuildWorkflowException("The renamed filename already has a rename to a different file: \r\nBeforehashing:{0} \r\nNewAfterHashing:{1} ExistingAfterhashing:{2}".InvariantFormat((object) fileBeforeHashing, (object) fileAfterHashing, (object) string.Join(",", this.renamedFilesLog.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (rfl => rfl.Key.Equals(fileBeforeHashing))).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (e => e.Key)))));
      }
      else
        this.renamedFilesLog[fileBeforeHashing] = fileAfterHashing;
    }

    private string MakeOutputAbsolute(string output)
    {
      if (!string.IsNullOrWhiteSpace(this.BasePrefixToAddToOutputPath) && output.StartsWith(this.BasePrefixToAddToOutputPath, StringComparison.OrdinalIgnoreCase))
        output = output.Substring(this.BasePrefixToAddToOutputPath.Length);
      return Path.Combine(this.BasePrefixToRemoveFromOutputPathInLog ?? this.DestinationDirectory, output.NormalizeUrl());
    }

    private string NormalizeFileForWorkLog(string file, string preFixToRemoveFromWorkLog)
    {
      if (Path.IsPathRooted(file))
        file = file.MakeRelativeToDirectory(preFixToRemoveFromWorkLog);
      else if (!string.IsNullOrWhiteSpace(preFixToRemoveFromWorkLog))
      {
        string directory = preFixToRemoveFromWorkLog.MakeRelativeToDirectory(this.DestinationDirectory);
        if (!string.IsNullOrWhiteSpace(directory) && file.StartsWith(directory, StringComparison.OrdinalIgnoreCase))
          file = file.Substring(directory.Length);
      }
      return file.NormalizeUrl();
    }

    private void WriteLog(bool appendToLog = true)
    {
      if (string.IsNullOrWhiteSpace(this.LogFileName))
        return;
      if (appendToLog)
        this.LoadBeforeWrite(this.LogFileName);
      StringBuilder output = new StringBuilder();
      XmlWriterSettings settings = new XmlWriterSettings()
      {
        Indent = true,
        OmitXmlDeclaration = true
      };
      using (XmlWriter xmlWriter1 = XmlWriter.Create(output, settings))
      {
        xmlWriter1.WriteStartDocument();
        xmlWriter1.WriteStartElement("RenamedFiles");
        xmlWriter1.WriteAttributeString("configType", this.ConfigType);
        if (this.renamedFilesLog == null || this.renamedFilesLog.Keys.Count < 1)
        {
          xmlWriter1.WriteComment(ResourceStrings.NoFilesProcessed);
        }
        else
        {
          foreach (var data in this.renamedFilesLog.OrderBy<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (rfl => rfl.Value)).GroupBy((Func<KeyValuePair<string, string>, string>) (rfl => rfl.Value), (Func<KeyValuePair<string, string>, string>) (rfl => rfl.Key), (key, g) => new
          {
            FileAfterHashing = key,
            FilesBeforeHashing = g.ToList<string>()
          }))
          {
            if (data.FilesBeforeHashing.Any<string>())
            {
              xmlWriter1.WriteStartElement("File");
              xmlWriter1.WriteStartElement("Output");
              string urlPath = FileHasherActivity.GetUrlPath(data.FileAfterHashing);
              string str1 = (this.BasePrefixToAddToOutputPath ?? Path.AltDirectorySeparatorChar.ToString((IFormatProvider) CultureInfo.InvariantCulture)) + urlPath.TrimStart(Path.AltDirectorySeparatorChar);
              xmlWriter1.WriteValue(str1);
              xmlWriter1.WriteEndElement();
              foreach (string key in (IEnumerable<string>) data.FilesBeforeHashing.OrderBy<string, string>((Func<string, string>) (r => r)))
              {
                xmlWriter1.WriteStartElement("Input");
                XmlWriter xmlWriter2 = xmlWriter1;
                // ISSUE: variable of a boxed type
                __Boxed<char> directorySeparatorChar = (ValueType) Path.AltDirectorySeparatorChar;
                string str2 = FileHasherActivity.GetUrlPath(key).TrimStart(Path.AltDirectorySeparatorChar);
                string str3 = directorySeparatorChar.ToString() + str2;
                xmlWriter2.WriteValue(str3);
                xmlWriter1.WriteEndElement();
              }
              xmlWriter1.WriteEndElement();
            }
          }
        }
        xmlWriter1.WriteEndElement();
      }
      FileHelper.WriteFile(this.LogFileName, output.ToString());
    }

    private void LoadBeforeWrite(string logFileName)
    {
      string configTypeLogFile1 = FileHasherActivity.GetConfigTypeLogFile(logFileName, this.ConfigType);
      XElement xelement = (XElement) null;
      if (!File.Exists(logFileName))
      {
        if (File.Exists(configTypeLogFile1))
          xelement = FileHasherActivity.GetLogRoot(configTypeLogFile1);
      }
      else
        xelement = FileHasherActivity.GetLogRoot(logFileName);
      if (xelement == null)
        return;
      string configType = (string) xelement.Attribute((XName) "configType");
      if (configType != this.ConfigType)
      {
        if (!string.IsNullOrWhiteSpace(configType))
        {
          string configTypeLogFile2 = FileHasherActivity.GetConfigTypeLogFile(logFileName, configType);
          File.Copy(logFileName, configTypeLogFile2, true);
        }
        if (!File.Exists(configTypeLogFile1))
          return;
        xelement = FileHasherActivity.GetLogRoot(configTypeLogFile1);
      }
      foreach (XElement element in xelement.Elements((XName) "File"))
      {
        string output = element.Elements((XName) "Output").Select<XElement, string>((Func<XElement, string>) (e => (string) e)).FirstOrDefault<string>();
        if (!string.IsNullOrWhiteSpace(output))
        {
          string str = this.MakeOutputAbsolute(output);
          if (File.Exists(str))
          {
            foreach (string fileBeforeHashing in element.Elements((XName) "Input").Select<XElement, string>((Func<XElement, string>) (e => (string) e)))
              this.AppendToWorkLog(fileBeforeHashing, str, true);
          }
        }
      }
    }

    private static XElement GetLogRoot(string logFileName) => XDocument.Load(logFileName).Element((XName) "RenamedFiles");

    private static string GetConfigTypeLogFile(string logFileName, string configType)
    {
      if (string.IsNullOrWhiteSpace(configType))
        return logFileName;
      string extension = configType + "." + Path.GetExtension(logFileName);
      return Path.ChangeExtension(logFileName, extension);
    }
  }
}
