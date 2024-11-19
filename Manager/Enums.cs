using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grievancemis.Manager
{
    public static class Enums
    {
        public enum eAlertType
        {
            error = 0,
            success = 1,
            info = 2,
            warning = 3
        }
        public enum ModuleBatches
        {
            Assessment = 2,
        }
        public enum ePlaced
        {
            Yes = 1,
            No = 0,
            //Other = 3,
        }
        public enum EnumRatingGB
        {
            Poor = 1,
            Excellent = 5,
        }
        public enum eCallLimitDuration
        {
            FixedCallLimit = 6,
        }
        public enum eTypeCall
        {
            Yes = 1,
            No = 2,
            //Other = 3,
        }
        public enum eTempCallStatus
        {
            [Description("Call")]
            CallNot = 0,
            [Description("Call Done")]
            CallDone = 1,
            [Description("Participant Not Available")]
            CallNotPick = 2,
            [Description("Call In Progress")]
            CallOnProgress = 3,
        }
        public enum eEnumExtension
        {
            [Description("image/*,application/pdf")]
            FileType = 1
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString(); //ToDo:Sunil, change to Enum.GetName() if if doesn't return the name of enum.
        }

        public enum eReturnReg
        {
            [Description("Record has been saved successfully.")]
            Insert = 1,
            [Description("Record has been updated successfully.")]
            Update = 2,
            [Description("Record is already exists.")]
            Already = 3,
            [Description("Error")]
            Error = 4,
            [Description("Record Not Found.")]
            RecordNotFound = 5,

            [Description("Something went wrong while processing your request! Please try again later.")]
            ExceptionError = 6,
            [Description("All fields are mandatory !!.")]
            AllFieldsRequired = 7,
        }

        public enum eIsStatus
        {
            [Description("All fields are mandatory !!.")]
            True = 1,
            False = 0
        }

        public enum ParticipantTypeValue
        {

            [Description("All")]
            All = 0,
            [Description("Male")]
            Male = 1,
            [Description("Female")]
            Female = 2,
            [Description("Male and Female")]
            MaleFemale = 3,
            [Description("Total")]
            Total = 4,
        }

        public enum QuarterTargetType
        {

            [Description("Numeric")]
            Numeric = 1,
            [Description("Boolean")]
            Boolean = 2,

        }
        public enum OptionYesNo
        {
            [Description("Yes")]
            Yes,
            [Description("No")]
            No,
        }
        public enum OptionRevision
        {
            [Description("Revised Request")]
            ReviseRequest=1,
            [Description("Revised")]
            Revised=2,
        }
        public enum OptionMailSubject
        {
            [Description("Member activity updation")]
            MemberUpdation = 1,
            [Description("SPMU Lead requested revision for ")]
            SPMULeadRevisionRequest = 2,
            [Description("Member activity updation revised")]
            MemberUpdationRevised = 3,
            [Description("SPMU Lead Review")]
            SPMULeadReview = 4,
            [Description("Finalize Weekly Report")]
            SPMULeadFinalize = 5,
            [Description("Requested For Compiled Lead Revision")]
            CompiledLeadRevisionRequest = 6,
            [Description("Summative AFD")]
            SummativeAFD =7,
            [Description("Summative AFD Link :")]
            SAFDLink = 8
        }
        public enum OptionMailBody
        {
            [Description("Updated the activity")]
            MemberUpdation = 1,
            [Description("SPMU Lead requested resivion for ")]
            SPMULeadRevisionRequest = 2,
            [Description("Ativity has been revised by SPMU Lead")]
            MemberUpdationRevised = 3,
            [Description("SMPU Lead Review Activity")]
            SPMULeadReview = 4,
            [Description("Finalize Weekly Report")]
            SPMULeadFinalize = 5,


            [Description("Requested For Compiled Lead Revision")]
            CompiledLeadRevisionRequest = 6,
        }
    }
}
