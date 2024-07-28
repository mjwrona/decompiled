// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.AssemblerActivity
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using WebGrease.Configuration;
using WebGrease.Extensions;

namespace WebGrease.Activities
{
  internal sealed class AssemblerActivity
  {
    private static readonly Regex EndsWithSemicolon = new Regex(";\\s*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private readonly IWebGreaseContext context;
    private bool endedInSemicolon;

    internal AssemblerActivity(IWebGreaseContext context)
    {
      this.context = context;
      this.Inputs = new List<InputSpec>();
    }

    internal List<InputSpec> Inputs { get; private set; }

    internal string OutputFile { get; set; }

    internal PreprocessingConfig PreprocessingConfig { private get; set; }

    internal bool AddSemicolons { private get; set; }

    internal bool MinimalOutput { get; set; }

    internal ContentItem Execute(ContentItemType resultContentItemType = ContentItemType.Path)
    {
      string str = !string.IsNullOrWhiteSpace(this.OutputFile) ? Path.GetExtension(this.OutputFile) : throw new ArgumentException("AssemblerActivity - The output file path cannot be null or whitespace.");
      if (!string.IsNullOrWhiteSpace(str))
        str = str.Trim('.');
      ContentItem contentItem = (ContentItem) null;
      this.context.SectionedAction(nameof (AssemblerActivity), str).MakeCachable((object) new
      {
        Inputs = this.Inputs,
        PreprocessingConfig = this.PreprocessingConfig,
        AddSemicolons = this.AddSemicolons,
        output = (resultContentItemType == ContentItemType.Path ? this.OutputFile : (string) null)
      }).RestoreFromCacheAction((Func<ICacheSection, bool>) (cacheSection =>
      {
        ContentItem cachedContentItem = cacheSection.GetCachedContentItem("AssemblerResult");
        if (cachedContentItem == null)
          return false;
        contentItem = ContentItem.FromContentItem(cachedContentItem, Path.GetFileName(this.OutputFile));
        return true;
      })).Execute((Func<ICacheSection, bool>) (cacheSection =>
      {
        try
        {
          this.Inputs.ForEach(new Action<InputSpec>(this.context.Cache.CurrentCacheSection.AddSourceDependency));
          string directoryName = Path.GetDirectoryName(this.OutputFile);
          if (resultContentItemType == ContentItemType.Path && !string.IsNullOrWhiteSpace(directoryName))
            Directory.CreateDirectory(directoryName);
          this.endedInSemicolon = true;
          contentItem = this.Bundle(resultContentItemType, directoryName, this.OutputFile, this.context.Configuration.SourceDirectory);
          cacheSection.AddResult(contentItem, "AssemblerResult");
        }
        catch (Exception ex)
        {
          throw new WorkflowException("AssemblerActivity - Error happened while executing the assembler activity", ex);
        }
        return true;
      }));
      return contentItem;
    }

    private ContentItem Bundle(
      ContentItemType targetContentItemType,
      string outputDirectory,
      string outputFile,
      string sourceDirectory)
    {
      StringBuilder sb = new StringBuilder();
      using (TextWriter writer = targetContentItemType == ContentItemType.Path ? (TextWriter) new StreamWriter(outputFile, false, Encoding.UTF8) : (TextWriter) new StringWriter(sb, (IFormatProvider) CultureInfo.InvariantCulture))
      {
        this.context.Log.Information("Start bundling output file: {0}".InvariantFormat((object) outputFile));
        foreach (string file in this.Inputs.GetFiles(sourceDirectory, this.context.Log, true))
          this.Append(writer, file, sourceDirectory, this.PreprocessingConfig);
        this.context.Log.Information("End bundling output file: {0}".InvariantFormat((object) outputFile));
      }
      return targetContentItemType != ContentItemType.Path ? ContentItem.FromContent(sb.ToString(), outputFile.MakeRelativeTo(outputDirectory), (string) null) : ContentItem.FromFile(outputFile, outputFile.MakeRelativeTo(outputDirectory), (string) null);
    }

    private void Append(
      TextWriter writer,
      string filePath,
      string sourceDirectory,
      PreprocessingConfig preprocessingConfig = null)
    {
      writer.WriteLine();
      writer.WriteLine();
      if (this.AddSemicolons && !this.endedInSemicolon)
        writer.Write(';');
      string relativeContentPath = !Path.IsPathRooted(filePath) || sourceDirectory.IsNullOrWhitespace() ? filePath : filePath.MakeRelativeTo(sourceDirectory);
      if (!this.MinimalOutput)
      {
        writer.WriteLine("/* {0} {1} */".InvariantFormat((object) relativeContentPath, filePath != relativeContentPath ? (object) ("(" + filePath + ")") : (object) string.Empty));
        writer.WriteLine();
      }
      ContentItem contentItem = ContentItem.FromFile(filePath, relativeContentPath, (string) null);
      if (preprocessingConfig != null && preprocessingConfig.Enabled)
      {
        contentItem = this.context.Preprocessing.Process(contentItem, preprocessingConfig, this.MinimalOutput);
        if (contentItem == null)
          throw new WorkflowException("Could not assembly the file {0} because one of the preprocessors threw an error.".InvariantFormat((object) filePath));
      }
      string content = contentItem.Content;
      writer.Write(content);
      writer.WriteLine();
      if (!this.AddSemicolons)
        return;
      this.endedInSemicolon = AssemblerActivity.EndsWithSemicolon.IsMatch(content);
    }
  }
}
