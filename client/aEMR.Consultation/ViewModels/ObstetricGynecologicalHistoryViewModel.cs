using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Common.ViewModels;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;

/*
 * 20230501 #001 DatTB: Thêm nút popup tính tuổi động mạch
 * 20230506 #002 DatTB: Fix lỗi check null
 * 20230527 #003 DatTB:
 * + Thêm giá trị mặc định các trường KSK
 * + Thêm service xóa kết quả KSK
 * + Fix lỗi chỉ cập nhật trạng thái phiếu KSK lần đầu tiên
 * + Clone model tab tình trạng thể chất - dấu hiệu sinh tồn vì model đã chung với màn hình khám truyền nhiều dữ liệu không cần thiết.
 * + Thêm Tab nhập tình trạng thể chất- dấu hiệu sinh tồn
 * 20230703 #004 DatTB: Thêm service tiền sử sản phụ khoa
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IObstetricGynecologicalHistory)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ObstetricGynecologicalHistoryViewModel : ViewModelBase, IObstetricGynecologicalHistory
    {
        #region Properties
        private ObservableCollection<Lookup> _V_Contraception;
        public ObservableCollection<Lookup> V_Contraception
        {
            get
            {
                return _V_Contraception;
            }
            set
            {
                _V_Contraception = value;
                NotifyOfPropertyChange(() => V_Contraception);
            }
        }

        private long _PatientID;
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                _PatientID = value;
                NotifyOfPropertyChange(() => PatientID);
                if (PatientID > 0)
                {
                    CurObstetricGynecologicalHistory.PatientID = PatientID;
                    GetObstetricGynecologicalHistoryLatest(PatientID);
                }
            }
        }

        private long _PtRegDetailID;
        public long PtRegDetailID
        {
            get
            {
                return _PtRegDetailID;
            }
            set
            {
                _PtRegDetailID = value;
                NotifyOfPropertyChange(() => PtRegDetailID);
                if (PtRegDetailID > 0)
                {
                    CurObstetricGynecologicalHistory.PtRegDetailID = PtRegDetailID;
                }
            }
        }

        private string _TitleEdit = "Lưu";
        public string TitleEdit
        {
            get
            {
                return _TitleEdit;
            }
            set
            {
                _TitleEdit = value;
                NotifyOfPropertyChange(() => TitleEdit);
            }
        }

        #endregion

        #region Events
        [ImportingConstructor]
        public ObstetricGynecologicalHistoryViewModel(IWindsorContainer aContainer, INavigationService aNavigation, IEventAggregator aEventAgrs, ISalePosCaching aCaching)
        {
            V_Contraception = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_Contraception).ToObservableCollection();
            CurObstetricGynecologicalHistory = new ObstetricGynecologicalHistory();
            CurObstetricGynecologicalHistory.MenstruationIsRegular = true;
            CurObstetricGynecologicalHistory.Dysmenorrhea = true;
            CurObstetricGynecologicalHistory.Married = true;
            CurObstetricGynecologicalHistory.HasOBGYNSurgeries = true;
            CurObstetricGynecologicalHistory.IsUseContraception = true;
        }

        public void rdtMIsRegular_Checked(object sender, RoutedEventArgs e)
        {
            CurObstetricGynecologicalHistory.MenstruationIsRegular = true;
        }

        public void rdtMIsNotRegular_Checked(object sender, RoutedEventArgs e)
        {
            CurObstetricGynecologicalHistory.MenstruationIsRegular = false;
        }

        public void rdtDysmenorrhea_Checked(object sender, RoutedEventArgs e)
        {
            CurObstetricGynecologicalHistory.Dysmenorrhea = true;
        }

        public void rdtNotDysmenorrhea_Checked(object sender, RoutedEventArgs e)
        {
            CurObstetricGynecologicalHistory.Dysmenorrhea = false;
        }

        public void rdtMarried_Checked(object sender, RoutedEventArgs e)
        {
            CurObstetricGynecologicalHistory.Married = true;
            IsEnablePara = true;
        }

        public void rdtNotMarried_Checked(object sender, RoutedEventArgs e)
        {
            CurObstetricGynecologicalHistory.Married = false;
            IsEnablePara = false;
        }

        public void rdtHasOBGYNSurgeries_Checked(object sender, RoutedEventArgs e)
        {
            CurObstetricGynecologicalHistory.HasOBGYNSurgeries = true;
            IsHasOBGYNSurgeries = true;
        }

        public void rdtNeverHadOBGYNSurgeries_Checked(object sender, RoutedEventArgs e)
        {
            CurObstetricGynecologicalHistory.HasOBGYNSurgeries = false;
            IsHasOBGYNSurgeries = false;
        }

        public void rdtIsUseContraception_Checked(object sender, RoutedEventArgs e)
        {
            CurObstetricGynecologicalHistory.IsUseContraception = true;
            IsUseContraception = true;
        }

        public void rdtIsNotUseContraception_Checked(object sender, RoutedEventArgs e)
        {
            CurObstetricGynecologicalHistory.IsUseContraception = false;
            CurObstetricGynecologicalHistory.V_Contraception = 0;
            IsUseContraception = false;
        }

        private bool _IsEnablePara = true;
        public bool IsEnablePara
        {
            get
            {
                return _IsEnablePara;
            }
            set
            {
                _IsEnablePara = value;
                NotifyOfPropertyChange(() => IsEnablePara);
            }
        }

        private bool _IsHasOBGYNSurgeries = true;
        public bool IsHasOBGYNSurgeries
        {
            get
            {
                return _IsHasOBGYNSurgeries;
            }
            set
            {
                _IsHasOBGYNSurgeries = value;
                NotifyOfPropertyChange(() => IsHasOBGYNSurgeries);
            }
        }

        private bool _IsUseContraception = true;
        public bool IsUseContraception
        {
            get
            {
                return _IsUseContraception;
            }
            set
            {
                _IsUseContraception = value;
                NotifyOfPropertyChange(() => IsUseContraception);
            }
        }

        private bool _IsMedicalExamination;
        public bool IsMedicalExamination
        {
            get
            {
                return _IsMedicalExamination;
            }
            set
            {
                _IsMedicalExamination = value;
                NotifyOfPropertyChange(() => IsMedicalExamination);
            }
        }
        
        private ObstetricGynecologicalHistory _CurObstetricGynecologicalHistory;
        public ObstetricGynecologicalHistory CurObstetricGynecologicalHistory
        {
            get
            {
                return _CurObstetricGynecologicalHistory;
            }
            set
            {
                _CurObstetricGynecologicalHistory = value;
                NotifyOfPropertyChange(() => CurObstetricGynecologicalHistory);
            }
        }

        private void GetObstetricGynecologicalHistoryLatest(long PatientID)
        {
            if (PatientID == 0)
            {
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var mThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetObstetricGynecologicalHistoryLatest(PatientID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var Result = contract.EndGetObstetricGynecologicalHistoryLatest(asyncResult);
                                if (Result != null)
                                {
                                    if (Result.PtRegDetailID != PtRegDetailID)
                                    {
                                        Result.ObstetricGynecologicalHistoryID = 0;
                                        TitleEdit = "Lưu";
                                    }
                                    else
                                    {
                                        TitleEdit = "Hiệu chỉnh tiền sử sản phụ khoa";
                                    }
                                    CurObstetricGynecologicalHistory = Result;
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            mThread.Start();
        }

        private void UpdateObstetricGynecologicalHistory()
        {
            if (CurObstetricGynecologicalHistory == null)
            {
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var mThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateObstetricGynecologicalHistory(CurObstetricGynecologicalHistory, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var Result = contract.EndUpdateObstetricGynecologicalHistory(asyncResult);
                                if (Result)
                                {
                                    GetObstetricGynecologicalHistoryLatest(CurObstetricGynecologicalHistory.PatientID);
                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            mThread.Start();
        }

        public void btnSave()
        {
            if (CurObstetricGynecologicalHistory == null || PatientID == 0)
            {
                return;
            }
            string error = "";
            if (CurObstetricGynecologicalHistory.Menarche == 0)
            {
                error += string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, "[Bắt đầu thấy kinh nguyệt năm bao nhiêu tuổi]") + "\n";
            }
            if (CurObstetricGynecologicalHistory.MenstrualCycle == 0)
            {
                error += string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, "[Chu kỳ kinh]") + "\n";
            }
            if (CurObstetricGynecologicalHistory.MenstrualVolume == 0)
            {
                error += string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, "[Lượng kinh]") + "\n";
            }
            if (CurObstetricGynecologicalHistory.Married && CurObstetricGynecologicalHistory.Para == 0)
            {
                error += string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, "[PARA]") + "\n";
            }
            if (CurObstetricGynecologicalHistory.HasOBGYNSurgeries && CurObstetricGynecologicalHistory.NumberOfOBGYNSurgeries == 0)
            {
                error += string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, "[Số lần mổ sản, phụ khoa]") + "\n";
            }
            if (CurObstetricGynecologicalHistory.HasOBGYNSurgeries && string.IsNullOrEmpty(CurObstetricGynecologicalHistory.NoteOBGYNSurgeries))
            {
                error += string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, "[Số lần mổ sản, phụ khoa - Ghi rõ]") + "\n";
            }
            if (CurObstetricGynecologicalHistory.Married && CurObstetricGynecologicalHistory.IsUseContraception == null)
            {
                error += string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, "[Có đang áp dụng BPTT không?]") + "\n";
            }
            if (CurObstetricGynecologicalHistory.IsUseContraception && CurObstetricGynecologicalHistory.V_Contraception == 0)
            {
                error += string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, "[Biệt pháp tránh thai]") + "\n";
            }
            if (error != "")
            {
                Globals.ShowMessage(error, "Cảnh báo");
                return;
            }
            else
            {
                CurObstetricGynecologicalHistory.CreatedStaff = Globals.LoggedUserAccount.Staff;
                CurObstetricGynecologicalHistory.LastUpdateStaff = Globals.LoggedUserAccount.Staff;
                UpdateObstetricGynecologicalHistory();
            }
        }
        #endregion
    }
}
