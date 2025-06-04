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
            Child = 1,      // 18 - 25
            Teenager = 2,   // 26 - 35
            Young = 3,      // 36 - 45
            Aged = 4,       // 46 - 50
            Old = 5         // > 50
        }



        public enum Experience
        {
            Undefined = -1,
            NoExperience = 0, // 0 - 1 years
            LowExperience = 1, // 2 - 3 years
            MediumExperience = 2, // 4 - 5 years
            HighExperience = 3, // > 6 -10 years
            Expert = 4 // > 10 years
        }
       

        public enum ProfileStatus
        {
            InActive = 0,
            Active = 1,
        }

        public enum MaritalStatus
        {
            Undefined = -1,
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
