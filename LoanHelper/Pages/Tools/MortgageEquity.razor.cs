using System.Collections.Generic;
using System.Linq;
using LoanHelper.Models;
using Microsoft.AspNetCore.Components;

namespace LoanHelper.Pages.Tools
{
    public partial class MortgageEquity : ComponentBase
    {
        private Loan _loan = new Loan();

        private void Calculate()
        {
            _loan.LoanStates = new List<LoanState>
            {
                new LoanState
                {
                    CurrentPrincipal = _loan.PrincipalValue,
                    Interest = 0,
                    PaymentNumber = 0
                }
            };
            
            while (_loan.LoanStates.Last().CurrentPrincipal > 0)
            {
                var interest = _loan.LoanStates.Last().CurrentPrincipal * ((_loan.InterestRate / 12d) / 100d);
                
                _loan.LoanStates.Add(new LoanState
                {
                    Interest = interest,
                    CurrentPrincipal = _loan.LoanStates.Last().CurrentPrincipal - (_loan.PaymentAmount - interest - _loan.EscrowPayment),
                    PaymentNumber = ++_loan.LoanStates.Last().PaymentNumber
                });    
            }
        }
    }
}