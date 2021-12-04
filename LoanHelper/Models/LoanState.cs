namespace LoanHelper.Models
{
    public class LoanState
    {
        /// <summary>
        /// Principal amount left on loan
        /// </summary>
        public double CurrentPrincipal { get; set; }
        /// <summary>
        /// Amount of interest added this period
        /// </summary>
        public double Interest { get; set; }
        public int PaymentNumber { get; set; }
    }
}