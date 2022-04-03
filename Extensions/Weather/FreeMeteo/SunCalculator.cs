using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeMeteo
{
    public class SunCalculator
    {
        #region Fields
        private bool ABOVE;
        public const double CheboksayLat = 56.13;
        public const double CheboksaryLon = 47.25;
        public DateTime Date;
        private double DEC;
        public DateTime DSunRise;
        public DateTime DSunSet;
        private double EOT;
        private double JD;
        public double Lat;
        public TimeSpan LengthOfDay;
        private double locOffset;
        public double Lon;
        private double myDay;
        private double myHour;
        private double myMinute;
        private double myMonth;
        private double myYear;
        private double NZ;
        private double offset;
        private double RA;
        private bool RISE;
        private bool SETT;
        private double UT;
        private double UTRISE;
        private double UTSET;
        private double Y0;
        private double YE;
        private double yMinus;
        private double yPlus;
        private double zero1;
        private double zero2;
        #endregion
        #region SunCalculator
        public SunCalculator(DateTime Date)
        {
            this.NZ = 0.0;
            this.Lat = double.MinValue;
            this.Lon = double.MinValue;
            this.Date = DateTime.MinValue;
            this.DSunRise = DateTime.MinValue;
            this.DSunSet = DateTime.MinValue;
            this.LengthOfDay = TimeSpan.MinValue;
            this.Calculate(Date, CheboksayLat, CheboksaryLon);
        }

        public SunCalculator(DateTime Date, double Lat, double Lon)
        {
            this.NZ = 0.0;
            this.Lat = double.MinValue;
            this.Lon = double.MinValue;
            this.Date = DateTime.MinValue;
            this.DSunRise = DateTime.MinValue;
            this.DSunSet = DateTime.MinValue;
            this.LengthOfDay = TimeSpan.MinValue;
            this.Lat = ((Lat == double.MinValue) || (Lon == double.MinValue)) ? CheboksayLat : Lat;
            this.Lon = ((Lat == double.MinValue) || (Lon == double.MinValue)) ? CheboksaryLon : Lon;
            this.Calculate(Date, this.Lat, this.Lon);
        }

        private void Calculate(DateTime Date, double lat, double longit)
        {
            this.Lat = lat;
            this.Lon = longit;
            this.myDay = Date.Day;
            this.myMonth = Date.Month;
            this.myYear = Date.Year;
            this.myHour = Date.Hour;
            this.myMinute = Date.Minute;
            this.offset = 0;
            if (this.offset >= 1320.0)
            {
                this.offset -= 1440.0;
            }
            this.locOffset = TimeZone.CurrentTimeZone.GetUtcOffset(Date).Hours * 1.0 + TimeZone.CurrentTimeZone.GetUtcOffset(Date).Minutes / 60.0;
            this.UT = (this.myHour + ((this.myMinute + this.offset) / 60.0)) + (Convert.ToDouble(Date.Second) / 3600.0);
            this.theJulDay();
            this.offset = 0;
            this.EOT = this.eot(this.myDay, this.myMonth, this.myYear, this.UT);
            this.Date = new DateTime(Date.Year, Date.Month, Date.Day);
            if (this.offset >= 1320.0)
            {
                this.offset -= 1440.0;
            }
            double nn = (-2.0 * this.offset) / 60.0;
            if (this.offset < 0.0)
            {
                nn--;
            }
            double GHA = this.computeGHA(this.myDay, this.myMonth, this.myYear, this.UT);
            double elev = Math.Round((double)(this.computeHeight(this.DEC, lat, longit, GHA) * 100.0)) / 100.0;
            this.RISE = false;
            this.SETT = false;
            for (double i = -this.locOffset; i < (-this.locOffset + 24.0); i++)
            {
                this.riseset(this.myDay, this.myMonth, this.myYear, i);
                if (this.RISE && this.SETT)
                {
                    break;
                }
            }
            double hRise1 = 0.0;
            double hSet1 = 0.0;
            double lengthOfDay = 0.0;
            if (this.RISE || this.SETT)
            {
                if (this.RISE)
                {
                    hRise1 = this.UTRISE + this.locOffset;
                    if (hRise1 > 24.0)
                    {
                        hRise1 -= 24.0;
                    }
                    if (hRise1 < 0.0)
                    {
                        hRise1 += 24.0;
                    }
                }
                if (this.SETT)
                {
                    hSet1 = this.UTSET + this.locOffset;
                    if (hSet1 > 24.0)
                    {
                        hSet1 -= 24.0;
                    }
                    if (hSet1 < 0.0)
                    {
                        hSet1 += 24.0;
                    }
                }
                lengthOfDay = hSet1 - hRise1;
            }
            this.LengthOfDay = new TimeSpan((long)Math.Floor((double)(lengthOfDay * 36000000000)));
            TimeSpan s = new TimeSpan((long)Math.Floor((double)(hRise1 * 36000000000)));
            this.DSunRise = this.RISE ? new DateTime(Convert.ToInt32(this.myYear), Convert.ToInt32(this.myMonth), Convert.ToInt32(this.myDay), s.Hours, s.Minutes, s.Seconds) : DateTime.MinValue;
            s = new TimeSpan((long)Math.Floor((double)(hSet1 * 36000000000)));
            this.DSunSet = this.SETT ? new DateTime(Convert.ToInt32(this.myYear), Convert.ToInt32(this.myMonth), Convert.ToInt32(this.myDay), s.Hours, s.Minutes, s.Seconds) : DateTime.MinValue;
            //this.DSunRise = this.DSunRise.Add(-TimeZone.CurrentTimeZone.GetUtcOffset(Date));
            //this.DSunSet = this.DSunSet.Add(-TimeZone.CurrentTimeZone.GetUtcOffset(Date));
            //-TimeZone.CurrentTimeZone.GetUtcOffset(Date).Hours * 60.0
        }
        #endregion
        #region computeGHA
        private double computeGHA(double T, double M, double J, double STD)
        {
            double K = 0.017453292519943295;
            double N = (((365.0 * J) + T) + (31.0 * M)) - 46.0;
            if (M < 3.0)
            {
                N += Math.Floor((double)((J - 1.0) / 4.0));
            }
            else
            {
                N = (N - Math.Floor((double)((0.4 * M) + 2.3))) + Math.Floor((double)(J / 4.0));
            }
            double P = STD / 24.0;
            double X = (((P + N) - 722449.0) * 0.98564734) + 279.306;
            X *= K;
            double XX = (((-104.55 * Math.Sin(X)) - (429.266 * Math.Cos(X))) + (595.63 * Math.Sin(2.0 * X))) - (2.283 * Math.Cos(2.0 * X));
            XX = (XX + (4.6 * Math.Sin(3.0 * X))) + (18.7333 * Math.Cos(3.0 * X));
            XX = ((((XX - (13.2 * Math.Sin(4.0 * X))) - Math.Cos(5.0 * X)) - (Math.Sin(5.0 * X) / 3.0)) + (0.5 * Math.Sin(6.0 * X))) + 0.231;
            XX = (XX / 240.0) + (360.0 * (P + 0.5));
            if (XX > 360.0)
            {
                XX -= 360.0;
            }
            return XX;
        }
        #endregion
        #region computeHeight
        private double computeHeight(double dec, double latitude, double longit, double gha)
        {
            double K = 0.017453292519943295;
            double lat_K = latitude * K;
            double dec_K = dec * K;
            double x = gha + longit;
            double sinHeight = (Math.Sin(dec_K) * Math.Sin(lat_K)) + ((Math.Cos(dec_K) * Math.Cos(lat_K)) * Math.Cos(K * x));
            return (Math.Asin(sinHeight) / K);
        }
        #endregion
        #region deltaPSI
        private double deltaPSI(double T)
        {
            double K = 0.017453292519943295;
            double LS = this.sunL(T);
            double LM = 218.3165 + (481267.8813 * T);
            LM = LM % 360.0;
            if (LM < 0.0)
            {
                LM += 360.0;
            }
            double omega = ((125.04452 - (1934.136261 * T)) + ((0.0020708 * T) * T)) + (((T * T) * T) / 450000.0);
            double deltaPsi = (((-17.2 * Math.Sin(K * omega)) - (1.32 * Math.Sin((K * 2.0) * LS))) - (0.23 * Math.Sin((K * 2.0) * LM))) + (0.21 * Math.Sin((K * 2.0) * omega));
            return (deltaPsi / 3600.0);
        }
        #endregion
        #region eot
        private double eot(double date, double month, double year, double UT)
        {
            double K = 0.017453292519943295;
            double T = (this.JD - 2451545.0) / 36525.0;
            double eps = this.EPS(T);
            double RA = this.RightAscension(T);
            double LS = this.sunL(T);
            double deltaPsi = this.deltaPSI(T);
            double E = ((LS - 0.0057183) - RA) + (deltaPsi * Math.Cos(K * eps));
            if (E > 5.0)
            {
                E -= 360.0;
            }
            E *= 4.0;
            return (Math.Round((double)(1000.0 * E)) / 1000.0);
        }
        #endregion
        #region EPS
        private double EPS(double T)
        {
            double K = 0.017453292519943295;
            double LS = this.sunL(T);
            double LM = 218.3165 + (481267.8813 * T);
            double eps0 = 23.43929111111111 - ((((46.815 * T) + ((0.00059 * T) * T)) - (((0.001813 * T) * T) * T)) / 3600.0);
            double omega = ((125.04452 - (1934.136261 * T)) + ((0.0020708 * T) * T)) + (((T * T) * T) / 450000.0);
            double deltaEps = ((((9.2 * Math.Cos(K * omega)) + (0.57 * Math.Cos((K * 2.0) * LS))) + (0.1 * Math.Cos((K * 2.0) * LM))) - (0.09 * Math.Cos((K * 2.0) * omega))) / 3600.0;
            return (eps0 + deltaEps);
        }
        #endregion
        #region frac
        private double frac(double X)
        {
            X -= Math.Floor(X);
            if (X < 0.0)
            {
                X++;
            }
            return X;
        }
        #endregion
        #region JulDay
        private double JulDay(double date, double month, double year, double UT)
        {
            if (year < 1900.0)
            {
                year += 1900.0;
            }
            if (month <= 2.0)
            {
                month += 12.0;
                year--;
            }
            double B = (Math.Floor((double)(year / 400.0)) - Math.Floor((double)(year / 100.0))) + Math.Floor((double)(year / 4.0));
            double A = (365.0 * year) - 679004.0;
            double jd = (((A + B) + Math.Floor((double)(30.6001 * (month + 1.0)))) + date) + (UT / 24.0);
            return (jd + 2400000.5);
        }
        #endregion
        #region QUAD
        private void QUAD(double yMinus, double yPlus)
        {
            this.NZ = 0.0;
            double A = (0.5 * (yMinus + yPlus)) - this.Y0;
            double B = 0.5 * (yPlus - yMinus);
            double C = this.Y0;
            double XE = -B / (2.0 * A);
            this.YE = (((A * XE) + B) * XE) + C;
            double DIS = (B * B) - ((4.0 * A) * C);
            if (DIS >= 0.0)
            {
                double DX = (0.5 * Math.Sqrt(DIS)) / Math.Abs(A);
                this.zero1 = XE - DX;
                this.zero2 = XE + DX;
                if (Math.Abs(this.zero1) <= 1.0)
                {
                    this.NZ++;
                }
                if (Math.Abs(this.zero2) <= 1.0)
                {
                    this.NZ++;
                }
                if (this.zero1 < -1.0)
                {
                    this.zero1 = this.zero2;
                }
            }
        }
        #endregion
        #region RightAscension
        private double RightAscension(double T)
        {
            double K = 0.017453292519943295;
            double L = this.sunL(T);
            double M = ((357.5291 + (35999.0503 * T)) - ((0.0001559 * T) * T)) - (((4.8E-07 * T) * T) * T);
            M = M % 360.0;
            if (M < 0.0)
            {
                M += 360.0;
            }
            double C = ((1.9146 - (0.004817 * T)) - ((1.4E-05 * T) * T)) * Math.Sin(K * M);
            C += (0.019993 - (0.000101 * T)) * Math.Sin((K * 2.0) * M);
            C += 0.00029 * Math.Sin((K * 3.0) * M);
            double theta = L + C;
            double eps = this.EPS(T) + (0.00256 * Math.Cos(K * (125.04 - (1934.136 * T))));
            double lambda = (theta - 0.00569) - (0.00478 * Math.Sin(K * (125.04 - (1934.136 * T))));
            this.RA = Math.Atan2(Math.Cos(K * eps) * Math.Sin(K * lambda), Math.Cos(K * lambda));
            this.RA /= K;
            if (this.RA < 0.0)
            {
                this.RA += 360.0;
            }
            double delta = Math.Asin(Math.Sin(K * eps) * Math.Sin(K * lambda)) / K;
            this.DEC = delta;
            return this.RA;
        }
        #endregion
        #region riseset
        private void riseset(double DATE, double MONTH, double YEAR, double HOUR)
        {
            double K = 0.017453292519943295;
            double sh = Math.Sin(-K * 0.8333);
            double jd = this.JulDay(DATE, MONTH, YEAR, HOUR);
            double dec = this.sunDecRA(1.0, jd);
            double ra = this.sunDecRA(2.0, jd);
            double gha = this.computeGHA(DATE, MONTH, YEAR, HOUR);
            this.Y0 = Math.Sin(K * this.computeHeight(dec, this.Lat, this.Lon, gha)) - sh;
            double jdPlus = this.JulDay(DATE, MONTH, YEAR, HOUR + 1.0);
            dec = this.sunDecRA(1.0, jdPlus);
            ra = this.sunDecRA(2.0, jdPlus);
            gha = this.computeGHA(DATE, MONTH, YEAR, HOUR + 1.0);
            this.yPlus = Math.Sin(K * this.computeHeight(dec, this.Lat, this.Lon, gha)) - sh;
            double jdMinus = this.JulDay(DATE, MONTH, YEAR, HOUR - 1.0);
            dec = this.sunDecRA(1.0, jdMinus);
            ra = this.sunDecRA(2.0, jdMinus);
            gha = this.computeGHA(DATE, MONTH, YEAR, HOUR - 1.0);
            this.yMinus = Math.Sin(K * this.computeHeight(dec, this.Lat, this.Lon, gha)) - sh;
            this.ABOVE = this.yMinus > 0.0;
            this.QUAD(this.yMinus, this.yPlus);
            switch (Convert.ToInt32(this.NZ))
            {
                case 0:
                    return;

                case 1:
                    if (this.yMinus < 0.0)
                    {
                        this.UTRISE = HOUR + this.zero1;
                        this.RISE = true;
                    }
                    else
                    {
                        this.UTSET = HOUR + this.zero1;
                        this.SETT = true;
                    }
                    return;

                case 2:
                    if (this.YE >= 0.0)
                    {
                        this.UTRISE = HOUR + this.zero1;
                        this.UTSET = HOUR + this.zero2;
                        break;
                    }
                    this.UTRISE = HOUR + this.zero2;
                    this.UTSET = HOUR + this.zero1;
                    break;

                default:
                    return;
            }
            this.RISE = true;
            this.SETT = true;
        }
        #endregion
        #region sunDecRA
        private double sunDecRA(double what, double jd)
        {
            double PI2 = 6.2831853071795862;
            double cos_eps = 0.917482;
            double sin_eps = 0.397778;
            double T = (jd - 2451545.0) / 36525.0;
            double M = PI2 * this.frac(0.993133 + (99.997361 * T));
            double DL = (6893.0 * Math.Sin(M)) + (72.0 * Math.Sin(2.0 * M));
            double L = PI2 * this.frac((0.7859453 + (M / PI2)) + (((6191.2 * T) + DL) / 1296000.0));
            double SL = Math.Sin(L);
            double X = Math.Cos(L);
            double Y = cos_eps * SL;
            double Z = sin_eps * SL;
            double R = Math.Sqrt(1.0 - (Z * Z));
            double dec = (360.0 / PI2) * Math.Atan(Z / R);
            double ra = (48.0 / PI2) * Math.Atan(Y / (X + R));
            if (ra < 0.0)
            {
                ra += 24.0;
            }
            if (what == 1.0)
            {
                return dec;
            }
            return ra;
        }
        #endregion
        #region sunL
        private double sunL(double T)
        {
            double L = (280.46645 + (36000.76983 * T)) + ((0.0003032 * T) * T);
            L = L % 360.0;
            if (L < 0.0)
            {
                L += 360.0;
            }
            return L;
        }
        #endregion
        #region theJulDay
        private void theJulDay()
        {
            if (this.myYear < 1900.0)
            {
                this.myYear += 1900.0;
            }
            this.JD = this.JulDay(this.myDay, this.myMonth, this.myYear, this.UT);
        }
        #endregion
    }
}
