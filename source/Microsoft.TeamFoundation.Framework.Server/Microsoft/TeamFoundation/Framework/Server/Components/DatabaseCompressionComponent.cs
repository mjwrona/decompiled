// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Components.DatabaseCompressionComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server.Components
{
  internal class DatabaseCompressionComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<DatabaseCompressionComponent>(1)
    }, "DatabaseCompression");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      [800110] = new SqlExceptionFactory(typeof (IndexRebuildInProgressException))
    };

    public void ChangeCompression(bool online)
    {
      try
      {
        this.InfoMessage += new SqlInfoMessageEventHandler(this.OnInfoMessage);
        this.PrepareStoredProcedure("prc_ChangeCompression", 0);
        this.BindBoolean("@online", online);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.InfoMessage -= new SqlInfoMessageEventHandler(this.OnInfoMessage);
      }
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) DatabaseCompressionComponent.s_sqlExceptionFactories;

    private void OnInfoMessage(object sender, SqlInfoMessageEventArgs e) => this.LogInfoMessage(e, this.Logger, true);
  }
}
