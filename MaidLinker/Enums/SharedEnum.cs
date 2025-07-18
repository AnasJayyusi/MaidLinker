﻿namespace MaidLinker.Enums
{
    public class SharedEnum
    {
        public enum AccountTypeEnum
        {
            Administrator = 0,
            Reception = 1,
            Accountant = 2,
            All = 3 // For all users
        }

        public enum NotificationTypeEnum
        {
            NewRequest = 1,
            TakeOverRequest = 2,
            Confirm = 3,
            Cancel = 4,
            Completed = 5
        }

        public enum RequestStatus
        {
            New = 0,      // الطلب قيد الانتظار
            InProgress = 1,   // الطلب قيد المعالجة
            Prepared = 2,    // تم تجهيز الطلب 
            Cancelled = 3,    // الطلب ملغى
            Completed = 4      // الطلب مكتمل
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
            Other = 3
        }


        public enum FinancialEntryType
        {
            Income = 1,
            Expense = 2
        }

        public enum FeedbackStatusEnum
        {
            Unread = 0,
            Seen = 1
        }

        public enum WorkFlowStatus
        {
            NotStarted = 0,              // لم يتم أي إجراء بعد
            ContactedCustomer = 1,        // اتصال بالزبون
            DocumentsDelivered = 2,       // تسليم الأوراق
            ProcessingStarted = 3         // تم السير بالمعاملة

        }
    }
}
