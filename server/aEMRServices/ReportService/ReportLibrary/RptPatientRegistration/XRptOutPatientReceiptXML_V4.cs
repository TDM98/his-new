﻿using System;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptOutPatientReceiptXML_V4 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptOutPatientReceiptXML_V4()
        {
            InitializeComponent();
        }
        private void XtraReport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXmlTableAdapter.Fill(outPatientReceipt1.sp_Rpt_spReportOutPatientCashReceipt_ByPaymentIDXml, this.param_PaymentID.Value);
        }
        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptOutPatientReceipt_V4)((XRSubreport)sender).ReportSource).param_PaymentID.Value = Convert.ToInt32(GetCurrentColumnValue("PaymentID"));
            ((XRptOutPatientReceipt_V4)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value.ToString();
            ((XRptOutPatientReceipt_V4)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = parDepartmentOfHealth.Value.ToString();
            ((XRptOutPatientReceipt_V4)((XRSubreport)sender).ReportSource).pOutPtCashAdvanceID.Value = GetCurrentColumnValue("OutPtCashAdvanceID") == null || GetCurrentColumnValue("OutPtCashAdvanceID") == DBNull.Value ? 0 : Convert.ToInt64(GetCurrentColumnValue("OutPtCashAdvanceID"));
            ((XRptOutPatientReceipt_V4)((XRSubreport)sender).ReportSource).parLogoUrl.Value = parLogoUrl.Value.ToString();
            ((XRptOutPatientReceipt_V4)((XRSubreport)sender).ReportSource).parDeptLocIDQMS.Value = parDeptLocIDQMS.Value.ToString();
        }
    }
}