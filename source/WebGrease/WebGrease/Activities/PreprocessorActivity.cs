// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.PreprocessorActivity
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.IO;
using WebGrease.Configuration;
using WebGrease.Extensions;

namespace WebGrease.Activities
{
  internal sealed class PreprocessorActivity
  {
    private readonly IWebGreaseContext context;

    internal PreprocessorActivity(IWebGreaseContext context)
    {
      this.context = context;
      this.Inputs = new List<InputSpec>();
    }

    internal List<InputSpec> Inputs { get; private set; }

    internal string OutputFolder { private get; set; }

    internal PreprocessingConfig PreprocessingConfig { private get; set; }

    internal bool MinimalOutput { get; set; }

    internal IEnumerable<ContentItem> Execute()
    {
      List<ContentItem> contentItemList = new List<ContentItem>();
      string sourceDirectory = this.context.Configuration.SourceDirectory;
      foreach (string file in this.Inputs.GetFiles(sourceDirectory))
      {
        if (!new FileInfo(file).Exists)
          throw new FileNotFoundException("Could not find the file {0} to preprocess on.");
        if (!Directory.Exists(this.OutputFolder))
          Directory.CreateDirectory(this.OutputFolder);
        contentItemList.Add(this.context.Preprocessing.Process(ContentItem.FromFile(file, file.MakeRelativeToDirectory(sourceDirectory), (string) null), this.PreprocessingConfig, this.MinimalOutput) ?? throw new WorkflowException("An error occurred while processing the file: " + file));
      }
      return (IEnumerable<ContentItem>) contentItemList;
    }
  }
}
