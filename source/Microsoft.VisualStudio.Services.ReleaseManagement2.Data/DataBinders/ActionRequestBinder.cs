// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ActionRequestBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ActionRequestBinder : ReleaseManagementObjectBinderBase<ActionRequest>
  {
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder resourceId = new SqlColumnBinder("ResourceId");
    private SqlColumnBinder actionRequestType = new SqlColumnBinder("ActionRequestType");
    private SqlColumnBinder createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder actionRequestData = new SqlColumnBinder("ActionRequestData");
    private SqlColumnBinder numAttempts = new SqlColumnBinder("NumAttempts");
    private SqlColumnBinder lastAttemptOn = new SqlColumnBinder("LastAttemptOn");
    private SqlColumnBinder lastException = new SqlColumnBinder("LastException");

    public ActionRequestBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ActionRequest Bind()
    {
      ActionRequestData actionRequestData = ActionRequestBinder.ReadActionRequestData(this.actionRequestData.GetString((IDataReader) this.Reader, false));
      return new ActionRequest()
      {
        Id = this.id.GetInt32((IDataReader) this.Reader),
        ResourceId = this.resourceId.GetString((IDataReader) this.Reader, false),
        ActionRequestType = (ActionRequestType) this.actionRequestType.GetInt32((IDataReader) this.Reader),
        CreatedOn = this.createdOn.GetDateTime((IDataReader) this.Reader),
        ActionRequestData = actionRequestData,
        NumberOfAttempts = (int) this.numAttempts.GetByte((IDataReader) this.Reader),
        LastAttemptOn = this.lastAttemptOn.GetDateTime((IDataReader) this.Reader),
        LastException = this.lastException.GetString((IDataReader) this.Reader, false)
      };
    }

    public static ActionRequestData ReadActionRequestData(string json)
    {
      MemoryStream memoryStream = (MemoryStream) null;
      try
      {
        ActionRequestData actionRequestData = new ActionRequestData();
        memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        return new DataContractJsonSerializer(actionRequestData.GetType()).ReadObject((Stream) memoryStream) as ActionRequestData;
      }
      finally
      {
        memoryStream?.Close();
      }
    }
  }
}
