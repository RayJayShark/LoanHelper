using System.Collections.Generic;
using System.Linq;
using LoanHelper.Models;
using Microsoft.AspNetCore.Components;

namespace LoanHelper.Pages.Tools
{
    public partial class MortgageEquity : ComponentBase
    {
        private readonly Loan _loan = new Loan();
    }
}