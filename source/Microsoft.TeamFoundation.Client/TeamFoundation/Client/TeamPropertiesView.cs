// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TeamPropertiesView
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

namespace Microsoft.TeamFoundation.Client
{
  public abstract class TeamPropertiesView
  {
    private string m_namespace;

    public TeamFoundationTeam Team { get; private set; }

    protected virtual string ViewNamespace
    {
      get
      {
        if (this.m_namespace == null)
          this.m_namespace = this.GetType().FullName + ".";
        return this.m_namespace;
      }
    }

    protected internal virtual void Initialize(TfsConnection tfs, TeamFoundationTeam team) => this.Team = team;

    public void SetViewProperty(string propertyName, object propertyValue) => this.Team.SetProperty(this.ViewNamespace + propertyName, propertyValue);

    public void RemoveViewProperty(string propertyName) => this.SetViewProperty(propertyName, (object) null);

    public object GetViewProperty(string propertyName) => this.Team.GetProperty(this.ViewNamespace + propertyName);

    public bool TryGetViewProperty(string propertyName, out object propertyValue) => this.Team.TryGetProperty(this.ViewNamespace + propertyName, out propertyValue);
  }
}
