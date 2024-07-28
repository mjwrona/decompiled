// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.IHtmlFilterWriter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  internal interface IHtmlFilterWriter
  {
    void WriteText(string s, int offs, int len);

    void WriteTag(string s, int offs, int len, string tag, bool endTag);

    void WriteEndOfTag(string s, int offs, int len, string tag);

    void WriteAttribute(string s, int offs, int len, string tag, string attr, int i1, int i2);
  }
}
