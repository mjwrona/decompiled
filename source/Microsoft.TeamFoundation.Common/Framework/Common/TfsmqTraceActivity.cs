// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.TfsmqTraceActivity
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Common
{
  internal abstract class TfsmqTraceActivity : ITeamFoundationTraceable
  {
    protected abstract void OnWriteXml(XmlDictionaryWriter writer, TraceLevel level);

    string ITeamFoundationTraceable.GetTraceString(TraceLevel traceLevel)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (XmlDictionaryWriter textWriter = XmlDictionaryWriter.CreateTextWriter((Stream) memoryStream, Encoding.UTF8, false))
          this.OnWriteXml(textWriter, traceLevel);
        memoryStream.Seek(0L, SeekOrigin.Begin);
        return Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
      }
    }

    public override string ToString() => ((ITeamFoundationTraceable) this).GetTraceString(TraceLevel.Verbose);
  }
}
