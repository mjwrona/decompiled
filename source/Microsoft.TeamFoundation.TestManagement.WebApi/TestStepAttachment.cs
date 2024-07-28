// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestStepAttachment
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  internal class TestStepAttachment : ITestStepAttachment, ITestAttachment
  {
    private string m_name;
    private string m_comment;
    private string m_url;

    public TestStepAttachment(string url, string name = null)
    {
      this.m_url = url;
      this.m_name = name;
      this.m_comment = (string) null;
    }

    public string Url => this.m_url;

    public string Comment
    {
      get => this.m_comment;
      set => this.m_comment = value;
    }

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }
  }
}
