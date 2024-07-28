// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ReconcileBlockedByProjectRenameException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class ReconcileBlockedByProjectRenameException : TeamFoundationServiceException
  {
    private readonly ExceptionPropertyCollection m_properties;

    public ReconcileBlockedByProjectRenameException()
      : base(Resources.Get(nameof (ReconcileBlockedByProjectRenameException)))
    {
      this.m_properties = new ExceptionPropertyCollection();
    }

    public string[] OldProjectNames
    {
      set => this.m_properties.Set("Microsoft.TeamFoundation.VersionControl.OldProjectNames", value);
    }

    public string[] NewProjectNames
    {
      set => this.m_properties.Set("Microsoft.TeamFoundation.VersionControl.NewProjectNames", value);
    }

    public int NewProjectRevisionId
    {
      set => this.m_properties.Set("Microsoft.TeamFoundation.VersionControl.NewProjectRevisionId", value);
    }

    public ExceptionPropertyCollection PropertyCollection => this.m_properties;

    public override void GetExceptionProperties(ExceptionPropertyCollection properties)
    {
      base.GetExceptionProperties(properties);
      if (this.m_properties == null)
        return;
      properties.Copy(this.m_properties);
    }
  }
}
