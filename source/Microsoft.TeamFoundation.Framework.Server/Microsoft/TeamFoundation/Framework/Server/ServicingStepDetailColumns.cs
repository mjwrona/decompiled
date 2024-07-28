// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingStepDetailColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingStepDetailColumns : ObjectBinder<ServicingStepDetail>
  {
    private SqlColumnBinder detailDataColumn = new SqlColumnBinder("DetailData");
    private SqlColumnBinder detailIdColumn = new SqlColumnBinder("DetailId");
    private SqlColumnBinder detailTimeColumn = new SqlColumnBinder("DetailTime");
    private static readonly XmlSerializer s_stepDetailArraySerializer = new XmlSerializer(typeof (ServicingStepDetail[]));

    protected override ServicingStepDetail Bind()
    {
      string s = this.detailDataColumn.GetString((IDataReader) this.Reader, false);
      ServicingStepDetail[] servicingStepDetailArray = (ServicingStepDetail[]) ServicingStepDetailColumns.s_stepDetailArraySerializer.Deserialize((TextReader) new StringReader(s));
      servicingStepDetailArray[0].DetailId = this.detailIdColumn.GetInt64((IDataReader) this.Reader);
      servicingStepDetailArray[0].DetailTime = this.detailTimeColumn.GetDateTime((IDataReader) this.Reader);
      return servicingStepDetailArray[0];
    }
  }
}
