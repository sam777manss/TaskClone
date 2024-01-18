using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonModel.Enum
{
    public class Enum
    {
        public enum SortDirectionType
        {
            Ascending = 0,
            Descending = 1
        }

        public enum FilterOperatorType
        {
            IsLessThan = 0,
            IsLessThanOrEqualTo = 1,
            IsEqualTo = 2,
            IsNotEqualTo = 3,
            IsGreaterThanOrEqualTo = 4,
            IsGreaterThan = 5,
            StartsWith = 6,
            EndsWith = 7,
            Contains = 8,
            IsContainedIn = 9,
            DoesNotContain = 10,
            IsNull = 11,
            IsNotNull = 12,
            IsEmpty = 13,
            IsNotEmpty = 14,
            IsNullOrEmpty = 15,
            IsNotNullOrEmpty = 16
        }

        public enum ResponseStatusCode : int
        {
            OK = 200,
            BadRequest = 400,
            Unauthorized = 401,
            Forbidden = 403,
            NotFound = 404,
            Conflict = 409,
            InternalServerError = 500,
            NotActivated = 300,
            UpgradeRequired = 426
        }

        public enum Status : int
        {
            Inactive = 0,
            Active = 1,
            Delete = 2
        }

        public enum FieldTypes : int
        {
            TextField = 1,
            Image = 2,
            Radio = 3,
            SelectBox = 4,
            NoteField = 5
        }

        public enum DocTypeFor : int
        {
            Default = 0,
            IncomeStatement = 1,
        }
    }
}
