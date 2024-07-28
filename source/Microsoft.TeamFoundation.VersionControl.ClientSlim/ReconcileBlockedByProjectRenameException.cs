// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Client.ReconcileBlockedByProjectRenameException
// Assembly: Microsoft.TeamFoundation.VersionControl.ClientSlim, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CF6BF9DB-38AD-4731-862B-31BA91580FFB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.ClientSlim.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Client
{
  [Serializable]
  public class ReconcileBlockedByProjectRenameException : VersionControlException
  {
    private string[] m_oldProjectNames;
    private string[] m_newProjectNames;

    public ReconcileBlockedByProjectRenameException(string message)
      : base(message)
    {
    }

    public ReconcileBlockedByProjectRenameException(string message, Exception ex)
      : base(message, ex)
    {
    }

    protected ReconcileBlockedByProjectRenameException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }

    public string[] OldProjectNames
    {
      get
      {
        string[] oldProjectNames = this.m_oldProjectNames;
        if (oldProjectNames == null)
        {
          object[] property = this.GetProperty<object[]>("Microsoft.TeamFoundation.VersionControl.OldProjectNames");
          if (property != null)
          {
            oldProjectNames = new string[property.Length];
            for (int index = 0; index < property.Length; ++index)
              oldProjectNames[index] = (string) property[index];
          }
          else
            oldProjectNames = Array.Empty<string>();
          this.m_oldProjectNames = oldProjectNames;
        }
        return oldProjectNames;
      }
    }

    public string[] NewProjectNames
    {
      get
      {
        string[] newProjectNames = this.m_newProjectNames;
        if (newProjectNames == null)
        {
          object[] property = this.GetProperty<object[]>("Microsoft.TeamFoundation.VersionControl.NewProjectNames");
          if (property != null)
          {
            newProjectNames = new string[property.Length];
            for (int index = 0; index < property.Length; ++index)
              newProjectNames[index] = (string) property[index];
          }
          else
            newProjectNames = Array.Empty<string>();
          this.m_newProjectNames = newProjectNames;
        }
        return newProjectNames;
      }
    }

    public int NewProjectRevisionId => this.GetProperty<int>("Microsoft.TeamFoundation.VersionControl.NewProjectRevisionId");
  }
}
