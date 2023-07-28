﻿using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptBaoCaoThuVienPhiNgoaiTru : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBaoCaoThuVienPhiNgoaiTru()
        {
            InitializeComponent();
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(XRptBaoCaoThuVienPhiNgoaiTru_BeforePhint);
        }

        public void FillData()
        {

            spReportPaymentReceiptByStaffDetailsTableAdapter.Fill(dsBaoCaoThuVienPhiBHYT.spReportPaymentReceiptByStaffDetails
                                                                , Convert.ToInt32(this.RepPaymentRecvID.Value)
                                                                , Convert.ToInt32(this.StaffID.Value)
                                                                , Convert.ToDateTime(this.FromDate.Value)
                                                                , Convert.ToDateTime(this.ToDate.Value)
                                                                , Convert.ToInt32(this.Case.Value)
                                                                , Convert.ToInt32(this.V_PaymentMode.Value)); //--26/01/2021 DatTB
        }

        private void XRptBaoCaoThuVienPhiNgoaiTru_BeforePhint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();

            decimal TotalAmount = 0;

            if (dsBaoCaoThuVienPhiBHYT.spReportPaymentReceiptByStaffDetails != null && dsBaoCaoThuVienPhiBHYT.spReportPaymentReceiptByStaffDetails.Rows.Count > 0)
            {

                for (int i = 0; i < dsBaoCaoThuVienPhiBHYT.spReportPaymentReceiptByStaffDetails.Rows.Count; i++)
                {
                    TotalAmount += ((Convert.ToDecimal(dsBaoCaoThuVienPhiBHYT.spReportPaymentReceiptByStaffDetails.Rows[i]["PayAmount"]))
                                     - (Convert.ToDecimal(dsBaoCaoThuVienPhiBHYT.spReportPaymentReceiptByStaffDetails.Rows[i]["CancelAmount"]))
                                     - (Convert.ToDecimal(dsBaoCaoThuVienPhiBHYT.spReportPaymentReceiptByStaffDetails.Rows[i]["RefundAmount"])));
                }
                this.Parameters["TotalAmount"].Value = TotalAmount;
                this.Parameters["parReadMoneyTotalAmount"].Value = Globals.ReadMoneyToString(TotalAmount);
            }
        }
    }
}

