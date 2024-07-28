// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.MopupExtensions
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.CodeSense.Server.Data;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public static class MopupExtensions
  {
    public static int GetSourceId(SliceSource sliceSource) => sliceSource == SliceSource.Keepup ? 0 : 1;

    public static void AddMopupItemToQueue(
      IVssRequestContext requestContext,
      int changesetId,
      SliceSource sliceSource)
    {
      using (requestContext.AcquireExemptionLock())
      {
        using (ICodeSenseSqlResourceComponent component = requestContext.CreateComponent<ICodeSenseSqlResourceComponent, CodeSenseSqlResourceComponent>())
        {
          component.AddMopupItemToQueue(changesetId, MopupExtensions.GetSourceId(sliceSource));
          requestContext.Trace(1025045, TraceLayer.Job, string.Format("Adding changeset Id {0} with slice souce {1} to mopup queue", (object) changesetId, (object) sliceSource));
        }
      }
    }
  }
}
