// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Compression.SR
// Assembly: Microsoft.TeamFoundation.Server.Compression, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E666AAE4-36CD-4581-80AF-1B631308AB46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.Compression.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.Compression
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class SR
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal SR()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (SR.resourceMan == null)
          SR.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.Compression.SR", typeof (SR).Assembly);
        return SR.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => SR.resourceCulture;
      set => SR.resourceCulture = value;
    }

    internal static string ArgumentOutOfRange_Enum => SR.ResourceManager.GetString(nameof (ArgumentOutOfRange_Enum), SR.resourceCulture);

    internal static string CannotReadFromDeflateStream => SR.ResourceManager.GetString(nameof (CannotReadFromDeflateStream), SR.resourceCulture);

    internal static string CannotWriteToDeflateStream => SR.ResourceManager.GetString(nameof (CannotWriteToDeflateStream), SR.resourceCulture);

    internal static string GenericInvalidData => SR.ResourceManager.GetString(nameof (GenericInvalidData), SR.resourceCulture);

    internal static string InvalidArgumentOffsetCount => SR.ResourceManager.GetString(nameof (InvalidArgumentOffsetCount), SR.resourceCulture);

    internal static string InvalidBeginCall => SR.ResourceManager.GetString(nameof (InvalidBeginCall), SR.resourceCulture);

    internal static string NotSupported => SR.ResourceManager.GetString(nameof (NotSupported), SR.resourceCulture);

    internal static string NotSupported_UnreadableStream => SR.ResourceManager.GetString(nameof (NotSupported_UnreadableStream), SR.resourceCulture);

    internal static string NotSupported_UnwritableStream => SR.ResourceManager.GetString(nameof (NotSupported_UnwritableStream), SR.resourceCulture);

    internal static string ObjectDisposed_StreamClosed => SR.ResourceManager.GetString(nameof (ObjectDisposed_StreamClosed), SR.resourceCulture);

    internal static string UnsupportedCompression => SR.ResourceManager.GetString(nameof (UnsupportedCompression), SR.resourceCulture);

    internal static string ZLibErrorDLLLoadError => SR.ResourceManager.GetString(nameof (ZLibErrorDLLLoadError), SR.resourceCulture);

    internal static string ZLibErrorInconsistentStream => SR.ResourceManager.GetString(nameof (ZLibErrorInconsistentStream), SR.resourceCulture);

    internal static string ZLibErrorIncorrectInitParameters => SR.ResourceManager.GetString(nameof (ZLibErrorIncorrectInitParameters), SR.resourceCulture);

    internal static string ZLibErrorNotEnoughMemory => SR.ResourceManager.GetString(nameof (ZLibErrorNotEnoughMemory), SR.resourceCulture);

    internal static string ZLibErrorUnexpected => SR.ResourceManager.GetString(nameof (ZLibErrorUnexpected), SR.resourceCulture);

    internal static string ZLibErrorVersionMismatch => SR.ResourceManager.GetString(nameof (ZLibErrorVersionMismatch), SR.resourceCulture);
  }
}
