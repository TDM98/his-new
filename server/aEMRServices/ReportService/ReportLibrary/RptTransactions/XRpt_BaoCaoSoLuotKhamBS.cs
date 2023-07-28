using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using eHCMS.Services.Core;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BaoCaoSoLuotKhamBS : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BaoCaoSoLuotKhamBS()
        {
            InitializeComponent();
            FillData();
        }
        public void FillData()
        {
            spXRpt_BaoCaoSoLuotKhamBacSiTableAdapter.Fill(dsBaoCaoSoLuotKhamBS1.spXRpt_BaoCaoSoLuotKhamBacSi, Convert.ToDateTime(this.parFromDate.Value), Convert.ToDateTime(this.parToDate.Value), Convert.ToInt32(this.CaseView.Value));
        }
        private void XRpt_BaoCaoSoLuotKhamBS_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
