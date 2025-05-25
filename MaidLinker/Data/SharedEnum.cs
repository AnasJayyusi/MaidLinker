namespace MaidLinker.Data
{
    public class SharedEnum
    {
        public enum AccountTypeEnum
        {
            Administrator = 0,
            Reception = 1,
            Accountant = 2
        }

        public enum NotificationTypeEnum
        {
            NewOrder = 1,
            ApprovedOrder = 2,
            RejectOrder = 3,
            SendProfileToReview = 4,
            RejectProfile = 5,
            SendNewService = 6,
            ApprovedNewService = 7,
            RejectNewService = 8,
            ActivateUserProfile = 9,
            DeactivateUserProfile = 10,

        }

        public enum Gender
        {
            Undefined = -1,
            Female = 0,
            Male = 1
        }

        public enum Age
        {
            Undefined = -1,
            Child = 0, // 4 - 10 
            Teenager = 1, // 11 -25
            Young = 2, // 26 - 50
            Aged = 3 // > 50 
        }

        public enum ProfileStatus
        {
            InActive = 0,
            Active = 1,
        }

        public enum MaritalStatus
        {
            Single = 0,
            Married = 1,
            Divorced = 2,
            Widowed = 3,
        }

        public enum AttachmentType
        {
            Medical = 0,
            Residency = 1,
            Passport = 2,
            Other=3
        }

    }
}
