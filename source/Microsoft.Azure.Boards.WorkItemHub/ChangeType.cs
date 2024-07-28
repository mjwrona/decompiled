// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WorkItemHub.ChangeType
// Assembly: Microsoft.Azure.Boards.WorkItemHub, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 749A696A-54F8-4B6F-8877-B350F1725C24
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.WorkItemHub.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.Boards.WorkItemHub
{
  [DataContract]
  public enum ChangeType
  {
    New,
    Change,
    Delete,
    Restore,
  }
}
