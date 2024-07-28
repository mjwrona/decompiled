// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.StreamUtil
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System.ComponentModel;
using System.IO;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class StreamUtil
  {
    private const int c_buffersize = 4096;

    public static void Copy(Stream input, Stream output, bool repositionOutputAfterCopy)
    {
      ArgumentUtility.CheckForNull<Stream>(input, nameof (input));
      ArgumentUtility.CheckForNull<Stream>(output, nameof (output));
      byte[] buffer = new byte[4096];
      int count;
      while ((count = input.Read(buffer, 0, 4096)) > 0)
        output.Write(buffer, 0, count);
      if (!repositionOutputAfterCopy)
        return;
      output.Position = 0L;
    }
  }
}
