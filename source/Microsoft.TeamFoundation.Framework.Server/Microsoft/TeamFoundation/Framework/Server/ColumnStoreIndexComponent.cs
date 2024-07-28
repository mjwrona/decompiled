// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ColumnStoreIndexComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ColumnStoreIndexComponent : TeamFoundationSqlResourceComponent
  {
    public const string ServiceName = "ColumnStoreIndex";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<ColumnStoreIndexComponent>(1)
    }, "ColumnStoreIndex");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    public void SwitchAnalytics(bool? on = null)
    {
      try
      {
        this.InfoMessage += new SqlInfoMessageEventHandler(this.OnInfoMessage);
        this.PrepareStoredProcedure("AnalyticsInternal.prc_ToggleColumnstoreIndexes", false);
        if (on.HasValue)
          this.BindBoolean("@enableCCI", on.Value);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.InfoMessage -= new SqlInfoMessageEventHandler(this.OnInfoMessage);
      }
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ColumnStoreIndexComponent.s_sqlExceptionFactories;

    private void OnInfoMessage(object sender, SqlInfoMessageEventArgs e) => this.LogInfoMessage(e, this.Logger, true);
  }
}
