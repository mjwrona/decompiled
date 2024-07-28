// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityMessageHelpers
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class IdentityMessageHelpers
  {
    public static IdentityMessageHelpers.IdentityMessageSummary CollapseMessages(
      List<IdentityMessage> messages)
    {
      IdentityMessageHelpers.IdentityMessageSummary identityMessageSummary = new IdentityMessageHelpers.IdentityMessageSummary();
      if (messages.Count > 1)
      {
        List<IdentityMessageHelpers.Range> rangeList = new List<IdentityMessageHelpers.Range>();
        IdentityMessageHelpers.Range range1 = new IdentityMessageHelpers.Range(0, 1);
        for (int index = 0; index < messages.Count - 1; ++index)
        {
          IdentityMessage message1 = messages[index];
          IdentityMessage message2 = messages[index + 1];
          if (message1.HostId == message2.HostId && message1.DescriptorChanges == null && message2.DescriptorChanges == null && message1.DescriptorChangeType == DescriptorChangeType.None && message2.DescriptorChangeType == DescriptorChangeType.None)
          {
            ++range1.Length;
          }
          else
          {
            rangeList.Add(range1);
            range1 = new IdentityMessageHelpers.Range(index + 1, 1);
          }
        }
        rangeList.Add(range1);
        for (int index1 = rangeList.Count - 1; index1 >= 0; --index1)
        {
          IdentityMessageHelpers.Range range2 = rangeList[index1];
          if (range2.Length > 1)
          {
            HashSet<Guid> source1 = new HashSet<Guid>();
            HashSet<Guid> source2 = new HashSet<Guid>();
            List<MembershipChangeInfo> membershipChangeInfoList = new List<MembershipChangeInfo>();
            for (int index2 = 0; index2 < range2.Length; ++index2)
            {
              IdentityMessage message = messages[range2.Start + index2];
              if (message.IdentityChanges != null)
              {
                foreach (Guid identityChange in message.IdentityChanges)
                  source1.Add(identityChange);
                identityMessageSummary.IdentityChangeCount += message.IdentityChanges.Length;
              }
              if (message.GroupChanges != null)
              {
                foreach (Guid groupChange in message.GroupChanges)
                  source2.Add(groupChange);
                identityMessageSummary.GroupChangeCount += message.GroupChanges.Length;
              }
              if (message.MembershipChanges != null)
              {
                membershipChangeInfoList.AddRange((IEnumerable<MembershipChangeInfo>) message.MembershipChanges);
                identityMessageSummary.MembershipChangeCount += message.MembershipChanges.Length;
              }
            }
            IdentityMessage identityMessage = new IdentityMessage()
            {
              HostId = messages[range2.Start].HostId,
              DescriptorChanges = (Guid[]) null,
              IdentityChanges = source1.Count > 0 ? source1.ToArray<Guid>() : (Guid[]) null,
              GroupChanges = source2.Count > 0 ? source2.ToArray<Guid>() : (Guid[]) null,
              MembershipChanges = membershipChangeInfoList.Count > 0 ? membershipChangeInfoList.ToArray() : (MembershipChangeInfo[]) null,
              DescriptorChangeType = DescriptorChangeType.None
            };
            messages.RemoveRange(range2.Start + 1, range2.Length - 1);
            messages[range2.Start] = identityMessage;
          }
          else
          {
            IdentityMessage message = messages[range2.Start];
            if (message.IdentityChanges != null)
              identityMessageSummary.IdentityChangeCount += message.IdentityChanges.Length;
            if (message.GroupChanges != null)
              identityMessageSummary.GroupChangeCount += message.GroupChanges.Length;
            if (message.MembershipChanges != null)
              identityMessageSummary.MembershipChangeCount += message.MembershipChanges.Length;
          }
        }
      }
      return identityMessageSummary;
    }

    public struct IdentityMessageSummary
    {
      public int IdentityChangeCount;
      public int GroupChangeCount;
      public int MembershipChangeCount;
    }

    private struct Range
    {
      public int Start;
      public int Length;

      public Range(int start, int length)
      {
        this.Start = start;
        this.Length = length;
      }
    }
  }
}
