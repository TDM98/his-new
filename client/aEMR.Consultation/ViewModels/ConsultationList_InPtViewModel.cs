﻿using eHCMSLanguage;
using aEMR.ServiceClient;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using DataEntities;
using System.Collections.ObjectModel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using System;
using System.Windows.Controls;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using System.Windows;
using aEMR.Common;
using System.Linq;
using Castle.Windsor;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IConsultationList_InPt)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConsultationList_InPtViewModel : ViewModelBase, IConsultationList_InPt
        //, IHandle<ShowPatientInfo_KHAMBENH_CHANDOAN<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<ClearAllDiagnosisListAfterAddNewEvent_InPt>
        , IHandle<ClearAllDiagnosisListAfterUpdateEvent_InPt>
    {

        public override bool IsProcessing
        {
            get
            {
                return _IsWaitingGetDiagTrmtsByPtID;
            }
        }

        public override string StatusText
        {
            get
            {
                if (_IsWaitingGetDiagTrmtsByPtID)
                {
                    return eHCMSResources.Z0485_G1_DSChanDoanTruoc;
                }

                return string.Empty;
            }
        }

        private bool _IsWaitingGetDiagTrmtsByPtID;
        public bool IsWaitingGetDiagTrmtsByPtID
        {
            get { return _IsWaitingGetDiagTrmtsByPtID; }
            set
            {
                if (_IsWaitingGetDiagTrmtsByPtID != value)
                {
                    _IsWaitingGetDiagTrmtsByPtID = value;
                    NotifyOfPropertyChange(() => IsWaitingGetDiagTrmtsByPtID);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }


        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
        }


        private const string ALLITEMS = "--Tất Cả--";
        [ImportingConstructor]
        public ConsultationList_InPtViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            GetLookupBehaving();

            DiagTrmtByPtIDList = new PagedSortableCollectionView<DiagnosisTreatment>();
            DiagTrmtByPtIDList.OnRefresh += new EventHandler<RefreshEventArgs>(DiagTrmtByPtIDList_OnRefresh);
            DiagTrmtByPtIDList.PageSize = Globals.PageSize;

            authorization();

            V_Behaving = 0;

            InitPatientInfo();

        }

        //public void Handle(ShowPatientInfo_KHAMBENH_CHANDOAN<Patient, PatientRegistration, PatientRegistrationDetail> message)
        //{
        //    InitPatientInfo();
        //}

        public void ResetAllDiagnosisList()
        {
            if (DiagTrmtByPtIDList != null)
            {
                DiagTrmtByPtIDList.Clear();
            }

            CurrentDiagTrmt = null;

            if (refIDC10List != null)
            {
                refIDC10List.Clear();
            }

            if (refICD9List != null)
            {
                refICD9List.Clear();
            }
        }

        public void InitPatientInfo()
        {
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                //Kiem tra phan quyen
                if (!mChanDoan_tabLanKhamTruoc_ThongTin)
                {
                    Globals.ShowMessage(eHCMSResources.Z0413_G1_ChuaDuocPQuyenXemBA, "");
                    return;
                }

                PatientID = Registration_DataStorage.CurrentPatient.PatientID;

                ResetAllDiagnosisList();

                //DiagTrmtByPtIDList.PageIndex = 0;
                //GetDiagTrmtsByPtID(Registration_DataStorage.CurrentPatient.PatientID, Globals.PatientAllDetails.RegistrationInfo.PtRegistrationID, 0, DiagTrmtByPtIDList.PageIndex, DiagTrmtByPtIDList.PageSize);
            }
        }

        void DiagTrmtByPtIDList_OnRefresh(object sender, RefreshEventArgs e)
        {
            if (Registration_DataStorage.CurrentPatient == null) return;
            GetDiagTrmtsByPtID_InPt(Registration_DataStorage.CurrentPatient.PatientID, DiagTrmtByPtIDList.PageIndex, DiagTrmtByPtIDList.PageSize);
        }

        #region Properties Member

        private long _PatientID;
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                if (_PatientID != value)
                {
                    _PatientID = value;
                    NotifyOfPropertyChange(() => PatientID);
                }
            }
        }
        private PagedSortableCollectionView<DiagnosisTreatment> _DiagTrmtByPtIDList;
        public PagedSortableCollectionView<DiagnosisTreatment> DiagTrmtByPtIDList
        {
            get
            {
                return _DiagTrmtByPtIDList;
            }
            set
            {
                if (_DiagTrmtByPtIDList != value)
                {
                    _DiagTrmtByPtIDList = value;
                    NotifyOfPropertyChange(() => DiagTrmtByPtIDList);
                }
            }
        }

        //private long? _IssueID;
        //public long? IssueID
        //{
        //    get
        //    {
        //        return _IssueID;
        //    }
        //    set
        //    {
        //        if (_IssueID != value)
        //        {
        //            _IssueID = value;
        //            NotifyOfPropertyChange(() => IssueID);
        //        }
        //    }
        //}

        private DiagnosisTreatment _CurrentDiagTrmt;
        public DiagnosisTreatment CurrentDiagTrmt
        {
            get
            {
                return _CurrentDiagTrmt;
            }
            set
            {
                if (_CurrentDiagTrmt != value)
                {
                    _CurrentDiagTrmt = value;
                    if (_CurrentDiagTrmt != null)
                    {
                        //IssueID = _CurrentDiagTrmt.IssueID;
                        IsEnabled = true;
                        //DiagnosisIcd10Items_Load(_CurrentDiagTrmt.ServiceRecID, PatientID, false);
                    }
                    else
                    {
                        //IssueID = 0;
                        IsEnabled = false;
                        if (refIDC10List != null)
                        {
                            refIDC10List.Clear();
                        }
                        if (refICD9List != null)
                        {
                            refICD9List.Clear();
                        }
                    }
                    NotifyOfPropertyChange(() => CurrentDiagTrmt);

                }
            }
        }

        private long? _V_Behaving;
        public long? V_Behaving
        {
            get
            {
                return _V_Behaving;
            }
            set
            {
                if (_V_Behaving != value)
                {
                    _V_Behaving = value;
                    NotifyOfPropertyChange(() => V_Behaving);
                }
            }
        }

        private ObservableCollection<Lookup> _RefBehaving;
        public ObservableCollection<Lookup> RefBehaving
        {
            get
            {
                return _RefBehaving;
            }
            set
            {
                if (_RefBehaving != value)
                {
                    _RefBehaving = value;
                    NotifyOfPropertyChange(() => RefBehaving);
                }
            }
        }

        private bool _IsEnabled = false;
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                _IsEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        public bool btnEditIsEnabled
        {
            get
            {
                return (IsEnabled);
            }
        }

        #endregion

        private void GetDiagTrmtsByPtID_InPt(long patientID, int PageIndex, int PageSize)
        {
            this.ShowBusyIndicator();
            int Total = 0;

            //IsWaitingGetDiagTrmtsByPtID = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisTreatmentListByPtID_InPt(patientID, V_Behaving, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisTreatmentListByPtID_InPt(out Total, asyncResult);
                            DiagTrmtByPtIDList.Clear();

                            CurrentDiagTrmt = null;

                            DiagTrmtByPtIDList.TotalItemCount = Total;
                            if (results != null)
                            {
                                foreach (DiagnosisTreatment p in results)
                                {
                                    DiagTrmtByPtIDList.Add(p);
                                }
                                if (DiagTrmtByPtIDList.Count > 0)
                                {
                                    CurrentDiagTrmt = DiagTrmtByPtIDList[0];
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsLoading = false;
                            //IsWaitingGetDiagTrmtsByPtID = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();

        }


        public void GetLookupBehaving()
        {
            ObservableCollection<Lookup> BehavingLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.BEHAVING).ToObservableCollection();

            if (BehavingLookupList == null || BehavingLookupList.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0740_G1_Msg_InfoKhTimThayLoaiKB, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (RefBehaving == null)
            {
                RefBehaving = new ObservableCollection<Lookup>();
            }
            else
            {
                RefBehaving.Clear();
            }
            Lookup ite = new Lookup();
            ite.LookupID = 0;
            ite.ObjectValue = ALLITEMS;
            RefBehaving.Add(ite);

            foreach (Lookup p in BehavingLookupList)
            {
                RefBehaving.Add(p);
            }

            NotifyOfPropertyChange(() => RefBehaving);

        }



        public void cboVBehaving_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cboVBehaving = sender as ComboBox;
            if (cboVBehaving != null)
            {
                if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null)
                {
                    DiagTrmtByPtIDList.PageIndex = 0;
                    GetDiagTrmtsByPtID_InPt(Registration_DataStorage.CurrentPatient.PatientID, DiagTrmtByPtIDList.PageIndex, DiagTrmtByPtIDList.PageSize);
                }
            }
        }

        public void grdConsultations_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }


        private ObservableCollection<DiagnosisIcd10Items> _refIDC10List;
        public ObservableCollection<DiagnosisIcd10Items> refIDC10List
        {
            get
            {
                return _refIDC10List;
            }
            set
            {
                if (_refIDC10List != value)
                {
                    _refIDC10List = value;
                }
                NotifyOfPropertyChange(() => refIDC10List);
            }
        }

        private void DiagnosisIcd10Items_Load_InPt(long DTItemID)
        {
            this.ShowBusyIndicator();
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                //IsLoading = true;

                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //contract.BeginGetDiagnosisIcd10Items_Load_InPt(ServiceRecID, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginGetDiagnosisIcd10Items_Load_InPt(DTItemID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisIcd10Items_Load_InPt(asyncResult);
                            refIDC10List = results.ToObservableCollection();
                            if (refIDC10List == null)
                            {
                                refIDC10List = new ObservableCollection<DiagnosisIcd10Items>();
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private ObservableCollection<DiagnosisICD9Items> _refICD9List;
        public ObservableCollection<DiagnosisICD9Items> refICD9List
        {
            get
            {
                return _refICD9List;
            }
            set
            {
                if (_refICD9List != value)
                {
                    _refICD9List = value;
                }
                NotifyOfPropertyChange(() => refICD9List);
            }
        }

        private void DiagnosisICD9Items_Load_InPt(long DTItemID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //contract.BeginGetDiagnosisIcd10Items_Load_InPt(ServiceRecID, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginGetDiagnosisICD9Items_Load_InPt(DTItemID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisICD9Items_Load_InPt(asyncResult);
                            refICD9List = results.ToObservableCollection();
                            if (refICD9List == null)
                            {
                                refICD9List = new ObservableCollection<DiagnosisICD9Items>();
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsLoading = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }
        public void grdConsultations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as DataGrid).SelectedItem != null)
            {
                CurrentDiagTrmt = (sender as DataGrid).SelectedItem as DiagnosisTreatment;

                //Đọc lại refIDC10List nếu không bị add dòng trống + dồn vào
                //DiagnosisIcd10Items_Load_InPt(CurrentDiagTrmt.ServiceRecID.GetValueOrDefault());
                DiagnosisIcd10Items_Load_InPt(CurrentDiagTrmt.DTItemID);
                //Đọc lại refIDC10List nếu không bị add dòng trống + dồn vào
                DiagnosisICD9Items_Load_InPt(CurrentDiagTrmt.DTItemID);
            }
        }

        public void grdConsultations_DblClick(object sender, EventArgs<object> e)
        {
            if (!mChanDoan_tabLanKhamTruoc_HieuChinh)
            {
                Globals.ShowMessage(eHCMSResources.Z0508_G1_ChuaDuocQuyenSua, "");
                return;
            }
            if ((sender as DataGrid).SelectedItem != null)
            {
                CurrentDiagTrmt = (sender as DataGrid).SelectedItem as DiagnosisTreatment;
                //KMx: Event grdConsultations_SelectionChanged đã load refIDC10List rồi, khi double click không cần load lại, vì load lại cũng vô ích (lỗi chạy đua, chưa load xong đã bắn event) (10/06/2015 10:21).
                //Đọc lại refIDC10List nếu không bị add dòng trống + dồn vào
                //DiagnosisIcd10Items_Load_InPt(CurrentDiagTrmt.ServiceRecID.GetValueOrDefault());
                //DiagnosisIcd10Items_Load_InPt(CurrentDiagTrmt.DTItemID);
                //Đọc lại refIDC10List nếu không bị add dòng trống + dồn vào

                Globals.EventAggregator.Publish(new ConsultationDoubleClickEvent_InPt_1());

                Globals.EventAggregator.Publish(new ConsultationDoubleClickEvent_InPt_2 { DiagTrmtItem = CurrentDiagTrmt.DeepCopy(), refIDC10List = refIDC10List.DeepCopy(), refICD9List = refICD9List.DeepCopy() });

            }
        }


        public void btnFind()
        {
            if (Registration_DataStorage.CurrentPatient != null)
            {
                DiagTrmtByPtIDList.PageIndex = 0;
                GetDiagTrmtsByPtID_InPt(Registration_DataStorage.CurrentPatient.PatientID, DiagTrmtByPtIDList.PageIndex, DiagTrmtByPtIDList.PageSize);
            }
        }

        public void GetDiagTrmtsByPtID_Ext()
        {
            DiagTrmtByPtIDList.PageIndex = 0;
            GetDiagTrmtsByPtID_InPt(Registration_DataStorage.CurrentPatient.PatientID, DiagTrmtByPtIDList.PageIndex, DiagTrmtByPtIDList.PageSize);
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mChanDoan_tabLanKhamTruoc_ThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPMRConsultationNew,
                                               (int)oConsultationEx.mChanDoan_tabLanKhamTruoc_ThongTin, (int)ePermission.mView);
            mChanDoan_tabLanKhamTruoc_HieuChinh = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPMRConsultationNew,
                                               (int)oConsultationEx.mChanDoan_tabLanKhamTruoc_HieuChinh, (int)ePermission.mView);

            mChanDoan_XemKetQuaCLS = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPMRConsultationNew,
                                               (int)oConsultationEx.mChanDoan_XemKetQuaCLS, (int)ePermission.mView);
            mChanDoan_XemToaThuoc_HienHanh = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPMRConsultationNew,
                                               (int)oConsultationEx.mChanDoan_XemToaThuoc_HienHanh, (int)ePermission.mView);
            mChanDoan_XemBenhAn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPMRConsultationNew,
                                               (int)oConsultationEx.mChanDoan_XemBenhAn, (int)ePermission.mView);
        }
        #region account checking

        private bool _mChanDoan_tabLanKhamTruoc_ThongTin = true;
        private bool _mChanDoan_tabLanKhamTruoc_HieuChinh = true;

        private bool _mChanDoan_XemKetQuaCLS = true;
        private bool _mChanDoan_XemToaThuoc_HienHanh = true;
        private bool _mChanDoan_XemBenhAn = true;

        public bool mChanDoan_XemKetQuaCLS
        {
            get
            {
                return _mChanDoan_XemKetQuaCLS;
            }
            set
            {
                if (_mChanDoan_XemKetQuaCLS == value)
                    return;
                _mChanDoan_XemKetQuaCLS = value;
            }
        }
        public bool mChanDoan_XemToaThuoc_HienHanh
        {
            get
            {
                return _mChanDoan_XemToaThuoc_HienHanh;
            }
            set
            {
                if (_mChanDoan_XemToaThuoc_HienHanh == value)
                    return;
                _mChanDoan_XemToaThuoc_HienHanh = value;
            }
        }
        public bool mChanDoan_XemBenhAn
        {
            get
            {
                return _mChanDoan_XemBenhAn;
            }
            set
            {
                if (_mChanDoan_XemBenhAn == value)
                    return;
                _mChanDoan_XemBenhAn = value;
            }
        }
        public bool mChanDoan_tabLanKhamTruoc_ThongTin
        {
            get
            {
                return _mChanDoan_tabLanKhamTruoc_ThongTin;
            }
            set
            {
                if (_mChanDoan_tabLanKhamTruoc_ThongTin == value)
                    return;
                _mChanDoan_tabLanKhamTruoc_ThongTin = value;
            }
        }
        public bool mChanDoan_tabLanKhamTruoc_HieuChinh
        {
            get
            {
                return _mChanDoan_tabLanKhamTruoc_HieuChinh;
            }
            set
            {
                if (_mChanDoan_tabLanKhamTruoc_HieuChinh == value)
                    return;
                _mChanDoan_tabLanKhamTruoc_HieuChinh = value;
            }
        }

        #endregion
        #region link member
        public void hpkViewEPrescription()
        {
            if (CurrentDiagTrmt != null && CurrentDiagTrmt.IssueID > 0)
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.IssueID = (int)CurrentDiagTrmt.IssueID;
                    proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC_INPT;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);

            }
            else
            {
                MessageBox.Show(eHCMSResources.A0413_G1_Msg_InfoCDChuaCoToaThuoc);
            }
        }
        #endregion

        public void Handle(ClearAllDiagnosisListAfterAddNewEvent_InPt message)
        {
            ResetAllDiagnosisList();
        }

        public void Handle(ClearAllDiagnosisListAfterUpdateEvent_InPt message)
        {
            ResetAllDiagnosisList();
        }
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }
    }
}