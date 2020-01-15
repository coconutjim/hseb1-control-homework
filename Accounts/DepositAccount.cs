using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounts
{
    class DepositAccount : BankAccount
    {
        public bool Overdraft { get; set; }
        public override string Balance
        {
            get
            {
                return balance;
            }
            set
            {
                double test;
                if (!double.TryParse(value, out test) || test < 0 && this.Overdraft == false)
                    throw new Exception("Ошибка в значении баланса!\nПамятка: при отсутствия возможности овердрафта баланс не может быть отрицательным.");
                balance = test.ToString("F3");
            }
        }
        public override string Percent
        {
            get
            {
                return percent;
            }
            set
            {
                double test;
                if (!double.TryParse(value, out test) || test > 13 || test < 8)
                    throw new Exception("Ошибка в значении процентной ставки!\nПамятка: При этом типе вклада процентная ставка должна принимать значения от 8 до 13");
                percent = test.ToString("F3");
            }
        }
        public DepositAccount(string name, string num, string ody, string odm, string odd, string bal, string per, bool over)
        {
            Name = name; Number = num;
            try
            {
                OpeningDate = new DateTime(int.Parse(ody), int.Parse(odm), int.Parse(odd));
            }
            catch
            {
                throw new Exception("Ошибка в значении даты! Дата должна соответствовать формату ДД-ММ-ГГГГ\nПамятка: Банк был открыт 10.10.2001!");
            }
            Overdraft = over;
            Balance = bal; Percent = per;
        }
        public DepositAccount(string line, byte dataItems)
        {
            string[] ar = line.Split(' ');
            if (ar.Length != dataItems)
                throw new Exception();
            Overdraft = bool.Parse(ar[5]);
            Name = ar[0];
            Number = ar[1];
            OpeningDate = DateTime.Parse(ar[2]);
            Balance = ar[3];
            Percent = ar[4];
        }
        public override string ToStringValue()
        {
            string res = String.Format("Депозитный {0} {1} {2} {3} {4} {5}", Name, Number,
                OpeningDate.ToShortDateString(), Balance, Percent, Overdraft);
            return res;
        }
        public override string ToShortString()
        {
            string res = String.Format("{0} Депозитный", Name);
            return res;
        }
        public override string CalcBal(string per, string bal, string fby, string fbm, string fbd)
        {
            try
            {
                DateTime d = new DateTime(int.Parse(fby), int.Parse(fbm), int.Parse(fbd));
                DateTime now = DateTime.Now;
                if (d.CompareTo(now) < 0 || d.CompareTo(OpeningDate) < 0 || d.CompareTo(new DateTime(2050, 01, 01)) > 0)
                    throw new Exception();
                int years = d.Year - now.Year;
                if (d.AddYears(OpeningDate.Year - d.Year).CompareTo(OpeningDate) < 0) years--;
                double newBal = double.Parse(bal);
                if (newBal > 0)
                    for (int i = 0; i < years; i++) newBal *= (double.Parse(per) / 100 + 1);
                return newBal.ToString("F3");
            }
            catch
            {
                throw new Exception("Ошибка в значении даты! Дата должна соответствовать формату ДД-ММ-ГГГГ!\nПамятка: 1) Дата должна быть позднее текущей.\n2) Проверьте, попадает ли дата в интервал времени существования счёта.\n3) Дата не может быть позднее 01.01.2050.");
            }
        }
    }
}
