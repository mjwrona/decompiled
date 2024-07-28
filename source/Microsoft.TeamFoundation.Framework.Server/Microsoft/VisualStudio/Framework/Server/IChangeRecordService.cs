// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Framework.Server.IChangeRecordService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Framework.Server
{
  [DefaultServiceImplementation(typeof (BaseChangeRecordService))]
  public interface IChangeRecordService : IVssFrameworkService
  {
    void LogCompletedChangeEvent(string title, string description, byte priority = 2);

    void LogFailedChangeEvent(string title, string description, byte priority = 2);

    StartedChangeRecordInfo StartChangeEvent(string title, string description, byte priority = 2);

    void EndChangeEvent(StartedChangeRecordInfo info, bool changeFailed);

    void SetReleaseContext(ChangeRecordReleaseContext releaseContext);
  }
}
