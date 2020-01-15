using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounts
{
    class ReplenishableUrgentAccount:BankAccount
    {
        string minReplSum;
        DateTime closingDate;
        public bool EarlyClosing { get; set; }
        public string MinReplSum
        {
            get
            {
                return minReplSum;
            }
            set
            {
                double test;
                if (!double.TryParse(value, out test))
                    throw new Exception("Ошибка в значении минимальный суммы пополнения!\nПамятка:");
                minReplSum = test.ToString("F3");
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
                if (!double.TryParse(value, out test) || test > 14 || test < 9)
                    throw new Exception("Ошибка в значении процентной ставки!\nПамятка: При этом типе вклада процентная ставка должна принимать значения от 9 до 14");
                percent = test.ToString("F3");
            }
        }
        public DateTime ClosingDate
        {
            get
            {
                return closingDate;
            }
            set
            {
                if (value.CompareTo(this.OpeningDate.AddMonths(8)) < 0 || value.CompareTo(OpeningDate.AddYears(12)) > 0)
                    throw new Exception();
                closingDate = value;
            }
        }
        public ReplenishableUrgentAccount(string name, string num, string ody, string odm, string odd, string bal, string per,
            string cdy, string cdm, string cdd, bool ec, string minsum)
            : base(name, num, ody, odm, odd, bal, per)
        {
            try
            {
                ClosingDate = new DateTime(int.Parse(cdy), int.Parse(cdm), int.Parse(cdd));
            }
            catch
            {
                throw new Exception("Ошибка в значении даты! Дата должна соответствовать формату ДД-ММ-ГГГГ\nПамятка: Минимальный срок счета - 8 месяцев!\n2) Максимальный срок счета - 12 лет!");
            }
            EarlyClosing = ec;
            MinReplSum = minsum;
        }
        public ReplenishableUrgentAccount(string line, byte dataItems)
            : base(line, dataItems)
        {
            string[] ar = line.Split(' ');
            ClosingDate = DateTime.Parse(ar[5]);
            EarlyClosing = bool.Parse(ar[6]);
            MinReplSum = ar[7];
        }
        public override string ToStringValue()
        {
            string res = String.Format("Срочный_пополняемый {0:15} {1:12} {2} {3} {4} {5} {6} {7}", Name, Number,
                OpeningDate.ToShortDateString(), Balance, Percent, ClosingDate.ToShortDateString(), EarlyClosing, MinReplSum);
            return res;
        }
        public override string ToShortString()
        {
            string res = String.Format("{0:60} Срочный_пополняемый", Name);
            return res;
        }
        public override string CalcBal(string per, string bal, string fby, string fbm, string fbd)
        {
            try
            {
                DateTime d = new DateTime(int.Parse(fby), int.Parse(fbm), int.Parse(fbd));
                DateTime now = DateTime.Now;
                if (d.CompareTo(now) < 0 || d.CompareTo(ClosingDate) > 0 || d.CompareTo(OpeningDate) < 0 || d.CompareTo(new DateTime(2050, 01, 01)) > 0)
                    throw new Exception();
                int years = d.Year - now.Year;
                if (d.AddYears(OpeningDate.Year - d.Year).CompareTo(OpeningDate) < 0) years--;
                double newBal = double.Parse(bal);
                for (int i = 0; i < years; i++) newBal *= (double.Parse(per) / 10 + 1);
                return newBal.ToString("F3");
            }
            catch
            {
                throw new Exception("Ошибка в значении даты! Дата должна соответствовать формату ДД-ММ-ГГГГ!\nПамятка: 1) Дата должна быть позднее текущей.\n2) Проверьте, попадает ли дата в интервал времени существования счёта.\n3) Дата не может быть позднее 01.01.2050.");
            }
        }
    }
}
