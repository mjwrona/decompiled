// Decompiled with JetBrains decompiler
// Type: WebGrease.Configuration.IFileSet
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;

namespace WebGrease.Configuration
{
  public interface IFileSet
  {
    ResourcePivotGroupCollection ResourcePivots { get; }

    IDictionary<string, PreprocessingConfig> Preprocessing { get; }

    IDictionary<string, BundlingConfig> Bundling { get; }

    string Output { get; }

    IList<InputSpec> InputSpecs { get; }

    IList<string> LoadedConfigurationFiles { get; }

    IList<string> Locales { get; }

    IList<string> Themes { get; }
  }
}
