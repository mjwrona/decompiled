// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.TracePoints
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.Directories
{
  public static class TracePoints
  {
    public static class Area
    {
      public const string Directories = "Directories";
      public const string DirectoryService = "DirectoryService";
    }

    public static class DirectoryEntityDescriptorToEntityConverter
    {
      public static class TryConvertDescriptor
      {
        public const int Enter = 10026000;
        public const int Input = 10026001;
        public const int InputNull = 10026002;
        public const int EntityTypeInvalid = 10026003;
        public const int OriginDirectoryInvalid = 10026004;
        public const int LocalDirectoryInvalid = 10026005;
        public const int OriginIdInvalid = 10026006;
        public const int LocalIdInvalid = 10026007;
        public const int CannotCreateEntity = 10026012;
        public const int OutputValid = 10026018;
        public const int Leave = 10026019;
      }
    }

    public static class FrameworkDirectoryService
    {
      public static class AddMemberByString
      {
        public const int Enter = 10026010;
        public const int Leave = 10026019;
      }

      public static class AddMemberByEntityDescriptor
      {
        public const int Enter = 10026020;
        public const int Leave = 10026029;
      }

      public static class AddMembersByEntityDescriptors
      {
        public const int Enter = 10026030;
        public const int Leave = 10026039;
      }
    }
  }
}
