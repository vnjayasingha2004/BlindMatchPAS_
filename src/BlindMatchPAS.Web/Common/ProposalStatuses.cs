namespace BlindMatchPAS.Web.Models.Common
{
    public static class ProposalStatuses
    {
        public const string Pending = "Pending";
        public const string UnderReview = "Under Review";
        public const string Matched = "Matched";
        public const string Withdrawn = "Withdrawn";

        public static readonly string[] ActiveStatuses = [Pending, UnderReview, Matched, Withdrawn];

        public static bool IsOpenForStudentChanges(string status) =>
            status == Pending || status == UnderReview;

        public static bool IsBlindReviewVisible(string status) =>
            status == Pending || status == UnderReview;
    }
}
