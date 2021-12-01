using System;
using LoanHelper.Models;
using Microsoft.AspNetCore.Components;

namespace LoanHelper.Pages.Tools
{
    public partial class PaymentCalculator : ComponentBase
    {
        private readonly Loan _loan = new Loan();
    }
}