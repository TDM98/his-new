using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Windows;
using System.Windows.Controls;
using DataEntities;
using aEMR.Common;
using aEMR.DataContracts;
using aEMR.CommonTasks;
using System.Collections.Generic;
using aEMR.Common.Collections;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using aEMR.ServiceClient;
using System.Linq;
using aEMR.Controls;
using Castle.Windsor;
using aEMR.Common.BaseModel;
//using Microsoft.Web.WebView2.Wpf;
using System.IO;
using DevExpress.ReportServer.Printing;
using aEMR.ReportModel.ReportModels;
/*
* 20180923 #001 TTM: 
* 20190815 #002 TTM:   BM 0013133: Không load lại màn hình kết quả khi tìm đăng ký mới. Dẫn tới có thể gây nhầm lẫn kết quả.
* 20220901 #003 BLQ: Issue:2174 Chỉnh lại mẫu hình ảnh theo cách mới
* 20230713 #004 TNHX: 3323 Thêm màn hình xem hình ảnh từ PAC GE
*   + Refactor code + Thêm busy
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPatientPCLImagingResult_V2)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientPCLImagingResult_V2ViewModel : ViewModelBase, IPatientPCLImagingResult_V2

    {
        #region Properties member
        //▼====: #004
        private bool _IsShowViewerImageResultFromPAC = Globals.ServerConfigSection.Pcls.AutoCreatePACWorklist;
        public bool IsShowViewerImageResultFromPAC
        {
            get { return _IsShowViewerImageResultFromPAC; }
            set
            {
                if (_IsShowViewerImageResultFromPAC != value)
                {
                    _IsShowViewerImageResultFromPAC = value;
                    NotifyOfPropertyChange(() => IsShowViewerImageResultFromPAC);
                }
            }
        }

        private Uri _SourceLink = null;
        public Uri SourceLink
        {
            get
            {
                return _SourceLink;
            }
            set
            {
                _SourceLink = value;
                NotifyOfPropertyChange(() => SourceLink);
            }
        }
        //▲====: #004
        #endregion
        [ImportingConstructor]
        public PatientPCLImagingResult_V2ViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
           
        }
        private PatientPCLImagingResult _curPatientPCLImagingResult;
        public PatientPCLImagingResult curPatientPCLImagingResult
        {
            get
            {
                return _curPatientPCLImagingResult;
            }
            set
            {
                if (_curPatientPCLImagingResult == value)
                    return;
                _curPatientPCLImagingResult = value;
                NotifyOfPropertyChange(() => curPatientPCLImagingResult);
            }
        }
        //WebView2 webView = null;
        public object TabHinhAnhPCL
        {
            get;
            set;
        }
        public object TabHinhAnhPCL_New
        {
            get;
            set;
        }
        private IHtmlReport _PCLOldView;
        public IHtmlReport PCLOldView
        {
            get { return _PCLOldView; }
            set
            {
                _PCLOldView = value;
                NotifyOfPropertyChange(() => PCLOldView);
            }
        }
        //public void WebView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    webView = (WebView2)sender;
        //}
        public void TabHinhAnhPCL_Loaded(object sender)
        {
            TabHinhAnhPCL = (TabItem)sender;
            if (PCLOldView == null)
            {
                var VMTT = Globals.GetViewModel<IHtmlReport>();
                PCLOldView = VMTT;
                this.ActivateItem(VMTT);
            }
        }
        public void TabHinhAnhPCL_New_Loaded(object sender)
        {
            TabHinhAnhPCL_New = (TabItem)sender;
        }
        private RemoteDocumentSource _reportModel;
        public RemoteDocumentSource ReportModel
        {
            get { return _reportModel; }
            set
            {
                _reportModel = value;
                NotifyOfPropertyChange(() => ReportModel);
            }
        }
        private PatientPCLRequest _ObjPatientPCLRequest;
        public PatientPCLRequest ObjPatientPCLRequest
        {
            get { return _ObjPatientPCLRequest; }
            set
            {
                _ObjPatientPCLRequest = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequest);
            }
        }
        private bool _mPCLImage_New = false;
        public bool mPCLImage_New
        {
            get
            {
                return _mPCLImage_New;
            }
            set
            {
                if (_mPCLImage_New == value)
                    return;
                _mPCLImage_New = value;
                NotifyOfPropertyChange(() => mPCLImage_New);
            }
        }
        private bool _mPCLImage = false;
        public bool mPCLImage
        {
            get
            {
                return _mPCLImage;
            }
            set
            {
                if (_mPCLImage == value)
                    return;
                _mPCLImage = value;
                NotifyOfPropertyChange(() => mPCLImage);
            }
        }
        public void InitHTML()
        {
            if (PCLOldView == null)
            {
                var VMTT = Globals.GetViewModel<IHtmlReport>();
                PCLOldView = VMTT;
                this.ActivateItem(VMTT);
            }
        }

        public void CheckTemplatePCLResultByReqID(long PatientPCLReqID,bool InPt)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginCheckTemplatePCLResultByReqID(PatientPCLReqID, InPt, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool IsNewTemplate = false;
                                long V_ReportForm = 0;
                                long PCLImgResultID = 0;
                                long V_PCLRequestType = 0;
                                string TemplateResultString = "";
                                var result = contract.EndCheckTemplatePCLResultByReqID(out IsNewTemplate, out V_ReportForm, out PCLImgResultID, out V_PCLRequestType, out TemplateResultString, asyncResult);
                                if (IsNewTemplate)
                                {
                                    mPCLImage_New = true;
                                    ((TabItem)TabHinhAnhPCL_New).IsSelected = true;
                                //PCLResultParamImpName = curPatientServicesTree.Description;
                                btnViewPrintNew(V_ReportForm, PCLImgResultID, V_PCLRequestType);
                                }
                                else
                                {
                                    mPCLImage = true;
                                    ((TabItem)TabHinhAnhPCL).IsSelected = true;
                                //PCLResultParamImpName = curPatientServicesTree.Description;
                                //var VMTT = Globals.GetViewModel<IHtmlReport>();
                                //PCLOldView = VMTT;
                                //this.ActivateItem(VMTT);
                                PCLOldView.NavigateToString(TemplateResultString);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
       
        public void btnViewPrintNew(long V_ReportForm, long PCLImgResultID, long V_PCLRequestType)
        {
            //if (curPatientPCLImagingResult.PatientPCLRequest == null)
            //{
            //    return;
            //}
            //if (curPatientPCLImagingResult.PCLExamType == null || curPatientPCLImagingResult.PCLExamType.V_ReportForm == 0)
            //{
            //    return;
            //}
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            ReportModel = null;
            switch (V_ReportForm)
            {
                case (long)AllLookupValues.V_ReportForm.Mau_0_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_0_Hinh").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_1_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_1_Hinh").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_2_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_3_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_3_Hinh").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_4_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_4_Hinh").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_6_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_6_Hinh").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Realtime_PCR:
                    if (Globals.ServerConfigSection.CommonItems.IsApplyPCRDual)
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Realtime_PCR_Cov_Dual").PreviewModel;
                    }
                    else
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Realtime_PCR_Cov").PreviewModel;
                    }
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Test_Nhanh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Test_Nhanh_Cov").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Xet_Nghiem:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Xet_Nghiem").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Helicobacter_Pylori:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Helicobacter_Pylori").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_Tim:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Sieu_Am_Tim").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_San_4D:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Sieu_Am_San_4D").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Dien_Tim:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Dien_Tim").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Dien_Nao:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Dien_Nao").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_ABI:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_ABI").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Dien_Tim_Gang_Suc:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Dien_Tim_Gang_Suc").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_CLVT_Hai_Ham:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_CLVT_Hai_Ham").PreviewModel;
                    break;
                //▼====: #003
                case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_San_4D_New:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Sieu_Am_San_4D_New").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_6_Hinh_2_New:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_6_Hinh_2_New").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_6_Hinh_1_New:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_6_Hinh_1_New").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Sieu_Am_Tim_New:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Sieu_Am_Tim_New").PreviewModel;
                    break;
                case (long)AllLookupValues.V_ReportForm.Mau_Noi_Soi_9_Hinh:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New_Noi_Soi_9_Hinh").PreviewModel;
                    break;
                
                //▲====: #003
                default:
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.PCLDepartment.XRpt_PCLImagingResult_New").PreviewModel;
                    break;
            }
            rParams["PCLImgResultID"].Value = PCLImgResultID;
            rParams["V_PCLRequestType"].Value = V_PCLRequestType;
            rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            rParams["parHospitalCode"].Value = Globals.ServerConfigSection.Hospitals.HospitalCode;
            rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            rParams["parHospitalAddress"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalAddress;
            ReportModel.CreateDocument(rParams);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }
        //public void LoadDataCoroutineEx(PatientPCLRequest aPCLRequest)
        //{
        //    Coroutine.BeginExecute(LoadDataCoroutine(aPCLRequest));
        //}
    }
}
