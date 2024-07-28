// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.ExceptionMessage
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class ExceptionMessage
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal ExceptionMessage()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ExceptionMessage.resourceMan == null)
          ExceptionMessage.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.Search.Common.ExceptionMessage", typeof (ExceptionMessage).Assembly);
        return ExceptionMessage.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ExceptionMessage.resourceCulture;
      set => ExceptionMessage.resourceCulture = value;
    }

    internal static string AccountMigratorException => ExceptionMessage.ResourceManager.GetString(nameof (AccountMigratorException), ExceptionMessage.resourceCulture);

    internal static string AdditionalDetails => ExceptionMessage.ResourceManager.GetString(nameof (AdditionalDetails), ExceptionMessage.resourceCulture);

    internal static string CrawlerException => ExceptionMessage.ResourceManager.GetString(nameof (CrawlerException), ExceptionMessage.resourceCulture);

    internal static string FeederException => ExceptionMessage.ResourceManager.GetString(nameof (FeederException), ExceptionMessage.resourceCulture);

    internal static string GitRepoSyncAnalyzerException => ExceptionMessage.ResourceManager.GetString(nameof (GitRepoSyncAnalyzerException), ExceptionMessage.resourceCulture);

    internal static string IndexerException => ExceptionMessage.ResourceManager.GetString(nameof (IndexerException), ExceptionMessage.resourceCulture);

    internal static string IndexProvisionException => ExceptionMessage.ResourceManager.GetString(nameof (IndexProvisionException), ExceptionMessage.resourceCulture);

    internal static string ParserException => ExceptionMessage.ResourceManager.GetString(nameof (ParserException), ExceptionMessage.resourceCulture);

    internal static string SearchServiceException => ExceptionMessage.ResourceManager.GetString(nameof (SearchServiceException), ExceptionMessage.resourceCulture);

    internal static string StoreException => ExceptionMessage.ResourceManager.GetString(nameof (StoreException), ExceptionMessage.resourceCulture);

    internal static string StreamLoggerException => ExceptionMessage.ResourceManager.GetString(nameof (StreamLoggerException), ExceptionMessage.resourceCulture);
  }
}
