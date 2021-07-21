using System;
namespace utils
{
    public static class RegexPatterns
    {
        //for numbers greater than zero with a 0.01 accuracy
        public const string PositiveNumberTwoDecimal =
            @"^([1-9]+\d*\.?\d?\d?)|(0\.((\d[1-9])|([1-9]\d?)))$";

        // for phone numbers in the form (0x) xxxx xxxx
        public const string PhoneNumberWithParentheses =
            @"^\(0\d\)(?:\s\d{4}){2}$";
    }
}
