// Decompiled with JetBrains decompiler
// Type: WebGrease.CacheSourceDependency
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.IO;
using System.Linq;
using WebGrease.Configuration;
using WebGrease.Extensions;

namespace WebGrease
{
  public class CacheSourceDependency
  {
    public InputSpec InputSpec { get; private set; }

    public string InputSpecHash { get; private set; }

    internal static CacheSourceDependency Create(IWebGreaseContext context, InputSpec inputSpec)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (inputSpec == null)
        throw new ArgumentNullException(nameof (inputSpec));
      CacheSourceDependency sourceDependency = new CacheSourceDependency();
      if (Directory.Exists(inputSpec.Path))
        inputSpec.Path.EnsureEndSeparator();
      sourceDependency.InputSpecHash = CacheSourceDependency.GetInputSpecHash(context, inputSpec);
      inputSpec.Path = inputSpec.Path.MakeRelativeToDirectory(context.Configuration.SourceDirectory);
      sourceDependency.InputSpec = inputSpec;
      return sourceDependency;
    }

    internal bool HasChanged(IWebGreaseContext context) => !this.InputSpecHash.Equals(CacheSourceDependency.GetInputSpecHash(context, this.InputSpec), StringComparison.Ordinal);

    private static string GetInputSpecHash(IWebGreaseContext context, InputSpec inputSpec) => inputSpec.GetFiles(context.Configuration.SourceDirectory).ToDictionary<string, string, string>((Func<string, string>) (f => f.MakeRelativeToDirectory(context.Configuration.SourceDirectory)), new Func<string, string>(context.GetFileHash)).ToJson();
  }
}
