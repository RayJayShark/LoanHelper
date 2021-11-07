using System;
using Microsoft.AspNetCore.Components;

namespace LoanHelper.Pages.Tools
{
    public partial class PaymentCalculator : ComponentBase
    {
        private double _loanValue;
        private double _interestRate;
        private int _numberOfPayments;
        private double _paymentAmount;

        private int _interestType = 0;

        private void ChangeInterestType()
        {
            // No calculation needed
            if (_interestRate == 0) return;
            
            // Changed to annual
            if (_interestType == 0)
            {
                _interestRate *= 12;
            }
            // Changed to monthly
            else
            {
                _interestRate /= 12d;
            }
        }
        
        private void CalculatePayment()
        {
            // r(PV)/1-(1+r)^-n
            var percentRate = _interestType == 0 ? (_interestRate / 100d) / 12d : _interestRate / 100d;
            _paymentAmount = (percentRate * _loanValue) / (1 - Math.Pow(1 + percentRate, _numberOfPayments * -1));
        }
    }
}